using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Miastro.Infrastructure.Geography.Catalog;

if (args.Length == 1 && args[0] == "--version")
{
    Console.WriteLine("Miastro.DataBuilder phase4-schema-1");
    return 0;
}

var parsed = ParseArgs(args);

if (!parsed.TryGetValue("--fixture", out var fixturePath) ||
    !parsed.TryGetValue("--output", out var outputPath) ||
    !parsed.TryGetValue("--dataset-version", out var datasetVersion))
{
    Console.Error.WriteLine(
        "Usage: Miastro.DataBuilder " +
        "--fixture <tsv> --output <sqlite> " +
        "--dataset-version <version>");
    return 2;
}

fixturePath = Path.GetFullPath(fixturePath);
outputPath = Path.GetFullPath(outputPath);

if (!File.Exists(fixturePath))
{
    Console.Error.WriteLine($"Fixture not found: {fixturePath}");
    return 3;
}

Directory.CreateDirectory(
    Path.GetDirectoryName(outputPath)!);

if (File.Exists(outputPath))
{
    File.Delete(outputPath);
}

var rows = File.ReadLines(fixturePath, Encoding.UTF8)
    .Where(line =>
        !string.IsNullOrWhiteSpace(line) &&
        !line.StartsWith('#'))
    .Select(ParseRow)
    .OrderBy(x => x.GeoNameId)
    .ToArray();

var cs = new SqliteConnectionStringBuilder
{
    DataSource = outputPath,
    Mode = SqliteOpenMode.ReadWriteCreate
}.ToString();

using (var connection = new SqliteConnection(cs))
{
    connection.Open();

    using (var command = connection.CreateCommand())
    {
        command.CommandText = """
PRAGMA journal_mode=DELETE;
PRAGMA synchronous=FULL;
PRAGMA foreign_keys=ON;
PRAGMA page_size=4096;
VACUUM;

CREATE TABLE metadata (
    key TEXT PRIMARY KEY,
    value TEXT NOT NULL
);

CREATE TABLE locations (
    geoname_id INTEGER PRIMARY KEY,
    name TEXT NOT NULL,
    ascii_name TEXT NOT NULL,
    normalized_name TEXT NOT NULL,
    normalized_ascii_name TEXT NOT NULL,
    country TEXT NOT NULL,
    country_code TEXT NOT NULL,
    admin1 TEXT NOT NULL,
    admin2 TEXT NULL,
    latitude REAL NOT NULL,
    longitude REAL NOT NULL,
    timezone_id TEXT NOT NULL,
    population INTEGER NULL,
    feature_class TEXT NOT NULL,
    feature_code TEXT NOT NULL
);

CREATE TABLE alternate_names (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    geoname_id INTEGER NOT NULL,
    name TEXT NOT NULL,
    normalized_name TEXT NOT NULL,
    FOREIGN KEY(geoname_id) REFERENCES locations(geoname_id)
);

CREATE INDEX ix_locations_normalized_name
ON locations(normalized_name);

CREATE INDEX ix_locations_ascii
ON locations(normalized_ascii_name);

CREATE INDEX ix_locations_country
ON locations(country_code);

CREATE INDEX ix_locations_admin1
ON locations(admin1);

CREATE INDEX ix_locations_admin2
ON locations(admin2);

CREATE INDEX ix_locations_timezone
ON locations(timezone_id);

CREATE INDEX ix_alternate_names_normalized
ON alternate_names(normalized_name);

CREATE VIRTUAL TABLE location_fts USING fts5(
    geoname_id UNINDEXED,
    text,
    tokenize='unicode61 remove_diacritics 2'
);
""";
        command.ExecuteNonQuery();
    }

    using var tx = connection.BeginTransaction();

    foreach (var row in rows)
    {
        using var insert = connection.CreateCommand();
        insert.Transaction = tx;
        insert.CommandText = """
INSERT INTO locations(
    geoname_id, name, ascii_name,
    normalized_name, normalized_ascii_name,
    country, country_code, admin1, admin2,
    latitude, longitude, timezone_id, population,
    feature_class, feature_code)
VALUES(
    $id, $name, $ascii,
    $norm, $normAscii,
    $country, $cc, $admin1, $admin2,
    $lat, $lon, $tz, $pop,
    $fc, $fcode);
""";

        insert.Parameters.AddWithValue("$id", row.GeoNameId);
        insert.Parameters.AddWithValue("$name", row.Name);
        insert.Parameters.AddWithValue("$ascii", row.AsciiName);
        insert.Parameters.AddWithValue(
            "$norm",
            GeoNameNormalizer.Normalize(row.Name));
        insert.Parameters.AddWithValue(
            "$normAscii",
            GeoNameNormalizer.Normalize(row.AsciiName));
        insert.Parameters.AddWithValue("$country", row.Country);
        insert.Parameters.AddWithValue("$cc", row.CountryCode);
        insert.Parameters.AddWithValue("$admin1", row.Admin1);
        insert.Parameters.AddWithValue(
            "$admin2",
            string.IsNullOrWhiteSpace(row.Admin2)
                ? DBNull.Value
                : row.Admin2);
        insert.Parameters.AddWithValue("$lat", row.Latitude);
        insert.Parameters.AddWithValue("$lon", row.Longitude);
        insert.Parameters.AddWithValue("$tz", row.TimeZoneId);
        insert.Parameters.AddWithValue(
            "$pop",
            row.Population is null
                ? DBNull.Value
                : row.Population);
        insert.Parameters.AddWithValue("$fc", row.FeatureClass);
        insert.Parameters.AddWithValue("$fcode", row.FeatureCode);
        insert.ExecuteNonQuery();

        foreach (var alt in row.AlternateNames
                     .Where(x => !string.IsNullOrWhiteSpace(x))
                     .Distinct(StringComparer.Ordinal)
                     .OrderBy(x => x, StringComparer.Ordinal))
        {
            using var a = connection.CreateCommand();
            a.Transaction = tx;
            a.CommandText = """
INSERT INTO alternate_names(geoname_id, name, normalized_name)
VALUES($id, $name, $normalized);
""";
            a.Parameters.AddWithValue("$id", row.GeoNameId);
            a.Parameters.AddWithValue("$name", alt);
            a.Parameters.AddWithValue(
                "$normalized",
                GeoNameNormalizer.Normalize(alt));
            a.ExecuteNonQuery();
        }

        using var fts = connection.CreateCommand();
        fts.Transaction = tx;
        fts.CommandText = """
INSERT INTO location_fts(geoname_id, text)
VALUES($id, $text);
""";
        fts.Parameters.AddWithValue("$id", row.GeoNameId);
        fts.Parameters.AddWithValue(
            "$text",
            string.Join(
                ' ',
                new[]
                {
                    row.Name,
                    row.AsciiName,
                    row.Country,
                    row.Admin1,
                    row.Admin2 ?? string.Empty
                }.Concat(row.AlternateNames)));
        fts.ExecuteNonQuery();
    }

    using (var metadata = connection.CreateCommand())
    {
        metadata.Transaction = tx;
        metadata.CommandText = """
INSERT INTO metadata(key, value) VALUES
('schema_version', '2'),
('dataset_version', $datasetVersion),
('source_format', 'miastro-phase4-fixture-tsv'),
('builder_version', 'phase4-schema-1');
""";
        metadata.Parameters.AddWithValue(
            "$datasetVersion",
            datasetVersion);
        metadata.ExecuteNonQuery();
    }

    tx.Commit();

    using var optimize = connection.CreateCommand();
    optimize.CommandText = """
ANALYZE;
PRAGMA optimize;
""";
    optimize.ExecuteNonQuery();
}

var dbBytes = await File.ReadAllBytesAsync(outputPath);
var fixtureBytes = await File.ReadAllBytesAsync(fixturePath);

var manifest = new
{
    schemaVersion = 1,
    datasetVersion,
    source = Path.GetFileName(fixturePath),
    sourceSha256 = Convert.ToHexString(
        SHA256.HashData(fixtureBytes)).ToLowerInvariant(),
    database = Path.GetFileName(outputPath),
    databaseSize = dbBytes.LongLength,
    databaseSha256 = Convert.ToHexString(
        SHA256.HashData(dbBytes)).ToLowerInvariant(),
    records = rows.Length
};

var manifestPath =
    Path.Combine(
        Path.GetDirectoryName(outputPath)!,
        "manifest.json");

await File.WriteAllTextAsync(
    manifestPath,
    JsonSerializer.Serialize(
        manifest,
        new JsonSerializerOptions { WriteIndented = true }) + "\n");

Console.WriteLine($"GeoNamesFixtureRecords={rows.Length}");
Console.WriteLine($"GeoNamesDatabase={outputPath}");
Console.WriteLine($"GeoNamesDatabaseSize={dbBytes.LongLength}");
Console.WriteLine(
    $"GeoNamesDatabaseSha256={manifest.databaseSha256}");
Console.WriteLine("DataBuilder=PASS");

return 0;

static Dictionary<string, string> ParseArgs(string[] args)
{
    var result =
        new Dictionary<string, string>(
            StringComparer.Ordinal);

    for (var i = 0; i < args.Length; i += 2)
    {
        if (i + 1 >= args.Length)
        {
            throw new ArgumentException(
                $"Missing value for argument {args[i]}");
        }

        result[args[i]] = args[i + 1];
    }

    return result;
}

static FixtureRow ParseRow(string line)
{
    var p = line.Split('\t');

    if (p.Length != 15)
    {
        throw new FormatException(
            $"Expected 15 TSV columns, got {p.Length}: {line}");
    }

    return new FixtureRow(
        long.Parse(p[0], CultureInfo.InvariantCulture),
        p[1],
        p[2],
        p[3],
        p[4],
        p[5],
        string.IsNullOrEmpty(p[6]) ? null : p[6],
        double.Parse(p[7], CultureInfo.InvariantCulture),
        double.Parse(p[8], CultureInfo.InvariantCulture),
        p[9],
        string.IsNullOrEmpty(p[10])
            ? null
            : long.Parse(p[10], CultureInfo.InvariantCulture),
        p[11],
        p[12],
        p[13].Split(
            '|',
            StringSplitOptions.RemoveEmptyEntries |
            StringSplitOptions.TrimEntries),
        p[14]);
}

internal sealed record FixtureRow(
    long GeoNameId,
    string Name,
    string AsciiName,
    string Country,
    string CountryCode,
    string Admin1,
    string? Admin2,
    double Latitude,
    double Longitude,
    string TimeZoneId,
    long? Population,
    string FeatureClass,
    string FeatureCode,
    string[] AlternateNames,
    string Notes);

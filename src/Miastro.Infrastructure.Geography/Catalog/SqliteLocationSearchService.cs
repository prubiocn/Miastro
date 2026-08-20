using Microsoft.Data.Sqlite;
using Miastro.Application.Geography;
using Miastro.Domain.Geography;
using NodaTime;

namespace Miastro.Infrastructure.Geography.Catalog;

public sealed class SqliteLocationSearchService
    : ILocationSearchService
{
    public const string ExpectedSchemaVersion = "2";

    private readonly GeoNamesCatalogOptions _options;

    public SqliteLocationSearchService(
        GeoNamesCatalogOptions options)
    {
        _options = options;
    }

    public async Task<IReadOnlyList<LocationSearchResult>> SearchAsync(
        LocationSearchQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.Text))
        {
            throw new GeographyException(
                GeographyErrorCode.InvalidSearch,
                "Location search text cannot be empty.");
        }

        if (query.Limit is < 1 or > 100)
        {
            throw new GeographyException(
                GeographyErrorCode.InvalidSearch,
                "Location search limit must be in [1, 100].");
        }

        var normalized = GeoNameNormalizer.Normalize(query.Text);

        await using var connection =
            await OpenValidatedAsync(cancellationToken);

        await using var command = connection.CreateCommand();

        command.CommandText = """
WITH matches AS (
    SELECT l.geoname_id, 0 AS rank
    FROM locations l
    WHERE l.normalized_name = $q

    UNION ALL

    SELECT l.geoname_id, 1 AS rank
    FROM alternate_names a
    JOIN locations l ON l.geoname_id = a.geoname_id
    WHERE a.normalized_name = $q

    UNION ALL

    SELECT l.geoname_id, 2 AS rank
    FROM locations l
    WHERE l.normalized_name LIKE $prefix
       OR l.normalized_ascii_name LIKE $prefix

    UNION ALL

    SELECT l.geoname_id, 3 AS rank
    FROM alternate_names a
    JOIN locations l ON l.geoname_id = a.geoname_id
    WHERE a.normalized_name LIKE $prefix

    UNION ALL

    SELECT l.geoname_id, 4 AS rank
    FROM location_fts
    JOIN locations l
      ON l.geoname_id = CAST(location_fts.geoname_id AS INTEGER)
    WHERE location_fts MATCH $fts
),
ranked AS (
    SELECT geoname_id, MIN(rank) AS rank
    FROM matches
    GROUP BY geoname_id
)
SELECT
    l.geoname_id,
    l.name,
    l.country,
    l.country_code,
    l.admin1,
    l.admin2,
    l.latitude,
    l.longitude,
    l.timezone_id,
    l.population,
    r.rank
FROM ranked r
JOIN locations l ON l.geoname_id = r.geoname_id
WHERE ($country IS NULL OR l.country_code = $country)
ORDER BY
    r.rank ASC,
    l.country_code ASC,
    l.admin1 ASC,
    l.admin2 ASC,
    l.name ASC,
    l.geoname_id ASC
LIMIT $limit;
""";

        command.Parameters.AddWithValue("$q", normalized);
        command.Parameters.AddWithValue("$prefix", normalized + "%");

        var ftsQuery = string.Join(
            " ",
            normalized
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(token =>
                    "\"" +
                    token.Replace(
                        "\"",
                        "\"\"",
                        StringComparison.Ordinal) +
                    "\"*"));

        command.Parameters.AddWithValue("$fts", ftsQuery);
        command.Parameters.AddWithValue(
            "$country",
            query.CountryCode is null
                ? DBNull.Value
                : query.CountryCode.Trim().ToUpperInvariant());
        command.Parameters.AddWithValue("$limit", query.Limit);

        var results = new List<LocationSearchResult>();

        await using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(ReadResult(reader));
        }

        return results;
    }

    public async Task<LocationSearchResult?> GetByGeoNameIdAsync(
        long geoNameId,
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            await OpenValidatedAsync(cancellationToken);

        await using var command = connection.CreateCommand();

        command.CommandText = """
SELECT
    geoname_id,
    name,
    country,
    country_code,
    admin1,
    admin2,
    latitude,
    longitude,
    timezone_id,
    population
FROM locations
WHERE geoname_id = $id;
""";

        command.Parameters.AddWithValue("$id", geoNameId);

        await using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        return await reader.ReadAsync(cancellationToken)
            ? ReadResult(reader)
            : null;
    }

    private async Task<SqliteConnection> OpenValidatedAsync(
        CancellationToken cancellationToken)
    {
        var path = Path.GetFullPath(_options.DatabasePath);

        if (!File.Exists(path))
        {
            throw new GeographyException(
                GeographyErrorCode.CatalogMissing,
                $"GeoNames catalog does not exist: {path}");
        }

        try
        {
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = path,
                Mode = SqliteOpenMode.ReadOnly,
                Cache = SqliteCacheMode.Shared
            };

            var connection =
                new SqliteConnection(builder.ToString());

            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText =
                "SELECT value FROM metadata WHERE key='schema_version';";

            var version =
                (string?)await command.ExecuteScalarAsync(
                    cancellationToken);

            if (version != ExpectedSchemaVersion)
            {
                await connection.DisposeAsync();

                throw new GeographyException(
                    GeographyErrorCode.SchemaMismatch,
                    $"GeoNames schema version '{version ?? "<missing>"}' " +
                    $"does not match expected '{ExpectedSchemaVersion}'.");
            }

            return connection;
        }
        catch (GeographyException)
        {
            throw;
        }
        catch (SqliteException ex)
        {
            throw new GeographyException(
                GeographyErrorCode.CatalogCorrupt,
                "GeoNames catalog cannot be opened or queried.",
                ex);
        }
    }

    private static LocationSearchResult ReadResult(
        SqliteDataReader reader)
    {
        var name = reader.GetString(1);
        var country = reader.GetString(2);
        var countryCode = reader.GetString(3);
        var admin1 = reader.GetString(4);
        var admin2 = reader.IsDBNull(5)
            ? null
            : reader.GetString(5);

        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(country) ||
            string.IsNullOrWhiteSpace(countryCode) ||
            string.IsNullOrWhiteSpace(admin1))
        {
            throw new GeographyException(
                GeographyErrorCode.IncompleteResult,
                "GeoNames result is missing mandatory fields.");
        }

        var zoneText = reader.GetString(8);

        if (DateTimeZoneProviders.Tzdb.GetZoneOrNull(zoneText) is null)
        {
            throw new GeographyException(
                GeographyErrorCode.InvalidTimeZoneId,
                $"GeoNames result contains unknown IANA zone: {zoneText}");
        }

        var zone = new IanaTimeZoneId(zoneText);

        var displayName = string.IsNullOrWhiteSpace(admin2)
            ? $"{name} — {admin1} — {country}"
            : $"{name} — {admin1} — {country}";

        return new LocationSearchResult(
            reader.GetInt64(0),
            name,
            admin1,
            admin2,
            country,
            countryCode,
            new Latitude(reader.GetDouble(6)),
            new Longitude(reader.GetDouble(7)),
            zone,
            reader.IsDBNull(9)
                ? null
                : reader.GetInt64(9),
            displayName);
    }
}

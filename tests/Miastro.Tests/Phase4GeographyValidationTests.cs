using Microsoft.Data.Sqlite;
using Miastro.Application.Geography;
using Miastro.Infrastructure.Geography.Catalog;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase4GeographyValidationTests
{
    [TestMethod]
    public async Task InvalidIanaId_IsTypedError()
    {
        var path = CreateCatalog(
            "Etc/DefinitelyMissing",
            "Madrid",
            "España",
            "ES");

        try
        {
            var service = new SqliteLocationSearchService(
                new GeoNamesCatalogOptions(path));

            GeographyException? captured = null;

            try
            {
                await service.GetByGeoNameIdAsync(1);
            }
            catch (GeographyException ex)
            {
                captured = ex;
            }

            Assert.IsNotNull(captured);
            Assert.AreEqual(
                GeographyErrorCode.InvalidTimeZoneId,
                captured.Code);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [TestMethod]
    public async Task IncompleteCountry_IsTypedError()
    {
        var path = CreateCatalog(
            "Europe/Madrid",
            "Madrid",
            "",
            "ES");

        try
        {
            var service = new SqliteLocationSearchService(
                new GeoNamesCatalogOptions(path));

            GeographyException? captured = null;

            try
            {
                await service.GetByGeoNameIdAsync(1);
            }
            catch (GeographyException ex)
            {
                captured = ex;
            }

            Assert.IsNotNull(captured);
            Assert.AreEqual(
                GeographyErrorCode.IncompleteResult,
                captured.Code);
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static string CreateCatalog(
        string zone,
        string name,
        string country,
        string countryCode)
    {
        var path = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid() + ".sqlite");

        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = path
        };

        using var connection =
            new SqliteConnection(builder.ToString());

        connection.Open();

        using var command = connection.CreateCommand();

        command.CommandText = """
CREATE TABLE metadata (
    key TEXT PRIMARY KEY,
    value TEXT NOT NULL
);

INSERT INTO metadata(key, value)
VALUES('schema_version', '2');

CREATE TABLE locations (
    geoname_id INTEGER PRIMARY KEY,
    name TEXT NOT NULL,
    ascii_name TEXT NOT NULL,
    normalized_name TEXT NOT NULL,
    normalized_ascii_name TEXT NOT NULL,
    country TEXT NOT NULL,
    country_code TEXT NOT NULL,
    admin1 TEXT NOT NULL,
    admin1_code TEXT NOT NULL,
    admin2 TEXT NULL,
    admin2_code TEXT NULL,
    latitude REAL NOT NULL,
    longitude REAL NOT NULL,
    timezone_id TEXT NOT NULL,
    population INTEGER NULL,
    feature_class TEXT NOT NULL,
    feature_code TEXT NOT NULL,
    modification_date TEXT NULL
);

CREATE TABLE alternate_names (
    alternate_name_id INTEGER PRIMARY KEY,
    geoname_id INTEGER NOT NULL,
    language TEXT NULL,
    name TEXT NOT NULL,
    normalized_name TEXT NOT NULL,
    is_preferred INTEGER NOT NULL,
    is_short INTEGER NOT NULL,
    is_colloquial INTEGER NOT NULL,
    is_historic INTEGER NOT NULL,
    valid_from TEXT NULL,
    valid_to TEXT NULL
);

CREATE VIRTUAL TABLE location_fts USING fts5(
    geoname_id UNINDEXED,
    text
);

INSERT INTO locations(
    geoname_id, name, ascii_name,
    normalized_name, normalized_ascii_name,
    country, country_code,
    admin1, admin1_code,
    admin2, admin2_code,
    latitude, longitude, timezone_id,
    population, feature_class, feature_code,
    modification_date
)
VALUES(
    1, $name, $name,
    'madrid', 'madrid',
    $country, $countryCode,
    'Madrid', '29',
    NULL, NULL,
    40.4168, -3.7038, $zone,
    1000, 'P', 'PPLA',
    '2026-08-20'
);
""";

        command.Parameters.AddWithValue("$name", name);
        command.Parameters.AddWithValue("$country", country);
        command.Parameters.AddWithValue(
            "$countryCode",
            countryCode);
        command.Parameters.AddWithValue("$zone", zone);

        command.ExecuteNonQuery();

        return path;
    }
}

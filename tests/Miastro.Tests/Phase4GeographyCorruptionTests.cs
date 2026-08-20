using Microsoft.Data.Sqlite;
using Miastro.Application.Geography;
using Miastro.Infrastructure.Geography.Catalog;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase4GeographyCorruptionTests
{
    [TestMethod]
    public async Task CorruptCatalog_ReturnsTypedError()
    {
        var path = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid() + ".sqlite");

        await File.WriteAllTextAsync(
            path,
            "not-a-sqlite-database");

        try
        {
            var service = new SqliteLocationSearchService(
                new GeoNamesCatalogOptions(path));

            GeographyException? captured = null;

            try
            {
                await service.SearchAsync(
                    new LocationSearchQuery("Madrid"));
            }
            catch (GeographyException ex)
            {
                captured = ex;
            }

            Assert.IsNotNull(captured);
            Assert.AreEqual(
                GeographyErrorCode.CatalogCorrupt,
                captured.Code);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [TestMethod]
    public async Task WrongSchema_ReturnsTypedError()
    {
        var path = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid() + ".sqlite");

        try
        {
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = path
            };

            using (var connection =
                new SqliteConnection(builder.ToString()))
            {
                connection.Open();

                using var command = connection.CreateCommand();

                command.CommandText = """
CREATE TABLE metadata(
    key TEXT PRIMARY KEY,
    value TEXT NOT NULL
);
INSERT INTO metadata(key, value)
VALUES('schema_version', '999');
""";

                command.ExecuteNonQuery();
            }

            var service = new SqliteLocationSearchService(
                new GeoNamesCatalogOptions(path));

            GeographyException? captured = null;

            try
            {
                await service.SearchAsync(
                    new LocationSearchQuery("Madrid"));
            }
            catch (GeographyException ex)
            {
                captured = ex;
            }

            Assert.IsNotNull(captured);
            Assert.AreEqual(
                GeographyErrorCode.SchemaMismatch,
                captured.Code);
        }
        finally
        {
            File.Delete(path);
        }
    }
}

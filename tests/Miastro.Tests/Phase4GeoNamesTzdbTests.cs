using Microsoft.Data.Sqlite;
using NodaTime;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase4GeoNamesTzdbTests
{
    [TestMethod]
    public void EveryOfficialCatalogZoneExistsInBundledTzdb()
    {
        var path = Phase4GeoCatalogTestPaths.Resolve();

        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = path,
            Mode = SqliteOpenMode.ReadOnly
        };

        using var connection =
            new SqliteConnection(builder.ToString());

        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT DISTINCT timezone_id " +
            "FROM locations ORDER BY timezone_id;";

        using var reader = command.ExecuteReader();

        var missing = new List<string>();

        while (reader.Read())
        {
            var id = reader.GetString(0);

            if (DateTimeZoneProviders.Tzdb.GetZoneOrNull(id) is null)
            {
                missing.Add(id);
            }
        }

        Assert.AreEqual(
            0,
            missing.Count,
            "GeoNames zones missing from bundled TZDB: " +
            string.Join(", ", missing));
    }
}

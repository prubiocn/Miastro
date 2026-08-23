using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.Domain.People;
using Miastro.Infrastructure.Persistence;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6NatalBirthDataIdentityTests
{
    [TestMethod]
    public void Birth_hash_is_independent_of_house_system()
    {
        var placidus =
            Input(HouseSystem.Placidus);

        var koch =
            placidus with
            {
                HouseSystem =
                    HouseSystem.Koch
            };

        Assert.AreEqual(
            NatalBirthDataIdentity.Compute(
                placidus),
            NatalBirthDataIdentity.Compute(
                koch));

        Assert.AreNotEqual(
            NatalInputHash.Compute(
                placidus),
            NatalInputHash.Compute(
                koch));
    }

    [TestMethod]
    public void Birth_hash_changes_when_historical_birth_identity_changes()
    {
        var first =
            Input(HouseSystem.Placidus);

        var second =
            first with
            {
                HistoricalOffsetSeconds =
                    7200,

                InstantUtc =
                    first.InstantUtc
                        .AddHours(-1)
            };

        Assert.AreNotEqual(
            NatalBirthDataIdentity.Compute(
                first),
            NatalBirthDataIdentity.Compute(
                second));
    }

    [TestMethod]
    public async Task Migration_contains_reproducible_birth_identity_columns()
    {
        await using var connection =
            new SqliteConnection(
                "Data Source=:memory:");

        await connection.OpenAsync();

        var options =
            new DbContextOptionsBuilder<
                MiastroDbContext>()
                .UseSqlite(connection)
                .Options;

        await using var context =
            new MiastroDbContext(
                options);

        await context.Database
            .MigrateAsync();

        await using var command =
            connection.CreateCommand();

        command.CommandText =
            "PRAGMA table_info('NatalCharts');";

        var columns =
            new HashSet<string>(
                StringComparer.Ordinal);

        await using var reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            columns.Add(
                reader.GetString(1));
        }

        foreach (var expected in new[]
        {
            "BirthDataVersion",
            "BirthDataHash",
            "BirthTimePrecision",
            "GeoNameId",
            "HistoricalOffsetSeconds",
            "AmbiguousSelection"
        })
        {
            Assert.IsTrue(
                columns.Contains(expected),
                $"Falta columna {expected}");
        }
    }

    private static NatalInputFingerprint Input(
        HouseSystem houseSystem)
        => new(
            new DateOnly(
                2000, 1, 1),
            new TimeOnly(
                12, 0),
            new DateTimeOffset(
                2000, 1, 1,
                11, 0, 0,
                TimeSpan.Zero),
            40.4168,
            -3.7038,
            "Europe/Madrid",
            "TZDB: 2026c",
            houseSystem,
            "miastro-v1",
            "Swiss Ephemeris",
            "2.10.03",
            "ephemeris-test",
            BirthTimePrecision.Exact,
            3117735,
            "Madrid",
            3600,
            null);
}

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Miastro.Astronomy.Abstractions.Models;
using Miastro.Application.Natal;
using Miastro.Domain.Calculation;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;
using Miastro.Infrastructure.Persistence;
using Miastro.Infrastructure.Persistence.Backup;
using Miastro.Infrastructure.Persistence.Entities;
using Miastro.Infrastructure.Persistence.Natal;
using Miastro.Infrastructure.SwissEphemeris.Calculation;
using Miastro.Infrastructure.SwissEphemeris.Configuration;
using Miastro.Infrastructure.SwissEphemeris.Houses;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6OperationalReliabilityTests
{
    private const string LibraryHash =
        "47e6fed985ccb5f067b7a0f6f746ec3567a7b54ce5f86140b2138616a8e6a653";

    private const string Phase5Migration =
        "20260820180432_Phase5PersonFunctionalSchema";

    [TestMethod]
    public async Task Backup_contains_complete_natal_snapshot_and_can_be_reopened()
    {
        var source =
            TempDatabase(
                "backup-source");

        var backup =
            TempDatabase(
                "backup-copy");

        try
        {
            var personId =
                Guid.NewGuid();

            await using (
                var db =
                    CreateContext(source))
            {
                await db.Database
                    .MigrateAsync();

                await InsertPersonAsync(
                    db,
                    personId);

                var snapshot =
                    Phase6NatalTestSnapshotFactory
                        .Create(
                            personId);

                var inputHash =
                    NatalInputHash.Compute(
                        snapshot.Input);

                var store =
                    new EfNatalChartStore(
                        db);

                var saved =
                    await store
                        .SaveOrGetExistingAsync(
                            snapshot,
                            inputHash);

                Assert.IsTrue(
                    saved.Created);

                var service =
                    new SqliteDatabaseBackupService(
                        db);

                await service
                    .BackupAsync(
                        backup);
            }

            await using var reopened =
                CreateContext(
                    backup);

            Assert.AreEqual(
                1,
                await reopened.NatalCharts
                    .CountAsync());

            Assert.AreEqual(
                21,
                await reopened.NatalPlacements
                    .CountAsync());

            Assert.AreEqual(
                12,
                await reopened.NatalHouseCusps
                    .CountAsync());

            var chart =
                await reopened.NatalCharts
                    .AsNoTracking()
                    .SingleAsync();

            Assert.AreEqual(
                personId,
                chart.PersonId);

            Assert.AreEqual(
                (int)NatalChartStatus.Current,
                chart.Status);

            await using var connection =
                new SqliteConnection(
                    $"Data Source={backup}");

            await connection.OpenAsync();

            foreach (
                var table
                in new[]
                {
                    "NatalCharts",
                    "NatalPlacements",
                    "NatalHouseCusps",
                    "NatalAspects",
                    "__EFMigrationsHistory"
                })
            {
                Assert.AreEqual(
                    1L,
                    await TableExistsAsync(
                        connection,
                        table),
                    $"Falta tabla en backup: {table}");
            }
        }
        finally
        {
            DeleteDatabase(source);
            DeleteDatabase(backup);
        }
    }

    [TestMethod]
    public async Task Real_phase5_schema_migrates_to_phase6_without_losing_person()
    {
        var path =
            TempDatabase(
                "phase5-upgrade");

        var personId =
            Guid.NewGuid();

        try
        {
            await using (
                var phase5 =
                    CreateContext(path))
            {
                var migrator =
                    phase5.GetService<
                        IMigrator>();

                await migrator
                    .MigrateAsync(
                        Phase5Migration);

                await InsertPersonRawAsync(
                    phase5,
                    personId);

                Assert.AreEqual(
                    1,
                    await CountPeopleRawAsync(
                        phase5));

                Assert.AreEqual(
                    0L,
                    await TableExistsAsync(
                        phase5,
                        "NatalCharts"));
            }

            await using (
                var phase6 =
                    CreateContext(path))
            {
                await phase6.Database
                    .MigrateAsync();

                Assert.AreEqual(
                    1,
                    await CountPeopleRawAsync(
                        phase6));

                foreach (
                    var table
                    in new[]
                    {
                        "NatalCharts",
                        "NatalPlacements",
                        "NatalHouseCusps",
                        "NatalAspects"
                    })
                {
                    Assert.AreEqual(
                        1L,
                        await TableExistsAsync(
                            phase6,
                            table),
                        $"Migración no creó {table}");
                }

                var migrations =
                    await AppliedMigrationsAsync(
                        phase6);

                CollectionAssert.Contains(
                    migrations,
                    "20260821113324_Phase6NatalSnapshotSchema");

                CollectionAssert.Contains(
                    migrations,
                    "20260821122501_Phase6BirthDataSnapshotIdentity");
            }

            await using (
                var reopened =
                    CreateContext(path))
            {
                Assert.AreEqual(
                    1,
                    await CountPeopleRawAsync(
                        reopened));

                Assert.AreEqual(
                    0,
                    await reopened.NatalCharts
                        .CountAsync());
            }
        }
        finally
        {
            DeleteDatabase(path);
        }
    }

    [TestMethod]
    public async Task Concurrent_position_and_house_calls_are_deterministic()
    {
        var options =
            Options();

        var positions =
            new SwissEphemerisPositionCalculator(
                options);

        var houses =
            new SwissEphemerisHouseCalculator(
                options);

        var instant =
            AstronomicalInstant.FromUtc(
                new DateTimeOffset(
                    2024,
                    1,
                    1,
                    12,
                    0,
                    0,
                    TimeSpan.Zero));

        var location =
            new GeographicLocation(
                40.4168,
                -3.7038);

        var baselinePosition =
            positions.Calculate(
                AstrologicalObjectId.Mercury,
                instant,
                CalculationProfile.MiastroV1);

        var baselineHouses =
            houses.Calculate(
                instant,
                location,
                HouseSystem.Placidus);

        Assert.IsTrue(
            baselineHouses.Success);

        var tasks =
            Enumerable.Range(
                    0,
                    24)
                .Select(
                    index =>
                        Task.Run(
                            () =>
                            {
                                if (
                                    index % 2
                                    == 0)
                                {
                                    var actual =
                                        positions
                                            .Calculate(
                                                AstrologicalObjectId.Mercury,
                                                instant,
                                                CalculationProfile.MiastroV1);

                                    AssertAngularClose(
                                        baselinePosition
                                            .Longitude
                                            .Degrees,
                                        actual.Longitude
                                            .Degrees);

                                    Assert.AreEqual(
                                        baselinePosition
                                            .LongitudeSpeedDegreesPerDay,
                                        actual.LongitudeSpeedDegreesPerDay,
                                        1e-12);

                                    return;
                                }

                                var actualHouses =
                                    houses.Calculate(
                                        instant,
                                        location,
                                        HouseSystem.Placidus);

                                Assert.IsTrue(
                                    actualHouses.Success);

                                Assert.AreEqual(
                                    12,
                                    actualHouses
                                        .Cusps
                                        .Count);

                                AssertAngularClose(
                                    baselineHouses
                                        .Ascendant!
                                        .Value
                                        .Degrees,
                                    actualHouses
                                        .Ascendant!
                                        .Value
                                        .Degrees);

                                AssertAngularClose(
                                    baselineHouses
                                        .Midheaven!
                                        .Value
                                        .Degrees,
                                    actualHouses
                                        .Midheaven!
                                        .Value
                                        .Degrees);

                                for (
                                    var cusp = 0;
                                    cusp < 12;
                                    cusp++)
                                {
                                    AssertAngularClose(
                                        baselineHouses
                                            .Cusps[cusp]
                                            .Longitude
                                            .Degrees,
                                        actualHouses
                                            .Cusps[cusp]
                                            .Longitude
                                            .Degrees);
                                }
                            }))
                .ToArray();

        await Task.WhenAll(
            tasks);
    }

    private static async Task InsertPersonAsync(
        MiastroDbContext db,
        Guid personId)
    {
        var now =
            new DateTimeOffset(
                2026,
                8,
                22,
                8,
                0,
                0,
                TimeSpan.Zero);

        db.People.Add(
            new PersonEntity
            {
                Id = personId,
                FirstName = "Phase",
                LastName = "Six",
                NormalizedName = "phase six",
                IsFavorite = false,
                CreatedAtUtc = now,
                ModifiedAtUtc = now
            });

        await db.SaveChangesAsync();

        db.ChangeTracker.Clear();

        var persisted =
            await db.People
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == personId);

        Assert.IsTrue(
            persisted,
            "La Persona debe existir antes de guardar la carta natal.");
    }

    private static async Task InsertPersonRawAsync(
        MiastroDbContext db,
        Guid personId)
    {
        await using var command =
            db.Database
                .GetDbConnection()
                .CreateCommand();

        var closeAfter =
            command.Connection!.State
                != System.Data
                    .ConnectionState.Open;

        if (closeAfter)
        {
            await command.Connection
                .OpenAsync();
        }

        try
        {
            command.CommandText =
                """
INSERT INTO People
(
    Id,
    FirstName,
    LastName,
    NormalizedName,
    Phone,
    Email,
    PrivateNote,
    IsFavorite,
    LastConsultationAtUtc,
    CreatedAtUtc,
    ModifiedAtUtc
)
VALUES
(
    $id,
    'Phase',
    'Six',
    'phase six',
    NULL,
    NULL,
    NULL,
    0,
    NULL,
    $created,
    $modified
);
""";

            var now =
                new DateTimeOffset(
                    2026,
                    8,
                    22,
                    8,
                    0,
                    0,
                    TimeSpan.Zero)
                    .ToString("O");

            var id =
                command.CreateParameter();

            id.ParameterName =
                "$id";

            id.Value =
                personId.ToString();

            command.Parameters.Add(
                id);

            var created =
                command.CreateParameter();

            created.ParameterName =
                "$created";

            created.Value =
                now;

            command.Parameters.Add(
                created);

            var modified =
                command.CreateParameter();

            modified.ParameterName =
                "$modified";

            modified.Value =
                now;

            command.Parameters.Add(
                modified);

            Assert.AreEqual(
                1,
                await command
                    .ExecuteNonQueryAsync());
        }
        finally
        {
            if (closeAfter)
            {
                await command.Connection
                    .CloseAsync();
            }
        }
    }

    private static async Task<int>
        CountPeopleRawAsync(
            MiastroDbContext db)
    {
        await using var command =
            db.Database
                .GetDbConnection()
                .CreateCommand();

        var closeAfter =
            command.Connection!.State
                != System.Data
                    .ConnectionState.Open;

        if (closeAfter)
        {
            await command.Connection
                .OpenAsync();
        }

        try
        {
            command.CommandText =
                "SELECT COUNT(*) FROM People;";

            return Convert.ToInt32(
                await command
                    .ExecuteScalarAsync());
        }
        finally
        {
            if (closeAfter)
            {
                await command.Connection
                    .CloseAsync();
            }
        }
    }

    private static async Task<long>
        TableExistsAsync(
            MiastroDbContext db,
            string table)
    {
        var connection =
            db.Database
                .GetDbConnection();

        var closeAfter =
            connection.State
                != System.Data
                    .ConnectionState.Open;

        if (closeAfter)
        {
            await connection.OpenAsync();
        }

        try
        {
            await using var command =
                connection
                    .CreateCommand();

            command.CommandText =
                """
SELECT COUNT(*)
FROM sqlite_master
WHERE type='table'
  AND name=$name;
""";

            var parameter =
                command.CreateParameter();

            parameter.ParameterName =
                "$name";

            parameter.Value =
                table;

            command.Parameters.Add(
                parameter);

            return Convert.ToInt64(
                await command
                    .ExecuteScalarAsync());
        }
        finally
        {
            if (closeAfter)
            {
                await connection
                    .CloseAsync();
            }
        }
    }

    private static async Task<long>
        TableExistsAsync(
            SqliteConnection connection,
            string table)
    {
        await using var command =
            connection.CreateCommand();

        command.CommandText =
            """
SELECT COUNT(*)
FROM sqlite_master
WHERE type='table'
  AND name=$name;
""";

        command.Parameters.AddWithValue(
            "$name",
            table);

        return Convert.ToInt64(
            await command
                .ExecuteScalarAsync());
    }

    private static async Task<string[]>
        AppliedMigrationsAsync(
            MiastroDbContext db)
    {
        var connection =
            db.Database
                .GetDbConnection();

        var closeAfter =
            connection.State
                != System.Data
                    .ConnectionState.Open;

        if (closeAfter)
        {
            await connection.OpenAsync();
        }

        try
        {
            await using var command =
                connection
                    .CreateCommand();

            command.CommandText =
                """
SELECT MigrationId
FROM __EFMigrationsHistory
ORDER BY MigrationId;
""";

            var result =
                new List<string>();

            await using var reader =
                await command
                    .ExecuteReaderAsync();

            while (
                await reader
                    .ReadAsync())
            {
                result.Add(
                    reader.GetString(0));
            }

            return
                result.ToArray();
        }
        finally
        {
            if (closeAfter)
            {
                await connection
                    .CloseAsync();
            }
        }
    }

    private static MiastroDbContext
        CreateContext(
            string path)
    {
        var options =
            new DbContextOptionsBuilder<
                MiastroDbContext>()
                .UseSqlite(
                    $"Data Source={path}")
                .Options;

        return new MiastroDbContext(
            options);
    }

    private static SwissEphemerisOptions
        Options()
    {
        var root =
            RepositoryRoot();

        return new SwissEphemerisOptions(
            Path.Combine(
                root,
                "src",
                "Miastro.Infrastructure.SwissEphemeris",
                "native",
                "linux-x64",
                "libswe.so"),
            Path.Combine(
                root,
                "data",
                "ephemeris"),
            LibraryHash,
            "2.10.03");
    }

    private static void AssertAngularClose(
        double expected,
        double actual)
    {
        var difference =
            Math.Abs(
                expected - actual);

        difference =
            Math.Min(
                difference,
                360.0 - difference);

        Assert.IsLessThanOrEqualTo(
            1e-10,
            difference);
    }

    private static string TempDatabase(
        string suffix)
        => Path.Combine(
            Path.GetTempPath(),
            $"miastro-phase6-{suffix}-{Guid.NewGuid():N}.sqlite");

    private static void DeleteDatabase(
        string path)
    {
        foreach (
            var candidate
            in new[]
            {
                path,
                path + "-wal",
                path + "-shm"
            })
        {
            if (File.Exists(candidate))
            {
                File.Delete(
                    candidate);
            }
        }
    }

    private static string RepositoryRoot()
        => Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "../../../../../"));
}

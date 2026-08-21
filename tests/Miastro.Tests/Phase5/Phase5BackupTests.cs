using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Miastro.Infrastructure.Persistence;
using Miastro.Infrastructure.Persistence.Backup;
using Miastro.Infrastructure.Persistence.Entities;

namespace Miastro.Tests.Phase5;

[TestClass]
public sealed class Phase5BackupTests
{
    [TestMethod]
    public async Task Backup_contains_phase5_person_tables_and_data()
    {
        var source =
            TempDatabase("source");

        var backup =
            TempDatabase("backup");

        try
        {
            var id = Guid.NewGuid();

            await using (var db = CreateContext(source))
            {
                await db.Database.MigrateAsync();

                db.People.Add(
                    new PersonEntity
                    {
                        Id = id,
                        FirstName = "Synthetic",
                        LastName = "Person",
                        NormalizedName =
                            "synthetic person",
                        Phone = "000000000",
                        Email =
                            "synthetic@example.invalid",
                        PrivateNote =
                            "synthetic-private-note",
                        CreatedAtUtc =
                            DateTimeOffset.UtcNow,
                        ModifiedAtUtc =
                            DateTimeOffset.UtcNow,
                        BirthData =
                            new BirthDataEntity
                            {
                                PersonId = id,
                                LocalDate =
                                    new DateOnly(
                                        2000,
                                        1,
                                        1),
                                TimePrecision = 5,
                                GeoNameId = 1,
                                Locality =
                                    "Synthetic City",
                                Country =
                                    "Synthetic Country",
                                Region =
                                    "Synthetic Region",
                                Latitude = 40,
                                Longitude = -3,
                                IanaTimeZoneId =
                                    "Europe/Madrid",
                                TemporalResolutionState =
                                    0
                            },
                        CurrentResidence =
                            new CurrentResidenceEntity
                            {
                                PersonId = id,
                                Locality =
                                    "Synthetic Residence",
                                Region =
                                    "Synthetic Region",
                                Country =
                                    "Synthetic Country",
                                Latitude = 41,
                                Longitude = -4,
                                IanaTimeZoneId =
                                    "Europe/Madrid"
                            },
                        History =
                        [
                            new PersonHistoryEntity
                            {
                                PersonId = id,
                                EventType = 1,
                                OccurredAtUtc =
                                    DateTimeOffset.UtcNow,
                                Summary =
                                    "Persona creada"
                            }
                        ]
                    });

                await db.SaveChangesAsync();

                var service =
                    new SqliteDatabaseBackupService(
                        db);

                await service.BackupAsync(
                    backup);
            }

            await using var connection =
                new SqliteConnection(
                    $"Data Source={backup}");

            await connection.OpenAsync();

            foreach (var table in new[]
            {
                "People",
                "BirthData",
                "CurrentResidences",
                "PersonHistory",
                "__EFMigrationsHistory"
            })
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

                var count =
                    Convert.ToInt32(
                        await command
                            .ExecuteScalarAsync());

                Assert.AreEqual(
                    1,
                    count,
                    $"Missing backup table: {table}");
            }

            await using (var command =
                connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT COUNT(*) FROM People;";

                Assert.AreEqual(
                    1L,
                    Convert.ToInt64(
                        await command
                            .ExecuteScalarAsync()));
            }
        }
        finally
        {
            DeleteDatabase(source);
            DeleteDatabase(backup);
        }
    }

    [TestMethod]
    public async Task Backup_refuses_to_overwrite_existing_file()
    {
        var source =
            TempDatabase("source");

        var backup =
            TempDatabase("backup");

        try
        {
            await using var db =
                CreateContext(source);

            await db.Database.MigrateAsync();

            await File.WriteAllTextAsync(
                backup,
                "existing");

            var service =
                new SqliteDatabaseBackupService(
                    db);

            await Assert.ThrowsExactlyAsync<
                IOException>(
                () => service.BackupAsync(
                    backup));
        }
        finally
        {
            DeleteDatabase(source);
            DeleteDatabase(backup);
        }
    }

    private static MiastroDbContext CreateContext(
        string path)
    {
        var options =
            new DbContextOptionsBuilder<
                MiastroDbContext>()
                .UseSqlite(
                    $"Data Source={path}")
                .Options;

        return new MiastroDbContext(options);
    }

    private static string TempDatabase(
        string suffix)
        => Path.Combine(
            Path.GetTempPath(),
            $"miastro-phase5-{suffix}-{Guid.NewGuid():N}.sqlite");

    private static void DeleteDatabase(
        string path)
    {
        foreach (var candidate in new[]
        {
            path,
            path + "-wal",
            path + "-shm"
        })
        {
            if (File.Exists(candidate))
            {
                File.Delete(candidate);
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Miastro.Infrastructure.Persistence;
using Miastro.Infrastructure.Persistence.Entities;

namespace Miastro.Tests.Phase5;

[TestClass]
public sealed class Phase5PersistenceSchemaTests
{
    [TestMethod]
    public async Task Existing_phase4_database_migrates_to_phase5()
    {
        var path = TempDatabase();

        try
        {
            await using (var db = CreateContext(path))
            {
                var migrator = db.GetService<IMigrator>();
                await migrator.MigrateAsync("20260820081958_InitialTechnicalSchema");
            }

            await using (var db = CreateContext(path))
            {
                await db.Database.MigrateAsync();

                var applied = (await db.Database.GetAppliedMigrationsAsync()).ToArray();

                Assert.IsTrue(
                    applied.Contains("20260820081958_InitialTechnicalSchema"));

                Assert.IsTrue(
                    applied.Any(
                        x => x.EndsWith(
                            "_Phase5PersonFunctionalSchema",
                            StringComparison.Ordinal)));

                var tables = await ReadTablesAsync(db);

                CollectionAssert.Contains(tables, "People");
                CollectionAssert.Contains(tables, "BirthData");
                CollectionAssert.Contains(tables, "CurrentResidences");
                CollectionAssert.Contains(tables, "PersonHistory");
                CollectionAssert.DoesNotContain(tables, "TechnicalProbes");
            }
        }
        finally
        {
            DeleteDatabase(path);
        }
    }

    [TestMethod]
    public async Task Crud_update_and_cascade_work_in_real_sqlite()
    {
        var path = TempDatabase();
        var personId = Guid.NewGuid();

        try
        {
            await using (var db = CreateContext(path))
            {
                await db.Database.MigrateAsync();
                db.People.Add(BuildEntity(personId));
                await db.SaveChangesAsync();
            }

            await using (var db = CreateContext(path))
            {
                var person = await db.People
                    .Include(x => x.BirthData)
                    .Include(x => x.CurrentResidence)
                    .Include(x => x.History)
                    .SingleAsync(x => x.Id == personId);

                Assert.AreEqual("Synthetic", person.FirstName);
                Assert.IsNotNull(person.BirthData);
                Assert.IsNotNull(person.CurrentResidence);
                Assert.AreEqual(1, person.History.Count);

                person.LastName = "Updated";
                person.NormalizedName = "synthetic updated";
                await db.SaveChangesAsync();
            }

            await using (var db = CreateContext(path))
            {
                var person = await db.People.SingleAsync(x => x.Id == personId);
                Assert.AreEqual("Updated", person.LastName);
                db.People.Remove(person);
                await db.SaveChangesAsync();
            }

            await using (var db = CreateContext(path))
            {
                Assert.AreEqual(0, await db.People.CountAsync());
                Assert.AreEqual(0, await db.BirthData.CountAsync());
                Assert.AreEqual(0, await db.CurrentResidences.CountAsync());
                Assert.AreEqual(0, await db.PersonHistory.CountAsync());
            }
        }
        finally
        {
            DeleteDatabase(path);
        }
    }

    [TestMethod]
    public async Task Functional_indexes_exist()
    {
        var path = TempDatabase();

        try
        {
            await using var db = CreateContext(path);
            await db.Database.MigrateAsync();

            var connection = db.Database.GetDbConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText =
                "SELECT name FROM sqlite_master WHERE type='index' AND tbl_name='People' ORDER BY name;";

            var names = new List<string>();

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                names.Add(reader.GetString(0));
            }

            Assert.IsTrue(names.Any(x => x.Contains("FirstName", StringComparison.Ordinal)));
            Assert.IsTrue(names.Any(x => x.Contains("LastName", StringComparison.Ordinal)));
            Assert.IsTrue(names.Any(x => x.Contains("IsFavorite", StringComparison.Ordinal)));
            Assert.IsTrue(names.Any(x => x.Contains("LastConsultationAtUtc", StringComparison.Ordinal)));
        }
        finally
        {
            DeleteDatabase(path);
        }
    }

    private static PersonEntity BuildEntity(Guid id)
        => new()
        {
            Id = id,
            FirstName = "Synthetic",
            LastName = "Person",
            NormalizedName = "synthetic person",
            CreatedAtUtc = DateTimeOffset.UtcNow,
            ModifiedAtUtc = DateTimeOffset.UtcNow,
            BirthData = new BirthDataEntity
            {
                PersonId = id,
                LocalDate = new DateOnly(2000, 1, 1),
                TimePrecision = 5,
                GeoNameId = 1,
                Locality = "Synthetic City",
                Country = "Synthetic Country",
                Region = "Synthetic Region",
                Latitude = 40.0,
                Longitude = -3.0,
                IanaTimeZoneId = "Europe/Madrid",
                TemporalResolutionState = 0
            },
            CurrentResidence = new CurrentResidenceEntity
            {
                PersonId = id,
                Locality = "Synthetic Residence",
                GeoNameId = 2,
                Region = "Synthetic Region",
                Country = "Synthetic Country",
                Latitude = 41.0,
                Longitude = -4.0,
                IanaTimeZoneId = "Europe/Madrid"
            },
            History =
            [
                new PersonHistoryEntity
                {
                    PersonId = id,
                    EventType = 1,
                    OccurredAtUtc = DateTimeOffset.UtcNow,
                    Summary = "Persona creada"
                }
            ]
        };

    private static async Task<string[]> ReadTablesAsync(MiastroDbContext db)
    {
        var connection = db.Database.GetDbConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;";

        var tables = new List<string>();

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tables.Add(reader.GetString(0));
        }

        return tables.ToArray();
    }

    private static MiastroDbContext CreateContext(string path)
    {
        var options =
            new DbContextOptionsBuilder<MiastroDbContext>()
                .UseSqlite($"Data Source={path}")
                .Options;

        return new MiastroDbContext(options);
    }

    private static string TempDatabase()
        => Path.Combine(
            Path.GetTempPath(),
            $"miastro-phase5-{Guid.NewGuid():N}.sqlite");

    private static void DeleteDatabase(string path)
    {
        foreach (var candidate in new[] { path, path + "-wal", path + "-shm" })
        {
            if (File.Exists(candidate))
            {
                File.Delete(candidate);
            }
        }
    }
}

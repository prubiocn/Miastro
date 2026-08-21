using Microsoft.EntityFrameworkCore;
using Miastro.Application.People;
using Miastro.Domain.People;
using Miastro.Infrastructure.Persistence;
using Miastro.Infrastructure.Persistence.People;
using Miastro.Infrastructure.Time.Historical;

namespace Miastro.Tests.Phase5;

[TestClass]
public sealed class Phase5PersonHeadlessE2ETests
{
    private static readonly DateTimeOffset Now =
        new(
            2026,
            8,
            21,
            9,
            30,
            0,
            TimeSpan.Zero);

    [TestMethod]
    public async Task Person_normal_birth_time_roundtrips()
    {
        var path = TempDatabase();

        try
        {
            await using var db =
                CreateContext(path);

            await db.Database.MigrateAsync();

            var store =
                new EfPersonStore(db);

            var time =
                new ResolveBirthHistoricalTimeUseCase(
                    new NodaTimeHistoricalTimeResolver());

            var rawBirth =
                Birth(
                    new DateOnly(2000, 1, 15),
                    new TimeOnly(12, 0),
                    BirthTimePrecision.Exact);

            var resolution =
                time.Execute(
                    rawBirth.LocalDate,
                    rawBirth.LocalTime!.Value,
                    rawBirth.IanaTimeZoneId);

            var resolved =
                BirthHistoricalTimeSnapshotMapper.Apply(
                    rawBirth,
                    resolution.Resolution);

            var id =
                await new CreatePersonUseCase(store)
                    .ExecuteAsync(
                        Command(resolved),
                        Now);

            var loaded =
                await new GetPersonUseCase(store)
                    .ExecuteAsync(id);

            Assert.IsNotNull(loaded);
            Assert.IsNotNull(
                loaded.BirthData);

            Assert.AreEqual(
                BirthTemporalResolutionState.Resolved,
                loaded.BirthData.ResolutionState);

            Assert.IsNotNull(
                loaded.BirthData.ResolvedInstantUtc);

            Assert.IsFalse(
                string.IsNullOrWhiteSpace(
                    loaded.BirthData.TzdbVersion));
        }
        finally
        {
            DeleteDatabase(path);
        }
    }

    [TestMethod]
    public async Task Ambiguous_birth_choice_roundtrips()
    {
        var path = TempDatabase();

        try
        {
            await using var db =
                CreateContext(path);

            await db.Database.MigrateAsync();

            var store =
                new EfPersonStore(db);

            var time =
                new ResolveBirthHistoricalTimeUseCase(
                    new NodaTimeHistoricalTimeResolver());

            var rawBirth =
                Birth(
                    new DateOnly(2025, 10, 26),
                    new TimeOnly(2, 30),
                    BirthTimePrecision.Exact);

            var resolution =
                time.Execute(
                    rawBirth.LocalDate,
                    rawBirth.LocalTime!.Value,
                    rawBirth.IanaTimeZoneId);

            Assert.AreEqual(
                Miastro.Application.Time
                    .HistoricalTimeResolutionStatus.Ambiguous,
                resolution.Resolution.Status);

            Assert.ThrowsExactly<
                InvalidOperationException>(
                () =>
                    BirthHistoricalTimeSnapshotMapper.Apply(
                        rawBirth,
                        resolution.Resolution));

            var selected =
                BirthHistoricalTimeSnapshotMapper.Apply(
                    rawBirth,
                    resolution.Resolution,
                    selectedCandidate: 2,
                    selectionRecordedAtUtc: Now);

            var id =
                await new CreatePersonUseCase(store)
                    .ExecuteAsync(
                        Command(selected),
                        Now);

            var loaded =
                await new GetPersonUseCase(store)
                    .ExecuteAsync(id);

            Assert.IsNotNull(
                loaded?.BirthData);

            Assert.AreEqual(
                BirthTemporalResolutionState.Ambiguous,
                loaded.BirthData.ResolutionState);

            Assert.AreEqual(
                2,
                loaded.BirthData
                    .AmbiguousSelectedCandidate);

            Assert.AreEqual(
                Now,
                loaded.BirthData
                    .AmbiguousSelectionRecordedAtUtc);

            Assert.IsNotNull(
                loaded.BirthData.ResolvedInstantUtc);
        }
        finally
        {
            DeleteDatabase(path);
        }
    }

    [TestMethod]
    public async Task Unknown_birth_time_roundtrips_without_instant()
    {
        var path = TempDatabase();

        try
        {
            await using var db =
                CreateContext(path);

            await db.Database.MigrateAsync();

            var store =
                new EfPersonStore(db);

            var birth =
                new BirthDataWriteModel(
                    new DateOnly(2000, 1, 15),
                    BirthTimePrecision.Unknown,
                    null,
                    null,
                    null,
                    null,
                    1,
                    "Synthetic City",
                    "Synthetic Country",
                    "Synthetic Region",
                    null,
                    40,
                    -3,
                    "Europe/Madrid",
                    null,
                    BirthTemporalResolutionState.NotApplicable,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    false,
                    null,
                    null);

            var id =
                await new CreatePersonUseCase(store)
                    .ExecuteAsync(
                        Command(birth),
                        Now);

            var loaded =
                await new GetPersonUseCase(store)
                    .ExecuteAsync(id);

            Assert.IsNotNull(
                loaded?.BirthData);

            Assert.AreEqual(
                BirthTimePrecision.Unknown,
                loaded.BirthData.TimePrecision);

            Assert.IsNull(
                loaded.BirthData.LocalTime);

            Assert.IsNull(
                loaded.BirthData.ResolvedInstantUtc);
        }
        finally
        {
            DeleteDatabase(path);
        }
    }

    [TestMethod]
    public void Changing_natal_data_invalidates_old_resolution()
    {
        var rawBirth =
            Birth(
                new DateOnly(2000, 1, 15),
                new TimeOnly(12, 0),
                BirthTimePrecision.Exact);

        var time =
            new ResolveBirthHistoricalTimeUseCase(
                new NodaTimeHistoricalTimeResolver());

        var resolution =
            time.Execute(
                rawBirth.LocalDate,
                rawBirth.LocalTime!.Value,
                rawBirth.IanaTimeZoneId);

        var resolved =
            BirthHistoricalTimeSnapshotMapper.Apply(
                rawBirth,
                resolution.Resolution);

        Assert.IsNotNull(
            resolved.ResolvedInstantUtc);

        var changed =
            resolved with
            {
                LocalTime =
                    new TimeOnly(13, 0)
            };

        var invalidated =
            BirthHistoricalTimeSnapshotMapper
                .Invalidate(changed);

        Assert.AreEqual(
            BirthTemporalResolutionState.Pending,
            invalidated.ResolutionState);

        Assert.IsNull(
            invalidated.ResolvedInstantUtc);

        Assert.IsNull(
            invalidated.TzdbVersion);
    }

    private static BirthDataWriteModel Birth(
        DateOnly date,
        TimeOnly time,
        BirthTimePrecision precision)
        => new(
            date,
            precision,
            time,
            null,
            null,
            null,
            1,
            "Synthetic City",
            "Synthetic Country",
            "Synthetic Region",
            null,
            40,
            -3,
            "Europe/Madrid",
            null,
            BirthTemporalResolutionState.Pending,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            false,
            null,
            null);

    private static CreatePersonCommand Command(
        BirthDataWriteModel birth)
        => new(
            "Synthetic",
            "Person",
            null,
            null,
            null,
            false,
            birth,
            null);

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

    private static string TempDatabase()
        => Path.Combine(
            Path.GetTempPath(),
            $"miastro-phase5-e2e-{Guid.NewGuid():N}.sqlite");

    private static void DeleteDatabase(
        string path)
    {
        foreach (var file in new[]
        {
            path,
            path + "-wal",
            path + "-shm"
        })
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Miastro.Application.Natal;
using Miastro.Application.People;
using Miastro.Bootstrap;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.Domain.People;
using Miastro.Infrastructure.Persistence;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6NatalInvalidationRecalculationE2ETests
{
    [TestMethod]
    public async Task Birth_change_invalidates_old_chart_and_recalculation_creates_new_current()
    {
        var root =
            Path.Combine(
                Path.GetTempPath(),
                "miastro-phase6-recalc-e2e",
                Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(root);

        var previous =
            new Dictionary<string, string?>();

        try
        {
            SetXdg(
                previous,
                "XDG_DATA_HOME",
                Path.Combine(root, "data"));

            SetXdg(
                previous,
                "XDG_CONFIG_HOME",
                Path.Combine(root, "config"));

            SetXdg(
                previous,
                "XDG_CACHE_HOME",
                Path.Combine(root, "cache"));

            SetXdg(
                previous,
                "XDG_STATE_HOME",
                Path.Combine(root, "state"));

            var services =
                MiastroBootstrap
                    .CreateServiceCollection();

            await using var provider =
                services.BuildServiceProvider(
                    new ServiceProviderOptions
                    {
                        ValidateOnBuild = true,
                        ValidateScopes = true
                    });

            await using var scope =
                provider.CreateAsyncScope();

            var db =
                scope.ServiceProvider
                    .GetRequiredService<
                        MiastroDbContext>();

            await db.Database.MigrateAsync();

            var create =
                scope.ServiceProvider
                    .GetRequiredService<
                        CreatePersonUseCase>();

            var personId =
                await create.ExecuteAsync(
                    new CreatePersonCommand(
                        FirstName:
                            "Persona",
                        LastName:
                            "RecalculoE2E",
                        Phone:
                            null,
                        Email:
                            null,
                        PrivateNote:
                            null,
                        IsFavorite:
                            false,
                        BirthData:
                            Birth(
                                new TimeOnly(
                                    12, 0),
                                new DateTimeOffset(
                                    2000, 1, 1,
                                    11, 0, 0,
                                    TimeSpan.Zero)),
                        CurrentResidence:
                            null),
                    new DateTimeOffset(
                        2026, 8, 21,
                        10, 0, 0,
                        TimeSpan.Zero));

            var calculate =
                scope.ServiceProvider
                    .GetRequiredService<
                        CalculateNatalChartUseCase>();

            var first =
                await calculate.ExecuteAsync(
                    personId,
                    HouseSystem.Placidus,
                    new DateTimeOffset(
                        2026, 8, 21,
                        10, 1, 0,
                        TimeSpan.Zero));

            Assert.IsTrue(
                first.Success,
                first.Message);

            Assert.AreEqual(
                NatalCalculationResultCode.Calculated,
                first.Code);

            Assert.IsNotNull(
                first.Snapshot);

            var firstId =
                first.Snapshot.Id;

            var firstHash =
                first.Snapshot.InputHash;

            var personStore =
                scope.ServiceProvider
                    .GetRequiredService<
                        IPersonStore>();

            await personStore.UpdateAsync(
                new UpdatePersonCommand(
                    Id:
                        personId,
                    FirstName:
                        "Persona",
                    LastName:
                        "RecalculoE2E",
                    Phone:
                        null,
                    Email:
                        null,
                    PrivateNote:
                        null,
                    IsFavorite:
                        false,
                    BirthData:
                        Birth(
                            new TimeOnly(
                                12, 15),
                            new DateTimeOffset(
                                2000, 1, 1,
                                11, 15, 0,
                                TimeSpan.Zero)),
                    CurrentResidence:
                        null),
                new DateTimeOffset(
                    2026, 8, 21,
                    10, 2, 0,
                    TimeSpan.Zero));

            db.ChangeTracker.Clear();

            var invalidated =
                await db.NatalCharts
                    .AsNoTracking()
                    .SingleAsync(x =>
                        x.Id == firstId);

            Assert.AreEqual(
                (int)NatalChartStatus.Invalidated,
                invalidated.Status);

            Assert.IsNotNull(
                invalidated.InvalidatedAtUtc);

            var store =
                scope.ServiceProvider
                    .GetRequiredService<
                        INatalChartStore>();

            Assert.IsNull(
                await store.GetCurrentAsync(
                    personId));

            var recalculate =
                scope.ServiceProvider
                    .GetRequiredService<
                        RecalculateNatalChartUseCase>();

            var second =
                await recalculate.ExecuteAsync(
                    personId,
                    HouseSystem.Placidus,
                    new DateTimeOffset(
                        2026, 8, 21,
                        10, 3, 0,
                        TimeSpan.Zero));

            Assert.IsTrue(
                second.Success,
                second.Message);

            Assert.AreEqual(
                NatalCalculationResultCode.Calculated,
                second.Code);

            Assert.IsNotNull(
                second.Snapshot);

            Assert.AreNotEqual(
                firstId,
                second.Snapshot.Id);

            Assert.AreNotEqual(
                firstHash,
                second.Snapshot.InputHash);

            db.ChangeTracker.Clear();

            var charts =
                await db.NatalCharts
                    .AsNoTracking()
                    .Where(x =>
                        x.PersonId == personId)
                    .ToListAsync();

            Assert.AreEqual(
                2,
                charts.Count);

            Assert.AreEqual(
                1,
                charts.Count(x =>
                    x.Status ==
                        (int)NatalChartStatus.Current));

            Assert.AreEqual(
                1,
                charts.Count(x =>
                    x.Status ==
                        (int)NatalChartStatus.Invalidated));

            var current =
                charts.Single(x =>
                    x.Status ==
                        (int)NatalChartStatus.Current);

            Assert.AreEqual(
                second.Snapshot.Id,
                current.Id);

            Assert.AreEqual(
                second.Snapshot.InputHash,
                current.InputHash);

            var history =
                await db.PersonHistory
                    .AsNoTracking()
                    .Where(x =>
                        x.PersonId == personId)
                    .ToListAsync();

            Assert.IsTrue(
                history.Any(x =>
                    x.EventType ==
                        (int)PersonHistoryEventType
                            .NatalChartCalculated));

            Assert.IsTrue(
                history.Any(x =>
                    x.EventType ==
                        (int)PersonHistoryEventType
                            .NatalChartInvalidated));

            Assert.IsTrue(
                history.Any(x =>
                    x.EventType ==
                        (int)PersonHistoryEventType
                            .NatalChartRecalculated));

            Assert.IsFalse(
                history.Any(x =>
                    x.Summary.Contains(
                        "RecalculoE2E",
                        StringComparison.OrdinalIgnoreCase)));

            Assert.IsFalse(
                history.Any(x =>
                    x.Summary.Contains(
                        "12:15",
                        StringComparison.OrdinalIgnoreCase)));

            Assert.IsFalse(
                history.Any(x =>
                    x.Summary.Contains(
                        "Madrid",
                        StringComparison.OrdinalIgnoreCase)));

            db.ChangeTracker.Clear();

            var loadedCurrent =
                await store.GetCurrentAsync(
                    personId);

            Assert.IsNotNull(
                loadedCurrent);

            Assert.AreEqual(
                second.Snapshot.Id,
                loadedCurrent.Id);

            Assert.AreEqual(
                21,
                loadedCurrent.Placements.Count);

            Assert.AreEqual(
                12,
                loadedCurrent.HouseCusps.Count);
        }
        finally
        {
            foreach (var item in previous)
            {
                Environment.SetEnvironmentVariable(
                    item.Key,
                    item.Value);
            }

            try
            {
                Directory.Delete(
                    root,
                    recursive: true);
            }
            catch
            {
            }
        }
    }

    private static BirthDataWriteModel Birth(
        TimeOnly localTime,
        DateTimeOffset instantUtc)
        => new(
            LocalDate:
                new DateOnly(
                    2000, 1, 1),
            TimePrecision:
                BirthTimePrecision.Exact,
            LocalTime:
                localTime,
            RangeStart:
                null,
            RangeEnd:
                null,
            DayPeriod:
                null,
            GeoNameId:
                3117735,
            Locality:
                "Madrid",
            Country:
                "España",
            Region:
                "Madrid",
            Subregion:
                null,
            Latitude:
                40.4168,
            Longitude:
                -3.7038,
            IanaTimeZoneId:
                "Europe/Madrid",
            TzdbVersion:
                "TZDB: 2026c",
            ResolutionState:
                BirthTemporalResolutionState.Resolved,
            HistoricalOffsetSeconds:
                3600,
            ResolvedInstantUtc:
                instantUtc,
            AmbiguousEarlierOffsetSeconds:
                null,
            AmbiguousEarlierInstantUtc:
                null,
            AmbiguousLaterOffsetSeconds:
                null,
            AmbiguousLaterInstantUtc:
                null,
            AmbiguousSelectedCandidate:
                null,
            AmbiguousSelectionRecordedAtUtc:
                null,
            ManualCoordinateOverride:
                false,
            OriginalGeoNamesLatitude:
                null,
            OriginalGeoNamesLongitude:
                null);

    private static void SetXdg(
        IDictionary<string, string?> previous,
        string name,
        string value)
    {
        previous[name] =
            Environment.GetEnvironmentVariable(
                name);

        Directory.CreateDirectory(
            value);

        Environment.SetEnvironmentVariable(
            name,
            value);
    }
}

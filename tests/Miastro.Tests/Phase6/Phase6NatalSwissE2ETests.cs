using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Miastro.Application.Natal;
using Miastro.Application.People;
using Miastro.Bootstrap;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;
using Miastro.Domain.People;
using Miastro.Infrastructure.Persistence;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6NatalSwissE2ETests
{
    [TestMethod]
    public async Task Exact_person_calculates_persists_and_reopens_real_natal_chart()
    {
        var root =
            Path.Combine(
                Path.GetTempPath(),
                "miastro-phase6-natal-e2e",
                Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(root);

        try
        {
            var dataHome =
                Path.Combine(root, "data");

            var configHome =
                Path.Combine(root, "config");

            var cacheHome =
                Path.Combine(root, "cache");

            var stateHome =
                Path.Combine(root, "state");

            Directory.CreateDirectory(dataHome);
            Directory.CreateDirectory(configHome);
            Directory.CreateDirectory(cacheHome);
            Directory.CreateDirectory(stateHome);

            using var environment =
                new XdgEnvironmentScope(
                    dataHome,
                    configHome,
                    cacheHome,
                    stateHome);

            Guid personId;
            Guid chartId;
            string inputHash;

            await using (
                var provider =
                    BuildProvider())
            {
                await using var scope =
                    provider.CreateAsyncScope();

                var db =
                    scope.ServiceProvider
                        .GetRequiredService<
                            MiastroDbContext>();

                await db.Database.MigrateAsync();

                var createPerson =
                    scope.ServiceProvider
                        .GetRequiredService<
                            CreatePersonUseCase>();

                personId =
                    await createPerson.ExecuteAsync(
                        new CreatePersonCommand(
                            FirstName:
                                "Persona",
                            LastName:
                                "NatalE2E",
                            Phone:
                                null,
                            Email:
                                null,
                            PrivateNote:
                                null,
                            IsFavorite:
                                false,
                            BirthData:
                                new BirthDataWriteModel(
                                    LocalDate:
                                        new DateOnly(
                                            2000, 1, 1),
                                    TimePrecision:
                                        BirthTimePrecision.Exact,
                                    LocalTime:
                                        new TimeOnly(
                                            12, 0),
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
                                        new DateTimeOffset(
                                            2000, 1, 1,
                                            11, 0, 0,
                                            TimeSpan.Zero),
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
                                        null),
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

                var result =
                    await calculate.ExecuteAsync(
                        personId,
                        HouseSystem.Placidus,
                        new DateTimeOffset(
                            2026, 8, 21,
                            10, 5, 0,
                            TimeSpan.Zero));

                Assert.IsTrue(
                    result.Success,
                    result.Message);

                Assert.AreEqual(
                    NatalCalculationResultCode.Calculated,
                    result.Code);

                Assert.IsNotNull(
                    result.Snapshot);

                Assert.IsNotNull(
                    result.Chart);

                Assert.AreEqual(
                    21,
                    result.Snapshot.Placements.Count);

                Assert.AreEqual(
                    12,
                    result.Snapshot.HouseCusps.Count);

                Assert.IsTrue(
                    result.Snapshot.Aspects.Count > 0);

                Assert.AreEqual(
                    HouseSystem.Placidus,
                    result.Snapshot.HouseSystem);

                Assert.AreEqual(
                    "miastro-v1",
                    result.Snapshot.CalculationProfileId);

                Assert.AreEqual(
                    "Europe/Madrid",
                    result.Snapshot.IanaTimeZoneId);

                Assert.AreEqual(
                    "TZDB: 2026c",
                    result.Snapshot.TzdbVersion);

                Assert.IsFalse(
                    result.Snapshot.IsApproximateBirthTime);

                Assert.IsTrue(
                    result.Snapshot
                        .InputHash.Length == 64);

                Assert.IsTrue(
                    result.Snapshot
                        .Placements
                        .Any(x =>
                            x.ObjectId ==
                            AstrologicalObjectId.Sun));

                Assert.IsTrue(
                    result.Snapshot
                        .Placements
                        .Any(x =>
                            x.ObjectId ==
                            AstrologicalObjectId.Moon));

                Assert.IsTrue(
                    result.Snapshot
                        .Placements
                        .Any(x =>
                            x.ObjectId ==
                            AstrologicalObjectId.NorthTrueNode));

                Assert.IsTrue(
                    result.Snapshot
                        .Placements
                        .Any(x =>
                            x.ObjectId ==
                            AstrologicalObjectId.SouthNode));

                Assert.IsTrue(
                    result.Snapshot
                        .Placements
                        .Any(x =>
                            x.ObjectId ==
                            AstrologicalObjectId.MeanLilith));

                Assert.IsTrue(
                    result.Snapshot
                        .Placements
                        .Any(x =>
                            x.ObjectId ==
                            AstrologicalObjectId.Chiron));

                Assert.IsTrue(
                    result.Snapshot
                        .Placements
                        .Any(x =>
                            x.ObjectId ==
                            AstrologicalObjectId.Ceres));

                Assert.IsTrue(
                    result.Snapshot
                        .Placements
                        .Any(x =>
                            x.ObjectId ==
                            AstrologicalObjectId.Pallas));

                Assert.IsTrue(
                    result.Snapshot
                        .Placements
                        .Any(x =>
                            x.ObjectId ==
                            AstrologicalObjectId.Juno));

                Assert.IsTrue(
                    result.Snapshot
                        .Placements
                        .Any(x =>
                            x.ObjectId ==
                            AstrologicalObjectId.Vesta));

                Assert.IsTrue(
                    result.Snapshot
                        .Placements
                        .Any(x =>
                            x.ObjectId ==
                            AstrologicalObjectId.Ascendant));

                Assert.IsTrue(
                    result.Snapshot
                        .Placements
                        .Any(x =>
                            x.ObjectId ==
                            AstrologicalObjectId.Midheaven));

                Assert.IsTrue(
                    result.Snapshot
                        .Placements
                        .Any(x =>
                            x.ObjectId ==
                            AstrologicalObjectId.PartOfFortune));

                var north =
                    result.Snapshot
                        .Placements
                        .Single(x =>
                            x.ObjectId ==
                            AstrologicalObjectId.NorthTrueNode);

                var south =
                    result.Snapshot
                        .Placements
                        .Single(x =>
                            x.ObjectId ==
                            AstrologicalObjectId.SouthNode);

                var expectedSouth =
                    (north.LongitudeDegrees + 180.0)
                    % 360.0;

                Assert.AreEqual(
                    expectedSouth,
                    south.LongitudeDegrees,
                    1e-9);

                chartId =
                    result.Snapshot.Id;

                inputHash =
                    result.Snapshot.InputHash;
            }

            await using (
                var reopened =
                    BuildProvider())
            {
                await using var scope =
                    reopened.CreateAsyncScope();

                var db =
                    scope.ServiceProvider
                        .GetRequiredService<
                            MiastroDbContext>();

                await db.Database.MigrateAsync();

                var natalStore =
                    scope.ServiceProvider
                        .GetRequiredService<
                            INatalChartStore>();

                var current =
                    await natalStore
                        .GetCurrentAsync(
                            personId);

                Assert.IsNotNull(current);

                Assert.AreEqual(
                    chartId,
                    current.Id);

                Assert.AreEqual(
                    inputHash,
                    current.InputHash);

                Assert.AreEqual(
                    NatalChartStatus.Current,
                    current.Status);

                Assert.AreEqual(
                    21,
                    current.Placements.Count);

                Assert.AreEqual(
                    12,
                    current.HouseCusps.Count);

                Assert.IsTrue(
                    current.Aspects.Count > 0);

                Assert.IsTrue(
                    current.Placements
                        .All(x =>
                            double.IsFinite(
                                x.LongitudeDegrees)));

                Assert.IsTrue(
                    current.HouseCusps
                        .All(x =>
                            double.IsFinite(
                                x.LongitudeDegrees)));
            }
        }
        finally
        {
            try
            {
                Directory.Delete(
                    root,
                    recursive: true);
            }
            catch
            {
                // El test no debe ocultar el resultado funcional
                // por limpieza best-effort.
            }
        }
    }

    [TestMethod]
    public async Task Approximate_person_calculates_real_chart_and_keeps_semantics()
    {
        var root =
            Path.Combine(
                Path.GetTempPath(),
                "miastro-phase6-natal-approx",
                Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(root);

        try
        {
            using var environment =
                new XdgEnvironmentScope(
                    Path.Combine(root, "data"),
                    Path.Combine(root, "config"),
                    Path.Combine(root, "cache"),
                    Path.Combine(root, "state"));

            await using var provider =
                BuildProvider();

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
                        "Persona",
                        "AproximadaE2E",
                        null,
                        null,
                        null,
                        false,
                        new BirthDataWriteModel(
                            new DateOnly(
                                1990, 6, 15),
                            BirthTimePrecision.Approximate,
                            new TimeOnly(
                                8, 30),
                            null,
                            null,
                            null,
                            3117735,
                            "Madrid",
                            "España",
                            "Madrid",
                            null,
                            40.4168,
                            -3.7038,
                            "Europe/Madrid",
                            "TZDB: 2026c",
                            BirthTemporalResolutionState.Resolved,
                            7200,
                            new DateTimeOffset(
                                1990, 6, 15,
                                6, 30, 0,
                                TimeSpan.Zero),
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            false,
                            null,
                            null),
                        null),
                    new DateTimeOffset(
                        2026, 8, 21,
                        10, 0, 0,
                        TimeSpan.Zero));

            var calculate =
                scope.ServiceProvider
                    .GetRequiredService<
                        CalculateNatalChartUseCase>();

            var result =
                await calculate.ExecuteAsync(
                    personId,
                    HouseSystem.Koch,
                    new DateTimeOffset(
                        2026, 8, 21,
                        10, 1, 0,
                        TimeSpan.Zero));

            Assert.IsTrue(
                result.Success,
                result.Message);

            Assert.IsNotNull(
                result.Snapshot);

            Assert.IsTrue(
                result.Snapshot
                    .IsApproximateBirthTime);

            Assert.AreEqual(
                HouseSystem.Koch,
                result.Snapshot.HouseSystem);

            Assert.AreEqual(
                21,
                result.Snapshot.Placements.Count);
        }
        finally
        {
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

    private static ServiceProvider BuildProvider()
    {
        var services =
            MiastroBootstrap
                .CreateServiceCollection();

        return services
            .BuildServiceProvider(
                new ServiceProviderOptions
                {
                    ValidateScopes = true,
                    ValidateOnBuild = true
                });
    }

    private sealed class XdgEnvironmentScope
        : IDisposable
    {
        private readonly Dictionary<
            string,
            string?> _previous = [];

        public XdgEnvironmentScope(
            string dataHome,
            string configHome,
            string cacheHome,
            string stateHome)
        {
            Directory.CreateDirectory(
                dataHome);

            Directory.CreateDirectory(
                configHome);

            Directory.CreateDirectory(
                cacheHome);

            Directory.CreateDirectory(
                stateHome);

            Set(
                "XDG_DATA_HOME",
                dataHome);

            Set(
                "XDG_CONFIG_HOME",
                configHome);

            Set(
                "XDG_CACHE_HOME",
                cacheHome);

            Set(
                "XDG_STATE_HOME",
                stateHome);
        }

        public void Dispose()
        {
            foreach (var item in _previous)
            {
                Environment.SetEnvironmentVariable(
                    item.Key,
                    item.Value);
            }
        }

        private void Set(
            string name,
            string value)
        {
            _previous[name] =
                Environment
                    .GetEnvironmentVariable(
                        name);

            Environment.SetEnvironmentVariable(
                name,
                value);
        }
    }
}

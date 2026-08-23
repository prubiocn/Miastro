using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Miastro.Astronomy.Abstractions.Contracts;
using Miastro.Astronomy.Abstractions.Models;
using Miastro.Application.Natal;
using Miastro.Application.People;
using Miastro.Bootstrap;
using Miastro.Domain.Angles;
using Miastro.Domain.Calculation;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;
using Miastro.Domain.People;
using Miastro.Domain.Placements;
using Miastro.Infrastructure.Persistence;

namespace Miastro.Tests.Phase6;

[TestClass]
[DoNotParallelize]
public sealed class Phase6NatalPerformanceBaselineTests
{
    private static readonly AstrologicalObjectId[] SwissObjects =
    [
        AstrologicalObjectId.Sun,
        AstrologicalObjectId.Moon,
        AstrologicalObjectId.Mercury,
        AstrologicalObjectId.Venus,
        AstrologicalObjectId.Mars,
        AstrologicalObjectId.Jupiter,
        AstrologicalObjectId.Saturn,
        AstrologicalObjectId.Uranus,
        AstrologicalObjectId.Neptune,
        AstrologicalObjectId.Pluto,
        AstrologicalObjectId.NorthTrueNode,
        AstrologicalObjectId.MeanLilith,
        AstrologicalObjectId.Chiron,
        AstrologicalObjectId.Ceres,
        AstrologicalObjectId.Pallas,
        AstrologicalObjectId.Juno,
        AstrologicalObjectId.Vesta
    ];

    [TestMethod]
    public async Task Real_natal_calculation_emits_non_gating_performance_baseline()
    {
        var root =
            Path.Combine(
                Path.GetTempPath(),
                "miastro-phase6-performance",
                Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(root);

        var previous =
            CaptureEnvironment();

        try
        {
            ConfigureXdg(root);

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

            await db.Database
                .MigrateAsync();

            var create =
                scope.ServiceProvider
                    .GetRequiredService<
                        CreatePersonUseCase>();

            var personId =
                await create.ExecuteAsync(
                    new CreatePersonCommand(
                        FirstName:
                            "Performance",
                        LastName:
                            "Baseline",
                        Phone:
                            null,
                        Email:
                            null,
                        PrivateNote:
                            null,
                        IsFavorite:
                            false,
                        BirthData:
                            Birth(),
                        CurrentResidence:
                            null),
                    FixedNow);

            db.ChangeTracker.Clear();

            var personStore =
                scope.ServiceProvider
                    .GetRequiredService<
                        IPersonStore>();

            var natalStore =
                scope.ServiceProvider
                    .GetRequiredService<
                        INatalChartStore>();

            var positions =
                scope.ServiceProvider
                    .GetRequiredService<
                        IEclipticPositionCalculator>();

            var houses =
                scope.ServiceProvider
                    .GetRequiredService<
                        IHouseCalculator>();

            var metadata =
                scope.ServiceProvider
                    .GetRequiredService<
                        INatalCalculationMetadataProvider>();

            // Warm-up mínimo: carga del motor/ABI y metadatos fuera
            // de la medición principal. No se persiste ninguna carta.
            _ = metadata.Get();

            var instant =
                AstronomicalInstant.FromUtc(
                    BirthInstant);

            var location =
                new GeographicLocation(
                    40.4168,
                    -3.7038);

            _ = positions.Calculate(
                AstrologicalObjectId.Sun,
                instant,
                CalculationProfile.MiastroV1);

            _ = houses.Calculate(
                instant,
                location,
                HouseSystem.Placidus);

            // ----------------------------------------------------
            // Persona
            // ----------------------------------------------------

            var timer =
                Stopwatch.StartNew();

            var loadedPerson =
                await personStore
                    .GetAsync(
                        personId);

            timer.Stop();

            var personLoadMs =
                timer.Elapsed.TotalMilliseconds;

            Assert.IsNotNull(
                loadedPerson);

            // ----------------------------------------------------
            // 17 cuerpos Swiss
            // ----------------------------------------------------

            var raw =
                new Dictionary<
                    AstrologicalObjectId,
                    EclipticPosition>();

            timer.Restart();

            foreach (var objectId in SwissObjects)
            {
                raw[objectId] =
                    positions.Calculate(
                        objectId,
                        instant,
                        CalculationProfile.MiastroV1);
            }

            timer.Stop();

            var swiss17Ms =
                timer.Elapsed.TotalMilliseconds;

            Assert.AreEqual(
                17,
                raw.Count);

            // ----------------------------------------------------
            // Casas
            // ----------------------------------------------------

            timer.Restart();

            var houseResult =
                houses.Calculate(
                    instant,
                    location,
                    HouseSystem.Placidus);

            timer.Stop();

            var housesMs =
                timer.Elapsed.TotalMilliseconds;

            Assert.IsTrue(
                houseResult.Success);

            Assert.AreEqual(
                12,
                houseResult.Cusps.Count);

            // ----------------------------------------------------
            // Derivados + ocupación + aspectos
            // Medición aislada sin persistencia.
            // ----------------------------------------------------

            timer.Restart();

            var placements =
                new List<AstrologicalPlacement>();

            foreach (var objectId in SwissObjects)
            {
                var position =
                    raw[objectId];

                placements.Add(
                    new AstrologicalPlacement(
                        objectId,
                        position.Longitude,
                        NatalHousePlacementResolver.Resolve(
                            position.Longitude,
                            houseResult.Cusps),
                        position.LongitudeSpeedDegreesPerDay));
            }

            var north =
                raw[
                    AstrologicalObjectId
                        .NorthTrueNode];

            var south =
                Miastro.Domain.DerivedPoints
                    .LunarNodeCalculator
                    .CalculateSouthNode(
                        north.Longitude);

            placements.Add(
                new AstrologicalPlacement(
                    AstrologicalObjectId.SouthNode,
                    south,
                    NatalHousePlacementResolver.Resolve(
                        south,
                        houseResult.Cusps),
                    north.LongitudeSpeedDegreesPerDay));

            var asc =
                houseResult.Ascendant!.Value;

            var mc =
                houseResult.Midheaven!.Value;

            placements.Add(
                new AstrologicalPlacement(
                    AstrologicalObjectId.Ascendant,
                    asc,
                    NatalHousePlacementResolver.Resolve(
                        asc,
                        houseResult.Cusps)));

            placements.Add(
                new AstrologicalPlacement(
                    AstrologicalObjectId.Midheaven,
                    mc,
                    NatalHousePlacementResolver.Resolve(
                        mc,
                        houseResult.Cusps)));

            var sect =
                NatalChartSectResolver.Resolve(
                    raw[AstrologicalObjectId.Sun]
                        .Longitude,
                    houseResult.Cusps);

            var fortune =
                Miastro.Domain.DerivedPoints
                    .PartOfFortuneCalculator
                    .Calculate(
                        asc,
                        raw[AstrologicalObjectId.Sun]
                            .Longitude,
                        raw[AstrologicalObjectId.Moon]
                            .Longitude,
                        sect);

            placements.Add(
                new AstrologicalPlacement(
                    AstrologicalObjectId.PartOfFortune,
                    fortune,
                    NatalHousePlacementResolver.Resolve(
                        fortune,
                        houseResult.Cusps)));

            var ordered =
                placements
                    .OrderBy(x =>
                        NatalObjectOrder.GetIndex(
                            x.ObjectId))
                    .ToArray();

            var aspects =
                NatalAspectCalculator.Calculate(
                    ordered);

            timer.Stop();

            var derivedAndAspectsMs =
                timer.Elapsed.TotalMilliseconds;

            Assert.AreEqual(
                21,
                ordered.Length);

            Assert.IsTrue(
                aspects.Count > 0);

            // ----------------------------------------------------
            // Cálculo completo real + escritura SQLite
            // ----------------------------------------------------

            var calculate =
                scope.ServiceProvider
                    .GetRequiredService<
                        CalculateNatalChartUseCase>();

            timer.Restart();

            var result =
                await calculate.ExecuteAsync(
                    personId,
                    HouseSystem.Placidus,
                    FixedNow.AddMinutes(1));

            timer.Stop();

            var fullCalculationMs =
                timer.Elapsed.TotalMilliseconds;

            Assert.IsTrue(
                result.Success,
                result.Message);

            Assert.AreEqual(
                NatalCalculationResultCode.Calculated,
                result.Code);

            Assert.IsNotNull(
                result.Snapshot);

            Assert.AreEqual(
                21,
                result.Snapshot.Placements.Count);

            Assert.AreEqual(
                12,
                result.Snapshot.HouseCusps.Count);

            Assert.IsTrue(
                result.Snapshot.Aspects.Count > 0);

            // ----------------------------------------------------
            // Lectura persistida del snapshot completo
            // ----------------------------------------------------

            db.ChangeTracker.Clear();

            timer.Restart();

            var persisted =
                await natalStore
                    .GetCurrentAsync(
                        personId);

            timer.Stop();

            var snapshotReloadMs =
                timer.Elapsed.TotalMilliseconds;

            Assert.IsNotNull(
                persisted);

            Assert.AreEqual(
                result.Snapshot.Id,
                persisted.Id);

            Assert.AreEqual(
                21,
                persisted.Placements.Count);

            Assert.AreEqual(
                12,
                persisted.HouseCusps.Count);

            // ----------------------------------------------------
            // Idempotencia: misma entrada debe evitar Swiss/rewrite.
            // ----------------------------------------------------

            timer.Restart();

            var existing =
                await calculate.ExecuteAsync(
                    personId,
                    HouseSystem.Placidus,
                    FixedNow.AddMinutes(2));

            timer.Stop();

            var existingSnapshotMs =
                timer.Elapsed.TotalMilliseconds;

            Assert.AreEqual(
                NatalCalculationResultCode
                    .ExistingCurrentSnapshot,
                existing.Code);

            Assert.IsNotNull(
                existing.Snapshot);

            Assert.AreEqual(
                result.Snapshot.Id,
                existing.Snapshot.Id);

            // ----------------------------------------------------
            // Baseline informativa. NO es gate temporal.
            // ----------------------------------------------------

            Console.WriteLine(
                "=== MIASTRO PHASE6 PERFORMANCE BASELINE ===");

            Console.WriteLine(
                $"PersonLoadMs={personLoadMs:F3}");

            Console.WriteLine(
                $"Swiss17ObjectsMs={swiss17Ms:F3}");

            Console.WriteLine(
                $"HouseCalculationMs={housesMs:F3}");

            Console.WriteLine(
                $"DerivedAndAspectsMs={derivedAndAspectsMs:F3}");

            Console.WriteLine(
                $"FullNatalCalculationAndPersistenceMs={fullCalculationMs:F3}");

            Console.WriteLine(
                $"PersistedSnapshotReloadMs={snapshotReloadMs:F3}");

            Console.WriteLine(
                $"ExistingSnapshotFastPathMs={existingSnapshotMs:F3}");

            Console.WriteLine(
                "SwissObjectsMeasured=17");

            Console.WriteLine(
                "CanonicalPlacementsMeasured=21");

            Console.WriteLine(
                $"AspectsMeasured={aspects.Count}");

            Console.WriteLine(
                "PerformanceThresholdApplied=NO");

            Console.WriteLine(
                "PerformanceBaselineFunctionalGate=PASS");
        }
        finally
        {
            RestoreEnvironment(
                previous);

            try
            {
                Directory.Delete(
                    root,
                    recursive: true);
            }
            catch
            {
                // Limpieza best-effort.
            }
        }
    }

    private static BirthDataWriteModel Birth()
        => new(
            LocalDate:
                new DateOnly(
                    2000,
                    1,
                    1),

            TimePrecision:
                BirthTimePrecision.Exact,

            LocalTime:
                new TimeOnly(
                    12,
                    0),

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
                BirthInstant,

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

    private static void ConfigureXdg(
        string root)
    {
        foreach (
            var item
            in new Dictionary<string, string>
            {
                ["XDG_DATA_HOME"] =
                    Path.Combine(
                        root,
                        "data"),

                ["XDG_CONFIG_HOME"] =
                    Path.Combine(
                        root,
                        "config"),

                ["XDG_CACHE_HOME"] =
                    Path.Combine(
                        root,
                        "cache"),

                ["XDG_STATE_HOME"] =
                    Path.Combine(
                        root,
                        "state")
            })
        {
            Directory.CreateDirectory(
                item.Value);

            Environment.SetEnvironmentVariable(
                item.Key,
                item.Value);
        }
    }

    private static Dictionary<string, string?>
        CaptureEnvironment()
    {
        var result =
            new Dictionary<
                string,
                string?>();

        foreach (
            var name
            in EnvironmentVariables)
        {
            result[name] =
                Environment
                    .GetEnvironmentVariable(
                        name);
        }

        return result;
    }

    private static void RestoreEnvironment(
        IReadOnlyDictionary<
            string,
            string?> values)
    {
        foreach (var item in values)
        {
            Environment.SetEnvironmentVariable(
                item.Key,
                item.Value);
        }
    }

    private static readonly string[]
        EnvironmentVariables =
    [
        "XDG_DATA_HOME",
        "XDG_CONFIG_HOME",
        "XDG_CACHE_HOME",
        "XDG_STATE_HOME"
    ];

    private static readonly DateTimeOffset
        BirthInstant =
        new(
            2000,
            1,
            1,
            11,
            0,
            0,
            TimeSpan.Zero);

    private static readonly DateTimeOffset
        FixedNow =
        new(
            2026,
            8,
            22,
            8,
            0,
            0,
            TimeSpan.Zero);
}

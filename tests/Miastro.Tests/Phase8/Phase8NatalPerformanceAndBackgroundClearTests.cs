using System.Diagnostics;
using Miastro.Application.Natal;
using Miastro.Application.Natal.Reading;
using Miastro.Domain.Aspects;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;
using Miastro.UI.Avalonia.ViewModels.NatalPanels;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalPerformanceAndBackgroundClearTests
{
    public TestContext TestContext { get; set; } =
        null!;

    [TestMethod]
    public void Wheel_background_no_hit_uses_neutral_selection_endpoint()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/ViewModels/"
                + "MainWindowViewModel.Natal.cs");

        var start =
            source.IndexOf(
                "public void SelectNatalWheelAt(",
                StringComparison.Ordinal);

        var end =
            source.IndexOf(
                "public void MoveNatalWheelSelection(",
                start,
                StringComparison.Ordinal);

        Assert.IsTrue(
            start >= 0);

        Assert.IsTrue(
            end > start);

        var method =
            source[
                start..
                end];

        StringAssert.Contains(
            method,
            "HitTestViewport(");

        StringAssert.Contains(
            method,
            "ApplyNatalWheelSelection(");

        StringAssert.Contains(
            method,
            "hit?.ObjectId");
    }

    [TestMethod]
    public void Wheel_pointer_press_routes_background_click_to_hit_test()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/Views/"
                + "MainWindow.axaml.cs");

        var start =
            source.IndexOf(
                "OnNatalWheelPointerPressed(",
                StringComparison.Ordinal);

        var end =
            source.IndexOf(
                "OnNatalWheelViewportHostSizeChanged(",
                start,
                StringComparison.Ordinal);

        Assert.IsTrue(
            start >= 0);

        Assert.IsTrue(
            end > start);

        var method =
            source[
                start..
                end];

        StringAssert.Contains(
            method,
            "viewModel.SelectNatalWheelAt(");
    }

    [TestMethod]
    public void Data_panel_construction_is_measured()
    {
        Measure(
            "DataPanel",
            200,
            () =>
                _ =
                    NatalDataPanelViewModel.From(
                        Snapshot()));
    }

    [TestMethod]
    public void Positions_panel_construction_is_measured()
    {
        Measure(
            "PositionsPanel",
            200,
            () =>
                _ =
                    NatalPositionsPanelViewModel.From(
                        Snapshot()));
    }

    [TestMethod]
    public void Aspects_panel_construction_is_measured()
    {
        Measure(
            "AspectsPanel",
            200,
            () =>
                _ =
                    NatalAspectsPanelViewModel.From(
                        Snapshot()));
    }

    [TestMethod]
    public void Distribution_panel_construction_is_measured()
    {
        Measure(
            "DistributionPanel",
            200,
            () =>
                _ =
                    NatalDistributionPanelViewModel.From(
                        Snapshot()));
    }

    [TestMethod]
    public void Summary_panel_construction_is_measured()
    {
        Measure(
            "SummaryPanel",
            200,
            () =>
                _ =
                    NatalSummaryPanelViewModel.From(
                        Snapshot()));
    }

    [TestMethod]
    public void Complete_panel_host_construction_is_measured()
    {
        Measure(
            "PanelHost",
            100,
            () =>
                _ =
                    NatalPanelHostViewModel.From(
                        Snapshot()));
    }

    [TestMethod]
    public void Simple_selection_is_measured()
    {
        var host =
            NatalPanelHostViewModel.From(
                Snapshot());

        Measure(
            "SimpleSelection",
            10000,
            () =>
                host.SyncSelectedObject(
                    AstrologicalObjectId.Sun,
                    openPositions: true));
    }

    [TestMethod]
    public void Dual_aspect_selection_is_measured()
    {
        var host =
            NatalPanelHostViewModel.From(
                Snapshot());

        var cell =
            host.Aspects.Cells
                .First(
                    item =>
                        item.HasAspect);

        Measure(
            "DualAspectSelection",
            10000,
            () =>
                host.SyncDualSelection(
                    cell));
    }

    [TestMethod]
    public void Clear_selection_is_measured()
    {
        var host =
            NatalPanelHostViewModel.From(
                Snapshot());

        Measure(
            "ClearSelection",
            10000,
            () =>
                host.ClearSelection());
    }

    private void Measure(
        string operation,
        int iterations,
        Action action)
    {
        for (var i = 0; i < 5; i++)
        {
            action();
        }

        var stopwatch =
            Stopwatch.StartNew();

        for (var i = 0; i < iterations; i++)
        {
            action();
        }

        stopwatch.Stop();

        var totalMilliseconds =
            stopwatch.Elapsed.TotalMilliseconds;

        var averageMicroseconds =
            totalMilliseconds
            * 1000.0
            / iterations;

        TestContext.WriteLine(
            $"{operation}: "
            + $"{iterations} iteraciones, "
            + $"{totalMilliseconds:F3} ms total, "
            + $"{averageMicroseconds:F3} µs/op");

        Assert.IsTrue(
            double.IsFinite(
                totalMilliseconds));

        Assert.IsTrue(
            totalMilliseconds >= 0.0);

        Assert.IsTrue(
            totalMilliseconds < 5000.0,
            $"{operation} excedió el umbral defensivo "
            + $"de 5000 ms: {totalMilliseconds:F3} ms.");
    }

    private static NatalChartSnapshotReadModel
        Snapshot()
    {
        var placements =
            new[]
            {
                Placement(
                    AstrologicalObjectId.Sun,
                    10.0,
                    1,
                    MotionState.Direct),

                Placement(
                    AstrologicalObjectId.Moon,
                    42.0,
                    2,
                    MotionState.Direct),

                Placement(
                    AstrologicalObjectId.Mercury,
                    75.0,
                    3,
                    MotionState.Direct),

                Placement(
                    AstrologicalObjectId.Venus,
                    108.0,
                    4,
                    MotionState.Direct),

                Placement(
                    AstrologicalObjectId.Mars,
                    142.0,
                    5,
                    MotionState.Direct),

                Placement(
                    AstrologicalObjectId.Jupiter,
                    176.0,
                    6,
                    MotionState.Retrograde),

                Placement(
                    AstrologicalObjectId.Saturn,
                    210.0,
                    7,
                    MotionState.Retrograde),

                Placement(
                    AstrologicalObjectId.Uranus,
                    244.0,
                    8,
                    MotionState.Direct),

                Placement(
                    AstrologicalObjectId.Neptune,
                    278.0,
                    9,
                    MotionState.Direct),

                Placement(
                    AstrologicalObjectId.Pluto,
                    312.0,
                    11,
                    MotionState.Direct),

                Placement(
                    AstrologicalObjectId.Ascendant,
                    17.0,
                    1,
                    null),

                Placement(
                    AstrologicalObjectId.Midheaven,
                    276.0,
                    10,
                    null)
            };

        var cusps =
            new[]
            {
                17.0,
                42.0,
                68.0,
                96.0,
                128.0,
                160.0,
                197.0,
                222.0,
                248.0,
                276.0,
                308.0,
                340.0
            }
            .Select(
                (longitude, index) =>
                    new NatalHouseCuspSnapshot(
                        index + 1,
                        longitude))
            .ToArray();

        var aspects =
            new[]
            {
                new NatalAspectSnapshot(
                    AstrologicalObjectId.Sun,
                    AstrologicalObjectId.Moon,
                    AspectKind.Trine,
                    120.0,
                    120.0,
                    0.0,
                    6.0,
                    0.0),

                new NatalAspectSnapshot(
                    AstrologicalObjectId.Sun,
                    AstrologicalObjectId.Saturn,
                    AspectKind.Square,
                    91.5,
                    90.0,
                    1.5,
                    7.0,
                    1.5),

                new NatalAspectSnapshot(
                    AstrologicalObjectId.Moon,
                    AstrologicalObjectId.Mars,
                    AspectKind.Opposition,
                    178.8,
                    180.0,
                    1.2,
                    8.0,
                    1.2),

                new NatalAspectSnapshot(
                    AstrologicalObjectId.Mercury,
                    AstrologicalObjectId.Venus,
                    AspectKind.Sextile,
                    60.7,
                    60.0,
                    0.7,
                    5.0,
                    0.7),

                new NatalAspectSnapshot(
                    AstrologicalObjectId.Jupiter,
                    AstrologicalObjectId.Pluto,
                    AspectKind.Quincunx,
                    151.0,
                    150.0,
                    1.0,
                    3.0,
                    1.0)
            };

        return new NatalChartSnapshotReadModel(
            Id:
                Guid.Parse(
                    "11111111-1111-1111-1111-111111111111"),

            PersonId:
                Guid.Parse(
                    "22222222-2222-2222-2222-222222222222"),

            Status:
                NatalChartStatus.Current,

            InputHash:
                "phase8-performance",

            IsApproximateBirthTime:
                false,

            BirthLocalDate:
                new DateOnly(
                    2000,
                    1,
                    1),

            BirthLocalTime:
                new TimeOnly(
                    12,
                    0),

            InstantUtc:
                new DateTimeOffset(
                    2000,
                    1,
                    1,
                    11,
                    0,
                    0,
                    TimeSpan.Zero),

            Locality:
                "Test",

            Latitude:
                40.0,

            Longitude:
                -3.0,

            IanaTimeZoneId:
                "Europe/Madrid",

            TzdbVersion:
                "test",

            HouseSystem:
                HouseSystem.Placidus,

            CalculationProfileId:
                "miastro-v1",

            MiastroVersion:
                "test",

            Engine:
                "test",

            EngineVersion:
                "test",

            AdapterVersion:
                "test",

            EphemerisVersion:
                "test",

            CalculatedAtUtc:
                new DateTimeOffset(
                    2026,
                    8,
                    27,
                    12,
                    0,
                    0,
                    TimeSpan.Zero),

            InvalidatedAtUtc:
                null,

            SupersededByChartId:
                null,

            Placements:
                placements,

            HouseCusps:
                cusps,

            Aspects:
                aspects);
    }

    private static NatalPlacementSnapshot Placement(
        AstrologicalObjectId objectId,
        double longitude,
        int house,
        MotionState? motion)
        => new(
            ObjectId:
                objectId,

            LongitudeDegrees:
                longitude,

            LatitudeDegrees:
                null,

            DistanceAu:
                null,

            LongitudeSpeedDegreesPerDay:
                null,

            LatitudeSpeedDegreesPerDay:
                null,

            DistanceSpeedAuPerDay:
                null,

            Motion:
                motion,

            ZodiacSign:
                (int)(
                    longitude
                    / 30.0),

            DegreeInSign:
                longitude
                % 30.0,

            HouseNumber:
                house);

    private static string Read(
        string relativePath)
        =>
            File.ReadAllText(
                Path.Combine(
                    FindRepoRoot(),
                    relativePath));

    private static string FindRepoRoot()
    {
        var current =
            new DirectoryInfo(
                AppContext.BaseDirectory);

        while (current is not null)
        {
            if (File.Exists(
                Path.Combine(
                    current.FullName,
                    "Miastro.sln")))
            {
                return current.FullName;
            }

            current =
                current.Parent;
        }

        throw new InvalidOperationException(
            "No se encontró la raíz del repositorio.");
    }
}

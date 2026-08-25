using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Application.Natal;
using Miastro.Domain.Aspects;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;
using Miastro.Graphics.Adapters.Natal;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Scene.Natal.Aspects;
using Miastro.UI.Avalonia.ViewModels;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7SnapshotGraphicsAdapterTests
{
    [TestMethod]
    public void Adapter_uses_persisted_ascendant_and_midheaven()
    {
        var snapshot =
            BuildSnapshot();

        var model =
            new NatalChartSnapshotGraphicsAdapter()
                .Adapt(
                    snapshot,
                    800,
                    800);

        Assert.AreEqual(
            17.0,
            model.Wheel
                .AscendantLongitudeDegrees,
            1e-12);

        Assert.AreEqual(
            103.0,
            model.Wheel
                .MidheavenLongitudeDegrees,
            1e-12);
    }

    [TestMethod]
    public void Adapter_uses_all_twelve_persisted_cusps()
    {
        var model =
            AdaptDefault();

        Assert.AreEqual(
            12,
            model.Wheel
                .HouseCusps
                .Count);

        CollectionAssert.AreEqual(
            Enumerable.Range(
                    1,
                    12)
                .ToArray(),
            model.Wheel
                .HouseCusps
                .Select(
                    x => x.HouseNumber)
                .ToArray());
    }

    [TestMethod]
    public void Default_projection_contains_ten_planets_chiron_asc_mc()
    {
        var model =
            AdaptDefault();

        Assert.AreEqual(
            13,
            model.Objects.Count);

        Assert.AreEqual(
            10,
            model.Objects.Count(
                x => x.Layer
                    == SceneLayer.BodyLayer));

        Assert.AreEqual(
            3,
            model.Objects.Count(
                x => x.Layer
                    == SceneLayer.PointLayer));

        CollectionAssert.Contains(
            model.Objects
                .Select(x => x.Id)
                .ToArray(),
            "Chiron");

        CollectionAssert.Contains(
            model.Objects
                .Select(x => x.Id)
                .ToArray(),
            "Ascendant");

        CollectionAssert.Contains(
            model.Objects
                .Select(x => x.Id)
                .ToArray(),
            "Midheaven");
    }

    [TestMethod]
    public void Optional_points_can_be_enabled()
    {
        var snapshot =
            BuildSnapshot();

        var model =
            new NatalChartSnapshotGraphicsAdapter()
                .Adapt(
                    snapshot,
                    800,
                    800,
                    new NatalSnapshotGraphicsOptions(
                        IncludeOptionalPoints: true));

        var ids =
            model.Objects
                .Select(x => x.Id)
                .ToArray();

        CollectionAssert.Contains(
            ids,
            "NorthTrueNode");

        CollectionAssert.Contains(
            ids,
            "SouthNode");

        CollectionAssert.Contains(
            ids,
            "MeanLilith");

        CollectionAssert.Contains(
            ids,
            "PartOfFortune");

        CollectionAssert.Contains(
            ids,
            "Ceres");

        CollectionAssert.Contains(
            ids,
            "Pallas");

        CollectionAssert.Contains(
            ids,
            "Juno");

        CollectionAssert.Contains(
            ids,
            "Vesta");
    }

    [TestMethod]
    public void Persisted_longitudes_flow_to_visual_placement_input()
    {
        var snapshot =
            BuildSnapshot();

        var model =
            new NatalChartSnapshotGraphicsAdapter()
                .Adapt(
                    snapshot,
                    800,
                    800);

        var sun =
            model.Placements
                .Placements
                .Single(
                    x => x.Id == "Sun");

        Assert.AreEqual(
            snapshot.Placements
                .Single(
                    x => x.ObjectId
                        == AstrologicalObjectId.Sun)
                .LongitudeDegrees,
            sun.RealLongitudeDegrees,
            1e-12);
    }

    [TestMethod]
    public void Persisted_aspects_are_mapped_without_detection()
    {
        var model =
            AdaptDefault();

        Assert.AreEqual(
            2,
            model.Aspects.Count);

        Assert.AreEqual(
            "Sun-Moon-Trine",
            model.Aspects[0].Id);

        Assert.AreEqual(
            "Sun",
            model.Aspects[0]
                .FirstObjectId);

        Assert.AreEqual(
            "Moon",
            model.Aspects[0]
                .SecondObjectId);
    }

    [TestMethod]
    public void Major_and_secondary_aspect_classes_come_from_persisted_kind()
    {
        var model =
            AdaptDefault();

        Assert.AreEqual(
            NatalAspectVisualClass.Major,
            model.Aspects
                .Single(
                    x => x.Id
                        == "Sun-Moon-Trine")
                .VisualClass);

        Assert.AreEqual(
            NatalAspectVisualClass.Secondary,
            model.Aspects
                .Single(
                    x => x.Id
                        == "Mercury-Venus-Quincunx")
                .VisualClass);
    }

    [TestMethod]
    public void Missing_ascendant_is_rejected()
    {
        var snapshot =
            BuildSnapshot();

        snapshot =
            snapshot with
            {
                Placements =
                    snapshot.Placements
                        .Where(
                            x => x.ObjectId
                                != AstrologicalObjectId
                                    .Ascendant)
                        .ToArray()
            };

        Assert.ThrowsExactly<
            ArgumentException>(
                () =>
                    new NatalChartSnapshotGraphicsAdapter()
                        .Adapt(
                            snapshot,
                            800,
                            800));
    }

    [TestMethod]
    public void Invalid_cusp_set_is_rejected()
    {
        var snapshot =
            BuildSnapshot();

        snapshot =
            snapshot with
            {
                HouseCusps =
                    snapshot.HouseCusps
                        .Take(11)
                        .ToArray()
            };

        Assert.ThrowsExactly<
            ArgumentException>(
                () =>
                    new NatalChartSnapshotGraphicsAdapter()
                        .Adapt(
                            snapshot,
                            800,
                            800));
    }

    [TestMethod]
    public void Persisted_retrograde_maps_to_scene_object_flag()
    {
        var model =
            new NatalChartSnapshotGraphicsAdapter()
                .Adapt(
                    BuildSnapshot(
                        MotionState.Retrograde),
                    800,
                    800);

        var mercury =
            model.Objects
                .Single(
                    x => x.Id
                        == "Mercury");

        Assert.IsTrue(
            mercury.IsRetrograde);
    }

    [TestMethod]
    public void Non_retrograde_motion_does_not_set_scene_object_flag()
    {
        foreach (
            var motion
            in new MotionState?[]
            {
                null,
                MotionState.Direct,
                MotionState.Stationary
            })
        {
            var model =
                new NatalChartSnapshotGraphicsAdapter()
                    .Adapt(
                        BuildSnapshot(
                            motion),
                        800,
                        800);

            var mercury =
                model.Objects
                    .Single(
                        x => x.Id
                            == "Mercury");

            Assert.IsFalse(
                mercury.IsRetrograde,
                $"Unexpected retrograde flag for {motion?.ToString() ?? "null"}.");
        }
    }

    [TestMethod]
    public void Persisted_retrograde_surfaces_through_motion_read_model()
    {
        var snapshot =
            BuildSnapshot(
                MotionState.Retrograde);

        var model =
            new NatalChartSnapshotGraphicsAdapter()
                .Adapt(
                    snapshot,
                    800,
                    800);

        var mercuryObject =
            model.Objects
                .Single(
                    x =>
                        x.Id
                        == "Mercury");

        Assert.IsTrue(
            mercuryObject.IsRetrograde);

        var mercuryPlacement =
            snapshot.Placements
                .Single(
                    x =>
                        x.ObjectId
                        == AstrologicalObjectId.Mercury);

        var row =
            NatalPlacementRowViewModel.From(
                mercuryPlacement);

        Assert.AreEqual(
            "Retrógrado",
            row.MotionText);

        // La rueda ya no imprime posiciones ni movimiento.
        var visualPlacement =
            model.Placements
                .Placements
                .Single(
                    x =>
                        x.Id
                        == "Mercury");

        var scene =
            new Miastro.Graphics.Scene.Natal
                .NatalWheelSceneBuilder()
                .Build(
                    model.Wheel,
                    new Miastro.Graphics.Layout.Placement
                        .NatalObjectPlacementSnapshot(
                            new[]
                            {
                                visualPlacement
                            }),
                    new[]
                    {
                        mercuryObject
                    });

        Assert.IsFalse(
            scene.Nodes
                .OfType<TextNode>()
                .Any(
                    x =>
                        x.Id
                        == "object-label-Mercury"));
    }


    [TestMethod]
    public void Direct_motion_surfaces_through_motion_read_model()
    {
        var snapshot =
            BuildSnapshot(
                MotionState.Direct);

        var model =
            new NatalChartSnapshotGraphicsAdapter()
                .Adapt(
                    snapshot,
                    800,
                    800);

        var mercuryObject =
            model.Objects
                .Single(
                    x =>
                        x.Id
                        == "Mercury");

        Assert.IsFalse(
            mercuryObject.IsRetrograde);

        var mercuryPlacement =
            snapshot.Placements
                .Single(
                    x =>
                        x.ObjectId
                        == AstrologicalObjectId.Mercury);

        var row =
            NatalPlacementRowViewModel.From(
                mercuryPlacement);

        Assert.AreEqual(
            "Directo",
            row.MotionText);

        var visualPlacement =
            model.Placements
                .Placements
                .Single(
                    x =>
                        x.Id
                        == "Mercury");

        var scene =
            new Miastro.Graphics.Scene.Natal
                .NatalWheelSceneBuilder()
                .Build(
                    model.Wheel,
                    new Miastro.Graphics.Layout.Placement
                        .NatalObjectPlacementSnapshot(
                            new[]
                            {
                                visualPlacement
                            }),
                    new[]
                    {
                        mercuryObject
                    });

        Assert.IsFalse(
            scene.Nodes
                .OfType<TextNode>()
                .Any(
                    x =>
                        x.Id
                        == "object-label-Mercury"));
    }


    [TestMethod]
    public void Adapter_is_deterministic()
    {
        var snapshot =
            BuildSnapshot();

        var adapter =
            new NatalChartSnapshotGraphicsAdapter();

        var first =
            Describe(
                adapter.Adapt(
                    snapshot,
                    800,
                    800));

        for (var i = 0; i < 50; i++)
        {
            Assert.AreEqual(
                first,
                Describe(
                    adapter.Adapt(
                        snapshot,
                        800,
                        800)));
        }
    }

    private static NatalSnapshotGraphicsModel
        AdaptDefault()
        =>
            new NatalChartSnapshotGraphicsAdapter()
                .Adapt(
                    BuildSnapshot(),
                    800,
                    800);

    private static NatalChartSnapshotReadModel
        BuildSnapshot(
            MotionState? mercuryMotion = null)
    {
        var values =
            new Dictionary<
                AstrologicalObjectId,
                double>
            {
                [AstrologicalObjectId.Sun] = 120,
                [AstrologicalObjectId.Moon] = 240,
                [AstrologicalObjectId.Mercury] = 130,
                [AstrologicalObjectId.Venus] = 280,
                [AstrologicalObjectId.Mars] = 30,
                [AstrologicalObjectId.Jupiter] = 60,
                [AstrologicalObjectId.Saturn] = 300,
                [AstrologicalObjectId.Uranus] = 320,
                [AstrologicalObjectId.Neptune] = 330,
                [AstrologicalObjectId.Pluto] = 250,
                [AstrologicalObjectId.NorthTrueNode] = 70,
                [AstrologicalObjectId.SouthNode] = 250,
                [AstrologicalObjectId.Chiron] = 200,
                [AstrologicalObjectId.Ceres] = 45,
                [AstrologicalObjectId.Pallas] = 75,
                [AstrologicalObjectId.Juno] = 105,
                [AstrologicalObjectId.Vesta] = 135,
                [AstrologicalObjectId.MeanLilith] = 160,
                [AstrologicalObjectId.PartOfFortune] = 190,
                [AstrologicalObjectId.Ascendant] = 17,
                [AstrologicalObjectId.Midheaven] = 103
            };

        var placements =
            values
                .OrderBy(
                    x => (int)x.Key)
                .Select(
                    x =>
                        new NatalPlacementSnapshot(
                            x.Key,
                            x.Value,
                            LatitudeDegrees: null,
                            DistanceAu: null,
                            LongitudeSpeedDegreesPerDay: null,
                            LatitudeSpeedDegreesPerDay: null,
                            DistanceSpeedAuPerDay: null,
                            Motion:
                                x.Key
                                    == AstrologicalObjectId.Mercury
                                    ? mercuryMotion
                                    : null,
                            ZodiacSign:
                                (int)(x.Value / 30.0),
                            DegreeInSign:
                                x.Value % 30.0,
                            HouseNumber: null))
                .ToArray();

        var cuspValues =
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
            };

        var cusps =
            cuspValues
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
                    SeparationDegrees: 120,
                    ExactAngleDegrees: 120,
                    DeviationDegrees: 0,
                    AllowedOrbDegrees: 6,
                    UsedOrbDegrees: 0),

                new NatalAspectSnapshot(
                    AstrologicalObjectId.Mercury,
                    AstrologicalObjectId.Venus,
                    AspectKind.Quincunx,
                    SeparationDegrees: 150,
                    ExactAngleDegrees: 150,
                    DeviationDegrees: 0,
                    AllowedOrbDegrees: 3,
                    UsedOrbDegrees: 0),

                new NatalAspectSnapshot(
                    AstrologicalObjectId.NorthTrueNode,
                    AstrologicalObjectId.Sun,
                    AspectKind.Sextile,
                    SeparationDegrees: 60,
                    ExactAngleDegrees: 60,
                    DeviationDegrees: 0,
                    AllowedOrbDegrees: 4,
                    UsedOrbDegrees: 0)
            };

        return new NatalChartSnapshotReadModel(
            Id: Guid.Parse(
                "11111111-1111-1111-1111-111111111111"),
            PersonId: Guid.Parse(
                "22222222-2222-2222-2222-222222222222"),
            Status: NatalChartStatus.Current,
            InputHash: "test-input-hash",
            IsApproximateBirthTime: false,
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
            Locality: "Test",
            Latitude: 40,
            Longitude: -3,
            IanaTimeZoneId: "Europe/Madrid",
            TzdbVersion: "test",
            HouseSystem: HouseSystem.Placidus,
            CalculationProfileId: "miastro-v1",
            MiastroVersion: "test",
            Engine: "test",
            EngineVersion: "test",
            AdapterVersion: "test",
            EphemerisVersion: "test",
            CalculatedAtUtc:
                new DateTimeOffset(
                    2026,
                    8,
                    23,
                    12,
                    0,
                    0,
                    TimeSpan.Zero),
            InvalidatedAtUtc: null,
            SupersededByChartId: null,
            Placements: placements,
            HouseCusps: cusps,
            Aspects: aspects);
    }

    private static string Describe(
        NatalSnapshotGraphicsModel model)
        =>
            string.Join(
                "\n",
                new[]
                {
                    model.ChartId.ToString(),
                    model.Wheel
                        .ToDiagnosticText(),
                    model.Placements
                        .ToDiagnosticText(),
                    string.Join(
                        "|",
                        model.Objects.Select(
                            x =>
                                $"{x.Id}:{x.GlyphKey}:{x.Layer}:{x.IsRetrograde}")),
                    string.Join(
                        "|",
                        model.Aspects.Select(
                            x =>
                                $"{x.Id}:{x.VisualClass}"))
                });
}

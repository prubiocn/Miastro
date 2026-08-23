using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Geometry;
using Miastro.Graphics.Scene;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7GeometryAndSceneGraphTests
{
    private const double Tolerance = 1e-10;

    [TestMethod]
    public void NormalizeDegrees_is_canonical()
    {
        Assert.AreEqual(
            0.0,
            NatalWheelCoordinates.NormalizeDegrees(360.0),
            Tolerance);

        Assert.AreEqual(
            359.0,
            NatalWheelCoordinates.NormalizeDegrees(-1.0),
            Tolerance);

        Assert.AreEqual(
            1.0,
            NatalWheelCoordinates.NormalizeDegrees(721.0),
            Tolerance);
    }

    [TestMethod]
    public void Ascendant_is_always_at_left()
    {
        var ascendants = new[]
        {
            0.0,
            1.25,
            89.999,
            180.0,
            359.999
        };

        foreach (var ascendant in ascendants)
        {
            var angle =
                NatalWheelCoordinates
                    .EclipticToScreenAngleDegrees(
                        ascendant,
                        ascendant);

            Assert.AreEqual(
                180.0,
                angle,
                Tolerance);
        }
    }

    [TestMethod]
    public void Zodiac_quadrants_follow_canonical_orientation()
    {
        const double asc = 123.0;

        Assert.AreEqual(
            180.0,
            NatalWheelCoordinates
                .EclipticToScreenAngleDegrees(
                    asc,
                    asc),
            Tolerance);

        Assert.AreEqual(
            90.0,
            NatalWheelCoordinates
                .EclipticToScreenAngleDegrees(
                    asc + 90.0,
                    asc),
            Tolerance);

        Assert.AreEqual(
            0.0,
            NatalWheelCoordinates
                .EclipticToScreenAngleDegrees(
                    asc + 180.0,
                    asc),
            Tolerance);

        Assert.AreEqual(
            270.0,
            NatalWheelCoordinates
                .EclipticToScreenAngleDegrees(
                    asc + 270.0,
                    asc),
            Tolerance);
    }

    [TestMethod]
    public void Ascendant_point_is_exactly_left_of_center()
    {
        var center =
            new ChartPoint(
                400.0,
                300.0);

        var point =
            NatalWheelCoordinates
                .PointForLongitude(
                    center,
                    200.0,
                    42.5,
                    42.5);

        Assert.AreEqual(
            200.0,
            point.X,
            1e-9);

        Assert.AreEqual(
            300.0,
            point.Y,
            1e-9);
    }

    [TestMethod]
    public void Coordinate_transform_is_deterministic()
    {
        const double longitude = 359.999999;
        const double ascendant = 17.123456;

        var first =
            NatalWheelCoordinates
                .EclipticToScreenAngleDegrees(
                    longitude,
                    ascendant);

        for (var i = 0; i < 100; i++)
        {
            var next =
                NatalWheelCoordinates
                    .EclipticToScreenAngleDegrees(
                        longitude,
                        ascendant);

            Assert.AreEqual(
                first,
                next);
        }
    }

    [TestMethod]
    public void ChartRect_detects_overlap_and_containment()
    {
        var first =
            new ChartRect(
                10,
                10,
                20,
                20);

        var overlapping =
            new ChartRect(
                25,
                25,
                20,
                20);

        var separate =
            new ChartRect(
                40,
                40,
                10,
                10);

        Assert.IsTrue(
            first.Intersects(overlapping));

        Assert.IsFalse(
            first.Intersects(separate));

        Assert.IsTrue(
            first.Contains(
                new ChartPoint(
                    20,
                    20)));
    }

    [TestMethod]
    public void Scene_layers_have_explicit_stable_paint_order()
    {
        var values =
            Enum.GetValues<SceneLayer>()
                .Select(x => (int)x)
                .ToArray();

        CollectionAssert.AreEqual(
            new[]
            {
                0,
                100,
                200,
                300,
                400,
                500,
                600,
                700,
                800,
                900
            },
            values);
    }

    [TestMethod]
    public void Scene_orders_by_layer_then_stable_id()
    {
        var scene =
            new NatalScene(
                800,
                800,
                new SceneNode[]
                {
                    new LineNode(
                        "house-b",
                        SceneLayer.HouseLayer,
                        new ChartPoint(0, 0),
                        new ChartPoint(1, 1)),

                    new CircleNode(
                        "background",
                        SceneLayer.Background,
                        new ChartPoint(0, 0),
                        1),

                    new LineNode(
                        "house-a",
                        SceneLayer.HouseLayer,
                        new ChartPoint(0, 0),
                        new ChartPoint(1, 1))
                });

        CollectionAssert.AreEqual(
            new[]
            {
                "background",
                "house-a",
                "house-b"
            },
            scene.OrderedNodes
                .Select(x => x.Id)
                .ToArray());
    }

    [TestMethod]
    public void Scene_graph_is_renderer_independent()
    {
        var references =
            typeof(SceneNode)
                .Assembly
                .GetReferencedAssemblies()
                .Select(x => x.Name)
                .Where(x => x is not null)
                .ToArray();

        Assert.IsFalse(
            references.Any(x =>
                x!.StartsWith(
                    "Avalonia",
                    StringComparison.Ordinal)));

        Assert.IsFalse(
            references.Any(x =>
                x!.StartsWith(
                    "SkiaSharp",
                    StringComparison.Ordinal)));

        Assert.IsFalse(
            references.Any(x =>
                x!.Contains(
                    "SwissEphemeris",
                    StringComparison.Ordinal)));
    }
}

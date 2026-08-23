using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Geometry;
using Miastro.Graphics.Interaction;
using Miastro.Graphics.Layout;
using Miastro.Graphics.Layout.Placement;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Scene.Natal;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7HitTestingTests
{
    [TestMethod]
    public void Hit_test_selects_body_at_visual_center()
    {
        var scene =
            BuildScene(
                out var placements);

        var sun =
            placements.Placements
                .Single(
                    x => x.Id == "Sun");

        var result =
            new NatalSceneHitTester()
                .HitTest(
                    scene,
                    sun.VisualCenter);

        Assert.IsNotNull(result);

        Assert.AreEqual(
            "Sun",
            result.ObjectId);

        Assert.AreEqual(
            NatalHitTargetKind.Body,
            result.Kind);
    }

    [TestMethod]
    public void Hit_test_uses_displaced_visual_geometry()
    {
        var scene =
            BuildScene(
                out var placements);

        var displaced =
            placements.Placements
                .Single(
                    x => x.RadialLevel != 0);

        Assert.AreNotEqual(
            displaced.RealAnchor,
            displaced.VisualCenter);

        var result =
            new NatalSceneHitTester()
                .HitTest(
                    scene,
                    displaced.VisualCenter);

        Assert.IsNotNull(result);

        Assert.AreEqual(
            displaced.Id,
            result.ObjectId);

        Assert.IsTrue(
            result.Bounds.Contains(
                displaced.VisualCenter));
    }

    [TestMethod]
    public void Empty_space_returns_no_hit()
    {
        var scene =
            BuildScene(
                out _);

        var result =
            new NatalSceneHitTester()
                .HitTest(
                    scene,
                    new ChartPoint(
                        400,
                        400));

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Tolerance_expands_click_target()
    {
        var scene =
            BuildScene(
                out var placements);

        var chiron =
            placements.Placements
                .Single(
                    x => x.Id == "Chiron");

        var point =
            new ChartPoint(
                chiron.Bounds.Right + 3.0,
                chiron.VisualCenter.Y);

        var withoutTolerance =
            new NatalSceneHitTester()
                .HitTest(
                    scene,
                    point);

        var withTolerance =
            new NatalSceneHitTester()
                .HitTest(
                    scene,
                    point,
                    tolerance: 4.0);

        Assert.IsNull(
            withoutTolerance);

        Assert.IsNotNull(
            withTolerance);

        Assert.AreEqual(
            "Chiron",
            withTolerance.ObjectId);
    }

    [TestMethod]
    public void Point_layer_is_reported_as_point()
    {
        var scene =
            BuildScene(
                out var placements);

        var chiron =
            placements.Placements
                .Single(
                    x => x.Id == "Chiron");

        var result =
            new NatalSceneHitTester()
                .HitTest(
                    scene,
                    chiron.VisualCenter);

        Assert.IsNotNull(result);

        Assert.AreEqual(
            NatalHitTargetKind.Point,
            result.Kind);
    }

    [TestMethod]
    public void Zodiac_glyphs_are_not_interaction_targets()
    {
        var scene =
            BuildScene(
                out _);

        var zodiac =
            scene.Nodes
                .OfType<GlyphNode>()
                .First(
                    x =>
                        x.Id.StartsWith(
                            "zodiac-glyph-",
                            StringComparison.Ordinal));

        var result =
            new NatalSceneHitTester()
                .HitTest(
                    scene,
                    zodiac.Position);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Hit_testing_is_deterministic()
    {
        var scene =
            BuildScene(
                out var placements);

        var target =
            placements.Placements
                .Single(
                    x => x.Id == "Sun")
                .VisualCenter;

        var tester =
            new NatalSceneHitTester();

        var first =
            tester.HitTest(
                scene,
                target);

        for (var i = 0; i < 100; i++)
        {
            Assert.AreEqual(
                first,
                tester.HitTest(
                    scene,
                    target));
        }
    }

    [TestMethod]
    public void Viewport_coordinates_map_to_scene_geometry()
    {
        var scene =
            BuildScene(
                out var placements);

        var sun =
            placements.Placements
                .Single(
                    x => x.Id == "Sun");

        var viewportWidth =
            320.0;

        var viewportHeight =
            320.0;

        var x =
            sun.VisualCenter.X
            * viewportWidth
            / scene.Width;

        var y =
            sun.VisualCenter.Y
            * viewportHeight
            / scene.Height;

        var result =
            new NatalSceneHitTester()
                .HitTestViewport(
                    scene,
                    x,
                    y,
                    viewportWidth,
                    viewportHeight);

        Assert.IsNotNull(result);

        Assert.AreEqual(
            "Sun",
            result.ObjectId);
    }

    private static NatalScene BuildScene(
        out NatalObjectPlacementSnapshot placements)
    {
        var wheel =
            new NatalWheelLayoutBuilder()
                .Build(
                    800,
                    800,
                    17,
                    103,
                    new double[]
                    {
                        17,
                        42,
                        68,
                        96,
                        128,
                        160,
                        197,
                        222,
                        248,
                        276,
                        308,
                        340
                    });

        placements =
            new NatalObjectPlacementEngine()
                .Layout(
                    wheel,
                    new[]
                    {
                        new NatalObjectLayoutInput(
                            "Moon",
                            120),

                        new NatalObjectLayoutInput(
                            "Sun",
                            120),

                        new NatalObjectLayoutInput(
                            "Chiron",
                            210)
                    });

        return new NatalWheelSceneBuilder()
            .Build(
                wheel,
                placements,
                new[]
                {
                    new NatalSceneObjectInput(
                        "Moon",
                        "planet-moon",
                        SceneLayer.BodyLayer),

                    new NatalSceneObjectInput(
                        "Sun",
                        "planet-sun",
                        SceneLayer.BodyLayer),

                    new NatalSceneObjectInput(
                        "Chiron",
                        "point-chiron",
                        SceneLayer.PointLayer)
                });
    }
}

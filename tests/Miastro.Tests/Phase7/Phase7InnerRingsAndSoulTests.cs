using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Layout;
using Miastro.Graphics.Layout.Placement;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Scene.Natal;
using Miastro.Graphics.Scene.Natal.Aspects;
using Miastro.Graphics.Styles;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7InnerRingsAndSoulTests
{
    [TestMethod]
    public void Metrics_expose_planet_orbit_and_soul_radius()
    {
        var metrics =
            NatalWheelMetrics.Create(
                800,
                800);

        Assert.AreEqual(
            metrics.OuterRadius * 0.70,
            metrics.PlanetOrbitRadius,
            1e-12);

        Assert.IsTrue(
            metrics.PlanetOrbitRadius
            > metrics.AspectRadius);

        Assert.IsTrue(
            metrics.AspectRadius
            > metrics.SoulRadius);
    }


    [TestMethod]
    public void Planet_orbit_matches_nominal_real_anchor_radius()
    {
        var wheel =
            BuildWheel();

        var placement =
            new NatalObjectPlacementEngine()
                .Layout(
                    wheel,
                    new[]
                    {
                        new NatalObjectLayoutInput(
                            "Sun",
                            120)
                    })
                .Placements
                .Single();

        Assert.AreEqual(
            wheel.Metrics.PlanetOrbitRadius,
            Distance(
                wheel.Metrics.Center,
                placement.RealAnchor),
            1e-9);
    }

    [TestMethod]
    public void Scene_contains_degree_aspect_and_soul_rings()
    {
        var scene =
            BuildScene();

        var degreeRing =
            scene.Nodes
                .OfType<CircleNode>()
                .Single(
                    x =>
                        x.Id
                        == "zodiac-degree-ring");

        var aspectRing =
            scene.Nodes
                .OfType<CircleNode>()
                .Single(
                    x =>
                        x.Id
                        == "aspect-anchor-ring");

        var soul =
            scene.Nodes
                .OfType<CircleNode>()
                .Single(
                    x =>
                        x.Id
                        == "soul-core");

        Assert.AreEqual(
            NatalSceneStyleKeys.DegreeBoundary,
            degreeRing.StyleKey);

        Assert.AreEqual(
            NatalSceneStyleKeys.AspectRing,
            aspectRing.StyleKey);

        Assert.AreEqual(
            NatalSceneStyleKeys.SoulCore,
            soul.StyleKey);

        Assert.AreEqual(
            SceneLayer.DegreeRing,
            degreeRing.Layer);

        Assert.AreEqual(
            SceneLayer.AspectLayer,
            aspectRing.Layer);

        Assert.AreEqual(
            SceneLayer.AspectLayer,
            soul.Layer);

        Assert.IsFalse(
            scene.Nodes
                .OfType<CircleNode>()
                .Any(
                    x =>
                        x.Id
                        == "planet-orbit-ring"));
    }


    [TestMethod]
    public void Soul_core_style_has_opaque_fill()
    {
        var style =
            new NatalSceneStyleCatalog()
                .GetRequired(
                    NatalSceneStyleKeys.SoulCore);

        Assert.IsNotNull(
            style.FillColor);

        Assert.AreEqual(
            1.0,
            style.Opacity,
            1e-12);
    }

    [TestMethod]
    public void Aspect_crossing_soul_core_is_split_into_two_visible_segments()
    {
        var wheel =
            BuildWheel();

        var placements =
            new NatalObjectPlacementEngine()
                .Layout(
                    wheel,
                    new[]
                    {
                        new NatalObjectLayoutInput(
                            "A",
                            17),

                        new NatalObjectLayoutInput(
                            "B",
                            197)
                    });

        var nodes =
            new NatalAspectSceneBuilder()
                .Build(
                    wheel,
                    placements,
                    new[]
                    {
                        new NatalAspectSceneInput(
                            "opposition",
                            "A",
                            "B",
                            NatalAspectVisualClass.Major)
                    });

        var lines =
            nodes
                .OfType<LineNode>()
                .OrderBy(
                    x => x.Id,
                    StringComparer.Ordinal)
                .ToArray();

        Assert.AreEqual(
            2,
            lines.Length);

        Assert.AreEqual(
            "aspect-opposition-1",
            lines[0].Id);

        Assert.AreEqual(
            "aspect-opposition-2",
            lines[1].Id);

        Assert.AreEqual(
            wheel.Metrics.SoulRadius,
            Distance(
                wheel.Metrics.Center,
                lines[0].End),
            1e-7);

        Assert.AreEqual(
            wheel.Metrics.SoulRadius,
            Distance(
                wheel.Metrics.Center,
                lines[1].Start),
            1e-7);
    }

    private static NatalScene BuildScene()
    {
        var wheel =
            BuildWheel();

        var placements =
            new NatalObjectPlacementEngine()
                .Layout(
                    wheel,
                    new[]
                    {
                        new NatalObjectLayoutInput(
                            "Sun",
                            120),

                        new NatalObjectLayoutInput(
                            "Moon",
                            210)
                    });

        return new NatalWheelSceneBuilder()
            .Build(
                wheel,
                placements,
                new[]
                {
                    new NatalSceneObjectInput(
                        "Sun",
                        "planet-sun",
                        SceneLayer.BodyLayer),

                    new NatalSceneObjectInput(
                        "Moon",
                        "planet-moon",
                        SceneLayer.BodyLayer)
                },
                new[]
                {
                    new NatalAspectSceneInput(
                        "sun-moon",
                        "Sun",
                        "Moon",
                        NatalAspectVisualClass.Major)
                });
    }

    private static NatalWheelLayoutSnapshot
        BuildWheel()
        =>
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

    private static double Distance(
        Miastro.Graphics.Geometry.ChartPoint first,
        Miastro.Graphics.Geometry.ChartPoint second)
    {
        var dx =
            first.X - second.X;

        var dy =
            first.Y - second.Y;

        return Math.Sqrt(
            dx * dx
            + dy * dy);
    }
}

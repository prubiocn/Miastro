using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Layout;
using Miastro.Graphics.Layout.Placement;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Scene.Natal;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7FinalPlanetBandPresentationTests
{
    [TestMethod]
    public void Four_final_radii_are_ordered()
    {
        var metrics =
            NatalWheelMetrics.Create(
                800,
                800);

        Assert.IsTrue(
            metrics.OuterRadius
            > metrics.DegreeRingRadius);

        Assert.IsTrue(
            metrics.DegreeRingRadius
            > metrics.AspectRadius);

        Assert.IsTrue(
            metrics.AspectRadius
            > metrics.SoulRadius);
    }

    [TestMethod]
    public void Planet_base_radius_is_inside_degree_aspect_band()
    {
        var metrics =
            NatalWheelMetrics.Create(
                800,
                800);

        Assert.IsTrue(
            metrics.PlanetOrbitRadius
            < metrics.PlanetBandOuterRadius);

        Assert.IsTrue(
            metrics.PlanetOrbitRadius
            > metrics.PlanetBandInnerRadius);
    }

    [TestMethod]
    public void Dense_planet_cluster_stays_inside_planet_band()
    {
        var wheel =
            BuildWheel();

        var placements =
            new NatalObjectPlacementEngine()
                .Layout(
                    wheel,
                    Enumerable
                        .Range(
                            0,
                            8)
                        .Select(
                            i =>
                                new NatalObjectLayoutInput(
                                    $"P{i}",
                                    120.0
                                    + i * 0.01))
                        .ToArray());

        foreach (
            var placement
            in placements.Placements)
        {
            Assert.IsTrue(
                placement.VisualRadius
                > wheel.Metrics.AspectRadius);

            Assert.IsTrue(
                placement.VisualRadius
                < wheel.Metrics.DegreeRingRadius);
        }
    }

    [TestMethod]
    public void Scene_has_degree_aspect_and_soul_rings_without_planet_ring()
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
                            120)
                    });

        var scene =
            new NatalWheelSceneBuilder()
                .Build(
                    wheel,
                    placements,
                    new[]
                    {
                        new NatalSceneObjectInput(
                            "Sun",
                            "planet-sun",
                            SceneLayer.BodyLayer)
                    });

        Assert.IsTrue(
            scene.Nodes.Any(
                x =>
                    x.Id
                    == "zodiac-degree-ring"));

        Assert.IsTrue(
            scene.Nodes.Any(
                x =>
                    x.Id
                    == "aspect-anchor-ring"));

        Assert.IsTrue(
            scene.Nodes.Any(
                x =>
                    x.Id
                    == "soul-core"));

        Assert.IsFalse(
            scene.Nodes.Any(
                x =>
                    x.Id
                    == "planet-orbit-ring"));

        Assert.IsFalse(
            scene.Nodes.Any(
                x =>
                    x.Id.StartsWith(
                        "real-mark-",
                        StringComparison.Ordinal)));

        Assert.IsFalse(
            scene.Nodes.Any(
                x =>
                    x.Id.StartsWith(
                        "object-label-",
                        StringComparison.Ordinal)));
    }

    private static NatalWheelLayoutSnapshot
        BuildWheel()
        =>
            new NatalWheelLayoutBuilder()
                .Build(
                    800,
                    800,
                    17,
                    276,
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
}

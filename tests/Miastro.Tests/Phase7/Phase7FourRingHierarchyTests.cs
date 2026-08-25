using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Geometry;
using Miastro.Graphics.Layout;
using Miastro.Graphics.Layout.Placement;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Scene.Natal;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7FourRingHierarchyTests
{
    [TestMethod]
    public void Final_semantic_radii_are_strictly_ordered()
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
    public void Planet_base_radius_lies_inside_planet_band()
    {
        var metrics =
            NatalWheelMetrics.Create(
                800,
                800);

        Assert.IsTrue(
            metrics.PlanetOrbitRadius
            < metrics.DegreeRingRadius);

        Assert.IsTrue(
            metrics.PlanetOrbitRadius
            > metrics.AspectRadius);
    }

    [TestMethod]
    public void Scene_contains_degree_aspect_and_soul_rings_without_planet_ring()
    {
        var wheel =
            BuildWheel();

        var scene =
            new NatalWheelSceneBuilder()
                .Build(
                    wheel,
                    new NatalObjectPlacementSnapshot(
                        Array.Empty<NatalVisualPlacement>()),
                    Array.Empty<NatalSceneObjectInput>());

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
    }

    [TestMethod]
    public void House_cusps_reach_from_aspect_ring_to_outer_radius()
    {
        var wheel =
            BuildWheel();

        foreach (
            var cusp
            in wheel.HouseCusps)
        {
            Assert.AreEqual(
                wheel.Metrics.AspectRadius,
                Distance(
                    wheel.Metrics.Center,
                    cusp.InnerPoint),
                1e-7);

            Assert.AreEqual(
                wheel.Metrics.OuterRadius,
                Distance(
                    wheel.Metrics.Center,
                    cusp.OuterPoint),
                1e-7);
        }
    }

    [TestMethod]
    public void Angle_axes_reach_from_aspect_ring_beyond_outer_radius()
    {
        var wheel =
            BuildWheel();

        foreach (
            var axis
            in wheel.AngleAxes)
        {
            Assert.AreEqual(
                wheel.Metrics.AspectRadius,
                Distance(
                    wheel.Metrics.Center,
                    axis.InnerPoint),
                1e-7);

            Assert.AreEqual(
                wheel.Metrics.OuterRadius
                    + 18.0
                    * wheel.Metrics.Scale,
                Distance(
                    wheel.Metrics.Center,
                    axis.OuterPoint),
                1e-7);

            Assert.IsTrue(
                Distance(
                    wheel.Metrics.Center,
                    axis.OuterPoint)
                > wheel.Metrics.OuterRadius);
        }
    }

    [TestMethod]
    public void House_numbers_remain_inside_planet_band()
    {
        var wheel =
            BuildWheel();

        foreach (
            var cusp
            in wheel.HouseCusps)
        {
            var radius =
                Distance(
                    wheel.Metrics.Center,
                    cusp.HouseNumberPosition);

            Assert.IsTrue(
                radius
                < wheel.Metrics.DegreeRingRadius);

            Assert.IsTrue(
                radius
                > wheel.Metrics.AspectRadius);
        }
    }

    private static NatalWheelLayoutSnapshot
        BuildWheel()
    {
        var cusps =
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
            };

        return new NatalWheelLayoutBuilder()
            .Build(
                800,
                800,
                17,
                cusps[9],
                cusps);
    }

    private static double Distance(
        ChartPoint first,
        ChartPoint second)
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Layout;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7RebalancedInnerGeometryTests
{
    [TestMethod]
    public void Planet_to_aspect_gap_is_visibly_wide()
    {
        var metrics =
            NatalWheelMetrics.Create(
                800,
                800);

        var gap =
            metrics.PlanetOrbitRadius
            - metrics.AspectRadius;

        Assert.IsTrue(
            gap
            >= metrics.OuterRadius * 0.20,
            $"gap={gap}, outer={metrics.OuterRadius}");
    }


    [TestMethod]
    public void Planet_orbit_is_second_semantic_circle()
    {
        var metrics =
            NatalWheelMetrics.Create(
                800,
                800);

        Assert.AreEqual(
            metrics.OuterRadius * 0.70,
            metrics.PlanetOrbitRadius,
            1e-12);

        Assert.AreEqual(
            metrics.HouseOuterRadius,
            metrics.PlanetOrbitRadius,
            1e-12);
    }


    [TestMethod]
    public void Aspects_reach_exactly_house_inner_circle()
    {
        var metrics =
            NatalWheelMetrics.Create(
                800,
                800);

        Assert.AreEqual(
            metrics.HouseInnerRadius,
            metrics.AspectRadius,
            1e-12);
    }

    [TestMethod]
    public void Soul_has_clear_space_inside_aspect_circle()
    {
        var metrics =
            NatalWheelMetrics.Create(
                800,
                800);

        Assert.IsTrue(
            metrics.AspectRadius
            > metrics.SoulRadius * 3.0);
    }
}

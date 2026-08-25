using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Geometry;
using Miastro.Graphics.Layout;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7OuterBoundaryAxisTests
{
    [TestMethod]
    public void All_house_cusps_reach_outer_wheel_radius()
    {
        var wheel =
            BuildWheel();

        foreach (
            var cusp
            in wheel.HouseCusps)
        {
            Assert.AreEqual(
                wheel.Metrics.OuterRadius,
                Distance(
                    wheel.Metrics.Center,
                    cusp.OuterPoint),
                1e-9);
        }
    }

    [TestMethod]
    public void All_four_angle_axes_extend_beyond_outer_wheel_radius()
    {
        var wheel =
            BuildWheel();

        foreach (
            var axis
            in wheel.AngleAxes)
        {
            var radius =
                Distance(
                    wheel.Metrics.Center,
                    axis.OuterPoint);

            Assert.AreEqual(
                wheel.Metrics.OuterRadius
                    + 18.0
                    * wheel.Metrics.Scale,
                radius,
                1e-9);

            Assert.IsTrue(
                radius
                > wheel.Metrics.OuterRadius);
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

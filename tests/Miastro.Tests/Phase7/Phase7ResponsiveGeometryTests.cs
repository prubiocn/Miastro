using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Layout;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7ResponsiveGeometryTests
{
    private static readonly double[] Sizes =
    [
        300.0,
        360.0,
        480.0,
        720.0,
        800.0
    ];

    [TestMethod]
    public void Outer_radius_never_exceeds_half_shortest_side()
    {
        foreach (var size in Sizes)
        {
            var metrics =
                NatalWheelMetrics.Create(
                    size,
                    size);

            Assert.IsTrue(
                metrics.OuterRadius <= size / 2.0,
                $"size={size}, radius={metrics.OuterRadius}");
        }
    }

    [TestMethod]
    public void Outer_circle_remains_inside_rectangular_canvas()
    {
        var cases =
            new[]
            {
                (Width: 300.0, Height: 480.0),
                (Width: 480.0, Height: 300.0),
                (Width: 360.0, Height: 720.0),
                (Width: 720.0, Height: 360.0),
                (Width: 800.0, Height: 600.0)
            };

        foreach (var item in cases)
        {
            var metrics =
                NatalWheelMetrics.Create(
                    item.Width,
                    item.Height);

            var radius =
                metrics.OuterRadius;

            Assert.IsTrue(
                metrics.Center.X - radius >= 0.0);

            Assert.IsTrue(
                metrics.Center.Y - radius >= 0.0);

            Assert.IsTrue(
                metrics.Center.X + radius <= item.Width);

            Assert.IsTrue(
                metrics.Center.Y + radius <= item.Height);
        }
    }

    [TestMethod]
    public void Concentric_radii_remain_strictly_ordered()
    {
        foreach (var size in Sizes)
        {
            var metrics =
                NatalWheelMetrics.Create(
                    size,
                    size);

            Assert.IsTrue(
                metrics.OuterRadius
                > metrics.ZodiacInnerRadius);

            Assert.IsTrue(
                metrics.ZodiacInnerRadius
                > metrics.DegreeInnerRadius);

            Assert.IsTrue(
                metrics.DegreeInnerRadius
                > metrics.HouseOuterRadius);

            Assert.IsTrue(
                metrics.HouseOuterRadius
                > metrics.HouseInnerRadius);

            Assert.IsTrue(
                metrics.HouseInnerRadius
                > metrics.AspectRadius);

            Assert.IsTrue(
                metrics.AspectRadius > 0.0);
        }
    }

    [TestMethod]
    public void Geometry_scales_monotonically()
    {
        var previous =
            0.0;

        foreach (var size in Sizes)
        {
            var radius =
                NatalWheelMetrics.Create(
                    size,
                    size)
                .OuterRadius;

            Assert.IsTrue(
                radius > previous,
                $"size={size}, radius={radius}");

            previous =
                radius;
        }
    }

    [TestMethod]
    public void Metrics_are_deterministic()
    {
        var first =
            NatalWheelMetrics.Create(
                480,
                480);

        for (var i = 0; i < 100; i++)
        {
            Assert.AreEqual(
                first,
                NatalWheelMetrics.Create(
                    480,
                    480));
        }
    }

    [TestMethod]
    public void Center_tracks_physical_canvas()
    {
        var cases =
            new[]
            {
                (Width: 300.0, Height: 480.0),
                (Width: 480.0, Height: 300.0),
                (Width: 360.0, Height: 360.0),
                (Width: 720.0, Height: 540.0),
                (Width: 800.0, Height: 800.0)
            };

        foreach (var item in cases)
        {
            var center =
                NatalWheelMetrics.Create(
                    item.Width,
                    item.Height)
                .Center;

            Assert.AreEqual(
                item.Width / 2.0,
                center.X,
                1e-12);

            Assert.AreEqual(
                item.Height / 2.0,
                center.Y,
                1e-12);
        }
    }
}

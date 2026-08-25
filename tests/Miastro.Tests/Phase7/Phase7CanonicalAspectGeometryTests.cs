using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Geometry;
using Miastro.Graphics.Layout;
using Miastro.Graphics.Layout.Placement;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Scene.Natal.Aspects;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7CanonicalAspectGeometryTests
{
    [TestMethod]
    public void Opposition_180_degrees_is_diametric_and_split_by_soul()
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
                            15.0),

                        new NatalObjectLayoutInput(
                            "B",
                            195.0)
                    });

        var lines =
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
                    })
                .OfType<LineNode>()
                .OrderBy(
                    x => x.Id,
                    StringComparer.Ordinal)
                .ToArray();

        Assert.AreEqual(
            2,
            lines.Length);

        var a =
            placements.Placements
                .Single(x => x.Id == "A");

        var b =
            placements.Placements
                .Single(x => x.Id == "B");

        var delta =
            NatalWheelCoordinates
                .NormalizeDegrees(
                    b.RealScreenAngleDegrees
                    - a.RealScreenAngleDegrees);

        Assert.AreEqual(
            180.0,
            delta,
            1e-9);

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

        // Los cuatro puntos deben estar en la misma recta diametral.
        Assert.AreEqual(
            0.0,
            Cross(
                lines[0].Start,
                lines[0].End,
                wheel.Metrics.Center),
            1e-7);

        Assert.AreEqual(
            0.0,
            Cross(
                lines[1].Start,
                lines[1].End,
                wheel.Metrics.Center),
            1e-7);
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

    private static double Cross(
        ChartPoint a,
        ChartPoint b,
        ChartPoint p)
        =>
            (
                b.X - a.X
            )
            * (
                p.Y - a.Y
            )
            - (
                b.Y - a.Y
            )
            * (
                p.X - a.X
            );

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

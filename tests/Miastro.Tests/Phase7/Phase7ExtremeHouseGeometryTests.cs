using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Layout;
using Miastro.Graphics.Layout.Placement;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Scene.Natal;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7ExtremeHouseGeometryTests
{
    [TestMethod]
    public void Extremely_narrow_house_renders_finite_cusp_and_number()
    {
        var scene =
            BuildScene(
                new double[]
                {
                    0.0,
                    0.05,
                    48.0,
                    82.0,
                    116.0,
                    151.0,
                    181.0,
                    211.0,
                    241.0,
                    271.0,
                    301.0,
                    331.0
                });

        AssertHouseGeometry(
            scene);
    }

    [TestMethod]
    public void Extremely_wide_and_wraparound_houses_render_safely()
    {
        var scene =
            BuildScene(
                new double[]
                {
                    350.0,
                    80.0,
                    110.0,
                    140.0,
                    170.0,
                    200.0,
                    230.0,
                    260.0,
                    290.0,
                    320.0,
                    340.0,
                    345.0
                });

        AssertHouseGeometry(
            scene);
    }

    private static NatalScene BuildScene(
        IReadOnlyList<double> cusps)
    {
        var wheel =
            new NatalWheelLayoutBuilder()
                .Build(
                    800,
                    800,
                    350,
                    80,
                    cusps);

        var placements =
            new NatalObjectPlacementEngine()
                .Layout(
                    wheel,
                    Array.Empty<
                        NatalObjectLayoutInput>());

        return new NatalWheelSceneBuilder()
            .Build(
                wheel,
                placements,
                Array.Empty<
                    NatalSceneObjectInput>());
    }

    private static void AssertHouseGeometry(
        NatalScene scene)
    {
        var cusps =
            scene.Nodes
                .OfType<LineNode>()
                .Where(
                    x =>
                        x.Id.StartsWith(
                            "house-cusp-",
                            StringComparison.Ordinal))
                .ToArray();

        var numbers =
            scene.Nodes
                .OfType<TextNode>()
                .Where(
                    x =>
                        x.Id.StartsWith(
                            "house-number-",
                            StringComparison.Ordinal))
                .ToArray();

        Assert.AreEqual(
            12,
            cusps.Length);

        Assert.AreEqual(
            12,
            numbers.Length);

        foreach (
            var cusp
            in cusps)
        {
            Assert.IsTrue(
                double.IsFinite(
                    cusp.Start.X));

            Assert.IsTrue(
                double.IsFinite(
                    cusp.Start.Y));

            Assert.IsTrue(
                double.IsFinite(
                    cusp.End.X));

            Assert.IsTrue(
                double.IsFinite(
                    cusp.End.Y));

            AssertInside(
                scene,
                cusp.Start.X,
                cusp.Start.Y);

            AssertInside(
                scene,
                cusp.End.X,
                cusp.End.Y);
        }

        foreach (
            var number
            in numbers)
        {
            Assert.IsTrue(
                double.IsFinite(
                    number.Position.X));

            Assert.IsTrue(
                double.IsFinite(
                    number.Position.Y));

            AssertInside(
                scene,
                number.Position.X,
                number.Position.Y);
        }
    }

    private static void AssertInside(
        NatalScene scene,
        double x,
        double y)
    {
        Assert.IsTrue(
            x >= 0.0
            && x <= scene.Width,
            $"x={x}");

        Assert.IsTrue(
            y >= 0.0
            && y <= scene.Height,
            $"y={y}");
    }
}

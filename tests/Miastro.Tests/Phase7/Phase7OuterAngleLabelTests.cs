using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Layout;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Scene.Natal;
using Miastro.Graphics.Styles;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7OuterAngleLabelTests
{
    [TestMethod]
    public void Four_angle_axes_extend_beyond_outer_radius()
    {
        var wheel =
            BuildWheel();

        foreach (
            var axis
            in wheel.AngleAxes)
        {
            Assert.IsTrue(
                Distance(
                    wheel.Metrics.Center,
                    axis.OuterPoint)
                > wheel.Metrics.OuterRadius);
        }
    }

    [TestMethod]
    public void Asc_label_is_above_exterior_line_and_inside_canvas()
    {
        var context =
            BuildScene();

        var axis =
            context.Wheel.AngleAxes.Single(
                x =>
                    x.Kind
                    == NatalAngleKind.Ascendant);

        var label =
            context.Scene.Nodes
                .OfType<TextNode>()
                .Single(
                    x =>
                        x.Id
                        == "angle-label-ASC");

        Assert.IsTrue(
            label.Position.Y
            < axis.OuterPoint.Y);

        Assert.IsTrue(
            label.Bounds.Left
            >= 0.0);

        Assert.AreEqual(
            NatalSceneStyleKeys.AngleLabelMajor,
            label.StyleKey);
    }

    [TestMethod]
    public void Mc_label_is_beside_exterior_line()
    {
        var context =
            BuildScene();

        var axis =
            context.Wheel.AngleAxes.Single(
                x =>
                    x.Kind
                    == NatalAngleKind.Midheaven);

        var label =
            context.Scene.Nodes
                .OfType<TextNode>()
                .Single(
                    x =>
                        x.Id
                        == "angle-label-MC");

        Assert.IsTrue(
            label.Position.X
            > axis.OuterPoint.X);

        Assert.AreEqual(
            axis.OuterPoint.Y,
            label.Position.Y,
            1e-9);

        Assert.AreEqual(
            NatalSceneStyleKeys.AngleLabelMajor,
            label.StyleKey);
    }

    [TestMethod]
    public void Dsc_is_above_exterior_line_and_inside_canvas()
    {
        var context =
            BuildScene();

        var axis =
            context.Wheel.AngleAxes.Single(
                x =>
                    x.Kind
                    == NatalAngleKind.Descendant);

        var label =
            context.Scene.Nodes
                .OfType<TextNode>()
                .Single(
                    x =>
                        x.Id
                        == "angle-label-DSC");

        Assert.IsTrue(
            label.Position.Y
            < axis.OuterPoint.Y);

        Assert.IsTrue(
            label.Bounds.Right
            <= context.Wheel.Metrics.Width);

        Assert.AreEqual(
            NatalSceneStyleKeys.AngleLabelMinor,
            label.StyleKey);
    }

    [TestMethod]
    public void Ic_is_beside_exterior_line_and_inside_canvas()
    {
        var context =
            BuildScene();

        var axis =
            context.Wheel.AngleAxes.Single(
                x =>
                    x.Kind
                    == NatalAngleKind.ImumCoeli);

        var label =
            context.Scene.Nodes
                .OfType<TextNode>()
                .Single(
                    x =>
                        x.Id
                        == "angle-label-IC");

        Assert.IsTrue(
            label.Position.X
            > axis.OuterPoint.X);

        Assert.IsTrue(
            label.Position.Y
            < axis.OuterPoint.Y);

        Assert.IsTrue(
            label.Bounds.Bottom
            <= context.Wheel.Metrics.Height);

        Assert.AreEqual(
            NatalSceneStyleKeys.AngleLabelMinor,
            label.StyleKey);
    }

    private static (
        NatalWheelLayoutSnapshot Wheel,
        NatalScene Scene)
        BuildScene()
    {
        var wheel =
            BuildWheel();

        var scene =
            new NatalWheelSceneBuilder()
                .Build(
                    wheel,
                    new Miastro.Graphics.Layout.Placement
                        .NatalObjectPlacementSnapshot(
                            Array.Empty<
                                Miastro.Graphics.Layout.Placement
                                    .NatalVisualPlacement>()),
                    Array.Empty<
                        NatalSceneObjectInput>());

        return (
            wheel,
            scene);
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

    private static double Distance(
        Miastro.Graphics.Geometry.ChartPoint a,
        Miastro.Graphics.Geometry.ChartPoint b)
    {
        var dx =
            a.X - b.X;

        var dy =
            a.Y - b.Y;

        return Math.Sqrt(
            dx * dx
            + dy * dy);
    }
}

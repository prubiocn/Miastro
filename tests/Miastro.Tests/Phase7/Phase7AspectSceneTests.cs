using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Geometry;
using Miastro.Graphics.Layout;
using Miastro.Graphics.Layout.Placement;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Scene.Natal;
using Miastro.Graphics.Scene.Natal.Aspects;
using Miastro.Graphics.Styles;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7AspectSceneTests
{
    [TestMethod]
    public void Supplied_aspects_become_scene_lines()
    {
        var context =
            BuildContext();

        var nodes =
            new NatalAspectSceneBuilder()
                .Build(
                    context.Wheel,
                    context.Placements,
                    new[]
                    {
                        new NatalAspectSceneInput(
                            "sun-moon",
                            "Sun",
                            "Moon",
                            NatalAspectVisualClass.Major),

                        new NatalAspectSceneInput(
                            "sun-mars",
                            "Sun",
                            "Mars",
                            NatalAspectVisualClass.Secondary)
                    });

        Assert.AreEqual(
            2,
            nodes.Count);

        Assert.IsTrue(
            nodes.All(
                x => x.Layer
                    == SceneLayer.AspectLayer));
    }

    [TestMethod]
    public void Major_and_secondary_styles_are_distinct()
    {
        var context =
            BuildContext();

        var nodes =
            new NatalAspectSceneBuilder()
                .Build(
                    context.Wheel,
                    context.Placements,
                    new[]
                    {
                        new NatalAspectSceneInput(
                            "major",
                            "Sun",
                            "Moon",
                            NatalAspectVisualClass.Major),

                        new NatalAspectSceneInput(
                            "secondary",
                            "Sun",
                            "Mars",
                            NatalAspectVisualClass.Secondary)
                    });

        Assert.AreEqual(
            NatalSceneStyleKeys.AspectMajor,
            nodes.Single(
                    x => x.Id == "aspect-major")
                .StyleKey);

        Assert.AreEqual(
            NatalSceneStyleKeys.AspectSecondary,
            nodes.Single(
                    x => x.Id == "aspect-secondary")
                .StyleKey);
    }

    [TestMethod]
    public void Hidden_aspects_generate_no_nodes()
    {
        var context =
            BuildContext();

        var nodes =
            new NatalAspectSceneBuilder()
                .Build(
                    context.Wheel,
                    context.Placements,
                    new[]
                    {
                        new NatalAspectSceneInput(
                            "sun-moon",
                            "Sun",
                            "Moon",
                            NatalAspectVisualClass.Major)
                    },
                    new NatalAspectSceneOptions(
                        ShowAspects: false));

        Assert.AreEqual(
            0,
            nodes.Count);
    }

    [TestMethod]
    public void Aspect_endpoints_use_real_not_displaced_positions()
    {
        var context =
            BuildContext(
                coincidentSunMoon: true);

        var displaced =
            context.Placements
                .Placements
                .Single(
                    x => x.RadialLevel != 0);

        Assert.AreNotEqual(
            displaced.RealAnchor,
            displaced.VisualCenter);

        var line =
            (LineNode)new NatalAspectSceneBuilder()
                .Build(
                    context.Wheel,
                    context.Placements,
                    new[]
                    {
                        new NatalAspectSceneInput(
                            "displaced-mars",
                            displaced.Id,
                            "Mars",
                            NatalAspectVisualClass.Major)
                    })
                .Single();

        var expected =
            NatalWheelCoordinates
                .PointOnCircle(
                    context.Wheel.Metrics.Center,
                    context.Wheel.Metrics.AspectRadius,
                    displaced.RealScreenAngleDegrees);

        Assert.AreEqual(
            expected,
            line.Start);

        Assert.AreNotEqual(
            displaced.VisualCenter,
            line.Start);
    }

    [TestMethod]
    public void Aspect_endpoints_stay_on_inner_aspect_radius()
    {
        var context =
            BuildContext();

        var line =
            (LineNode)new NatalAspectSceneBuilder()
                .Build(
                    context.Wheel,
                    context.Placements,
                    new[]
                    {
                        new NatalAspectSceneInput(
                            "sun-moon",
                            "Sun",
                            "Moon",
                            NatalAspectVisualClass.Major)
                    })
                .Single();

        Assert.AreEqual(
            context.Wheel.Metrics.AspectRadius,
            Distance(
                context.Wheel.Metrics.Center,
                line.Start),
            1e-9);

        Assert.AreEqual(
            context.Wheel.Metrics.AspectRadius,
            Distance(
                context.Wheel.Metrics.Center,
                line.End),
            1e-9);
    }

    [TestMethod]
    public void Missing_hidden_object_endpoint_skips_aspect()
    {
        var context =
            BuildContext();

        var nodes =
            new NatalAspectSceneBuilder()
                .Build(
                    context.Wheel,
                    context.Placements,
                    new[]
                    {
                        new NatalAspectSceneInput(
                            "sun-hidden",
                            "Sun",
                            "NotVisible",
                            NatalAspectVisualClass.Major)
                    });

        Assert.AreEqual(
            0,
            nodes.Count);
    }

    [TestMethod]
    public void Aspect_scene_is_deterministic_and_input_order_independent()
    {
        var context =
            BuildContext();

        var aspects =
            new[]
            {
                new NatalAspectSceneInput(
                    "c",
                    "Moon",
                    "Mars",
                    NatalAspectVisualClass.Secondary),

                new NatalAspectSceneInput(
                    "a",
                    "Sun",
                    "Moon",
                    NatalAspectVisualClass.Major),

                new NatalAspectSceneInput(
                    "b",
                    "Sun",
                    "Mars",
                    NatalAspectVisualClass.Major)
            };

        var builder =
            new NatalAspectSceneBuilder();

        var first =
            Describe(
                builder.Build(
                    context.Wheel,
                    context.Placements,
                    aspects));

        var reversed =
            Describe(
                builder.Build(
                    context.Wheel,
                    context.Placements,
                    aspects.Reverse().ToArray()));

        Assert.AreEqual(
            first,
            reversed);
    }

    [TestMethod]
    public void Full_natal_scene_can_toggle_aspects_without_relayout()
    {
        var context =
            BuildContext();

        var objects =
            new[]
            {
                new NatalSceneObjectInput(
                    "Sun",
                    "planet-sun",
                    SceneLayer.BodyLayer),

                new NatalSceneObjectInput(
                    "Moon",
                    "planet-moon",
                    SceneLayer.BodyLayer),

                new NatalSceneObjectInput(
                    "Mars",
                    "planet-mars",
                    SceneLayer.BodyLayer)
            };

        var aspects =
            new[]
            {
                new NatalAspectSceneInput(
                    "sun-moon",
                    "Sun",
                    "Moon",
                    NatalAspectVisualClass.Major)
            };

        var builder =
            new NatalWheelSceneBuilder();

        var visible =
            builder.Build(
                context.Wheel,
                context.Placements,
                objects,
                aspects,
                new NatalAspectSceneOptions(true));

        var hidden =
            builder.Build(
                context.Wheel,
                context.Placements,
                objects,
                aspects,
                new NatalAspectSceneOptions(false));

        Assert.AreEqual(
            1,
            visible.Nodes.Count(
                x => x.Layer
                    == SceneLayer.AspectLayer));

        Assert.AreEqual(
            0,
            hidden.Nodes.Count(
                x => x.Layer
                    == SceneLayer.AspectLayer));

        var visibleObjects =
            DescribeObjects(
                visible);

        var hiddenObjects =
            DescribeObjects(
                hidden);

        Assert.AreEqual(
            visibleObjects,
            hiddenObjects);
    }

    private static (
        NatalWheelLayoutSnapshot Wheel,
        NatalObjectPlacementSnapshot Placements)
        BuildContext(
            bool coincidentSunMoon = false)
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
                            coincidentSunMoon
                                ? 120
                                : 210),

                        new NatalObjectLayoutInput(
                            "Mars",
                            20)
                    });

        return (
            wheel,
            placements);
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

    private static string Describe(
        IReadOnlyList<SceneNode> nodes)
        =>
            string.Join(
                "\n",
                nodes.Select(
                    x =>
                    {
                        var line =
                            (LineNode)x;

                        return FormattableString.Invariant(
                            $"{line.Id}|{line.StyleKey}|{line.Start.X:F9}|{line.Start.Y:F9}|{line.End.X:F9}|{line.End.Y:F9}");
                    }));

    private static string DescribeObjects(
        NatalScene scene)
        =>
            string.Join(
                "\n",
                scene.Nodes
                    .Where(
                        x =>
                            x.Layer
                                is SceneLayer.BodyLayer
                                or SceneLayer.PointLayer)
                    .OrderBy(
                        x => x.Id,
                        StringComparer.Ordinal)
                    .Select(
                        x => $"{x.Id}|{x.StyleKey}"));
}

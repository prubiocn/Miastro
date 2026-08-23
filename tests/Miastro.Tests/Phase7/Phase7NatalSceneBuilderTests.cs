using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Layout;
using Miastro.Graphics.Layout.Placement;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Scene.Natal;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7NatalSceneBuilderTests
{
    [TestMethod]
    public void Scene_contains_twelve_zodiac_glyphs()
    {
        var scene =
            BuildScene();

        Assert.AreEqual(
            12,
            scene.Nodes
                .OfType<GlyphNode>()
                .Count(
                    x => x.Id.StartsWith(
                        "zodiac-glyph-",
                        StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Scene_contains_all_360_degree_ticks()
    {
        var scene =
            BuildScene();

        Assert.AreEqual(
            360,
            scene.Nodes
                .OfType<LineNode>()
                .Count(
                    x => x.Id.StartsWith(
                        "degree-",
                        StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Scene_contains_twelve_house_cusps_and_numbers()
    {
        var scene =
            BuildScene();

        Assert.AreEqual(
            12,
            scene.Nodes
                .OfType<LineNode>()
                .Count(
                    x => x.Id.StartsWith(
                        "house-cusp-",
                        StringComparison.Ordinal)));

        Assert.AreEqual(
            12,
            scene.Nodes
                .OfType<TextNode>()
                .Count(
                    x => x.Id.StartsWith(
                        "house-number-",
                        StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Scene_contains_all_four_angle_axes()
    {
        var scene =
            BuildScene();

        var axes =
            scene.Nodes
                .OfType<LineNode>()
                .Where(
                    x => x.Id.StartsWith(
                        "angle-axis-",
                        StringComparison.Ordinal))
                .Select(x => x.Id)
                .OrderBy(
                    x => x,
                    StringComparer.Ordinal)
                .ToArray();

        CollectionAssert.AreEqual(
            new[]
            {
                "angle-axis-ASC",
                "angle-axis-DSC",
                "angle-axis-IC",
                "angle-axis-MC"
            },
            axes);
    }

    [TestMethod]
    public void Each_object_has_real_mark_and_visual_glyph()
    {
        var scene =
            BuildScene();

        foreach (
            var id
            in new[] { "Sun", "Moon" })
        {
            Assert.AreEqual(
                1,
                scene.Nodes.Count(
                    x => x.Id
                        == $"real-mark-{id}"));

            Assert.AreEqual(
                1,
                scene.Nodes.Count(
                    x => x.Id
                        == $"object-glyph-{id}"));
        }
    }

    [TestMethod]
    public void Displaced_object_has_leader_line()
    {
        var scene =
            BuildScene();

        Assert.AreEqual(
            1,
            scene.Nodes
                .OfType<LineNode>()
                .Count(
                    x => x.Id.StartsWith(
                        "leader-",
                        StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Real_mark_and_visual_glyph_are_distinct_when_displaced()
    {
        var scene =
            BuildScene();

        var leader =
            scene.Nodes
                .OfType<LineNode>()
                .Single(
                    x => x.Id.StartsWith(
                        "leader-",
                        StringComparison.Ordinal));

        var id =
            leader.Id["leader-".Length..];

        var mark =
            scene.Nodes
                .OfType<CircleNode>()
                .Single(
                    x => x.Id
                        == $"real-mark-{id}");

        var glyph =
            scene.Nodes
                .OfType<GlyphNode>()
                .Single(
                    x => x.Id
                        == $"object-glyph-{id}");

        Assert.AreNotEqual(
            mark.Center,
            glyph.Position);
    }

    [TestMethod]
    public void Scene_generation_is_deterministic()
    {
        var first =
            Describe(
                BuildScene());

        for (var i = 0; i < 50; i++)
        {
            Assert.AreEqual(
                first,
                Describe(
                    BuildScene()));
        }
    }

    [TestMethod]
    public void Ordered_scene_respects_layer_order()
    {
        var scene =
            BuildScene();

        var values =
            scene.OrderedNodes
                .Select(x => (int)x.Layer)
                .ToArray();

        for (
            var i = 1;
            i < values.Length;
            i++)
        {
            Assert.IsTrue(
                values[i - 1]
                <= values[i]);
        }
    }

    private static NatalScene BuildScene()
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
                            120.0),

                        new NatalObjectLayoutInput(
                            "Moon",
                            120.0)
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
                });
    }

    private static string Describe(
        NatalScene scene)
        =>
            string.Join(
                "\n",
                scene.OrderedNodes.Select(
                    x => x switch
                    {
                        CircleNode circle =>
                            $"{circle.Layer}|{circle.Id}|C|{circle.Center.X:F9}|{circle.Center.Y:F9}|{circle.Radius:F9}",

                        LineNode line =>
                            $"{line.Layer}|{line.Id}|L|{line.Start.X:F9}|{line.Start.Y:F9}|{line.End.X:F9}|{line.End.Y:F9}",

                        GlyphNode glyph =>
                            $"{glyph.Layer}|{glyph.Id}|G|{glyph.GlyphKey}|{glyph.Position.X:F9}|{glyph.Position.Y:F9}",

                        TextNode text =>
                            $"{text.Layer}|{text.Id}|T|{text.Text}|{text.Position.X:F9}|{text.Position.Y:F9}",

                        ArcNode arc =>
                            $"{arc.Layer}|{arc.Id}|A|{arc.StartAngleDegrees:F9}|{arc.SweepAngleDegrees:F9}",

                        _ =>
                            $"{x.Layer}|{x.Id}|{x.GetType().Name}"
                    }));
}

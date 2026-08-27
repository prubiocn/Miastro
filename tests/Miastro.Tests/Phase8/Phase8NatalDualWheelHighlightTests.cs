using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Geometry;
using Miastro.Graphics.Interaction;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Skia.Rendering;
using Miastro.Graphics.Styles;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalDualWheelHighlightTests
{
    [TestMethod]
    public void Neutral_selection_adds_no_overlay()
    {
        var result =
            Overlay().Apply(
                BaseScene());

        Assert.AreEqual(
            0,
            SelectionNodes(result).Count);
    }

    [TestMethod]
    public void Simple_selection_highlights_exactly_one_object()
    {
        var result =
            Overlay().Apply(
                BaseScene(),
                new[]
                {
                    "Sun"
                });

        var selected =
            SelectionNodes(result);

        Assert.AreEqual(
            1,
            selected.Count);

        Assert.AreEqual(
            "selection-object-Sun",
            selected[0].Id);
    }

    [TestMethod]
    public void Dual_selection_highlights_both_objects_and_active_aspect()
    {
        var result =
            Overlay().Apply(
                BaseScene(),
                new[]
                {
                    "Sun",
                    "Moon"
                },
                "Sun",
                "Moon");

        var ids =
            SelectionNodes(result)
                .Select(x => x.Id)
                .ToArray();

        CollectionAssert.Contains(
            ids,
            "selection-object-Sun");

        CollectionAssert.Contains(
            ids,
            "selection-object-Moon");

        CollectionAssert.Contains(
            ids,
            "selection-aspect-Sun-Moon-Trine-1");

        CollectionAssert.Contains(
            ids,
            "selection-aspect-Sun-Moon-Trine-2");

        CollectionAssert.DoesNotContain(
            ids,
            "selection-aspect-Sun-Mars-Square");
    }

    [TestMethod]
    public void Reversed_pair_orientation_matches_same_aspect()
    {
        var result =
            Overlay().Apply(
                BaseScene(),
                new[]
                {
                    "Moon",
                    "Sun"
                },
                "Moon",
                "Sun");

        Assert.AreEqual(
            2,
            SelectionNodes(result)
                .OfType<LineNode>()
                .Count(
                    node =>
                        node.StyleKey
                            == NatalSceneStyleKeys
                                .InteractionSelected));
    }

    [TestMethod]
    public void Every_selection_node_uses_interaction_overlay_style()
    {
        var result =
            Overlay().Apply(
                BaseScene(),
                new[]
                {
                    "Sun",
                    "Moon"
                },
                "Sun",
                "Moon");

        var selectionNodes =
            SelectionNodes(result);

        foreach (
            var node
            in selectionNodes)
        {
            Assert.AreEqual(
                SceneLayer.InteractionOverlay,
                node.Layer);

            Assert.IsTrue(
                node.StyleKey
                    == NatalSceneStyleKeys
                        .InteractionSelected
                || node.StyleKey
                    == NatalSceneStyleKeys
                        .InteractionDimmed);
        }

        Assert.IsTrue(
            selectionNodes.Any(
                node =>
                    node.StyleKey
                        == NatalSceneStyleKeys
                            .InteractionSelected));

        Assert.IsTrue(
            selectionNodes.Any(
                node =>
                    node.StyleKey
                        == NatalSceneStyleKeys
                            .InteractionDimmed));
    }

    [TestMethod]
    public void Base_geometry_and_base_styles_are_not_mutated()
    {
        var original =
            BaseScene();

        var originalDescription =
            DescribeBase(
                original);

        var result =
            Overlay().Apply(
                original,
                new[]
                {
                    "Sun",
                    "Moon"
                },
                "Sun",
                "Moon");

        Assert.AreEqual(
            originalDescription,
            DescribeBase(
                result));
    }

    [TestMethod]
    public void Applying_overlay_twice_is_idempotent()
    {
        var first =
            Overlay().Apply(
                BaseScene(),
                new[]
                {
                    "Sun",
                    "Moon"
                },
                "Sun",
                "Moon");

        var second =
            Overlay().Apply(
                first,
                new[]
                {
                    "Sun",
                    "Moon"
                },
                "Sun",
                "Moon");

        CollectionAssert.AreEqual(
            SelectionNodes(first)
                .Select(x => x.Id)
                .ToArray(),
            SelectionNodes(second)
                .Select(x => x.Id)
                .ToArray());
    }

    [TestMethod]
    public void Highlighted_render_is_deterministic_and_differs_from_neutral()
    {
        var renderer =
            new SkiaNatalSceneRenderer();

        var neutral =
            renderer.RenderPng(
                BaseScene(),
                400,
                400);

        var highlightedScene =
            Overlay().Apply(
                BaseScene(),
                new[]
                {
                    "Sun",
                    "Moon"
                },
                "Sun",
                "Moon");

        var first =
            renderer.RenderPng(
                highlightedScene,
                400,
                400);

        var second =
            renderer.RenderPng(
                highlightedScene,
                400,
                400);

        CollectionAssert.AreEqual(
            first,
            second);

        Assert.IsFalse(
            neutral.SequenceEqual(
                first));
    }

    [TestMethod]
    public void Missing_hidden_object_is_ignored_without_error()
    {
        var result =
            Overlay().Apply(
                BaseScene(),
                new[]
                {
                    "Sun",
                    "NotVisible"
                });

        Assert.AreEqual(
            1,
            SelectionNodes(result).Count);
    }

    private static NatalSceneSelectionOverlay
        Overlay()
        =>
            new();

    private static NatalScene BaseScene()
    {
        var sun =
            new GlyphNode(
                "object-glyph-Sun",
                SceneLayer.BodyLayer,
                "planet-sun",
                new ChartPoint(
                    110,
                    100),
                28,
                new ChartRect(
                    96,
                    86,
                    28,
                    28))
            {
                StyleKey =
                    NatalSceneStyleKeys.BodyGlyph
            };

        var moon =
            new GlyphNode(
                "object-glyph-Moon",
                SceneLayer.BodyLayer,
                "planet-moon",
                new ChartPoint(
                    290,
                    100),
                28,
                new ChartRect(
                    276,
                    86,
                    28,
                    28))
            {
                StyleKey =
                    NatalSceneStyleKeys.BodyGlyph
            };

        var active1 =
            new LineNode(
                "aspect-Sun-Moon-Trine-1",
                SceneLayer.AspectLayer,
                new ChartPoint(
                    130,
                    150),
                new ChartPoint(
                    190,
                    190))
            {
                StyleKey =
                    NatalSceneStyleKeys.AspectMajor
            };

        var active2 =
            new LineNode(
                "aspect-Sun-Moon-Trine-2",
                SceneLayer.AspectLayer,
                new ChartPoint(
                    210,
                    210),
                new ChartPoint(
                    270,
                    250))
            {
                StyleKey =
                    NatalSceneStyleKeys.AspectMajor
            };

        var unrelated =
            new LineNode(
                "aspect-Sun-Mars-Square",
                SceneLayer.AspectLayer,
                new ChartPoint(
                    100,
                    280),
                new ChartPoint(
                    300,
                    280))
            {
                StyleKey =
                    NatalSceneStyleKeys.AspectMajor
            };

        return new NatalScene(
            400,
            400,
            new SceneNode[]
            {
                sun,
                moon,
                active1,
                active2,
                unrelated
            });
    }

    private static IReadOnlyList<SceneNode>
        SelectionNodes(
            NatalScene scene)
        =>
            scene.Nodes
                .Where(
                    x =>
                        x.Layer
                            == SceneLayer.InteractionOverlay
                        && x.Id.StartsWith(
                            "selection-",
                            StringComparison.Ordinal))
                .OrderBy(
                    x =>
                        x.Id,
                    StringComparer.Ordinal)
                .ToArray();

    private static string DescribeBase(
        NatalScene scene)
        =>
            string.Join(
                "\n",
                scene.Nodes
                    .Where(
                        x =>
                            !x.Id.StartsWith(
                                "selection-",
                                StringComparison.Ordinal))
                    .OrderBy(
                        x =>
                            x.Id,
                        StringComparer.Ordinal)
                    .Select(
                        x =>
                            x switch
                            {
                                GlyphNode glyph =>
                                    string.Join(
                                        "|",
                                        glyph.Id,
                                        glyph.Layer,
                                        glyph.StyleKey,
                                        glyph.GlyphKey,
                                        glyph.Position,
                                        glyph.Size,
                                        glyph.Bounds),

                                LineNode line =>
                                    string.Join(
                                        "|",
                                        line.Id,
                                        line.Layer,
                                        line.StyleKey,
                                        line.Start,
                                        line.End),

                                _ =>
                                    string.Join(
                                        "|",
                                        x.Id,
                                        x.Layer,
                                        x.StyleKey)
                            }));
}

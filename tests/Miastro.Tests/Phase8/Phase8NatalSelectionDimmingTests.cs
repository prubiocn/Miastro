using Miastro.Graphics.Geometry;
using Miastro.Graphics.Interaction;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Skia.Rendering;
using Miastro.Graphics.Styles;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalSelectionDimmingTests
{
    [TestMethod]
    public void Neutral_selection_adds_no_dimming()
    {
        var result =
            Overlay().Apply(
                Scene());

        Assert.AreEqual(
            0,
            DimmedNodes(result).Count);
    }

    [TestMethod]
    public void Simple_selection_does_not_dim_other_objects()
    {
        var result =
            Overlay().Apply(
                Scene(),
                new[]
                {
                    "Sun"
                });

        Assert.AreEqual(
            0,
            DimmedNodes(result).Count);

        Assert.AreEqual(
            1,
            SelectedNodes(result).Count);
    }

    [TestMethod]
    public void Dual_aspect_selection_dims_unselected_objects()
    {
        var result =
            Overlay().Apply(
                Scene(),
                new[]
                {
                    "Sun",
                    "Moon"
                },
                "Sun",
                "Moon");

        var ids =
            DimmedNodes(result)
                .Select(
                    node =>
                        node.Id)
                .ToArray();

        CollectionAssert.Contains(
            ids,
            "selection-dim-object-glyph-Mars");

        CollectionAssert.DoesNotContain(
            ids,
            "selection-dim-object-glyph-Sun");

        CollectionAssert.DoesNotContain(
            ids,
            "selection-dim-object-glyph-Moon");
    }

    [TestMethod]
    public void Dual_aspect_selection_dims_unrelated_aspects()
    {
        var result =
            Overlay().Apply(
                Scene(),
                new[]
                {
                    "Sun",
                    "Moon"
                },
                "Sun",
                "Moon");

        var ids =
            DimmedNodes(result)
                .OfType<LineNode>()
                .Select(
                    node =>
                        node.Id)
                .ToArray();

        CollectionAssert.Contains(
            ids,
            "selection-dim-aspect-Sun-Mars-Square");

        CollectionAssert.DoesNotContain(
            ids,
            "selection-dim-aspect-Sun-Moon-Trine-1");

        CollectionAssert.DoesNotContain(
            ids,
            "selection-dim-aspect-Sun-Moon-Trine-2");
    }

    [TestMethod]
    public void Active_pair_and_aspect_remain_selected()
    {
        var result =
            Overlay().Apply(
                Scene(),
                new[]
                {
                    "Sun",
                    "Moon"
                },
                "Sun",
                "Moon");

        var selected =
            SelectedNodes(result)
                .Select(
                    node =>
                        node.Id)
                .ToArray();

        CollectionAssert.Contains(
            selected,
            "selection-object-Sun");

        CollectionAssert.Contains(
            selected,
            "selection-object-Moon");

        CollectionAssert.Contains(
            selected,
            "selection-aspect-Sun-Moon-Trine-1");

        CollectionAssert.Contains(
            selected,
            "selection-aspect-Sun-Moon-Trine-2");
    }

    [TestMethod]
    public void Reversed_pair_has_identical_dimming_targets()
    {
        var forward =
            Overlay().Apply(
                Scene(),
                new[]
                {
                    "Sun",
                    "Moon"
                },
                "Sun",
                "Moon");

        var reverse =
            Overlay().Apply(
                Scene(),
                new[]
                {
                    "Moon",
                    "Sun"
                },
                "Moon",
                "Sun");

        CollectionAssert.AreEqual(
            DimmedNodes(forward)
                .Select(
                    node =>
                        node.Id)
                .ToArray(),
            DimmedNodes(reverse)
                .Select(
                    node =>
                        node.Id)
                .ToArray());
    }

    [TestMethod]
    public void Dimming_does_not_modify_base_geometry_or_styles()
    {
        var original =
            Scene();

        var before =
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
            before,
            DescribeBase(
                result));
    }

    [TestMethod]
    public void Dimming_is_idempotent()
    {
        var first =
            Overlay().Apply(
                Scene(),
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
            InteractionNodes(first)
                .Select(
                    node =>
                        $"{node.Id}|{node.StyleKey}")
                .ToArray(),
            InteractionNodes(second)
                .Select(
                    node =>
                        $"{node.Id}|{node.StyleKey}")
                .ToArray());
    }

    [TestMethod]
    public void Dimmed_style_is_registered_with_lower_opacity()
    {
        var catalog =
            new NatalSceneStyleCatalog();

        var dimmed =
            catalog.GetRequired(
                NatalSceneStyleKeys
                    .InteractionDimmed);

        var selected =
            catalog.GetRequired(
                NatalSceneStyleKeys
                    .InteractionSelected);

        Assert.IsTrue(
            dimmed.Opacity < selected.Opacity);

        Assert.AreNotEqual(
            dimmed.Key,
            selected.Key);
    }

    [TestMethod]
    public void Dual_dimming_render_is_deterministic()
    {
        var renderer =
            new SkiaNatalSceneRenderer();

        var scene =
            Overlay().Apply(
                Scene(),
                new[]
                {
                    "Sun",
                    "Moon"
                },
                "Sun",
                "Moon");

        var first =
            renderer.RenderPng(
                scene,
                400,
                400);

        var second =
            renderer.RenderPng(
                scene,
                400,
                400);

        CollectionAssert.AreEqual(
            first,
            second);
    }

    private static NatalSceneSelectionOverlay
        Overlay()
        =>
            new();

    private static NatalScene Scene()
    {
        var sun =
            Glyph(
                "Sun",
                105,
                100);

        var moon =
            Glyph(
                "Moon",
                295,
                100);

        var mars =
            Glyph(
                "Mars",
                200,
                310);

        var active1 =
            Line(
                "aspect-Sun-Moon-Trine-1",
                130,
                145,
                190,
                190);

        var active2 =
            Line(
                "aspect-Sun-Moon-Trine-2",
                210,
                210,
                270,
                255);

        var unrelated =
            Line(
                "aspect-Sun-Mars-Square",
                115,
                270,
                285,
                270);

        return new NatalScene(
            400,
            400,
            new SceneNode[]
            {
                sun,
                moon,
                mars,
                active1,
                active2,
                unrelated
            });
    }

    private static GlyphNode Glyph(
        string id,
        double x,
        double y)
        => new(
            $"object-glyph-{id}",
            SceneLayer.BodyLayer,
            id switch
            {
                "Sun" =>
                    "planet-sun",

                "Moon" =>
                    "planet-moon",

                _ =>
                    "planet-mars"
            },
            new ChartPoint(
                x,
                y),
            28,
            new ChartRect(
                x - 14,
                y - 14,
                28,
                28))
        {
            StyleKey =
                NatalSceneStyleKeys
                    .BodyGlyph
        };

    private static LineNode Line(
        string id,
        double x1,
        double y1,
        double x2,
        double y2)
        => new(
            id,
            SceneLayer.AspectLayer,
            new ChartPoint(
                x1,
                y1),
            new ChartPoint(
                x2,
                y2))
        {
            StyleKey =
                NatalSceneStyleKeys
                    .AspectMajor
        };

    private static IReadOnlyList<SceneNode>
        InteractionNodes(
            NatalScene scene)
        =>
            scene.Nodes
                .Where(
                    node =>
                        node.Layer
                            == SceneLayer
                                .InteractionOverlay
                        && node.Id.StartsWith(
                            "selection-",
                            StringComparison.Ordinal))
                .OrderBy(
                    node =>
                        node.Id,
                    StringComparer.Ordinal)
                .ToArray();

    private static IReadOnlyList<SceneNode>
        DimmedNodes(
            NatalScene scene)
        =>
            InteractionNodes(
                scene)
                .Where(
                    node =>
                        node.StyleKey
                            == NatalSceneStyleKeys
                                .InteractionDimmed)
                .ToArray();

    private static IReadOnlyList<SceneNode>
        SelectedNodes(
            NatalScene scene)
        =>
            InteractionNodes(
                scene)
                .Where(
                    node =>
                        node.StyleKey
                            == NatalSceneStyleKeys
                                .InteractionSelected)
                .ToArray();

    private static string DescribeBase(
        NatalScene scene)
        =>
            string.Join(
                "\n",
                scene.Nodes
                    .Where(
                        node =>
                            !node.Id.StartsWith(
                                "selection-",
                                StringComparison.Ordinal))
                    .OrderBy(
                        node =>
                            node.Id,
                        StringComparer.Ordinal)
                    .Select(
                        node =>
                            node switch
                            {
                                GlyphNode glyph =>
                                    $"{glyph.Id}|{glyph.Layer}|"
                                    + $"{glyph.StyleKey}|"
                                    + $"{glyph.Position.X:F6}|"
                                    + $"{glyph.Position.Y:F6}|"
                                    + $"{glyph.Size:F6}",

                                LineNode line =>
                                    $"{line.Id}|{line.Layer}|"
                                    + $"{line.StyleKey}|"
                                    + $"{line.Start.X:F6}|"
                                    + $"{line.Start.Y:F6}|"
                                    + $"{line.End.X:F6}|"
                                    + $"{line.End.Y:F6}",

                                _ =>
                                    $"{node.Id}|{node.Layer}|"
                                    + $"{node.StyleKey}"
                            }));
}

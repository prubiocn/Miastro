using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Layout;
using Miastro.Graphics.Layout.Placement;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Scene.Natal;
using Miastro.Graphics.Styles;
using Miastro.Graphics.Skia.Rendering;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7SceneStyleTests
{
    [TestMethod]
    public void Style_catalog_contains_required_semantic_styles()
    {
        var catalog =
            new NatalSceneStyleCatalog();

        var required =
            new[]
            {
                NatalSceneStyleKeys.Background,
                NatalSceneStyleKeys.ZodiacBoundary,
                NatalSceneStyleKeys.ZodiacGlyph,
                NatalSceneStyleKeys.DegreeMinor,
                NatalSceneStyleKeys.DegreeFive,
                NatalSceneStyleKeys.DegreeTen,
                NatalSceneStyleKeys.HouseCusp,
                NatalSceneStyleKeys.HouseNumber,
                NatalSceneStyleKeys.AngleMajor,
                NatalSceneStyleKeys.AngleMinor,
                NatalSceneStyleKeys.BodyGlyph,
                NatalSceneStyleKeys.PointGlyph,
                NatalSceneStyleKeys.RealPositionMark,
                NatalSceneStyleKeys.LeaderLine,
                NatalSceneStyleKeys.AspectMajor,
                NatalSceneStyleKeys.AspectSecondary
            };

        foreach (var key in required)
        {
            Assert.IsTrue(
                catalog.TryGet(
                    key,
                    out _),
                key);
        }
    }

    [TestMethod]
    public void Degree_hierarchy_has_increasing_stroke_weight()
    {
        var catalog =
            new NatalSceneStyleCatalog();

        var minor =
            catalog.GetRequired(
                NatalSceneStyleKeys.DegreeMinor);

        var five =
            catalog.GetRequired(
                NatalSceneStyleKeys.DegreeFive);

        var ten =
            catalog.GetRequired(
                NatalSceneStyleKeys.DegreeTen);

        Assert.IsTrue(
            minor.StrokeWidth
            < five.StrokeWidth);

        Assert.IsTrue(
            five.StrokeWidth
            < ten.StrokeWidth);
    }

    [TestMethod]
    public void Asc_mc_are_stronger_than_dsc_ic()
    {
        var catalog =
            new NatalSceneStyleCatalog();

        var major =
            catalog.GetRequired(
                NatalSceneStyleKeys.AngleMajor);

        var minor =
            catalog.GetRequired(
                NatalSceneStyleKeys.AngleMinor);

        Assert.IsTrue(
            major.StrokeWidth
            > minor.StrokeWidth);

        Assert.AreEqual(
            SceneLinePattern.Dashed,
            minor.LinePattern);
    }

    [TestMethod]
    public void Major_aspects_are_not_color_only()
    {
        var catalog =
            new NatalSceneStyleCatalog();

        var major =
            catalog.GetRequired(
                NatalSceneStyleKeys.AspectMajor);

        var secondary =
            catalog.GetRequired(
                NatalSceneStyleKeys.AspectSecondary);

        Assert.IsTrue(
            major.StrokeWidth
            > secondary.StrokeWidth);

        Assert.AreNotEqual(
            major.LinePattern,
            secondary.LinePattern);

        Assert.IsTrue(
            major.Opacity
            > secondary.Opacity);
    }

    [TestMethod]
    public void Scene_builder_assigns_degree_styles()
    {
        var scene =
            BuildScene();

        var minor =
            scene.Nodes
                .Single(
                    x => x.Id == "degree-001");

        var five =
            scene.Nodes
                .Single(
                    x => x.Id == "degree-005");

        var ten =
            scene.Nodes
                .Single(
                    x => x.Id == "degree-010");

        Assert.AreEqual(
            NatalSceneStyleKeys.DegreeMinor,
            minor.StyleKey);

        Assert.AreEqual(
            NatalSceneStyleKeys.DegreeFive,
            five.StyleKey);

        Assert.AreEqual(
            NatalSceneStyleKeys.DegreeTen,
            ten.StyleKey);
    }

    [TestMethod]
    public void Scene_builder_distinguishes_major_and_minor_angles()
    {
        var scene =
            BuildScene();

        Assert.AreEqual(
            NatalSceneStyleKeys.AngleMajor,
            scene.Nodes.Single(
                    x => x.Id
                        == "angle-axis-ASC")
                .StyleKey);

        Assert.AreEqual(
            NatalSceneStyleKeys.AngleMajor,
            scene.Nodes.Single(
                    x => x.Id
                        == "angle-axis-MC")
                .StyleKey);

        Assert.AreEqual(
            NatalSceneStyleKeys.AngleMinor,
            scene.Nodes.Single(
                    x => x.Id
                        == "angle-axis-DSC")
                .StyleKey);

        Assert.AreEqual(
            NatalSceneStyleKeys.AngleMinor,
            scene.Nodes.Single(
                    x => x.Id
                        == "angle-axis-IC")
                .StyleKey);
    }

    [TestMethod]
    public void Body_and_point_glyph_styles_are_distinct()
    {
        var scene =
            BuildScene();

        Assert.AreEqual(
            NatalSceneStyleKeys.BodyGlyph,
            scene.Nodes.Single(
                    x => x.Id
                        == "object-glyph-Sun")
                .StyleKey);

        Assert.AreEqual(
            NatalSceneStyleKeys.PointGlyph,
            scene.Nodes.Single(
                    x => x.Id
                        == "object-glyph-Chiron")
                .StyleKey);
    }

    [TestMethod]
    public void Styled_scene_renders_headlessly()
    {
        var png =
            new SkiaNatalSceneRenderer()
                .RenderPng(
                    BuildScene(),
                    800,
                    800);

        Assert.IsTrue(
            png.Length > 1000);
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
                            120),

                        new NatalObjectLayoutInput(
                            "Chiron",
                            120)
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
                        "Chiron",
                        "point-chiron",
                        SceneLayer.PointLayer)
                });
    }
}

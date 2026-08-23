using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Geometry;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Skia.Rendering;
using Miastro.Graphics.Skia.Typography;
using SkiaSharp;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7TypographyTests
{
    [TestMethod]
    public void Embedded_font_resource_exists()
    {
        var names =
            typeof(SkiaTypographyProvider)
                .Assembly
                .GetManifestResourceNames();

        CollectionAssert.Contains(
            names,
            "Miastro.Graphics.Skia.Resources.Fonts.SourceSans3-Regular.ttf");
    }

    [TestMethod]
    public void Embedded_license_resource_exists()
    {
        var names =
            typeof(SkiaTypographyProvider)
                .Assembly
                .GetManifestResourceNames();

        CollectionAssert.Contains(
            names,
            "Miastro.Graphics.Skia.Resources.Fonts.SourceSans3-LICENSE.md");
    }

    [TestMethod]
    public void Typeface_loads_without_system_font_lookup()
    {
        using var typography =
            new SkiaTypographyProvider();

        Assert.IsNotNull(
            typography.Typeface);

        Assert.IsFalse(
            string.IsNullOrWhiteSpace(
                typography.FamilyName));
    }

    [TestMethod]
    public void Text_node_renders_headlessly()
    {
        var scene =
            BuildTextScene();

        var png =
            new SkiaNatalSceneRenderer()
                .RenderPng(
                    scene,
                    400,
                    200);

        Assert.IsTrue(
            png.Length > 500);

        using var bitmap =
            SKBitmap.Decode(
                png);

        Assert.IsNotNull(
            bitmap);

        Assert.AreEqual(
            400,
            bitmap.Width);

        Assert.AreEqual(
            200,
            bitmap.Height);
    }

    [TestMethod]
    public void Typography_render_is_deterministic()
    {
        var renderer =
            new SkiaNatalSceneRenderer();

        var scene =
            BuildTextScene();

        var first =
            renderer.RenderPng(
                scene,
                400,
                200);

        var second =
            renderer.RenderPng(
                scene,
                400,
                200);

        CollectionAssert.AreEqual(
            first,
            second);
    }

    [TestMethod]
    public void Spanish_and_degree_symbols_render()
    {
        var scene =
            new NatalScene(
                500,
                200,
                new SceneNode[]
                {
                    new TextNode(
                        "sample",
                        SceneLayer.LabelLayer,
                        "Júpiter 14°23' Leo",
                        new ChartPoint(
                            250,
                            100),
                        28,
                        new ChartRect(
                            100,
                            70,
                            300,
                            60))
                });

        var png =
            new SkiaNatalSceneRenderer()
                .RenderPng(
                    scene,
                    500,
                    200);

        Assert.IsTrue(
            png.Length > 500);
    }

    private static NatalScene BuildTextScene()
        =>
            new(
                400,
                200,
                new SceneNode[]
                {
                    new TextNode(
                        "title",
                        SceneLayer.LabelLayer,
                        "Casa 10",
                        new ChartPoint(
                            200,
                            70),
                        28,
                        new ChartRect(
                            120,
                            40,
                            160,
                            60)),

                    new TextNode(
                        "position",
                        SceneLayer.LabelLayer,
                        "Sol 14°23' Leo",
                        new ChartPoint(
                            200,
                            130),
                        22,
                        new ChartRect(
                            100,
                            105,
                            200,
                            50))
                });
}

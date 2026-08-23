using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Geometry;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Skia.Rendering;
using SkiaSharp;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7SkiaHeadlessRendererTests
{
    [TestMethod]
    public void Renderer_produces_valid_png()
    {
        var renderer =
            new SkiaNatalSceneRenderer();

        var png =
            renderer.RenderPng(
                BuildScene(),
                800,
                800);

        Assert.IsTrue(
            png.Length > 100);

        CollectionAssert.AreEqual(
            new byte[]
            {
                137,
                80,
                78,
                71,
                13,
                10,
                26,
                10
            },
            png.Take(8).ToArray());
    }

    [TestMethod]
    public void Png_has_requested_dimensions()
    {
        var png =
            new SkiaNatalSceneRenderer()
                .RenderPng(
                    BuildScene(),
                    640,
                    480);

        using var bitmap =
            SKBitmap.Decode(
                png);

        Assert.IsNotNull(
            bitmap);

        Assert.AreEqual(
            640,
            bitmap.Width);

        Assert.AreEqual(
            480,
            bitmap.Height);
    }

    [TestMethod]
    public void Same_scene_produces_same_png_bytes()
    {
        var renderer =
            new SkiaNatalSceneRenderer();

        var scene =
            BuildScene();

        var first =
            renderer.RenderPng(
                scene,
                500,
                500);

        var second =
            renderer.RenderPng(
                scene,
                500,
                500);

        CollectionAssert.AreEqual(
            first,
            second);
    }

    [TestMethod]
    public void Renderer_scales_scene_to_target_surface()
    {
        var renderer =
            new SkiaNatalSceneRenderer();

        var small =
            renderer.RenderPng(
                BuildScene(),
                400,
                400);

        var large =
            renderer.RenderPng(
                BuildScene(),
                800,
                800);

        using var smallBitmap =
            SKBitmap.Decode(
                small);

        using var largeBitmap =
            SKBitmap.Decode(
                large);

        Assert.AreEqual(
            400,
            smallBitmap.Width);

        Assert.AreEqual(
            800,
            largeBitmap.Width);
    }

    [TestMethod]
    public void Technical_png_writer_creates_file()
    {
        var directory =
            Path.Combine(
                Path.GetTempPath(),
                "miastro-phase7-skia-tests",
                Guid.NewGuid()
                    .ToString("N"));

        var path =
            Path.Combine(
                directory,
                "wheel.png");

        try
        {
            var writer =
                new SkiaTechnicalPngWriter(
                    new SkiaNatalSceneRenderer());

            writer.Write(
                BuildScene(),
                path,
                512,
                512);

            Assert.IsTrue(
                File.Exists(
                    path));

            Assert.IsTrue(
                new FileInfo(path)
                    .Length > 100);
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(
                    directory,
                    recursive: true);
            }
        }
    }

    [TestMethod]
    public void Graphics_core_still_has_no_skia_reference()
    {
        var graphicsAssembly =
            typeof(SceneNode)
                .Assembly;

        var references =
            graphicsAssembly
                .GetReferencedAssemblies();

        Assert.IsFalse(
            references.Any(
                x =>
                    x.Name is not null
                    && x.Name.StartsWith(
                        "SkiaSharp",
                        StringComparison.Ordinal)));
    }

    private static NatalScene BuildScene()
        =>
            new(
                800,
                800,
                new SceneNode[]
                {
                    new CircleNode(
                        "outer",
                        SceneLayer.ZodiacRing,
                        new ChartPoint(
                            400,
                            400),
                        350),

                    new LineNode(
                        "asc",
                        SceneLayer.AngleLayer,
                        new ChartPoint(
                            50,
                            400),
                        new ChartPoint(
                            250,
                            400)),

                    new GlyphNode(
                        "sun",
                        SceneLayer.BodyLayer,
                        "planet-sun",
                        new ChartPoint(
                            300,
                            220),
                        28,
                        new ChartRect(
                            286,
                            206,
                            28,
                            28)),

                    new TextNode(
                        "house-1",
                        SceneLayer.HouseLayer,
                        "1",
                        new ChartPoint(
                            200,
                            400),
                        16,
                        new ChartRect(
                            192,
                            392,
                            16,
                            16))
                });
}

using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Geometry;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Skia.Rendering;
using SkiaSharp;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7HiDpiAndPerformanceTests
{
    [TestMethod]
    public void Renderer_supports_1x_1_5x_2x_and_3x()
    {
        var renderer =
            new SkiaNatalSceneRenderer();

        var scene =
            BuildScene();

        var cases =
            new[]
            {
                (Scale: 1.0, Pixels: 400),
                (Scale: 1.5, Pixels: 600),
                (Scale: 2.0, Pixels: 800),
                (Scale: 3.0, Pixels: 1200)
            };

        foreach (var item in cases)
        {
            var png =
                renderer.RenderPng(
                    scene,
                    item.Pixels,
                    item.Pixels);

            using var bitmap =
                SKBitmap.Decode(
                    png);

            Assert.IsNotNull(
                bitmap,
                $"scale={item.Scale}");

            Assert.AreEqual(
                item.Pixels,
                bitmap.Width);

            Assert.AreEqual(
                item.Pixels,
                bitmap.Height);
        }
    }

    [TestMethod]
    public void Same_scale_is_byte_deterministic()
    {
        var renderer =
            new SkiaNatalSceneRenderer();

        var scene =
            BuildScene();

        foreach (
            var pixels
            in new[]
            {
                400,
                600,
                800,
                1200
            })
        {
            var first =
                renderer.RenderPng(
                    scene,
                    pixels,
                    pixels);

            var second =
                renderer.RenderPng(
                    scene,
                    pixels,
                    pixels);

            CollectionAssert.AreEqual(
                first,
                second,
                $"pixels={pixels}");
        }
    }

    [TestMethod]
    public void Rectangular_target_keeps_scene_aspect_ratio()
    {
        var renderer =
            new SkiaNatalSceneRenderer();

        var scene =
            new NatalScene(
                400,
                400,
                new SceneNode[]
                {
                    new CircleNode(
                        "circle",
                        SceneLayer.ZodiacRing,
                        new ChartPoint(
                            200,
                            200),
                        100)
                });

        var png =
            renderer.RenderPng(
                scene,
                800,
                400);

        using var bitmap =
            SKBitmap.Decode(
                png);

        Assert.IsNotNull(bitmap);

        var bounds =
            FindNonBackgroundBounds(
                bitmap);

        var width =
            bounds.Right
            - bounds.Left
            + 1;

        var height =
            bounds.Bottom
            - bounds.Top
            + 1;

        Assert.IsTrue(
            Math.Abs(
                width - height)
            <= 2,
            $"Rendered circle distorted: {width}x{height}");

        Assert.IsTrue(
            bounds.Left > 150,
            $"Scene not horizontally centered: left={bounds.Left}");

        Assert.IsTrue(
            bounds.Right < 650,
            $"Scene not horizontally centered: right={bounds.Right}");
    }

    [TestMethod]
    public void HiDpi_output_grows_with_requested_pixel_density()
    {
        var renderer =
            new SkiaNatalSceneRenderer();

        var scene =
            BuildScene();

        var oneX =
            renderer.RenderPng(
                scene,
                400,
                400);

        var threeX =
            renderer.RenderPng(
                scene,
                1200,
                1200);

        using var first =
            SKBitmap.Decode(oneX);

        using var second =
            SKBitmap.Decode(threeX);

        Assert.IsNotNull(first);
        Assert.IsNotNull(second);

        Assert.AreEqual(
            first.Width * 3,
            second.Width);

        Assert.AreEqual(
            first.Height * 3,
            second.Height);

        Assert.IsTrue(
            threeX.Length
            > oneX.Length);
    }

    [TestMethod]
    public void Representative_render_has_no_catastrophic_performance_regression()
    {
        var renderer =
            new SkiaNatalSceneRenderer();

        var scene =
            BuildBusyScene();

        _ =
            renderer.RenderPng(
                scene,
                800,
                800);

        var stopwatch =
            Stopwatch.StartNew();

        for (var i = 0; i < 5; i++)
        {
            var png =
                renderer.RenderPng(
                    scene,
                    800,
                    800);

            Assert.IsTrue(
                png.Length > 1000);
        }

        stopwatch.Stop();

        Assert.IsTrue(
            stopwatch.Elapsed
                < TimeSpan.FromSeconds(10),
            $"Five renders took {stopwatch.Elapsed}.");
    }

    [TestMethod]
    public void Renderer_remains_independent_of_astronomy()
    {
        var references =
            typeof(SkiaNatalSceneRenderer)
                .Assembly
                .GetReferencedAssemblies()
                .Select(x => x.Name)
                .Where(x => x is not null)
                .ToArray();

        Assert.IsFalse(
            references.Any(
                x =>
                    x!.Contains(
                        "SwissEphemeris",
                        StringComparison.Ordinal)));

        Assert.IsFalse(
            references.Any(
                x =>
                    x!.Contains(
                        "Astronomy",
                        StringComparison.Ordinal)));
    }

    private static NatalScene BuildScene()
        =>
            new(
                400,
                400,
                new SceneNode[]
                {
                    new CircleNode(
                        "outer",
                        SceneLayer.ZodiacRing,
                        new ChartPoint(
                            200,
                            200),
                        170),

                    new CircleNode(
                        "inner",
                        SceneLayer.ZodiacRing,
                        new ChartPoint(
                            200,
                            200),
                        145),

                    new LineNode(
                        "asc",
                        SceneLayer.AngleLayer,
                        new ChartPoint(
                            30,
                            200),
                        new ChartPoint(
                            180,
                            200)),

                    new GlyphNode(
                        "sun",
                        SceneLayer.BodyLayer,
                        "planet-sun",
                        new ChartPoint(
                            130,
                            105),
                        24,
                        new ChartRect(
                            118,
                            93,
                            24,
                            24)),

                    new TextNode(
                        "label",
                        SceneLayer.LabelLayer,
                        "Sol 14°23′ Leo",
                        new ChartPoint(
                            280,
                            300),
                        16,
                        new ChartRect(
                            220,
                            288,
                            120,
                            24))
                });

    private static NatalScene BuildBusyScene()
    {
        var nodes =
            new List<SceneNode>();

        nodes.Add(
            new CircleNode(
                "outer",
                SceneLayer.ZodiacRing,
                new ChartPoint(
                    400,
                    400),
                360));

        for (var degree = 0; degree < 360; degree++)
        {
            var radians =
                degree
                * Math.PI
                / 180.0;

            var outer =
                new ChartPoint(
                    400
                    + Math.Cos(radians) * 350,
                    400
                    + Math.Sin(radians) * 350);

            var inner =
                new ChartPoint(
                    400
                    + Math.Cos(radians) * 330,
                    400
                    + Math.Sin(radians) * 330);

            nodes.Add(
                new LineNode(
                    $"tick-{degree:000}",
                    SceneLayer.DegreeRing,
                    outer,
                    inner));
        }

        var glyphs =
            new[]
            {
                "planet-sun",
                "planet-moon",
                "planet-mercury",
                "planet-venus",
                "planet-mars",
                "planet-jupiter",
                "planet-saturn",
                "planet-uranus",
                "planet-neptune",
                "planet-pluto"
            };

        for (var index = 0; index < 30; index++)
        {
            var angle =
                index
                * 12.0
                * Math.PI
                / 180.0;

            var center =
                new ChartPoint(
                    400
                    + Math.Cos(angle) * 245,
                    400
                    + Math.Sin(angle) * 245);

            nodes.Add(
                new GlyphNode(
                    $"glyph-{index:00}",
                    SceneLayer.BodyLayer,
                    glyphs[
                        index % glyphs.Length],
                    center,
                    28,
                    new ChartRect(
                        center.X - 14,
                        center.Y - 14,
                        28,
                        28)));
        }

        for (var index = 0; index < 40; index++)
        {
            var firstAngle =
                index
                * 9.0
                * Math.PI
                / 180.0;

            var secondAngle =
                (
                    index * 9.0
                    + 120.0
                )
                * Math.PI
                / 180.0;

            nodes.Add(
                new LineNode(
                    $"aspect-{index:00}",
                    SceneLayer.AspectLayer,
                    new ChartPoint(
                        400
                        + Math.Cos(firstAngle)
                            * 160,
                        400
                        + Math.Sin(firstAngle)
                            * 160),
                    new ChartPoint(
                        400
                        + Math.Cos(secondAngle)
                            * 160,
                        400
                        + Math.Sin(secondAngle)
                            * 160)));
        }

        return new NatalScene(
            800,
            800,
            nodes);
    }

    private static (
        int Left,
        int Top,
        int Right,
        int Bottom)
        FindNonBackgroundBounds(
            SKBitmap bitmap)
    {
        const byte backgroundRed =
            250;

        const byte backgroundGreen =
            248;

        const byte backgroundBlue =
            243;

        var left =
            bitmap.Width;

        var top =
            bitmap.Height;

        var right =
            -1;

        var bottom =
            -1;

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel =
                    bitmap.GetPixel(
                        x,
                        y);

                if (pixel.Red
                        == backgroundRed
                    && pixel.Green
                        == backgroundGreen
                    && pixel.Blue
                        == backgroundBlue)
                {
                    continue;
                }

                left =
                    Math.Min(
                        left,
                        x);

                top =
                    Math.Min(
                        top,
                        y);

                right =
                    Math.Max(
                        right,
                        x);

                bottom =
                    Math.Max(
                        bottom,
                        y);
            }
        }

        Assert.IsTrue(
            right >= left);

        Assert.IsTrue(
            bottom >= top);

        return (
            left,
            top,
            right,
            bottom);
    }
}

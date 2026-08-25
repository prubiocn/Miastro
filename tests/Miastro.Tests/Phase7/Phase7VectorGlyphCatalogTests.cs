using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Glyphs;
using Miastro.Graphics.Geometry;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Skia.Rendering;
using SkiaSharp;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7VectorGlyphCatalogTests
{
    [TestMethod]
    public void Catalog_contains_all_twelve_zodiac_signs()
    {
        var catalog =
            new NatalVectorGlyphCatalog();

        for (var i = 0; i < 12; i++)
        {
            Assert.IsTrue(
                catalog.TryGet(
                    $"zodiac-{i:00}",
                    out var glyph));

            Assert.IsFalse(
                glyph.IsEmpty);
        }
    }

    [TestMethod]
    public void Taurus_uses_horn_strokes_and_circular_head()
    {
        var glyph =
            new NatalVectorGlyphCatalog()
                .GetRequired(
                    "zodiac-01");

        Assert.AreEqual(
            2,
            glyph.Strokes.Count);

        Assert.AreEqual(
            1,
            glyph.Circles.Count);
    }

    [TestMethod]
    public void Pluto_uses_circle_cup_vertical_and_crossbar()
    {
        var glyph =
            new NatalVectorGlyphCatalog()
                .GetRequired(
                    "planet-pluto");

        Assert.AreEqual(
            3,
            glyph.Strokes.Count);

        Assert.AreEqual(
            1,
            glyph.Circles.Count);

        var circle =
            glyph.Circles.Single();

        Assert.IsTrue(
            circle.Center.Y < 0.0);

        Assert.IsTrue(
            glyph.Strokes.Any(
                stroke =>
                    stroke.Points.Count == 2
                    && Math.Abs(
                        stroke.Points[0].X
                        - stroke.Points[1].X)
                        < 1e-12));

        Assert.IsTrue(
            glyph.Strokes.Any(
                stroke =>
                    stroke.Points.Count == 2
                    && Math.Abs(
                        stroke.Points[0].Y
                        - stroke.Points[1].Y)
                        < 1e-12));
    }

    [TestMethod]
    public void Catalog_contains_required_planets()
    {
        var catalog =
            new NatalVectorGlyphCatalog();

        var required =
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

        foreach (var key in required)
        {
            Assert.IsFalse(
                catalog
                    .GetRequired(key)
                    .IsEmpty,
                key);
        }
    }

    [TestMethod]
    public void Catalog_contains_required_points_and_asteroids()
    {
        var catalog =
            new NatalVectorGlyphCatalog();

        var required =
            new[]
            {
                "point-north-node",
                "point-south-node",
                "point-lilith",
                "point-fortuna",
                "point-chiron",
                "asteroid-ceres",
                "asteroid-pallas",
                "asteroid-juno",
                "asteroid-vesta",
                "angle-asc",
                "angle-mc"
            };

        foreach (var key in required)
        {
            Assert.IsFalse(
                catalog
                    .GetRequired(key)
                    .IsEmpty,
                key);
        }
    }

    [TestMethod]
    public void Catalog_contains_major_aspect_glyphs()
    {
        var catalog =
            new NatalVectorGlyphCatalog();

        var required =
            new[]
            {
                "aspect-conjunction",
                "aspect-opposition",
                "aspect-trine",
                "aspect-square",
                "aspect-sextile",
                "aspect-quincunx"
            };

        foreach (var key in required)
        {
            Assert.IsFalse(
                catalog
                    .GetRequired(key)
                    .IsEmpty,
                key);
        }
    }

    [TestMethod]
    public void Glyph_geometry_stays_inside_normalized_canvas()
    {
        var catalog =
            new NatalVectorGlyphCatalog();

        foreach (var key in catalog.Keys)
        {
            var glyph =
                catalog.GetRequired(key);

            foreach (
                var stroke
                in glyph.Strokes)
            {
                foreach (
                    var point
                    in stroke.Points)
                {
                    Assert.IsTrue(
                        point.X >= -0.5
                        && point.X <= 0.5,
                        $"{key}: X={point.X}");

                    Assert.IsTrue(
                        point.Y >= -0.5
                        && point.Y <= 0.5,
                        $"{key}: Y={point.Y}");
                }
            }

            foreach (
                var circle
                in glyph.Circles)
            {
                Assert.IsTrue(
                    circle.Radius > 0
                    && circle.Radius <= 0.5,
                    key);

                Assert.IsTrue(
                    Math.Abs(circle.Center.X)
                        + circle.Radius
                        <= 0.5,
                    key);

                Assert.IsTrue(
                    Math.Abs(circle.Center.Y)
                        + circle.Radius
                        <= 0.5,
                    key);
            }
        }
    }

    [TestMethod]
    public void Renderer_draws_catalog_glyphs_headlessly()
    {
        var catalog =
            new NatalVectorGlyphCatalog();

        var nodes =
            new List<SceneNode>();

        var keys =
            catalog.Keys
                .ToArray();

        for (
            var i = 0;
            i < keys.Length;
            i++)
        {
            var column =
                i % 8;

            var row =
                i / 8;

            var center =
                new ChartPoint(
                    50 + column * 80,
                    50 + row * 80);

            nodes.Add(
                new GlyphNode(
                    $"glyph-{i:00}",
                    SceneLayer.BodyLayer,
                    keys[i],
                    center,
                    42,
                    new ChartRect(
                        center.X - 21,
                        center.Y - 21,
                        42,
                        42)));
        }

        var scene =
            new NatalScene(
                700,
                600,
                nodes);

        var png =
            new SkiaNatalSceneRenderer()
                .RenderPng(
                    scene,
                    700,
                    600);

        using var bitmap =
            SKBitmap.Decode(
                png);

        Assert.IsNotNull(bitmap);
        Assert.AreEqual(700, bitmap.Width);
        Assert.AreEqual(600, bitmap.Height);
        Assert.IsTrue(png.Length > 1000);
    }

    [TestMethod]
    public void Vector_glyph_render_is_deterministic()
    {
        var scene =
            new NatalScene(
                200,
                200,
                new SceneNode[]
                {
                    new GlyphNode(
                        "sun",
                        SceneLayer.BodyLayer,
                        "planet-sun",
                        new ChartPoint(
                            100,
                            100),
                        50,
                        new ChartRect(
                            75,
                            75,
                            50,
                            50))
                });

        var renderer =
            new SkiaNatalSceneRenderer();

        var first =
            renderer.RenderPng(
                scene,
                200,
                200);

        var second =
            renderer.RenderPng(
                scene,
                200,
                200);

        CollectionAssert.AreEqual(
            first,
            second);
    }
}

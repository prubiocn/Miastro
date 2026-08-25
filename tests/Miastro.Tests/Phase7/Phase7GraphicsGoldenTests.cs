using System.Globalization;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Layout;
using Miastro.Graphics.Layout.Placement;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Scene.Natal;
using Miastro.Graphics.Scene.Natal.Aspects;
using Miastro.Graphics.Scene.Natal.Configuration;
using Miastro.Graphics.Skia.Rendering;
using SkiaSharp;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7GraphicsGoldenTests
{
    private const string UpdateVariable =
        "MIASTRO_UPDATE_PHASE7_GOLDENS";

    private const int ChannelDifferenceThreshold =
        12;

    private const double MaximumChangedPixelRatio =
        0.01;

    private const double MaximumMeanChannelDifference =
        0.50;

    private static readonly string[]
        RequiredCaseIds =
        [
            "simple",
            "stellium",
            "many-aspects",
            "placidus",
            "koch"
        ];

    [TestMethod]
    public void Golden_baselines_can_be_regenerated_explicitly()
    {
        if (Environment.GetEnvironmentVariable(
                UpdateVariable)
            == "1")
        {
            WriteGoldens();
        }

        Assert.IsTrue(
            File.Exists(
                ManifestPath()),
            "Phase 7 graphics golden manifest is missing.");
    }

    [TestMethod]
    public void Golden_corpus_contains_all_required_cases()
    {
        var entries =
            LoadManifest();

        CollectionAssert.AreEquivalent(
            RequiredCaseIds,
            entries
                .Select(x => x.Id)
                .ToArray());
    }

    [TestMethod]
    public void Golden_files_match_integrity_manifest()
    {
        foreach (
            var entry
            in LoadManifest())
        {
            var path =
                GoldenPngPath(
                    entry.Id);

            Assert.IsTrue(
                File.Exists(path),
                path);

            var bytes =
                File.ReadAllBytes(
                    path);

            Assert.AreEqual(
                entry.PngSha256,
                Sha256(bytes),
                entry.Id);

            using var bitmap =
                SKBitmap.Decode(
                    bytes);

            Assert.IsNotNull(
                bitmap,
                entry.Id);

            Assert.AreEqual(
                entry.PixelWidth,
                bitmap.Width,
                entry.Id);

            Assert.AreEqual(
                entry.PixelHeight,
                bitmap.Height,
                entry.Id);
        }
    }

    [TestMethod]
    public void Current_render_matches_goldens_with_documented_tolerance()
    {
        var renderer =
            new SkiaNatalSceneRenderer();

        foreach (
            var entry
            in LoadManifest())
        {
            var fixture =
                BuildFixture(
                    entry.Id);

            var actualBytes =
                renderer.RenderPng(
                    fixture.Scene,
                    entry.PixelWidth,
                    entry.PixelHeight);

            var expectedBytes =
                File.ReadAllBytes(
                    GoldenPngPath(
                        entry.Id));

            AssertImagesEquivalent(
                entry.Id,
                expectedBytes,
                actualBytes);
        }
    }

    [TestMethod]
    public void Complex_headless_render_is_byte_deterministic()
    {
        var fixture =
            BuildFixture(
                "many-aspects");

        var renderer =
            new SkiaNatalSceneRenderer();

        var first =
            renderer.RenderPng(
                fixture.Scene,
                fixture.PixelWidth,
                fixture.PixelHeight);

        for (var index = 0; index < 5; index++)
        {
            var next =
                renderer.RenderPng(
                    fixture.Scene,
                    fixture.PixelWidth,
                    fixture.PixelHeight);

            CollectionAssert.AreEqual(
                first,
                next);
        }
    }

    [TestMethod]
    public void Golden_scenes_preserve_structural_invariants()
    {
        foreach (
            var id
            in RequiredCaseIds)
        {
            var scene =
                BuildFixture(id)
                    .Scene;

            Assert.AreEqual(
                12,
                scene.Nodes
                    .OfType<GlyphNode>()
                    .Count(
                        x =>
                            x.Id.StartsWith(
                                "zodiac-glyph-",
                                StringComparison.Ordinal)),
                id);

            Assert.AreEqual(
                12,
                scene.Nodes
                    .OfType<LineNode>()
                    .Count(
                        x =>
                            x.Id.StartsWith(
                                "house-cusp-",
                                StringComparison.Ordinal)),
                id);

            AssertObjectLabelsDoNotOverlap(
                id,
                scene);

            AssertSceneCoordinatesAreFinite(
                id,
                scene);
        }
    }

    private static void WriteGoldens()
    {
        Directory.CreateDirectory(
            GoldenDirectory());

        var renderer =
            new SkiaNatalSceneRenderer();

        var lines =
            new List<string>
            {
                "# id\tpixelWidth\tpixelHeight\tpngSha256\tsceneSha256"
            };

        foreach (
            var id
            in RequiredCaseIds)
        {
            var fixture =
                BuildFixture(id);

            var png =
                renderer.RenderPng(
                    fixture.Scene,
                    fixture.PixelWidth,
                    fixture.PixelHeight);

            File.WriteAllBytes(
                GoldenPngPath(id),
                png);

            lines.Add(
                string.Join(
                    "\t",
                    id,
                    fixture.PixelWidth.ToString(
                        CultureInfo.InvariantCulture),
                    fixture.PixelHeight.ToString(
                        CultureInfo.InvariantCulture),
                    Sha256(png),
                    Sha256(
                        System.Text.Encoding.UTF8
                            .GetBytes(
                                DescribeScene(
                                    fixture.Scene)))));
        }

        File.WriteAllLines(
            ManifestPath(),
            lines);
    }

    private static IReadOnlyList<ManifestEntry>
        LoadManifest()
    {
        var path =
            ManifestPath();

        Assert.IsTrue(
            File.Exists(path),
            path);

        return File.ReadAllLines(path)
            .Where(
                x =>
                    !string.IsNullOrWhiteSpace(x)
                    && !x.StartsWith(
                        "#",
                        StringComparison.Ordinal))
            .Select(
                line =>
                {
                    var parts =
                        line.Split('\t');

                    Assert.AreEqual(
                        5,
                        parts.Length,
                        line);

                    return new ManifestEntry(
                        parts[0],
                        int.Parse(
                            parts[1],
                            CultureInfo.InvariantCulture),
                        int.Parse(
                            parts[2],
                            CultureInfo.InvariantCulture),
                        parts[3],
                        parts[4]);
                })
            .ToArray();
    }

    private static GoldenFixture BuildFixture(
        string id)
        =>
            id switch
            {
                "simple" =>
                    Build(
                        PlacidusCusps(),
                        new[]
                        {
                            Body(
                                "Sun",
                                120.0,
                                "planet-sun"),

                            Body(
                                "Moon",
                                241.0,
                                "planet-moon"),

                            Body(
                                "Mercury",
                                133.25,
                                "planet-mercury",
                                retrograde: true),

                            Point(
                                "Chiron",
                                200.0,
                                "point-chiron")
                        },
                        new[]
                        {
                            Aspect(
                                "simple-sun-moon",
                                "Sun",
                                "Moon",
                                NatalAspectVisualClass.Major)
                        }),

                "stellium" =>
                    Build(
                        PlacidusCusps(),
                        Enumerable
                            .Range(
                                0,
                                9)
                            .Select(
                                index =>
                                    Body(
                                        $"P{index:00}",
                                        203.0
                                            + index * 0.01,
                                        PlanetGlyph(
                                            index)))
                            .ToArray(),
                        Array.Empty<
                            NatalAspectSceneInput>()),

                "many-aspects" =>
                    Build(
                        PlacidusCusps(),
                        ManyAspectObjects(),
                        ManyAspects()),

                "placidus" =>
                    Build(
                        PlacidusCusps(),
                        StandardObjects(),
                        StandardAspects()),

                "koch" =>
                    Build(
                        KochCusps(),
                        StandardObjects(),
                        StandardAspects()),

                _ =>
                    throw new ArgumentOutOfRangeException(
                        nameof(id),
                        id,
                        "Unknown Phase 7 golden fixture.")
            };

    private static GoldenFixture Build(
        IReadOnlyList<double> cusps,
        IReadOnlyList<ObjectFixture> objects,
        IReadOnlyList<NatalAspectSceneInput> aspects)
    {
        const double width =
            800.0;

        const double height =
            800.0;

        var wheel =
            new NatalWheelLayoutBuilder()
                .Build(
                    width,
                    height,
                    17.0,
                    cusps[9],
                    cusps);

        var placements =
            new NatalObjectPlacementEngine()
                .Layout(
                    wheel,
                    objects
                        .Select(
                            x =>
                                new NatalObjectLayoutInput(
                                    x.Id,
                                    x.Longitude))
                        .ToArray());

        var sceneObjects =
            objects
                .Select(
                    x =>
                        new NatalSceneObjectInput(
                            x.Id,
                            x.GlyphKey,
                            x.Layer,
                            labelText: x.Id,
                            isRetrograde:
                                x.IsRetrograde))
                .ToArray();

        var composed =
            new NatalWheelSceneComposer()
                .Compose(
                    wheel,
                    placements,
                    sceneObjects,
                    aspects,
                    NatalWheelSceneConfiguration
                        .ConsultationDefault);

        return new GoldenFixture(
            composed.Scene,
            800,
            800);
    }

    private static ObjectFixture[] StandardObjects()
        =>
        [
            Body("Sun", 120.0, "planet-sun"),
            Body("Moon", 241.0, "planet-moon"),
            Body(
                "Mercury",
                133.25,
                "planet-mercury",
                retrograde: true),
            Body("Venus", 281.0, "planet-venus"),
            Body("Mars", 31.0, "planet-mars"),
            Body("Jupiter", 62.0, "planet-jupiter"),
            Body("Saturn", 301.0, "planet-saturn"),
            Body("Uranus", 321.0, "planet-uranus"),
            Body("Neptune", 331.0, "planet-neptune"),
            Body("Pluto", 251.0, "planet-pluto"),
            Point("Chiron", 200.0, "point-chiron")
        ];

    private static ObjectFixture[] ManyAspectObjects()
        =>
        [
            Body("Sun", 15.0, "planet-sun"),
            Body("Saturn", 45.0, "planet-saturn"),
            Body("Mercury", 75.0, "planet-mercury"),
            Body("Venus", 105.0, "planet-venus"),
            Body("Mars", 135.0, "planet-mars"),
            Body("Jupiter", 165.0, "planet-jupiter"),
            Body("Moon", 195.0, "planet-moon"),
            Body("Uranus", 255.0, "planet-uranus"),
            Body("Neptune", 285.0, "planet-neptune"),
            Body("Pluto", 315.0, "planet-pluto")
        ];

    private static NatalAspectSceneInput[] ManyAspects()
        =>
        [
            // 180° — oposición explícita.
            Aspect(
                "sun-moon-opposition",
                "Sun",
                "Moon",
                NatalAspectVisualClass.Major),

            // Aspectos del Sol.
            Aspect(
                "sun-saturn-semisextile",
                "Sun",
                "Saturn",
                NatalAspectVisualClass.Secondary),

            Aspect(
                "sun-mercury-sextile",
                "Sun",
                "Mercury",
                NatalAspectVisualClass.Major),

            Aspect(
                "sun-venus-square",
                "Sun",
                "Venus",
                NatalAspectVisualClass.Major),

            Aspect(
                "sun-mars-trine",
                "Sun",
                "Mars",
                NatalAspectVisualClass.Major),

            Aspect(
                "sun-jupiter-quincunx",
                "Sun",
                "Jupiter",
                NatalAspectVisualClass.Secondary),

            // Red adicional, siempre con separaciones canónicas.
            Aspect(
                "saturn-moon-quincunx",
                "Saturn",
                "Moon",
                NatalAspectVisualClass.Secondary),

            Aspect(
                "mercury-venus-semisextile",
                "Mercury",
                "Venus",
                NatalAspectVisualClass.Secondary),

            Aspect(
                "mercury-mars-sextile",
                "Mercury",
                "Mars",
                NatalAspectVisualClass.Major),

            Aspect(
                "venus-jupiter-sextile",
                "Venus",
                "Jupiter",
                NatalAspectVisualClass.Major),

            Aspect(
                "mars-moon-sextile",
                "Mars",
                "Moon",
                NatalAspectVisualClass.Major),

            Aspect(
                "moon-uranus-sextile",
                "Moon",
                "Uranus",
                NatalAspectVisualClass.Major),

            Aspect(
                "moon-neptune-square",
                "Moon",
                "Neptune",
                NatalAspectVisualClass.Major),

            Aspect(
                "moon-pluto-trine",
                "Moon",
                "Pluto",
                NatalAspectVisualClass.Major),

            Aspect(
                "neptune-sun-square",
                "Neptune",
                "Sun",
                NatalAspectVisualClass.Major),

            Aspect(
                "pluto-sun-sextile",
                "Pluto",
                "Sun",
                NatalAspectVisualClass.Major)
        ];


    private static NatalAspectSceneInput[] StandardAspects()
        =>
        [
            Aspect(
                "sun-moon",
                "Sun",
                "Moon",
                NatalAspectVisualClass.Major),

            Aspect(
                "sun-jupiter",
                "Sun",
                "Jupiter",
                NatalAspectVisualClass.Major),

            Aspect(
                "mercury-venus",
                "Mercury",
                "Venus",
                NatalAspectVisualClass.Secondary),

            Aspect(
                "mars-saturn",
                "Mars",
                "Saturn",
                NatalAspectVisualClass.Major),

            Aspect(
                "uranus-pluto",
                "Uranus",
                "Pluto",
                NatalAspectVisualClass.Secondary)
        ];

    private static NatalAspectSceneInput Aspect(
        string id,
        string first,
        string second,
        NatalAspectVisualClass visualClass)
        =>
            new(
                id,
                first,
                second,
                visualClass);

    private static ObjectFixture Body(
        string id,
        double longitude,
        string glyph,
        bool retrograde = false)
        =>
            new(
                id,
                longitude,
                glyph,
                SceneLayer.BodyLayer,
                retrograde);

    private static ObjectFixture Point(
        string id,
        double longitude,
        string glyph)
        =>
            new(
                id,
                longitude,
                glyph,
                SceneLayer.PointLayer,
                false);

    private static string PlanetGlyph(
        int index)
    {
        var values =
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
                "planet-neptune"
            };

        return values[
            index % values.Length];
    }

    private static double[] PlacidusCusps()
        =>
        [
            17.0,
            42.0,
            68.0,
            96.0,
            128.0,
            160.0,
            197.0,
            222.0,
            248.0,
            276.0,
            308.0,
            340.0
        ];

    private static double[] KochCusps()
        =>
        [
            17.0,
            47.0,
            74.0,
            103.0,
            132.0,
            159.0,
            197.0,
            227.0,
            254.0,
            283.0,
            312.0,
            339.0
        ];

    private static void AssertImagesEquivalent(
        string id,
        byte[] expectedBytes,
        byte[] actualBytes)
    {
        using var expected =
            SKBitmap.Decode(
                expectedBytes);

        using var actual =
            SKBitmap.Decode(
                actualBytes);

        Assert.IsNotNull(
            expected,
            id);

        Assert.IsNotNull(
            actual,
            id);

        Assert.AreEqual(
            expected.Width,
            actual.Width,
            id);

        Assert.AreEqual(
            expected.Height,
            actual.Height,
            id);

        var expectedPixels =
            expected.Pixels;

        var actualPixels =
            actual.Pixels;

        Assert.AreEqual(
            expectedPixels.Length,
            actualPixels.Length,
            id);

        long channelDifferenceTotal =
            0;

        var changedPixels =
            0;

        for (
            var index = 0;
            index < expectedPixels.Length;
            index++)
        {
            var first =
                expectedPixels[index];

            var second =
                actualPixels[index];

            var dr =
                Math.Abs(
                    first.Red
                    - second.Red);

            var dg =
                Math.Abs(
                    first.Green
                    - second.Green);

            var db =
                Math.Abs(
                    first.Blue
                    - second.Blue);

            var da =
                Math.Abs(
                    first.Alpha
                    - second.Alpha);

            channelDifferenceTotal +=
                dr + dg + db + da;

            if (Math.Max(
                    Math.Max(dr, dg),
                    Math.Max(db, da))
                > ChannelDifferenceThreshold)
            {
                changedPixels++;
            }
        }

        var changedRatio =
            changedPixels
            / (double)expectedPixels.Length;

        var meanChannelDifference =
            channelDifferenceTotal
            / (
                expectedPixels.Length
                * 4.0
            );

        Assert.IsTrue(
            changedRatio
                <= MaximumChangedPixelRatio,
            $"{id}: changedRatio={changedRatio:F6}");

        Assert.IsTrue(
            meanChannelDifference
                <= MaximumMeanChannelDifference,
            $"{id}: meanChannelDifference={meanChannelDifference:F6}");
    }

    private static void AssertObjectLabelsDoNotOverlap(
        string id,
        NatalScene scene)
    {
        var labels =
            scene.Nodes
                .OfType<TextNode>()
                .Where(
                    x =>
                        x.Id.StartsWith(
                            "object-label-",
                            StringComparison.Ordinal))
                .ToArray();

        var glyphs =
            scene.Nodes
                .OfType<GlyphNode>()
                .Where(
                    x =>
                        x.Id.StartsWith(
                            "object-glyph-",
                            StringComparison.Ordinal))
                .ToArray();

        for (
            var first = 0;
            first < labels.Length;
            first++)
        {
            foreach (
                var glyph
                in glyphs)
            {
                Assert.IsFalse(
                    labels[first]
                        .Bounds
                        .Intersects(
                            glyph.Bounds),
                    $"{id}: {labels[first].Id} overlaps {glyph.Id}");
            }

            for (
                var second = first + 1;
                second < labels.Length;
                second++)
            {
                Assert.IsFalse(
                    labels[first]
                        .Bounds
                        .Intersects(
                            labels[second]
                                .Bounds),
                    $"{id}: {labels[first].Id} overlaps {labels[second].Id}");
            }
        }
    }

    private static void AssertSceneCoordinatesAreFinite(
        string id,
        NatalScene scene)
    {
        foreach (
            var node
            in scene.Nodes)
        {
            switch (node)
            {
                case CircleNode circle:
                    AssertFinite(
                        id,
                        circle.Center.X,
                        circle.Center.Y,
                        circle.Radius);
                    break;

                case LineNode line:
                    AssertFinite(
                        id,
                        line.Start.X,
                        line.Start.Y,
                        line.End.X,
                        line.End.Y);
                    break;

                case GlyphNode glyph:
                    AssertFinite(
                        id,
                        glyph.Position.X,
                        glyph.Position.Y,
                        glyph.Size);
                    break;

                case TextNode text:
                    AssertFinite(
                        id,
                        text.Position.X,
                        text.Position.Y,
                        text.Size);
                    break;

                case ArcNode arc:
                    AssertFinite(
                        id,
                        arc.Center.X,
                        arc.Center.Y,
                        arc.Radius,
                        arc.StartAngleDegrees,
                        arc.SweepAngleDegrees);
                    break;
            }
        }
    }

    private static void AssertFinite(
        string id,
        params double[] values)
    {
        Assert.IsTrue(
            values.All(
                double.IsFinite),
            id);
    }

    private static string DescribeScene(
        NatalScene scene)
        =>
            string.Join(
                "\n",
                scene.OrderedNodes
                    .Select(
                        DescribeNode));

    private static string DescribeNode(
        SceneNode node)
        =>
            node switch
            {
                CircleNode x =>
                    Join(
                        x.Layer,
                        x.Id,
                        "C",
                        x.Center.X,
                        x.Center.Y,
                        x.Radius),

                LineNode x =>
                    Join(
                        x.Layer,
                        x.Id,
                        "L",
                        x.Start.X,
                        x.Start.Y,
                        x.End.X,
                        x.End.Y),

                GlyphNode x =>
                    Join(
                        x.Layer,
                        x.Id,
                        "G",
                        x.GlyphKey,
                        x.Position.X,
                        x.Position.Y,
                        x.Size),

                TextNode x =>
                    Join(
                        x.Layer,
                        x.Id,
                        "T",
                        x.Text,
                        x.Position.X,
                        x.Position.Y,
                        x.Size),

                ArcNode x =>
                    Join(
                        x.Layer,
                        x.Id,
                        "A",
                        x.Center.X,
                        x.Center.Y,
                        x.Radius,
                        x.StartAngleDegrees,
                        x.SweepAngleDegrees),

                _ =>
                    $"{node.Layer}|{node.Id}|{node.GetType().Name}"
            };

    private static string Join(
        params object[] values)
        =>
            string.Join(
                "|",
                values.Select(
                    value =>
                        value is double number
                            ? number.ToString(
                                "F9",
                                CultureInfo.InvariantCulture)
                            : Convert.ToString(
                                value,
                                CultureInfo.InvariantCulture)
                              ?? string.Empty));

    private static string Sha256(
        byte[] bytes)
        =>
            Convert.ToHexString(
                SHA256.HashData(
                    bytes))
            .ToLowerInvariant();

    private static string GoldenPngPath(
        string id)
        =>
            Path.Combine(
                GoldenDirectory(),
                $"{id}.png");

    private static string ManifestPath()
        =>
            Path.Combine(
                GoldenDirectory(),
                "manifest.tsv");

    private static string GoldenDirectory()
        =>
            Path.Combine(
                RepoRoot(),
                "tests",
                "golden",
                "phase7",
                "graphics");

    private static string RepoRoot()
    {
        var current =
            new DirectoryInfo(
                AppContext.BaseDirectory);

        while (current is not null)
        {
            if (File.Exists(
                Path.Combine(
                    current.FullName,
                    "Miastro.sln")))
            {
                return current.FullName;
            }

            current =
                current.Parent;
        }

        throw new DirectoryNotFoundException(
            "Miastro repository root not found.");
    }

    private sealed record GoldenFixture(
        NatalScene Scene,
        int PixelWidth,
        int PixelHeight);

    private sealed record ObjectFixture(
        string Id,
        double Longitude,
        string GlyphKey,
        SceneLayer Layer,
        bool IsRetrograde);

    private sealed record ManifestEntry(
        string Id,
        int PixelWidth,
        int PixelHeight,
        string PngSha256,
        string SceneSha256);
}

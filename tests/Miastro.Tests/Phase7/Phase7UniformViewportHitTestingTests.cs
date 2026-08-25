using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Geometry;
using Miastro.Graphics.Interaction;
using Miastro.Graphics.Scene;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7UniformViewportHitTestingTests
{
    [TestMethod]
    public void Uniform_viewport_maps_centered_scene_correctly()
    {
        var scene =
            BuildScene();

        var result =
            new NatalSceneHitTester()
                .HitTestViewport(
                    scene,
                    400,
                    200,
                    800,
                    400);

        Assert.IsNotNull(
            result);

        Assert.AreEqual(
            "Sun",
            result.ObjectId);
    }

    [TestMethod]
    public void Letterbox_area_is_not_hit_testable()
    {
        var scene =
            BuildScene();

        var result =
            new NatalSceneHitTester()
                .HitTestViewport(
                    scene,
                    100,
                    200,
                    800,
                    400,
                    tolerance: 100);

        Assert.IsNull(
            result);
    }

    [TestMethod]
    public void Selectable_ids_are_stable_and_ignore_non_object_glyphs()
    {
        var scene =
            new NatalScene(
                400,
                400,
                new SceneNode[]
                {
                    Glyph(
                        "object-glyph-Moon",
                        SceneLayer.BodyLayer,
                        200,
                        120),

                    Glyph(
                        "zodiac-glyph-0",
                        SceneLayer.ZodiacRing,
                        100,
                        100),

                    Glyph(
                        "object-glyph-Sun",
                        SceneLayer.BodyLayer,
                        200,
                        200),

                    Glyph(
                        "object-glyph-Chiron",
                        SceneLayer.PointLayer,
                        200,
                        280)
                });

        var ids =
            new NatalSceneHitTester()
                .GetSelectableObjectIds(
                    scene);

        CollectionAssert.AreEqual(
            new[]
            {
                "Moon",
                "Sun",
                "Chiron"
            },
            ids.ToArray());
    }

    private static NatalScene BuildScene()
        =>
            new(
                400,
                400,
                new SceneNode[]
                {
                    Glyph(
                        "object-glyph-Sun",
                        SceneLayer.BodyLayer,
                        200,
                        200)
                });

    private static GlyphNode Glyph(
        string id,
        SceneLayer layer,
        double x,
        double y)
        =>
            new(
                id,
                layer,
                "planet-sun",
                new ChartPoint(
                    x,
                    y),
                40,
                new ChartRect(
                    x - 20,
                    y - 20,
                    40,
                    40));
}

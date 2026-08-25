using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Layout;
using Miastro.Graphics.Layout.Placement;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Scene.Natal;
using Miastro.Graphics.Scene.Natal.Aspects;
using Miastro.Graphics.Scene.Natal.Configuration;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7VisibilityAndModeTests
{
    [TestMethod]
    public void Consultation_default_shows_configurable_content()
    {
        var result =
            Compose(
                800,
                NatalWheelSceneConfiguration
                    .ConsultationDefault);

        Assert.AreEqual(
            NatalWheelViewMode.Consultation,
            result.Mode);

        Assert.IsTrue(
            result.Scene.Nodes.Any(
                x => x.Layer
                    == SceneLayer.BodyLayer));

        Assert.IsTrue(
            result.Scene.Nodes.Any(
                x => x.Layer
                    == SceneLayer.PointLayer));

        Assert.IsTrue(
            result.Scene.Nodes.Any(
                x => x.Layer
                    == SceneLayer.AspectLayer));

        Assert.IsTrue(
            result.Scene.Nodes.Any(
                x => x is TextNode));
    }

    [TestMethod]
    public void Presentation_default_reduces_labels()
    {
        var result =
            Compose(
                800,
                NatalWheelSceneConfiguration
                    .PresentationDefault);

        Assert.AreEqual(
            NatalWheelViewMode.Presentation,
            result.Mode);

        Assert.IsFalse(
            result.Scene.Nodes.Any(
                x => x is TextNode));

        Assert.IsTrue(
            result.Scene.Nodes.Any(
                x => x.Layer
                    == SceneLayer.BodyLayer));
    }

    [TestMethod]
    public void Planets_can_be_hidden_without_hiding_points()
    {
        var result =
            Compose(
                800,
                Configuration(
                    showPlanets: false,
                    showPoints: true));

        Assert.IsFalse(
            result.Scene.Nodes.Any(
                x => x.Layer
                    == SceneLayer.BodyLayer));

        Assert.IsTrue(
            result.Scene.Nodes.Any(
                x => x.Layer
                    == SceneLayer.PointLayer));
    }

    [TestMethod]
    public void Points_can_be_hidden_without_hiding_planets()
    {
        var result =
            Compose(
                800,
                Configuration(
                    showPlanets: true,
                    showPoints: false));

        Assert.IsTrue(
            result.Scene.Nodes.Any(
                x => x.Layer
                    == SceneLayer.BodyLayer));

        Assert.IsFalse(
            result.Scene.Nodes.Any(
                x => x.Layer
                    == SceneLayer.PointLayer));
    }

    [TestMethod]
    public void Aspects_can_be_hidden_without_relayout()
    {
        var context =
            BuildContext(
                800);

        var before =
            context.Placements
                .ToDiagnosticText();

        var result =
            new NatalWheelSceneComposer()
                .Compose(
                    context.Wheel,
                    context.Placements,
                    context.Objects,
                    context.Aspects,
                    Configuration(
                        showAspects: false));

        var after =
            context.Placements
                .ToDiagnosticText();

        Assert.AreEqual(
            before,
            after);

        Assert.IsFalse(
            result.Scene.Nodes.OfType<LineNode>().Any(
                x => x.Layer
                    == SceneLayer.AspectLayer));
    }

    [TestMethod]
    public void Cusps_can_be_hidden_but_major_axes_remain()
    {
        var result =
            Compose(
                800,
                Configuration(
                    showCusps: false));

        Assert.IsFalse(
            result.Scene.Nodes.Any(
                x => x.Id.StartsWith(
                    "house-cusp-",
                    StringComparison.Ordinal)));

        Assert.IsTrue(
            result.Scene.Nodes.Any(
                x => x.Id
                    == "angle-axis-ASC"));

        Assert.IsTrue(
            result.Scene.Nodes.Any(
                x => x.Id
                    == "angle-axis-MC"));
    }

    [TestMethod]
    public void Labels_can_be_hidden()
    {
        var result =
            Compose(
                800,
                Configuration(
                    showLabels: false));

        Assert.IsFalse(
            result.Scene.Nodes.Any(
                x => x is TextNode));
    }

    [TestMethod]
    public void Responsive_policy_has_three_levels()
    {
        Assert.AreEqual(
            NatalWheelDetailLevel.Full,
            NatalWheelResponsivePolicy.Resolve(
                800,
                800));

        Assert.AreEqual(
            NatalWheelDetailLevel.Compact,
            NatalWheelResponsivePolicy.Resolve(
                600,
                600));

        Assert.AreEqual(
            NatalWheelDetailLevel.Minimal,
            NatalWheelResponsivePolicy.Resolve(
                360,
                360));
    }

    [TestMethod]
    public void Compact_mode_keeps_five_degree_ticks()
    {
        var result =
            Compose(
                600,
                NatalWheelSceneConfiguration
                    .ConsultationDefault);

        Assert.AreEqual(
            NatalWheelDetailLevel.Compact,
            result.DetailLevel);

        Assert.IsFalse(
            result.Scene.Nodes.Any(
                x => x.Id == "degree-001"));

        Assert.IsTrue(
            result.Scene.Nodes.Any(
                x => x.Id == "degree-005"));

        Assert.IsTrue(
            result.Scene.Nodes.Any(
                x => x.Id == "degree-010"));
    }

    [TestMethod]
    public void Minimal_mode_keeps_major_geometry()
    {
        var result =
            Compose(
                360,
                NatalWheelSceneConfiguration
                    .ConsultationDefault);

        Assert.AreEqual(
            NatalWheelDetailLevel.Minimal,
            result.DetailLevel);

        Assert.IsFalse(
            result.Scene.Nodes.Any(
                x => x.Id == "degree-005"));

        Assert.IsTrue(
            result.Scene.Nodes.Any(
                x => x.Id == "degree-010"));

        Assert.IsFalse(
            result.Scene.Nodes.Any(
                x => x.Id.StartsWith(
                    "house-number-",
                    StringComparison.Ordinal)));

        Assert.IsTrue(
            result.Scene.Nodes.Any(
                x => x.Id == "angle-axis-ASC"));

        Assert.IsTrue(
            result.Scene.Nodes.Any(
                x => x.Id == "angle-axis-MC"));

        Assert.IsTrue(
            result.Scene.Nodes.Any(
                x => x.Id
                    == "object-glyph-Sun"));
    }

    [TestMethod]
    public void Visual_configuration_does_not_modify_placements()
    {
        var context =
            BuildContext(
                800);

        var before =
            context.Placements
                .ToDiagnosticText();

        var composer =
            new NatalWheelSceneComposer();

        _ =
            composer.Compose(
                context.Wheel,
                context.Placements,
                context.Objects,
                context.Aspects,
                Configuration(
                    showPlanets: false,
                    showPoints: true,
                    showAspects: false,
                    showCusps: false,
                    showLabels: false));

        Assert.AreEqual(
            before,
            context.Placements
                .ToDiagnosticText());
    }

    [TestMethod]
    public void Same_configuration_is_deterministic()
    {
        var first =
            Describe(
                Compose(
                    800,
                    NatalWheelSceneConfiguration
                        .ConsultationDefault));

        for (var i = 0; i < 50; i++)
        {
            var next =
                Describe(
                    Compose(
                        800,
                        NatalWheelSceneConfiguration
                            .ConsultationDefault));

            Assert.AreEqual(
                first,
                next);
        }
    }

    private static NatalWheelSceneCompositionResult Compose(
        double size,
        NatalWheelSceneConfiguration configuration)
    {
        var context =
            BuildContext(
                size);

        return new NatalWheelSceneComposer()
            .Compose(
                context.Wheel,
                context.Placements,
                context.Objects,
                context.Aspects,
                configuration);
    }

    private static NatalWheelSceneConfiguration Configuration(
        bool showPlanets = true,
        bool showPoints = true,
        bool showAspects = true,
        bool showCusps = true,
        bool showLabels = true)
        =>
            new(
                NatalWheelViewMode.Consultation,
                new NatalWheelVisibilityOptions(
                    showPlanets,
                    showPoints,
                    showAspects,
                    showCusps,
                    showLabels));

    private static (
        NatalWheelLayoutSnapshot Wheel,
        NatalObjectPlacementSnapshot Placements,
        IReadOnlyList<NatalSceneObjectInput> Objects,
        IReadOnlyList<NatalAspectSceneInput> Aspects)
        BuildContext(
            double size)
    {
        var wheel =
            new NatalWheelLayoutBuilder()
                .Build(
                    size,
                    size,
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
                            "Moon",
                            120.1),

                        new NatalObjectLayoutInput(
                            "Chiron",
                            120.2)
                    });

        var objects =
            new NatalSceneObjectInput[]
            {
                new(
                    "Sun",
                    "planet-sun",
                    SceneLayer.BodyLayer),

                new(
                    "Moon",
                    "planet-moon",
                    SceneLayer.BodyLayer),

                new(
                    "Chiron",
                    "point-chiron",
                    SceneLayer.PointLayer)
            };

        var aspects =
            new NatalAspectSceneInput[]
            {
                new(
                    "sun-moon",
                    "Sun",
                    "Moon",
                    NatalAspectVisualClass.Major),

                new(
                    "moon-chiron",
                    "Moon",
                    "Chiron",
                    NatalAspectVisualClass.Secondary)
            };

        return (
            wheel,
            placements,
            objects,
            aspects);
    }

    private static string Describe(
        NatalWheelSceneCompositionResult result)
        =>
            string.Join(
                "\n",
                result.Scene.OrderedNodes
                    .Select(
                        x =>
                            $"{result.Mode}|{result.DetailLevel}|{x.Layer}|{x.Id}|{x.StyleKey}"));
}

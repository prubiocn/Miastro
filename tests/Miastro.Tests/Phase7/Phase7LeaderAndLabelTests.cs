using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Layout;
using Miastro.Graphics.Layout.Placement;
using Miastro.Graphics.Scene;
using Miastro.Graphics.Scene.Natal;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7LeaderAndLabelTests
{
    [TestMethod]
    public void Displaced_objects_keep_discrete_leader_lines()
    {
        var context =
            BuildContext();

        Assert.IsTrue(
            context.Scene.Nodes
                .OfType<LineNode>()
                .Any(
                    x =>
                        x.Id.StartsWith(
                            "leader-",
                            StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Wheel_does_not_print_planet_position_labels()
    {
        var context =
            BuildContext();

        Assert.IsFalse(
            context.Scene.Nodes
                .OfType<TextNode>()
                .Any(
                    x =>
                        x.Id.StartsWith(
                            "object-label-",
                            StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Wheel_does_not_render_real_position_marks()
    {
        var context =
            BuildContext();

        Assert.IsFalse(
            context.Scene.Nodes.Any(
                x =>
                    x.Id.StartsWith(
                        "real-mark-",
                        StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Real_longitudes_remain_unchanged_without_marks()
    {
        var context =
            BuildContext();

        CollectionAssert.AreEqual(
            new[]
            {
                120.0,
                120.0,
                120.0
            },
            context.Placements
                .Placements
                .Select(
                    x =>
                        x.RealLongitudeDegrees)
                .ToArray());
    }

    [TestMethod]
    public void Scene_without_printed_labels_is_deterministic()
    {
        var first =
            Describe(
                BuildContext().Scene);

        for (var i = 0; i < 20; i++)
        {
            Assert.AreEqual(
                first,
                Describe(
                    BuildContext().Scene));
        }
    }

    private static (
        NatalObjectPlacementSnapshot Placements,
        NatalScene Scene)
        BuildContext()
    {
        var wheel =
            new NatalWheelLayoutBuilder()
                .Build(
                    800,
                    800,
                    17,
                    276,
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
                            120.0),

                        new NatalObjectLayoutInput(
                            "Moon",
                            120.0),

                        new NatalObjectLayoutInput(
                            "Mercury",
                            120.0)
                    });

        var scene =
            new NatalWheelSceneBuilder()
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
                            "Moon",
                            "planet-moon",
                            SceneLayer.BodyLayer),

                        new NatalSceneObjectInput(
                            "Mercury",
                            "planet-mercury",
                            SceneLayer.BodyLayer)
                    });

        return (
            placements,
            scene);
    }

    private static string Describe(
        NatalScene scene)
        =>
            string.Join(
                "\n",
                scene.OrderedNodes
                    .Select(
                        x =>
                            x.Id
                            + "|"
                            + x.StyleKey));
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Layout;
using Miastro.Graphics.Layout.Placement;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7ObjectPlacementTests
{
    [TestMethod]
    public void Single_object_keeps_real_and_visual_angle()
    {
        var result =
            Layout(
                new NatalObjectLayoutInput(
                    "Sun",
                    120.0));

        var sun =
            result.Placements.Single();

        Assert.AreEqual(
            120.0,
            sun.RealLongitudeDegrees);

        Assert.AreEqual(
            sun.RealScreenAngleDegrees,
            sun.VisualScreenAngleDegrees);

        Assert.AreEqual(
            0,
            sun.RadialLevel);

        Assert.IsFalse(
            sun.HasLeaderLine);
    }

    [TestMethod]
    public void Two_coincident_objects_do_not_overlap()
    {
        var result =
            Layout(
                new NatalObjectLayoutInput(
                    "Moon",
                    120.0),
                new NatalObjectLayoutInput(
                    "Sun",
                    120.0));

        AssertNoOverlap(
            result);

        Assert.AreNotEqual(
            result.Placements[0].RadialLevel,
            result.Placements[1].RadialLevel);
    }

    [TestMethod]
    public void Three_coincident_objects_do_not_overlap()
    {
        var result =
            Layout(
                new NatalObjectLayoutInput(
                    "A",
                    120),
                new NatalObjectLayoutInput(
                    "B",
                    120),
                new NatalObjectLayoutInput(
                    "C",
                    120));

        AssertNoOverlap(
            result);
    }

    [TestMethod]
    public void Five_body_stellium_has_zero_overlaps()
    {
        var result =
            Layout(
                new NatalObjectLayoutInput(
                    "A",
                    150.00),
                new NatalObjectLayoutInput(
                    "B",
                    150.10),
                new NatalObjectLayoutInput(
                    "C",
                    150.20),
                new NatalObjectLayoutInput(
                    "D",
                    150.30),
                new NatalObjectLayoutInput(
                    "E",
                    150.40));

        AssertNoOverlap(
            result);

        Assert.AreEqual(
            5,
            result.Placements.Count);
    }

    [TestMethod]
    public void Extreme_stellium_has_zero_overlaps()
    {
        var inputs =
            Enumerable
                .Range(1, 9)
                .Select(
                    i =>
                        new NatalObjectLayoutInput(
                            $"P{i:00}",
                            203.0
                                + i * 0.01))
                .ToArray();

        var result =
            Layout(inputs);

        AssertNoOverlap(
            result);

        Assert.AreEqual(
            9,
            result.Placements.Count);
    }

    [TestMethod]
    public void Cluster_across_zero_degree_boundary_has_zero_overlaps()
    {
        var result =
            Layout(
                new NatalObjectLayoutInput(
                    "A",
                    359.80),
                new NatalObjectLayoutInput(
                    "B",
                    359.95),
                new NatalObjectLayoutInput(
                    "C",
                    0.05),
                new NatalObjectLayoutInput(
                    "D",
                    0.20));

        AssertNoOverlap(
            result);
    }

    [TestMethod]
    public void Real_longitudes_are_never_modified()
    {
        var original =
            new[]
            {
                359.9,
                0.1,
                120.123456789,
                250.0
            };

        var result =
            Layout(
                original
                    .Select(
                        (longitude, index) =>
                            new NatalObjectLayoutInput(
                                $"P{index}",
                                longitude))
                    .ToArray());

        foreach (var placement in result.Placements)
        {
            var expected =
                original[
                    int.Parse(
                        placement.Id[1..])];

            Assert.AreEqual(
                expected,
                placement.RealLongitudeDegrees,
                1e-12);
        }
    }

    [TestMethod]
    public void Visual_layout_preserves_zodiac_order()
    {
        var result =
            Layout(
                new NatalObjectLayoutInput(
                    "D",
                    90),
                new NatalObjectLayoutInput(
                    "B",
                    20),
                new NatalObjectLayoutInput(
                    "C",
                    45),
                new NatalObjectLayoutInput(
                    "A",
                    10));

        CollectionAssert.AreEqual(
            new[]
            {
                "A",
                "B",
                "C",
                "D"
            },
            result.Placements
                .Select(x => x.Id)
                .ToArray());

        foreach (var placement in result.Placements)
        {
            Assert.AreEqual(
                placement.RealScreenAngleDegrees,
                placement.VisualScreenAngleDegrees);
        }
    }

    [TestMethod]
    public void Input_order_does_not_change_layout()
    {
        var engine =
            new NatalObjectPlacementEngine();

        var wheel =
            BuildWheel();

        var inputs =
            new[]
            {
                new NatalObjectLayoutInput("Sun", 120.0),
                new NatalObjectLayoutInput("Moon", 120.1),
                new NatalObjectLayoutInput("Mercury", 120.2),
                new NatalObjectLayoutInput("Venus", 120.3),
                new NatalObjectLayoutInput("Mars", 120.4)
            };

        var forward =
            engine.Layout(
                    wheel,
                    inputs)
                .ToDiagnosticText();

        var reversed =
            engine.Layout(
                    wheel,
                    inputs.Reverse().ToArray())
                .ToDiagnosticText();

        Assert.AreEqual(
            forward,
            reversed);
    }

    [TestMethod]
    public void Layout_is_exactly_repeatable()
    {
        var inputs =
            new[]
            {
                new NatalObjectLayoutInput("A", 45.00),
                new NatalObjectLayoutInput("B", 45.05),
                new NatalObjectLayoutInput("C", 45.10),
                new NatalObjectLayoutInput("D", 45.15),
                new NatalObjectLayoutInput("E", 45.20)
            };

        var first =
            Layout(inputs)
                .ToDiagnosticText();

        for (var i = 0; i < 100; i++)
        {
            Assert.AreEqual(
                first,
                Layout(inputs)
                    .ToDiagnosticText());
        }
    }

    [TestMethod]
    public void Displaced_object_gets_leader_line()
    {
        var result =
            Layout(
                new NatalObjectLayoutInput(
                    "A",
                    100),
                new NatalObjectLayoutInput(
                    "B",
                    100));

        var displaced =
            result.Placements.Single(
                x => x.RadialLevel != 0);

        Assert.IsTrue(
            displaced.HasLeaderLine);

        Assert.IsNotNull(
            displaced.LeaderLineStart);

        Assert.IsNotNull(
            displaced.LeaderLineEnd);

        Assert.AreEqual(
            displaced.RealAnchor,
            displaced.LeaderLineStart);

        Assert.AreEqual(
            displaced.VisualCenter,
            displaced.LeaderLineEnd);
    }

    [TestMethod]
    public void Duplicate_ids_are_rejected()
    {
        Assert.ThrowsExactly<ArgumentException>(
            () =>
                Layout(
                    new NatalObjectLayoutInput(
                        "Sun",
                        10),
                    new NatalObjectLayoutInput(
                        "Sun",
                        20)));
    }

    private static NatalObjectPlacementSnapshot Layout(
        params NatalObjectLayoutInput[] inputs)
        =>
            new NatalObjectPlacementEngine()
                .Layout(
                    BuildWheel(),
                    inputs);

    private static NatalWheelLayoutSnapshot BuildWheel()
        =>
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

    private static void AssertNoOverlap(
        NatalObjectPlacementSnapshot snapshot)
    {
        for (
            var i = 0;
            i < snapshot.Placements.Count;
            i++)
        {
            for (
                var j = i + 1;
                j < snapshot.Placements.Count;
                j++)
            {
                Assert.IsFalse(
                    snapshot.Placements[i]
                        .Bounds
                        .Intersects(
                            snapshot.Placements[j]
                                .Bounds),
                    $"{snapshot.Placements[i].Id} overlaps {snapshot.Placements[j].Id}");
            }
        }
    }
}

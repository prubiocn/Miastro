using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Layout;
using Miastro.Graphics.Layout.Placement;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7PlacementBoundaryTests
{
    [TestMethod]
    public void Extreme_stellium_stays_inside_safe_annulus()
    {
        var wheel =
            BuildWheel(
                800);

        var result =
            Layout(
                wheel,
                BuildCluster(
                    9,
                    203.0,
                    0.01));

        AssertInsideSafeAnnulus(
            wheel,
            result);

        AssertNoOverlap(
            result);
    }

    [TestMethod]
    public void Small_canvas_stellium_stays_inside_safe_annulus()
    {
        var wheel =
            BuildWheel(
                360);

        var result =
            Layout(
                wheel,
                BuildCluster(
                    9,
                    203.0,
                    0.01));

        AssertInsideSafeAnnulus(
            wheel,
            result);

        AssertNoOverlap(
            result);
    }

    [TestMethod]
    public void Ascendant_cluster_has_zero_overlap_and_safe_bounds()
    {
        var wheel =
            BuildWheel(
                800);

        var result =
            Layout(
                wheel,
                BuildCluster(
                    9,
                    17.0,
                    0.01));

        AssertInsideSafeAnnulus(
            wheel,
            result);

        AssertNoOverlap(
            result);
    }

    [TestMethod]
    public void Midheaven_cluster_has_zero_overlap_and_safe_bounds()
    {
        var wheel =
            BuildWheel(
                800);

        var result =
            Layout(
                wheel,
                BuildCluster(
                    9,
                    103.0,
                    0.01));

        AssertInsideSafeAnnulus(
            wheel,
            result);

        AssertNoOverlap(
            result);
    }

    [TestMethod]
    public void Dense_cluster_uses_angular_displacement_when_needed()
    {
        var result =
            Layout(
                BuildWheel(
                    800),
                BuildCluster(
                    9,
                    203.0,
                    0.01));

        Assert.IsTrue(
            result.Placements.Any(
                x =>
                    Math.Abs(
                        x.AngularOffsetDegrees)
                    > 1e-9));
    }

    [TestMethod]
    public void Angular_displacement_preserves_zodiac_order()
    {
        const double ascendant =
            17.0;

        var result =
            Layout(
                BuildWheel(
                    800),
                BuildCluster(
                    12,
                    203.0,
                    0.01));

        double? previous =
            null;

        foreach (
            var placement
            in result.Placements)
        {
            var realUnwrapped =
                180.0
                - (
                    placement.RealLongitudeDegrees
                    - ascendant
                );

            var visualUnwrapped =
                realUnwrapped
                + placement.AngularOffsetDegrees;

            if (previous is double previousValue)
            {
                Assert.IsTrue(
                    visualUnwrapped
                        <= previousValue + 1e-9,
                    $"{placement.Id}: {visualUnwrapped} > {previousValue}");
            }

            previous =
                visualUnwrapped;
        }
    }

    [TestMethod]
    public void Boundary_layout_is_input_order_independent()
    {
        var wheel =
            BuildWheel(
                800);

        var inputs =
            BuildCluster(
                12,
                203.0,
                0.01);

        var forward =
            Layout(
                wheel,
                inputs)
            .ToDiagnosticText();

        var reversed =
            Layout(
                wheel,
                inputs
                    .Reverse()
                    .ToArray())
            .ToDiagnosticText();

        Assert.AreEqual(
            forward,
            reversed);
    }

    [TestMethod]
    public void Boundary_layout_never_changes_real_longitudes()
    {
        var wheel =
            BuildWheel(
                800);

        var inputs =
            new[]
            {
                new NatalObjectLayoutInput(
                    "A",
                    359.95),

                new NatalObjectLayoutInput(
                    "B",
                    0.05),

                new NatalObjectLayoutInput(
                    "C",
                    17.0),

                new NatalObjectLayoutInput(
                    "D",
                    103.0),

                new NatalObjectLayoutInput(
                    "E",
                    203.123456789)
            };

        var expected =
            inputs.ToDictionary(
                x => x.Id,
                x =>
                    Normalize(
                        x.RealLongitudeDegrees),
                StringComparer.Ordinal);

        var result =
            Layout(
                wheel,
                inputs);

        foreach (
            var placement
            in result.Placements)
        {
            Assert.AreEqual(
                expected[placement.Id],
                placement.RealLongitudeDegrees,
                1e-12);
        }
    }

    private static NatalObjectPlacementSnapshot Layout(
        NatalWheelLayoutSnapshot wheel,
        IReadOnlyList<NatalObjectLayoutInput> inputs)
        =>
            new NatalObjectPlacementEngine()
                .Layout(
                    wheel,
                    inputs);

    private static NatalObjectLayoutInput[] BuildCluster(
        int count,
        double start,
        double step)
        =>
            Enumerable
                .Range(
                    0,
                    count)
                .Select(
                    index =>
                        new NatalObjectLayoutInput(
                            $"P{index:00}",
                            start
                            + index * step))
                .ToArray();

    private static NatalWheelLayoutSnapshot BuildWheel(
        double size)
        =>
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

    private static void AssertInsideSafeAnnulus(
        NatalWheelLayoutSnapshot wheel,
        NatalObjectPlacementSnapshot result)
    {
        var policy =
            NatalGlyphLayoutPolicy
                .FromMetrics(
                    wheel.Metrics);

        var protectedSize =
            policy.GlyphSize
            + policy.MinimumGap;

        var halfDiagonal =
            protectedSize
            / Math.Sqrt(2.0);

        var minimum =
            wheel.Metrics.HouseInnerRadius
            + halfDiagonal;

        var maximum =
            wheel.Metrics.ZodiacInnerRadius
            - halfDiagonal;

        foreach (
            var placement
            in result.Placements)
        {
            Assert.IsTrue(
                placement.VisualRadius
                    >= minimum - 1e-9,
                $"{placement.Id}: radius below safe annulus.");

            Assert.IsTrue(
                placement.VisualRadius
                    <= maximum + 1e-9,
                $"{placement.Id}: radius above safe annulus.");
        }
    }

    private static void AssertNoOverlap(
        NatalObjectPlacementSnapshot result)
    {
        for (
            var first = 0;
            first < result.Placements.Count;
            first++)
        {
            for (
                var second = first + 1;
                second < result.Placements.Count;
                second++)
            {
                Assert.IsFalse(
                    result.Placements[first]
                        .Bounds
                        .Intersects(
                            result.Placements[second]
                                .Bounds),
                    $"{result.Placements[first].Id} overlaps {result.Placements[second].Id}");
            }
        }
    }

    private static double Normalize(
        double value)
    {
        var normalized =
            value % 360.0;

        return normalized < 0.0
            ? normalized + 360.0
            : normalized;
    }
}

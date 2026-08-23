using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Layout;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7LayoutSnapshotTests
{
    private const double Tolerance = 1e-9;

    private static readonly double[] Cusps =
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

    [TestMethod]
    public void Snapshot_has_twelve_zodiac_sectors()
    {
        var snapshot =
            Build();

        Assert.AreEqual(
            12,
            snapshot.ZodiacSectors.Count);

        for (var i = 0; i < 12; i++)
        {
            Assert.AreEqual(
                i,
                snapshot.ZodiacSectors[i].SignIndex);

            Assert.AreEqual(
                i * 30.0,
                snapshot.ZodiacSectors[i]
                    .StartLongitudeDegrees,
                Tolerance);

            Assert.AreEqual(
                -30.0,
                snapshot.ZodiacSectors[i]
                    .SweepAngleDegrees,
                Tolerance);
        }
    }

    [TestMethod]
    public void Snapshot_has_360_degree_ticks()
    {
        var snapshot =
            Build();

        Assert.AreEqual(
            360,
            snapshot.DegreeTicks.Count);

        Assert.AreEqual(
            36,
            snapshot.DegreeTicks.Count(
                x => x.Kind
                    == DegreeTickKind.TenDegree));

        Assert.AreEqual(
            36,
            snapshot.DegreeTicks.Count(
                x => x.Kind
                    == DegreeTickKind.FiveDegree));

        Assert.AreEqual(
            288,
            snapshot.DegreeTicks.Count(
                x => x.Kind
                    == DegreeTickKind.Minor));
    }

    [TestMethod]
    public void Snapshot_preserves_real_house_cusps()
    {
        var snapshot =
            Build();

        Assert.AreEqual(
            12,
            snapshot.HouseCusps.Count);

        for (var i = 0; i < 12; i++)
        {
            Assert.AreEqual(
                Cusps[i],
                snapshot.HouseCusps[i]
                    .RealLongitudeDegrees,
                Tolerance);

            Assert.AreEqual(
                i + 1,
                snapshot.HouseCusps[i]
                    .HouseNumber);
        }
    }

    [TestMethod]
    public void Ascendant_axis_is_exactly_left()
    {
        var snapshot =
            Build();

        var asc =
            snapshot.AngleAxes.Single(
                x => x.Kind
                    == NatalAngleKind.Ascendant);

        Assert.AreEqual(
            180.0,
            asc.ScreenAngleDegrees,
            Tolerance);

        Assert.IsTrue(
            asc.OuterPoint.X
                < snapshot.Metrics.Center.X);

        Assert.AreEqual(
            snapshot.Metrics.Center.Y,
            asc.OuterPoint.Y,
            Tolerance);
    }

    [TestMethod]
    public void Descendant_is_derived_from_ascendant()
    {
        var snapshot =
            Build();

        var dsc =
            snapshot.AngleAxes.Single(
                x => x.Kind
                    == NatalAngleKind.Descendant);

        Assert.AreEqual(
            197.0,
            dsc.RealLongitudeDegrees,
            Tolerance);

        Assert.AreEqual(
            0.0,
            dsc.ScreenAngleDegrees,
            Tolerance);
    }

    [TestMethod]
    public void ImumCoeli_is_derived_from_midheaven()
    {
        var snapshot =
            Build();

        var ic =
            snapshot.AngleAxes.Single(
                x => x.Kind
                    == NatalAngleKind.ImumCoeli);

        Assert.AreEqual(
            283.0,
            ic.RealLongitudeDegrees,
            Tolerance);
    }

    [TestMethod]
    public void House_center_handles_zero_degree_wrap()
    {
        var cusps =
            new double[]
            {
                350,
                20,
                50,
                80,
                110,
                140,
                170,
                200,
                230,
                260,
                290,
                320
            };

        var snapshot =
            new NatalWheelLayoutBuilder()
                .Build(
                    800,
                    800,
                    350,
                    260,
                    cusps);

        Assert.AreEqual(
            5.0,
            snapshot.HouseCusps[0]
                .HouseCenterLongitudeDegrees,
            Tolerance);
    }

    [TestMethod]
    public void Layout_is_exactly_deterministic()
    {
        var builder =
            new NatalWheelLayoutBuilder();

        var first =
            builder.Build(
                    800,
                    800,
                    17,
                    103,
                    Cusps)
                .ToDiagnosticText();

        for (var i = 0; i < 50; i++)
        {
            var next =
                builder.Build(
                        800,
                        800,
                        17,
                        103,
                        Cusps)
                    .ToDiagnosticText();

            Assert.AreEqual(
                first,
                next);
        }
    }

    [TestMethod]
    public void Layout_scales_from_reference_size()
    {
        var large =
            new NatalWheelLayoutBuilder()
                .Build(
                    800,
                    800,
                    17,
                    103,
                    Cusps);

        var medium =
            new NatalWheelLayoutBuilder()
                .Build(
                    400,
                    400,
                    17,
                    103,
                    Cusps);

        Assert.AreEqual(
            1.0,
            large.Metrics.Scale,
            Tolerance);

        Assert.AreEqual(
            0.5,
            medium.Metrics.Scale,
            Tolerance);

        Assert.AreEqual(
            large.Metrics.OuterRadius / 2.0,
            medium.Metrics.OuterRadius,
            Tolerance);
    }

    [TestMethod]
    public void Minimum_layout_remains_usable()
    {
        var snapshot =
            new NatalWheelLayoutBuilder()
                .Build(
                    300,
                    300,
                    17,
                    103,
                    Cusps);

        Assert.AreEqual(
            NatalWheelMetrics.MinimumUsableSize
                / NatalWheelMetrics.ReferenceSize,
            snapshot.Metrics.Scale,
            Tolerance);

        Assert.IsTrue(
            snapshot.Metrics.OuterRadius > 0);
    }

    [TestMethod]
    public void Builder_rejects_non_twelve_house_input()
    {
        Assert.ThrowsExactly<ArgumentException>(
            () =>
                new NatalWheelLayoutBuilder()
                    .Build(
                        800,
                        800,
                        17,
                        103,
                        new double[11]));
    }

    private static NatalWheelLayoutSnapshot Build()
        =>
            new NatalWheelLayoutBuilder()
                .Build(
                    800,
                    800,
                    17,
                    103,
                    Cusps);
}

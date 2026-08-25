using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Geometry;
using Miastro.Graphics.Layout;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7CounterclockwiseWheelTests
{
    private const double Tolerance =
        1e-9;

    [TestMethod]
    public void Zodiac_longitudes_advance_counterclockwise_from_ascendant()
    {
        const double ascendant =
            17.0;

        var center =
            new ChartPoint(
                400,
                400);

        var asc =
            NatalWheelCoordinates
                .PointForLongitude(
                    center,
                    200,
                    ascendant,
                    ascendant);

        var plus90 =
            NatalWheelCoordinates
                .PointForLongitude(
                    center,
                    200,
                    ascendant + 90,
                    ascendant);

        Assert.AreEqual(
            200.0,
            asc.X,
            Tolerance);

        Assert.AreEqual(
            400.0,
            asc.Y,
            Tolerance);

        // Desde las 9 hacia las 6: lectura antihoraria.
        Assert.AreEqual(
            400.0,
            plus90.X,
            Tolerance);

        Assert.AreEqual(
            600.0,
            plus90.Y,
            Tolerance);
    }

    [TestMethod]
    public void Zodiac_sectors_have_counterclockwise_sweep()
    {
        var wheel =
            BuildWheel();

        Assert.IsTrue(
            wheel.ZodiacSectors.All(
                x =>
                    Math.Abs(
                        x.SweepAngleDegrees
                        - 30.0)
                    < Tolerance));
    }

    [TestMethod]
    public void Houses_advance_counterclockwise_from_house_one()
    {
        var wheel =
            BuildWheel();

        var house1 =
            wheel.HouseCusps[0];

        var house2 =
            wheel.HouseCusps[1];

        Assert.AreEqual(
            180.0,
            house1.ScreenAngleDegrees,
            Tolerance);

        var forward =
            NatalWheelCoordinates
                .NormalizeDegrees(
                    house2.ScreenAngleDegrees
                    - house1.ScreenAngleDegrees);

        Assert.IsTrue(
            forward > 0.0
            && forward < 180.0);
    }

    [TestMethod]
    public void Ascendant_and_descendant_remain_left_and_right()
    {
        var wheel =
            BuildWheel();

        var asc =
            wheel.AngleAxes.Single(
                x =>
                    x.Kind
                    == NatalAngleKind.Ascendant);

        var dsc =
            wheel.AngleAxes.Single(
                x =>
                    x.Kind
                    == NatalAngleKind.Descendant);

        Assert.AreEqual(
            180.0,
            asc.ScreenAngleDegrees,
            Tolerance);

        Assert.AreEqual(
            0.0,
            dsc.ScreenAngleDegrees,
            Tolerance);
    }

    [TestMethod]
    public void Consistent_house_ten_midheaven_is_in_upper_hemisphere()
    {
        var wheel =
            BuildWheel();

        var mc =
            wheel.AngleAxes.Single(
                x =>
                    x.Kind
                    == NatalAngleKind.Midheaven);

        Assert.IsTrue(
            mc.OuterPoint.Y
            < wheel.Metrics.Center.Y);
    }

    private static NatalWheelLayoutSnapshot
        BuildWheel()
    {
        var cusps =
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
            };

        return new NatalWheelLayoutBuilder()
            .Build(
                800,
                800,
                17,
                cusps[9],
                cusps);
    }
}

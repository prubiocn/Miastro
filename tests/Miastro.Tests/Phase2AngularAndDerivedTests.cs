using Miastro.Domain.Angles;
using Miastro.Domain.DerivedPoints;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase2AngularAndDerivedTests
{
    [TestMethod]
    [DataRow(360.0, 0.0)]
    [DataRow(361.0, 1.0)]
    [DataRow(-1.0, 359.0)]
    [DataRow(720.0, 0.0)]
    public void Longitude_normalizes(
        double input,
        double expected)
    {
        var longitude =
            EclipticLongitude.FromDegrees(input);

        Assert.AreEqual(
            expected,
            longitude.Degrees,
            1e-12);
    }

    [TestMethod]
    public void Separation_across_zero_is_two_degrees()
    {
        var a = EclipticLongitude.FromDegrees(359.0);
        var b = EclipticLongitude.FromDegrees(1.0);

        var separation =
            AngularSeparation.Between(a, b);

        Assert.AreEqual(
            2.0,
            separation.Degrees,
            1e-12);
    }

    [TestMethod]
    public void Separation_can_reach_180()
    {
        var a = EclipticLongitude.FromDegrees(0.0);
        var b = EclipticLongitude.FromDegrees(180.0);

        Assert.AreEqual(
            180.0,
            AngularSeparation.Between(a, b).Degrees,
            1e-12);
    }

    [TestMethod]
    [DataRow(0.0, 180.0)]
    [DataRow(180.0, 0.0)]
    [DataRow(359.0, 179.0)]
    public void South_node_is_opposite_true_north_node(
        double north,
        double expectedSouth)
    {
        var result =
            LunarNodeCalculator.CalculateSouthNode(
                EclipticLongitude.FromDegrees(north));

        Assert.AreEqual(
            expectedSouth,
            result.Degrees,
            1e-12);
    }

    [TestMethod]
    public void Part_of_fortune_day_formula_is_correct()
    {
        var result =
            PartOfFortuneCalculator.Calculate(
                EclipticLongitude.FromDegrees(100.0),
                EclipticLongitude.FromDegrees(20.0),
                EclipticLongitude.FromDegrees(50.0),
                ChartSect.Day);

        Assert.AreEqual(
            130.0,
            result.Degrees,
            1e-12);
    }

    [TestMethod]
    public void Part_of_fortune_night_formula_is_correct()
    {
        var result =
            PartOfFortuneCalculator.Calculate(
                EclipticLongitude.FromDegrees(100.0),
                EclipticLongitude.FromDegrees(20.0),
                EclipticLongitude.FromDegrees(50.0),
                ChartSect.Night);

        Assert.AreEqual(
            70.0,
            result.Degrees,
            1e-12);
    }

    [TestMethod]
    public void Part_of_fortune_normalizes_negative_result()
    {
        var result =
            PartOfFortuneCalculator.Calculate(
                EclipticLongitude.FromDegrees(10.0),
                EclipticLongitude.FromDegrees(300.0),
                EclipticLongitude.FromDegrees(20.0),
                ChartSect.Day);

        Assert.AreEqual(
            90.0,
            result.Degrees,
            1e-12);
    }

    [TestMethod]
    public void Part_of_fortune_normalizes_result_above_360()
    {
        var result =
            PartOfFortuneCalculator.Calculate(
                EclipticLongitude.FromDegrees(350.0),
                EclipticLongitude.FromDegrees(10.0),
                EclipticLongitude.FromDegrees(100.0),
                ChartSect.Day);

        Assert.AreEqual(
            80.0,
            result.Degrees,
            1e-12);
    }
}

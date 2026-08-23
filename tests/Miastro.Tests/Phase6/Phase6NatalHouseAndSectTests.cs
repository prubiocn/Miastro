using Miastro.Domain.Angles;
using Miastro.Domain.Charts;
using Miastro.Domain.DerivedPoints;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6NatalHouseAndSectTests
{
    [TestMethod]
    public void Object_inside_house_is_assigned_correctly()
    {
        var house =
            NatalHousePlacementResolver.Resolve(
                EclipticLongitude.FromDegrees(45.0),
                EqualCusps());

        Assert.AreEqual(
            2,
            house.Number);
    }

    [TestMethod]
    public void Exact_cusp_belongs_to_house_that_begins_there()
    {
        var house =
            NatalHousePlacementResolver.Resolve(
                EclipticLongitude.FromDegrees(60.0),
                EqualCusps());

        Assert.AreEqual(
            3,
            house.Number);
    }

    [TestMethod]
    public void Wrap_zero_is_assigned_correctly()
    {
        var cusps =
            Cusps(
                350, 20, 50, 80, 110, 140,
                170, 200, 230, 260, 290, 320);

        var house =
            NatalHousePlacementResolver.Resolve(
                EclipticLongitude.FromDegrees(5.0),
                cusps);

        Assert.AreEqual(
            1,
            house.Number);
    }

    [TestMethod]
    public void Unequal_houses_are_not_assigned_by_sign()
    {
        var cusps =
            Cusps(
                10, 55, 82, 109, 137, 165,
                190, 218, 246, 277, 309, 338);

        var house =
            NatalHousePlacementResolver.Resolve(
                EclipticLongitude.FromDegrees(50.0),
                cusps);

        Assert.AreEqual(
            1,
            house.Number);
    }

    [TestMethod]
    public void Sun_in_upper_hemisphere_is_day_chart()
    {
        var sect =
            NatalChartSectResolver.Resolve(
                EclipticLongitude.FromDegrees(200.0),
                EqualCusps());

        Assert.AreEqual(
            ChartSect.Day,
            sect);
    }

    [TestMethod]
    public void Sun_in_lower_hemisphere_is_night_chart()
    {
        var sect =
            NatalChartSectResolver.Resolve(
                EclipticLongitude.FromDegrees(20.0),
                EqualCusps());

        Assert.AreEqual(
            ChartSect.Night,
            sect);
    }

    [TestMethod]
    public void Sun_exactly_on_descendant_is_day_by_cusp_rule()
    {
        var sect =
            NatalChartSectResolver.Resolve(
                EclipticLongitude.FromDegrees(180.0),
                EqualCusps());

        Assert.AreEqual(
            ChartSect.Day,
            sect);
    }

    [TestMethod]
    public void Sun_exactly_on_ascendant_is_night_by_cusp_rule()
    {
        var sect =
            NatalChartSectResolver.Resolve(
                EclipticLongitude.FromDegrees(0.0),
                EqualCusps());

        Assert.AreEqual(
            ChartSect.Night,
            sect);
    }

    private static IReadOnlyList<HouseCusp>
        EqualCusps()
        => Cusps(
            0, 30, 60, 90, 120, 150,
            180, 210, 240, 270, 300, 330);

    private static IReadOnlyList<HouseCusp> Cusps(
        params double[] values)
        => values
            .Select(
                (longitude, index) =>
                    new HouseCusp(
                        AstrologicalHouse
                            .FromNumber(index + 1),
                        EclipticLongitude
                            .FromDegrees(longitude)))
            .ToArray();
}

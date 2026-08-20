using Miastro.Domain.Angles;
using Miastro.Domain.Aspects;
using Miastro.Domain.Calculation;
using Miastro.Domain.Charts;
using Miastro.Domain.Houses;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase2ChartTests
{
    [TestMethod]
    public void Minimal_chart_can_be_created()
    {
        var chart = new AstrologicalChart(
            Guid.NewGuid(),
            ChartType.Natal,
            [
                new AstrologicalPlacement(
                    AstrologicalObjectId.Sun,
                    EclipticLongitude.FromDegrees(15.0))
            ],
            CalculationProfile.MiastroV1,
            MiastroV1AspectProfile.Instance,
            new CalculationMetadata());

        Assert.AreEqual(ChartType.Natal, chart.Type);
        Assert.HasCount(1, chart.Placements);
        Assert.HasCount(0, chart.HouseCusps);
        Assert.IsNull(chart.HouseSystem);
    }

    [TestMethod]
    public void Empty_chart_id_is_rejected()
    {
        Assert.ThrowsExactly<ArgumentException>(
            () => new AstrologicalChart(
                Guid.Empty,
                ChartType.Natal,
                [],
                CalculationProfile.MiastroV1,
                MiastroV1AspectProfile.Instance,
                new CalculationMetadata()));
    }

    [TestMethod]
    public void Duplicate_objects_are_rejected()
    {
        var placements =
            new[]
            {
                new AstrologicalPlacement(
                    AstrologicalObjectId.Mars,
                    EclipticLongitude.FromDegrees(10.0)),
                new AstrologicalPlacement(
                    AstrologicalObjectId.Mars,
                    EclipticLongitude.FromDegrees(20.0))
            };

        Assert.ThrowsExactly<ArgumentException>(
            () => new AstrologicalChart(
                Guid.NewGuid(),
                ChartType.Natal,
                placements,
                CalculationProfile.MiastroV1,
                MiastroV1AspectProfile.Instance,
                new CalculationMetadata()));
    }

    [TestMethod]
    public void Complete_house_cusps_are_accepted()
    {
        var cusps =
            Enumerable.Range(1, 12)
                .Select(number =>
                    new HouseCusp(
                        AstrologicalHouse.FromNumber(number),
                        EclipticLongitude.FromDegrees(
                            (number - 1) * 30.0)))
                .ToArray();

        var chart = new AstrologicalChart(
            Guid.NewGuid(),
            ChartType.Natal,
            [],
            CalculationProfile.MiastroV1,
            MiastroV1AspectProfile.Instance,
            new CalculationMetadata(
                houseSystem: HouseSystem.Placidus),
            cusps,
            HouseSystem.Placidus);

        Assert.HasCount(12, chart.HouseCusps);
        Assert.AreEqual(
            HouseSystem.Placidus,
            chart.HouseSystem);
    }

    [TestMethod]
    public void House_cusps_require_house_system()
    {
        var cusps =
            Enumerable.Range(1, 12)
                .Select(number =>
                    new HouseCusp(
                        AstrologicalHouse.FromNumber(number),
                        EclipticLongitude.FromDegrees(
                            (number - 1) * 30.0)))
                .ToArray();

        Assert.ThrowsExactly<ArgumentException>(
            () => new AstrologicalChart(
                Guid.NewGuid(),
                ChartType.Natal,
                [],
                CalculationProfile.MiastroV1,
                MiastroV1AspectProfile.Instance,
                new CalculationMetadata(),
                cusps));
    }

    [TestMethod]
    public void Incomplete_house_cusps_are_rejected()
    {
        var cusps =
            Enumerable.Range(1, 11)
                .Select(number =>
                    new HouseCusp(
                        AstrologicalHouse.FromNumber(number),
                        EclipticLongitude.FromDegrees(
                            (number - 1) * 30.0)))
                .ToArray();

        Assert.ThrowsExactly<ArgumentException>(
            () => new AstrologicalChart(
                Guid.NewGuid(),
                ChartType.Natal,
                [],
                CalculationProfile.MiastroV1,
                MiastroV1AspectProfile.Instance,
                new CalculationMetadata(),
                cusps,
                HouseSystem.Koch));
    }

    [TestMethod]
    public void All_required_chart_types_exist()
    {
        var values = Enum.GetValues<ChartType>();

        Assert.Contains(ChartType.Natal, values);
        Assert.Contains(ChartType.SolarReturn, values);
        Assert.Contains(ChartType.LunarReturn, values);
        Assert.Contains(ChartType.Transit, values);
        Assert.Contains(
            ChartType.SecondaryProgression,
            values);
        Assert.Contains(
            ChartType.SynastryReference,
            values);
    }
}

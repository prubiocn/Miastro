using Miastro.Domain.Angles;
using Miastro.Domain.Aspects;
using Miastro.Domain.Calculation;
using Miastro.Domain.Houses;
using Miastro.Domain.Objects;
using Miastro.Domain.Rulerships;
using Miastro.Domain.Zodiac;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase2FinalCoverageTests
{
    [TestMethod]
    public void All_twelve_zodiac_signs_have_valid_canonical_properties()
    {
        var signs = Enum.GetValues<ZodiacSign>();

        Assert.HasCount(12, signs);

        foreach (var sign in signs)
        {
            var index = ZodiacSignInfo.GetIndex(sign);

            Assert.IsGreaterThanOrEqualTo(0, index);
            Assert.IsLessThan(12, index);

            var opposite = ZodiacSignInfo.GetOpposite(sign);

            Assert.AreEqual(
                sign,
                ZodiacSignInfo.GetOpposite(opposite));

            var axis = ZodiacSignInfo.GetAxis(sign);

            Assert.AreEqual(sign, axis.First);
            Assert.AreEqual(opposite, axis.Second);

            _ = ZodiacSignInfo.GetElement(sign);
            _ = ZodiacSignInfo.GetModality(sign);
            _ = ZodiacSignInfo.GetPolarity(sign);
        }
    }

    [TestMethod]
    public void Zodiac_elements_are_canonical()
    {
        Assert.AreEqual(ZodiacElement.Fire, ZodiacSignInfo.GetElement(ZodiacSign.Aries));
        Assert.AreEqual(ZodiacElement.Earth, ZodiacSignInfo.GetElement(ZodiacSign.Taurus));
        Assert.AreEqual(ZodiacElement.Air, ZodiacSignInfo.GetElement(ZodiacSign.Gemini));
        Assert.AreEqual(ZodiacElement.Water, ZodiacSignInfo.GetElement(ZodiacSign.Cancer));
        Assert.AreEqual(ZodiacElement.Fire, ZodiacSignInfo.GetElement(ZodiacSign.Leo));
        Assert.AreEqual(ZodiacElement.Earth, ZodiacSignInfo.GetElement(ZodiacSign.Virgo));
        Assert.AreEqual(ZodiacElement.Air, ZodiacSignInfo.GetElement(ZodiacSign.Libra));
        Assert.AreEqual(ZodiacElement.Water, ZodiacSignInfo.GetElement(ZodiacSign.Scorpio));
        Assert.AreEqual(ZodiacElement.Fire, ZodiacSignInfo.GetElement(ZodiacSign.Sagittarius));
        Assert.AreEqual(ZodiacElement.Earth, ZodiacSignInfo.GetElement(ZodiacSign.Capricorn));
        Assert.AreEqual(ZodiacElement.Air, ZodiacSignInfo.GetElement(ZodiacSign.Aquarius));
        Assert.AreEqual(ZodiacElement.Water, ZodiacSignInfo.GetElement(ZodiacSign.Pisces));
    }

    [TestMethod]
    public void Zodiac_modalities_are_canonical()
    {
        Assert.AreEqual(ZodiacModality.Cardinal, ZodiacSignInfo.GetModality(ZodiacSign.Aries));
        Assert.AreEqual(ZodiacModality.Fixed, ZodiacSignInfo.GetModality(ZodiacSign.Taurus));
        Assert.AreEqual(ZodiacModality.Mutable, ZodiacSignInfo.GetModality(ZodiacSign.Gemini));
        Assert.AreEqual(ZodiacModality.Cardinal, ZodiacSignInfo.GetModality(ZodiacSign.Cancer));
        Assert.AreEqual(ZodiacModality.Fixed, ZodiacSignInfo.GetModality(ZodiacSign.Leo));
        Assert.AreEqual(ZodiacModality.Mutable, ZodiacSignInfo.GetModality(ZodiacSign.Virgo));
        Assert.AreEqual(ZodiacModality.Cardinal, ZodiacSignInfo.GetModality(ZodiacSign.Libra));
        Assert.AreEqual(ZodiacModality.Fixed, ZodiacSignInfo.GetModality(ZodiacSign.Scorpio));
        Assert.AreEqual(ZodiacModality.Mutable, ZodiacSignInfo.GetModality(ZodiacSign.Sagittarius));
        Assert.AreEqual(ZodiacModality.Cardinal, ZodiacSignInfo.GetModality(ZodiacSign.Capricorn));
        Assert.AreEqual(ZodiacModality.Fixed, ZodiacSignInfo.GetModality(ZodiacSign.Aquarius));
        Assert.AreEqual(ZodiacModality.Mutable, ZodiacSignInfo.GetModality(ZodiacSign.Pisces));
    }

    [TestMethod]
    public void Zodiac_polarities_are_canonical()
    {
        foreach (var sign in Enum.GetValues<ZodiacSign>())
        {
            var expected =
                ((int)sign % 2 == 0)
                    ? ZodiacPolarity.Masculine
                    : ZodiacPolarity.Feminine;

            Assert.AreEqual(
                expected,
                ZodiacSignInfo.GetPolarity(sign));
        }
    }

    [TestMethod]
    public void All_house_axes_are_symmetric()
    {
        for (var number = 1; number <= 12; number++)
        {
            var house = AstrologicalHouse.FromNumber(number);
            var opposite = house.Opposite;

            Assert.AreEqual(
                house,
                opposite.Opposite);

            Assert.AreEqual(
                opposite,
                house.Axis.Second);
        }
    }

    [TestMethod]
    public void Both_house_systems_exist()
    {
        var systems = Enum.GetValues<HouseSystem>();

        Assert.Contains(HouseSystem.Placidus, systems);
        Assert.Contains(HouseSystem.Koch, systems);
    }

    [TestMethod]
    public void All_v1_objects_exist_and_are_categorizable()
    {
        var objects = Enum.GetValues<AstrologicalObjectId>();

        Assert.HasCount(21, objects);

        foreach (var objectId in objects)
        {
            _ = AstrologicalObjectCatalog.GetCategory(objectId);
        }

        Assert.AreEqual(
            AstrologicalObjectCategory.Node,
            AstrologicalObjectCatalog.GetCategory(
                AstrologicalObjectId.NorthTrueNode));

        Assert.AreEqual(
            AstrologicalObjectCategory.CalculatedPoint,
            AstrologicalObjectCatalog.GetCategory(
                AstrologicalObjectId.MeanLilith));
    }

    [TestMethod]
    public void Calculation_profile_v1_is_exactly_canonical()
    {
        var profile = CalculationProfile.MiastroV1;

        Assert.AreEqual("miastro-v1", profile.Id);
        Assert.AreEqual(ZodiacMode.Tropical, profile.Zodiac);
        Assert.AreEqual(ReferenceFrame.Geocentric, profile.ReferenceFrame);
        Assert.AreEqual(CoordinateType.EclipticLongitude, profile.Coordinate);
        Assert.AreEqual(ApparentPositionMode.Apparent, profile.PositionMode);
        Assert.IsTrue(profile.IncludeSpeed);
        Assert.IsFalse(profile.Topocentric);
        Assert.AreEqual(NodeConvention.TrueNode, profile.NodeConvention);
        Assert.AreEqual(LilithVariant.Mean, profile.LilithVariant);
    }

    [TestMethod]
    public void Rulership_catalog_has_exactly_twelve_signs()
    {
        Assert.HasCount(12, RulershipCatalog.All);

        foreach (var sign in Enum.GetValues<ZodiacSign>())
        {
            Assert.AreEqual(
                sign,
                RulershipCatalog.Get(sign).Sign);
        }
    }

    [TestMethod]
    public void Invalid_aspect_definition_values_are_rejected()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new AspectDefinition(
                AspectKind.Conjunction,
                -1.0,
                8.0,
                0));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new AspectDefinition(
                AspectKind.Conjunction,
                0.0,
                -1.0,
                0));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new AspectDefinition(
                AspectKind.Conjunction,
                0.0,
                8.0,
                -1));
    }

    [TestMethod]
    public void Non_finite_angles_are_rejected()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Angle.FromDegrees(double.NaN));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => EclipticLongitude.FromDegrees(
                double.PositiveInfinity));
    }
}

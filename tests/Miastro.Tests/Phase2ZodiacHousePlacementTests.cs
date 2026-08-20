using Miastro.Domain.Angles;
using Miastro.Domain.Houses;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;
using Miastro.Domain.Zodiac;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase2ZodiacHousePlacementTests
{
    [TestMethod]
    [DataRow(0.0, ZodiacSign.Aries, 0.0)]
    [DataRow(29.999, ZodiacSign.Aries, 29.999)]
    [DataRow(30.0, ZodiacSign.Taurus, 0.0)]
    [DataRow(45.0, ZodiacSign.Taurus, 15.0)]
    [DataRow(359.0, ZodiacSign.Pisces, 29.0)]
    [DataRow(359.999, ZodiacSign.Pisces, 29.999)]
    public void Zodiac_position_is_derived_correctly(
        double longitude,
        ZodiacSign expectedSign,
        double expectedDegree)
    {
        var position =
            ZodiacPosition.FromLongitude(
                EclipticLongitude.FromDegrees(longitude));

        Assert.AreEqual(expectedSign, position.Sign);
        Assert.AreEqual(
            expectedDegree,
            position.DegreeInSign,
            1e-9);
    }

    [TestMethod]
    public void Zodiac_properties_are_correct()
    {
        Assert.AreEqual(
            ZodiacElement.Fire,
            ZodiacSignInfo.GetElement(ZodiacSign.Aries));

        Assert.AreEqual(
            ZodiacModality.Cardinal,
            ZodiacSignInfo.GetModality(ZodiacSign.Aries));

        Assert.AreEqual(
            ZodiacPolarity.Masculine,
            ZodiacSignInfo.GetPolarity(ZodiacSign.Aries));

        Assert.AreEqual(
            ZodiacSign.Libra,
            ZodiacSignInfo.GetOpposite(ZodiacSign.Aries));
    }

    [TestMethod]
    public void House_range_is_enforced()
    {
        for (var number = 1; number <= 12; number++)
        {
            Assert.AreEqual(
                number,
                AstrologicalHouse.FromNumber(number).Number);
        }

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => AstrologicalHouse.FromNumber(0));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => AstrologicalHouse.FromNumber(13));
    }

    [TestMethod]
    [DataRow(1, 7)]
    [DataRow(2, 8)]
    [DataRow(3, 9)]
    [DataRow(4, 10)]
    [DataRow(5, 11)]
    [DataRow(6, 12)]
    public void House_opposite_is_correct(
        int number,
        int expected)
    {
        var house =
            AstrologicalHouse.FromNumber(number);

        Assert.AreEqual(
            expected,
            house.Opposite.Number);
    }

    [TestMethod]
    [DataRow(1.0, MotionState.Direct)]
    [DataRow(-0.01, MotionState.Retrograde)]
    [DataRow(0.0, MotionState.Stationary)]
    public void Motion_state_is_derived_without_rounding(
        double speed,
        MotionState expected)
    {
        Assert.AreEqual(
            expected,
            MotionStateResolver.FromSpeed(speed));
    }

    [TestMethod]
    public void Placement_derives_sign_degree_and_motion()
    {
        var placement =
            new AstrologicalPlacement(
                AstrologicalObjectId.Mercury,
                EclipticLongitude.FromDegrees(45.0),
                AstrologicalHouse.FromNumber(2),
                -0.5);

        Assert.AreEqual(
            ZodiacSign.Taurus,
            placement.Sign);

        Assert.AreEqual(
            15.0,
            placement.DegreeInSign,
            1e-12);

        Assert.AreEqual(
            2,
            placement.House!.Value.Number);

        Assert.AreEqual(
            MotionState.Retrograde,
            placement.Motion);

        Assert.AreEqual(
            true,
            placement.IsRetrograde);
    }
}

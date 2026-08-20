using Miastro.Domain.Objects;
using Miastro.Domain.Rulerships;
using Miastro.Domain.Zodiac;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase2RulershipTests
{
    [TestMethod]
    [DataRow(ZodiacSign.Aries, AstrologicalObjectId.Mars)]
    [DataRow(ZodiacSign.Taurus, AstrologicalObjectId.Venus)]
    [DataRow(ZodiacSign.Gemini, AstrologicalObjectId.Mercury)]
    [DataRow(ZodiacSign.Cancer, AstrologicalObjectId.Moon)]
    [DataRow(ZodiacSign.Leo, AstrologicalObjectId.Sun)]
    [DataRow(ZodiacSign.Virgo, AstrologicalObjectId.Mercury)]
    [DataRow(ZodiacSign.Libra, AstrologicalObjectId.Venus)]
    [DataRow(ZodiacSign.Sagittarius, AstrologicalObjectId.Jupiter)]
    [DataRow(ZodiacSign.Capricorn, AstrologicalObjectId.Saturn)]
    public void Traditional_rulerships_are_correct(
        ZodiacSign sign,
        AstrologicalObjectId ruler)
    {
        Assert.AreEqual(
            ruler,
            RulershipCatalog.Get(sign).Traditional);
    }

    [TestMethod]
    public void Scorpio_has_Mars_and_Pluto()
    {
        var result =
            RulershipCatalog.Get(ZodiacSign.Scorpio);

        Assert.AreEqual(
            AstrologicalObjectId.Mars,
            result.Traditional);

        Assert.AreEqual(
            AstrologicalObjectId.Pluto,
            result.Modern);
    }

    [TestMethod]
    public void Aquarius_has_Saturn_and_Uranus()
    {
        var result =
            RulershipCatalog.Get(ZodiacSign.Aquarius);

        Assert.AreEqual(
            AstrologicalObjectId.Saturn,
            result.Traditional);

        Assert.AreEqual(
            AstrologicalObjectId.Uranus,
            result.Modern);
    }

    [TestMethod]
    public void Pisces_has_Jupiter_and_Neptune()
    {
        var result =
            RulershipCatalog.Get(ZodiacSign.Pisces);

        Assert.AreEqual(
            AstrologicalObjectId.Jupiter,
            result.Traditional);

        Assert.AreEqual(
            AstrologicalObjectId.Neptune,
            result.Modern);
    }
}

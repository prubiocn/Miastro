using Miastro.Domain.Objects;
using Miastro.Domain.Zodiac;

namespace Miastro.Domain.Rulerships;

public static class RulershipCatalog
{
    private static readonly IReadOnlyDictionary<ZodiacSign, Rulership>
        Rulerships =
        new Dictionary<ZodiacSign, Rulership>
        {
            [ZodiacSign.Aries] =
                new(
                    ZodiacSign.Aries,
                    AstrologicalObjectId.Mars),

            [ZodiacSign.Taurus] =
                new(
                    ZodiacSign.Taurus,
                    AstrologicalObjectId.Venus),

            [ZodiacSign.Gemini] =
                new(
                    ZodiacSign.Gemini,
                    AstrologicalObjectId.Mercury),

            [ZodiacSign.Cancer] =
                new(
                    ZodiacSign.Cancer,
                    AstrologicalObjectId.Moon),

            [ZodiacSign.Leo] =
                new(
                    ZodiacSign.Leo,
                    AstrologicalObjectId.Sun),

            [ZodiacSign.Virgo] =
                new(
                    ZodiacSign.Virgo,
                    AstrologicalObjectId.Mercury),

            [ZodiacSign.Libra] =
                new(
                    ZodiacSign.Libra,
                    AstrologicalObjectId.Venus),

            [ZodiacSign.Scorpio] =
                new(
                    ZodiacSign.Scorpio,
                    AstrologicalObjectId.Mars,
                    AstrologicalObjectId.Pluto),

            [ZodiacSign.Sagittarius] =
                new(
                    ZodiacSign.Sagittarius,
                    AstrologicalObjectId.Jupiter),

            [ZodiacSign.Capricorn] =
                new(
                    ZodiacSign.Capricorn,
                    AstrologicalObjectId.Saturn),

            [ZodiacSign.Aquarius] =
                new(
                    ZodiacSign.Aquarius,
                    AstrologicalObjectId.Saturn,
                    AstrologicalObjectId.Uranus),

            [ZodiacSign.Pisces] =
                new(
                    ZodiacSign.Pisces,
                    AstrologicalObjectId.Jupiter,
                    AstrologicalObjectId.Neptune)
        };

    public static Rulership Get(
        ZodiacSign sign)
    {
        if (!Rulerships.TryGetValue(sign, out var rulership))
        {
            throw new ArgumentOutOfRangeException(nameof(sign));
        }

        return rulership;
    }

    public static IReadOnlyCollection<Rulership> All =>
        Rulerships.Values.ToArray();
}

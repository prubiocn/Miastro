using Miastro.Domain.Angles;
using Miastro.Domain.Zodiac;

namespace Miastro.Domain.Placements;

public readonly record struct ZodiacPosition
{
    public ZodiacSign Sign { get; }

    public double DegreeInSign { get; }

    private ZodiacPosition(
        ZodiacSign sign,
        double degreeInSign)
    {
        if (degreeInSign is < 0.0 or >= 30.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(degreeInSign));
        }

        Sign = sign;
        DegreeInSign = degreeInSign;
    }

    public static ZodiacPosition FromLongitude(
        EclipticLongitude longitude) =>
        new(
            ZodiacSignInfo.FromLongitude(longitude),
            ZodiacSignInfo.GetDegreeInSign(longitude));
}

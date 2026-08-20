using Miastro.Domain.Angles;

namespace Miastro.Domain.Zodiac;

public static class ZodiacSignInfo
{
    public static int GetIndex(ZodiacSign sign) =>
        Validate(sign);

    public static EclipticLongitude GetStart(
        ZodiacSign sign) =>
        EclipticLongitude.FromDegrees(
            Validate(sign) * 30.0);

    public static EclipticLongitude GetEndExclusive(
        ZodiacSign sign) =>
        EclipticLongitude.FromDegrees(
            (Validate(sign) + 1) * 30.0);

    public static ZodiacElement GetElement(
        ZodiacSign sign) =>
        Validate(sign) switch
        {
            0 or 4 or 8 => ZodiacElement.Fire,
            1 or 5 or 9 => ZodiacElement.Earth,
            2 or 6 or 10 => ZodiacElement.Air,
            3 or 7 or 11 => ZodiacElement.Water,
            _ => throw new InvalidOperationException()
        };

    public static ZodiacModality GetModality(
        ZodiacSign sign) =>
        Validate(sign) switch
        {
            0 or 3 or 6 or 9 => ZodiacModality.Cardinal,
            1 or 4 or 7 or 10 => ZodiacModality.Fixed,
            2 or 5 or 8 or 11 => ZodiacModality.Mutable,
            _ => throw new InvalidOperationException()
        };

    public static ZodiacPolarity GetPolarity(
        ZodiacSign sign) =>
        Validate(sign) switch
        {
            0 or 2 or 4 or 6 or 8 or 10 =>
                ZodiacPolarity.Masculine,
            _ =>
                ZodiacPolarity.Feminine
        };

    public static ZodiacSign GetOpposite(
        ZodiacSign sign) =>
        ZodiacAxis.GetOpposite(sign);

    public static ZodiacAxis GetAxis(
        ZodiacSign sign) =>
        new(sign, GetOpposite(sign));

    public static ZodiacSign FromLongitude(
        EclipticLongitude longitude) =>
        (ZodiacSign)(int)(
            longitude.Degrees / 30.0);

    public static double GetDegreeInSign(
        EclipticLongitude longitude) =>
        longitude.Degrees % 30.0;

    private static int Validate(ZodiacSign sign)
    {
        var value = (int)sign;

        if (value is < 0 or > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(sign));
        }

        return value;
    }
}

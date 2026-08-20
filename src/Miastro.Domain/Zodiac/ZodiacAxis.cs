namespace Miastro.Domain.Zodiac;

public readonly record struct ZodiacAxis
{
    public ZodiacSign First { get; }

    public ZodiacSign Second { get; }

    public ZodiacAxis(
        ZodiacSign first,
        ZodiacSign second)
    {
        if (GetOpposite(first) != second)
        {
            throw new ArgumentException(
                "Los signos no forman un eje zodiacal válido.");
        }

        First = first;
        Second = second;
    }

    public static ZodiacSign GetOpposite(ZodiacSign sign)
    {
        var value = (int)sign;

        if (value is < 0 or > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(sign));
        }

        return (ZodiacSign)((value + 6) % 12);
    }
}

namespace Miastro.Domain.Houses;

public readonly record struct AstrologicalHouse
{
    public int Number { get; }

    private AstrologicalHouse(int number)
    {
        if (number is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(
                nameof(number),
                "La casa debe estar entre 1 y 12.");
        }

        Number = number;
    }

    public static AstrologicalHouse FromNumber(
        int number) =>
        new(number);

    public AstrologicalHouse Opposite =>
        new(((Number + 5) % 12) + 1);

    public HouseAxis Axis =>
        new(this, Opposite);
}

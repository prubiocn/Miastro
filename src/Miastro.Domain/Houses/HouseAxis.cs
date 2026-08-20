namespace Miastro.Domain.Houses;

public readonly record struct HouseAxis
{
    public AstrologicalHouse First { get; }

    public AstrologicalHouse Second { get; }

    public HouseAxis(
        AstrologicalHouse first,
        AstrologicalHouse second)
    {
        if (first.Opposite != second)
        {
            throw new ArgumentException(
                "Las casas no forman un eje válido.");
        }

        First = first;
        Second = second;
    }
}

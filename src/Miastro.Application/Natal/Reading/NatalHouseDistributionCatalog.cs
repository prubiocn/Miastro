namespace Miastro.Application.Natal.Reading;

public static class NatalHouseDistributionCatalog
{
    public static NatalEastWestHemisphere EastWest(
        int houseNumber)
    {
        ValidateHouse(
            houseNumber);

        return houseNumber
            is 10 or 11 or 12 or 1 or 2 or 3
                ? NatalEastWestHemisphere.East
                : NatalEastWestHemisphere.West;
    }

    public static NatalUpperLowerHemisphere UpperLower(
        int houseNumber)
    {
        ValidateHouse(
            houseNumber);

        return houseNumber >= 7
            ? NatalUpperLowerHemisphere.Upper
            : NatalUpperLowerHemisphere.Lower;
    }

    public static NatalHouseQuadrant Quadrant(
        int houseNumber)
    {
        ValidateHouse(
            houseNumber);

        return houseNumber switch
        {
            >= 1 and <= 3 =>
                NatalHouseQuadrant.First,

            >= 4 and <= 6 =>
                NatalHouseQuadrant.Second,

            >= 7 and <= 9 =>
                NatalHouseQuadrant.Third,

            >= 10 and <= 12 =>
                NatalHouseQuadrant.Fourth,

            _ =>
                throw new InvalidOperationException()
        };
    }

    public static NatalHouseMode HouseMode(
        int houseNumber)
    {
        ValidateHouse(
            houseNumber);

        return houseNumber switch
        {
            1 or 4 or 7 or 10 =>
                NatalHouseMode.Angular,

            2 or 5 or 8 or 11 =>
                NatalHouseMode.Succedent,

            3 or 6 or 9 or 12 =>
                NatalHouseMode.Cadent,

            _ =>
                throw new InvalidOperationException()
        };
    }

    public static string EastWestLabel(
        NatalEastWestHemisphere value)
        => value switch
        {
            NatalEastWestHemisphere.East =>
                "Este",

            NatalEastWestHemisphere.West =>
                "Oeste",

            _ =>
                value.ToString()
        };

    public static string UpperLowerLabel(
        NatalUpperLowerHemisphere value)
        => value switch
        {
            NatalUpperLowerHemisphere.Upper =>
                "Superior",

            NatalUpperLowerHemisphere.Lower =>
                "Inferior",

            _ =>
                value.ToString()
        };

    public static string QuadrantLabel(
        NatalHouseQuadrant value)
        => value switch
        {
            NatalHouseQuadrant.First =>
                "I",

            NatalHouseQuadrant.Second =>
                "II",

            NatalHouseQuadrant.Third =>
                "III",

            NatalHouseQuadrant.Fourth =>
                "IV",

            _ =>
                value.ToString()
        };

    public static string HouseModeLabel(
        NatalHouseMode value)
        => value switch
        {
            NatalHouseMode.Angular =>
                "Angulares",

            NatalHouseMode.Succedent =>
                "Sucedentes",

            NatalHouseMode.Cadent =>
                "Cadentes",

            _ =>
                value.ToString()
        };

    private static void ValidateHouse(
        int houseNumber)
    {
        if (houseNumber is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(
                nameof(houseNumber),
                houseNumber,
                "La casa debe estar entre 1 y 12.");
        }
    }
}

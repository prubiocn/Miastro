namespace Miastro.Application.Natal.Reading;

public enum NatalEastWestHemisphere
{
    East = 0,
    West = 1
}

public enum NatalUpperLowerHemisphere
{
    Upper = 0,
    Lower = 1
}

public enum NatalHouseQuadrant
{
    First = 0,
    Second = 1,
    Third = 2,
    Fourth = 3
}

public enum NatalHouseMode
{
    Angular = 0,
    Succedent = 1,
    Cadent = 2
}

public sealed record NatalHouseDistributionReadModel(
    string ProfileId,
    NatalDistributionSection<NatalEastWestHemisphere> EastWest,
    NatalDistributionSection<NatalUpperLowerHemisphere> UpperLower,
    NatalDistributionSection<NatalHouseQuadrant> Quadrants,
    NatalDistributionSection<NatalHouseMode> HouseModes);

using Miastro.Domain.Objects;

namespace Miastro.Domain.Calculation;

public sealed record CalculationProfile(
    string Id,
    ZodiacMode Zodiac,
    ReferenceFrame ReferenceFrame,
    CoordinateType Coordinate,
    ApparentPositionMode PositionMode,
    bool IncludeSpeed,
    bool Topocentric,
    NodeConvention NodeConvention,
    LilithVariant LilithVariant)
{
    public static CalculationProfile MiastroV1 { get; } =
        new(
            "miastro-v1",
            ZodiacMode.Tropical,
            ReferenceFrame.Geocentric,
            CoordinateType.EclipticLongitude,
            ApparentPositionMode.Apparent,
            IncludeSpeed: true,
            Topocentric: false,
            NodeConvention.TrueNode,
            LilithVariant.Mean);
}

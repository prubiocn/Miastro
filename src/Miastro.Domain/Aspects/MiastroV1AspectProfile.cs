using Miastro.Domain.Objects;

namespace Miastro.Domain.Aspects;

public static class MiastroV1AspectProfile
{
    public static AspectProfile Instance { get; } =
        new(
            id: "miastro-v1",
            aspects:
            [
                new(
                    AspectKind.Conjunction,
                    0.0,
                    8.0,
                    0),

                new(
                    AspectKind.Semisextile,
                    30.0,
                    2.0,
                    1),

                new(
                    AspectKind.Sextile,
                    60.0,
                    4.0,
                    2),

                new(
                    AspectKind.Square,
                    90.0,
                    6.0,
                    3),

                new(
                    AspectKind.Trine,
                    120.0,
                    6.0,
                    4),

                new(
                    AspectKind.Quincunx,
                    150.0,
                    3.0,
                    5),

                new(
                    AspectKind.Opposition,
                    180.0,
                    8.0,
                    6),

                new(
                    AspectKind.Quintile,
                    72.0,
                    2.0,
                    7),

                new(
                    AspectKind.Biquintile,
                    144.0,
                    2.0,
                    8)
            ],
            participants:
            [
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.Moon,
                AstrologicalObjectId.Mercury,
                AstrologicalObjectId.Venus,
                AstrologicalObjectId.Mars,
                AstrologicalObjectId.Jupiter,
                AstrologicalObjectId.Saturn,
                AstrologicalObjectId.Uranus,
                AstrologicalObjectId.Neptune,
                AstrologicalObjectId.Pluto,
                AstrologicalObjectId.Chiron,
                AstrologicalObjectId.Ceres,
                AstrologicalObjectId.Pallas,
                AstrologicalObjectId.Juno,
                AstrologicalObjectId.Vesta,
                AstrologicalObjectId.Ascendant,
                AstrologicalObjectId.Midheaven
            ],
            luminaryOrbBonusDegrees: 1.0);
}

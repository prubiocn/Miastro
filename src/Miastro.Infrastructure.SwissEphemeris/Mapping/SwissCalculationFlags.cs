namespace Miastro.Infrastructure.SwissEphemeris.Mapping;

internal static class SwissCalculationFlags
{
    public const int SwissEphemeris = 2;
    public const int Speed = 256;

    public const int MiastroV1 =
        SwissEphemeris |
        Speed;

    public static IReadOnlyList<string> MiastroV1Names { get; } =
        [
            "SEFLG_SWIEPH",
            "SEFLG_SPEED"
        ];
}

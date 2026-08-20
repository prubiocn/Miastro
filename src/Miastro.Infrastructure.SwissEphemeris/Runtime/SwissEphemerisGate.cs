namespace Miastro.Infrastructure.SwissEphemeris.Runtime;

internal static class SwissEphemerisGate
{
    public static object SyncRoot { get; } = new();
}

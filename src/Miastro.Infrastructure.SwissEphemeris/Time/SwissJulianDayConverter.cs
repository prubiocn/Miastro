using Miastro.Astronomy.Abstractions.Models;
using Miastro.Infrastructure.SwissEphemeris.Interop;

namespace Miastro.Infrastructure.SwissEphemeris.Time;

internal static class SwissJulianDayConverter
{
    private const int GregorianCalendar = 1;

    public static double ToJulianDayUt(
        AstronomicalInstant instant,
        SwissNativeApi api)
    {
        ArgumentNullException.ThrowIfNull(api);

        var utc = instant.Utc;

        var hour =
            utc.Hour +
            utc.Minute / 60.0 +
            utc.Second / 3600.0 +
            utc.Millisecond / 3_600_000.0 +
            (utc.Ticks % TimeSpan.TicksPerMillisecond)
                / (double)TimeSpan.TicksPerHour;

        var result = api.JulianDay(
            utc.Year,
            utc.Month,
            utc.Day,
            hour,
            GregorianCalendar);

        if (!double.IsFinite(result))
        {
            throw new InvalidOperationException(
                "Swiss Ephemeris devolvió un Julian Day no finito.");
        }

        return result;
    }
}

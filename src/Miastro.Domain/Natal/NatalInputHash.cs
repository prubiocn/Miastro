using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Miastro.Domain.Natal;

public static class NatalInputHash
{
    private static readonly CultureInfo Invariant =
        CultureInfo.InvariantCulture;

    public static string Compute(
        NatalInputFingerprint input)
    {
        ArgumentNullException.ThrowIfNull(input);

        Validate(input);

        var canonical =
            string.Join(
                "\n",
                "miastro-natal-input-v3",
                input.LocalDate.ToString(
                    "yyyy-MM-dd",
                    Invariant),
                input.LocalTime.ToString(
                    "HH:mm:ss.fffffff",
                    Invariant),
                input.InstantUtc
                    .ToUniversalTime()
                    .ToString(
                        "O",
                        Invariant),
                input.Latitude.ToString(
                    "R",
                    Invariant),
                input.Longitude.ToString(
                    "R",
                    Invariant),
                input.IanaTimeZoneId.Trim(),
                input.TzdbVersion.Trim(),
                ((int)input.HouseSystem)
                    .ToString(Invariant),
                input.CalculationProfileId.Trim(),
                input.Engine.Trim(),
                input.EngineVersion.Trim(),
                input.EphemerisVersion.Trim(),
                ((int)input.TimePrecision)
                    .ToString(Invariant),
                input.GeoNameId.ToString(
                    Invariant),
                input.Locality.Trim(),
                input.HistoricalOffsetSeconds?
                    .ToString(Invariant)
                    ?? "",
                input.AmbiguousSelection?
                    .Trim()
                    ?? "",
                input.RangeStart?
                    .ToString(
                        "HH:mm:ss.fffffff",
                        Invariant)
                    ?? "",
                input.RangeEnd?
                    .ToString(
                        "HH:mm:ss.fffffff",
                        Invariant)
                    ?? "",
                input.DayPeriod?
                    .ToString()
                    ?? "",
                input.Country.Trim(),
                input.Region.Trim(),
                input.Subregion?
                    .Trim()
                    ?? "",
                ((int)input.ResolutionState)
                    .ToString(Invariant),
                input.AmbiguousEarlierOffsetSeconds?
                    .ToString(Invariant)
                    ?? "",
                input.AmbiguousEarlierInstantUtc?
                    .ToUniversalTime()
                    .ToString(
                        "O",
                        Invariant)
                    ?? "",
                input.AmbiguousLaterOffsetSeconds?
                    .ToString(Invariant)
                    ?? "",
                input.AmbiguousLaterInstantUtc?
                    .ToUniversalTime()
                    .ToString(
                        "O",
                        Invariant)
                    ?? "",
                input.ManualCoordinateOverride
                    ? "1"
                    : "0");

        var bytes =
            Encoding.UTF8.GetBytes(
                canonical);

        return Convert
            .ToHexString(
                SHA256.HashData(bytes))
            .ToLowerInvariant();
    }

    private static void Validate(
        NatalInputFingerprint input)
    {
        if (!double.IsFinite(
                input.Latitude)
            || input.Latitude < -90.0
            || input.Latitude > 90.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(input),
                "Latitud natal no válida.");
        }

        if (!double.IsFinite(
                input.Longitude)
            || input.Longitude < -180.0
            || input.Longitude > 180.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(input),
                "Longitud natal no válida.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(
            input.IanaTimeZoneId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            input.TzdbVersion);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            input.CalculationProfileId);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            input.Engine);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            input.EngineVersion);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            input.EphemerisVersion);
    }
}

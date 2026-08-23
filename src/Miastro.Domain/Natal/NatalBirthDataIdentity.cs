using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Miastro.Domain.Natal;

public static class NatalBirthDataIdentity
{
    public const int CurrentVersion = 1;

    private static readonly CultureInfo Invariant =
        CultureInfo.InvariantCulture;

    public static string Compute(
        NatalInputFingerprint input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var canonical =
            string.Join(
                "\n",
                "miastro-birth-data-v1",
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
                ((int)input.TimePrecision)
                    .ToString(Invariant),
                input.GeoNameId.ToString(
                    Invariant),
                input.Locality.Trim(),
                input.Latitude.ToString(
                    "R",
                    Invariant),
                input.Longitude.ToString(
                    "R",
                    Invariant),
                input.IanaTimeZoneId.Trim(),
                input.TzdbVersion.Trim(),
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

        return Convert
            .ToHexString(
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(
                        canonical)))
            .ToLowerInvariant();
    }
}

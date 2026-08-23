using Miastro.Domain.Natal;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6NatalInvalidationIdentityContractTests
{
    [TestMethod]
    public void Fingerprint_contains_all_phase6_invalidation_identity_fields()
    {
        var properties =
            typeof(NatalInputFingerprint)
                .GetProperties()
                .Select(x => x.Name)
                .ToHashSet(
                    StringComparer.Ordinal);

        string[] required =
        [
            "LocalDate",
            "TimePrecision",
            "LocalTime",
            "RangeStart",
            "RangeEnd",
            "DayPeriod",
            "GeoNameId",
            "Locality",
            "Country",
            "Region",
            "Subregion",
            "Latitude",
            "Longitude",
            "IanaTimeZoneId",
            "TzdbVersion",
            "ResolutionState",
            "HistoricalOffsetSeconds",
            "InstantUtc",
            "AmbiguousSelection",
            "AmbiguousEarlierOffsetSeconds",
            "AmbiguousEarlierInstantUtc",
            "AmbiguousLaterOffsetSeconds",
            "AmbiguousLaterInstantUtc",
            "ManualCoordinateOverride"
        ];

        foreach (var field in required)
        {
            Assert.IsTrue(
                properties.Contains(field),
                $"Fingerprint sin campo de invalidación: {field}");
        }
    }
}

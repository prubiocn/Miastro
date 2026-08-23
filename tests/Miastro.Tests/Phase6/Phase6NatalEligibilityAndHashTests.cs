using Miastro.Application.Natal;
using Miastro.Application.People;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.Domain.People;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6NatalEligibilityAndHashTests
{
    [TestMethod]
    public void Exact_resolved_birth_is_eligible()
    {
        var result =
            NatalCalculationEligibilityPolicy.Evaluate(
                Concrete(
                    BirthTimePrecision.Exact,
                    BirthTemporalResolutionState.Resolved,
                    resolvedInstant:
                        new DateTimeOffset(
                            2000, 1, 1,
                            11, 0, 0,
                            TimeSpan.Zero)));

        Assert.IsTrue(result.CanCalculate);
        Assert.IsFalse(result.IsApproximate);

        Assert.AreEqual(
            NatalCalculationEligibilityStatus.EligibleExact,
            result.Status);
    }

    [TestMethod]
    public void Approximate_resolved_birth_is_eligible_and_marked()
    {
        var result =
            NatalCalculationEligibilityPolicy.Evaluate(
                Concrete(
                    BirthTimePrecision.Approximate,
                    BirthTemporalResolutionState.Resolved,
                    resolvedInstant:
                        new DateTimeOffset(
                            2000, 1, 1,
                            11, 0, 0,
                            TimeSpan.Zero)));

        Assert.IsTrue(result.CanCalculate);
        Assert.IsTrue(result.IsApproximate);

        Assert.AreEqual(
            NatalCalculationEligibilityStatus.EligibleApproximate,
            result.Status);
    }

    [TestMethod]
    public void Range_does_not_create_complete_natal_chart()
    {
        var birth =
            Base(
                BirthTimePrecision.Range,
                null,
                BirthTemporalResolutionState.NotApplicable,
                null,
                null,
                rangeStart: new TimeOnly(10, 0),
                rangeEnd: new TimeOnly(12, 0));

        var result =
            NatalCalculationEligibilityPolicy.Evaluate(birth);

        Assert.IsFalse(result.CanCalculate);

        Assert.AreEqual(
            NatalCalculationEligibilityStatus
                .BirthTimeRangeRequiresResolution,
            result.Status);
    }

    [TestMethod]
    public void Day_period_is_insufficient()
    {
        var birth =
            Base(
                BirthTimePrecision.DayPeriod,
                null,
                BirthTemporalResolutionState.NotApplicable,
                null,
                null,
                dayPeriod: DayPeriod.Morning);

        var result =
            NatalCalculationEligibilityPolicy.Evaluate(birth);

        Assert.IsFalse(result.CanCalculate);

        Assert.AreEqual(
            NatalCalculationEligibilityStatus
                .BirthTimeDayPeriodInsufficient,
            result.Status);
    }

    [TestMethod]
    public void Unknown_time_is_insufficient()
    {
        var birth =
            Base(
                BirthTimePrecision.Unknown,
                null,
                BirthTemporalResolutionState.NotApplicable,
                null,
                null);

        var result =
            NatalCalculationEligibilityPolicy.Evaluate(birth);

        Assert.IsFalse(result.CanCalculate);

        Assert.AreEqual(
            NatalCalculationEligibilityStatus.BirthTimeUnknown,
            result.Status);
    }

    [TestMethod]
    public void Ambiguous_without_choice_blocks_calculation()
    {
        var birth =
            Concrete(
                BirthTimePrecision.Exact,
                BirthTemporalResolutionState.Ambiguous,
                resolvedInstant: null);

        var result =
            NatalCalculationEligibilityPolicy.Evaluate(birth);

        Assert.IsFalse(result.CanCalculate);

        Assert.AreEqual(
            NatalCalculationEligibilityStatus
                .HistoricalTimeAmbiguousUnresolved,
            result.Status);
    }

    [TestMethod]
    public void Skipped_time_blocks_calculation()
    {
        var birth =
            Concrete(
                BirthTimePrecision.Exact,
                BirthTemporalResolutionState.Skipped,
                resolvedInstant: null);

        var result =
            NatalCalculationEligibilityPolicy.Evaluate(birth);

        Assert.IsFalse(result.CanCalculate);

        Assert.AreEqual(
            NatalCalculationEligibilityStatus
                .HistoricalTimeSkipped,
            result.Status);
    }

    [TestMethod]
    public void Natal_input_hash_is_deterministic()
    {
        var input = Fingerprint(
            HouseSystem.Placidus);

        var first =
            NatalInputHash.Compute(input);

        var second =
            NatalInputHash.Compute(input);

        Assert.AreEqual(first, second);
        Assert.AreEqual(64, first.Length);
    }

    [TestMethod]
    public void Natal_input_hash_changes_with_house_system()
    {
        var placidus =
            NatalInputHash.Compute(
                Fingerprint(
                    HouseSystem.Placidus));

        var koch =
            NatalInputHash.Compute(
                Fingerprint(
                    HouseSystem.Koch));

        Assert.AreNotEqual(
            placidus,
            koch);
    }

    [TestMethod]
    public void Natal_object_order_is_stable()
    {
        Assert.AreEqual(
            21,
            NatalObjectOrder.All.Count);

        Assert.AreEqual(
            Miastro.Domain.Objects.AstrologicalObjectId.Sun,
            NatalObjectOrder.All[0]);

        Assert.AreEqual(
            Miastro.Domain.Objects.AstrologicalObjectId.Midheaven,
            NatalObjectOrder.All[^1]);
    }

    private static NatalInputFingerprint Fingerprint(
        HouseSystem houseSystem)
        => new(
            new DateOnly(2000, 1, 1),
            new TimeOnly(12, 0),
            new DateTimeOffset(
                2000, 1, 1,
                11, 0, 0,
                TimeSpan.Zero),
            40.4168,
            -3.7038,
            "Europe/Madrid",
            "TZDB: 2026c",
            houseSystem,
            "miastro-v1",
            "Swiss Ephemeris",
            "2.10.03",
            "phase3-ephemeris-1800-2399");

    private static BirthDataReadModel Concrete(
        BirthTimePrecision precision,
        BirthTemporalResolutionState state,
        DateTimeOffset? resolvedInstant)
        => Base(
            precision,
            new TimeOnly(12, 0),
            state,
            resolvedInstant,
            state is BirthTemporalResolutionState.Resolved
                or BirthTemporalResolutionState.Ambiguous
                    ? "TZDB: 2026c"
                    : state == BirthTemporalResolutionState.Skipped
                        ? "TZDB: 2026c"
                        : null);

    private static BirthDataReadModel Base(
        BirthTimePrecision precision,
        TimeOnly? localTime,
        BirthTemporalResolutionState state,
        DateTimeOffset? resolvedInstant,
        string? tzdbVersion,
        TimeOnly? rangeStart = null,
        TimeOnly? rangeEnd = null,
        DayPeriod? dayPeriod = null)
        => new(
            LocalDate: new DateOnly(2000, 1, 1),
            TimePrecision: precision,
            LocalTime: localTime,
            RangeStart: rangeStart,
            RangeEnd: rangeEnd,
            DayPeriod: dayPeriod,
            GeoNameId: 3117735,
            Locality: "Madrid",
            Country: "España",
            Region: "Madrid",
            Subregion: null,
            Latitude: 40.4168,
            Longitude: -3.7038,
            IanaTimeZoneId: "Europe/Madrid",
            TzdbVersion: tzdbVersion,
            ResolutionState: state,
            HistoricalOffsetSeconds:
                resolvedInstant is null
                    ? null
                    : 3600,
            ResolvedInstantUtc: resolvedInstant,
            AmbiguousEarlierOffsetSeconds:
                state == BirthTemporalResolutionState.Ambiguous
                    ? 7200
                    : null,
            AmbiguousEarlierInstantUtc:
                state == BirthTemporalResolutionState.Ambiguous
                    ? new DateTimeOffset(
                        2000, 10, 29,
                        0, 30, 0,
                        TimeSpan.Zero)
                    : null,
            AmbiguousLaterOffsetSeconds:
                state == BirthTemporalResolutionState.Ambiguous
                    ? 3600
                    : null,
            AmbiguousLaterInstantUtc:
                state == BirthTemporalResolutionState.Ambiguous
                    ? new DateTimeOffset(
                        2000, 10, 29,
                        1, 30, 0,
                        TimeSpan.Zero)
                    : null,
            AmbiguousSelectedCandidate: null,
            AmbiguousSelectionRecordedAtUtc: null,
            ManualCoordinateOverride: false,
            OriginalGeoNamesLatitude: null,
            OriginalGeoNamesLongitude: null);
}

using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.Domain.People;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6NatalBirthIdentityAlignmentTests
{
    [TestMethod]
    public void Manual_coordinate_override_changes_both_natal_hashes()
    {
        var baseline =
            Input();

        var changed =
            baseline with
            {
                ManualCoordinateOverride =
                    true
            };

        AssertDifferent(
            baseline,
            changed);
    }

    [TestMethod]
    public void Administrative_birth_location_identity_changes_both_hashes()
    {
        var baseline =
            Input();

        var changed =
            baseline with
            {
                Country = "Portugal",
                Region = "Lisboa",
                Subregion = "Lisboa"
            };

        AssertDifferent(
            baseline,
            changed);
    }

    [TestMethod]
    public void Historical_resolution_identity_changes_both_hashes()
    {
        var baseline =
            Input();

        var changed =
            baseline with
            {
                ResolutionState =
                    BirthTemporalResolutionState.Ambiguous,

                AmbiguousEarlierOffsetSeconds =
                    3600,

                AmbiguousEarlierInstantUtc =
                    new DateTimeOffset(
                        1990,
                        5,
                        17,
                        9,
                        30,
                        0,
                        TimeSpan.Zero),

                AmbiguousLaterOffsetSeconds =
                    7200,

                AmbiguousLaterInstantUtc =
                    new DateTimeOffset(
                        1990,
                        5,
                        17,
                        8,
                        30,
                        0,
                        TimeSpan.Zero),

                AmbiguousSelection =
                    "1"
            };

        AssertDifferent(
            baseline,
            changed);
    }

    [TestMethod]
    public void Range_and_day_period_identity_are_cryptographically_represented()
    {
        var baseline =
            Input();

        var rangeChanged =
            baseline with
            {
                RangeStart =
                    new TimeOnly(
                        9,
                        0),

                RangeEnd =
                    new TimeOnly(
                        11,
                        0)
            };

        var dayPeriodChanged =
            baseline with
            {
                DayPeriod =
                    DayPeriod.Morning
            };

        AssertDifferent(
            baseline,
            rangeChanged);

        AssertDifferent(
            baseline,
            dayPeriodChanged);
    }

    [TestMethod]
    public void House_system_changes_input_hash_but_not_birth_data_hash()
    {
        var placidus =
            Input();

        var koch =
            placidus with
            {
                HouseSystem =
                    HouseSystem.Koch
            };

        Assert.AreNotEqual(
            NatalInputHash.Compute(
                placidus),
            NatalInputHash.Compute(
                koch));

        Assert.AreEqual(
            NatalBirthDataIdentity.Compute(
                placidus),
            NatalBirthDataIdentity.Compute(
                koch));
    }

    private static void AssertDifferent(
        NatalInputFingerprint first,
        NatalInputFingerprint second)
    {
        Assert.AreNotEqual(
            NatalInputHash.Compute(
                first),
            NatalInputHash.Compute(
                second));

        Assert.AreNotEqual(
            NatalBirthDataIdentity.Compute(
                first),
            NatalBirthDataIdentity.Compute(
                second));
    }

    private static NatalInputFingerprint
        Input()
        => new(
            LocalDate:
                new DateOnly(
                    1990,
                    5,
                    17),

            LocalTime:
                new TimeOnly(
                    10,
                    30),

            InstantUtc:
                new DateTimeOffset(
                    1990,
                    5,
                    17,
                    8,
                    30,
                    0,
                    TimeSpan.Zero),

            Latitude:
                40.4168,

            Longitude:
                -3.7038,

            IanaTimeZoneId:
                "Europe/Madrid",

            TzdbVersion:
                "2026c",

            HouseSystem:
                HouseSystem.Placidus,

            CalculationProfileId:
                "miastro-v1",

            Engine:
                "Swiss Ephemeris",

            EngineVersion:
                "2.10.03",

            EphemerisVersion:
                "test",

            TimePrecision:
                BirthTimePrecision.Exact,

            GeoNameId:
                3117735,

            Locality:
                "Madrid",

            HistoricalOffsetSeconds:
                7200,

            AmbiguousSelection:
                null,

            Country:
                "España",

            Region:
                "Comunidad de Madrid",

            Subregion:
                "Madrid",

            ResolutionState:
                BirthTemporalResolutionState.Resolved,

            ManualCoordinateOverride:
                false);
}

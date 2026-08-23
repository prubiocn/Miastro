namespace Miastro.Domain.Natal;

public enum NatalCalculationEligibilityStatus
{
    EligibleExact = 1,
    EligibleApproximate = 2,

    BirthDataMissing = 10,
    BirthTimeRangeRequiresResolution = 11,
    BirthTimeDayPeriodInsufficient = 12,
    BirthTimeUnknown = 13,

    HistoricalTimePending = 20,
    HistoricalTimeAmbiguousUnresolved = 21,
    HistoricalTimeSkipped = 22,
    HistoricalInstantMissing = 23
}

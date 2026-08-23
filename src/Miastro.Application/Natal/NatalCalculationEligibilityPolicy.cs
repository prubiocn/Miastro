using Miastro.Application.People;
using Miastro.Domain.Natal;
using Miastro.Domain.People;

namespace Miastro.Application.Natal;

public static class NatalCalculationEligibilityPolicy
{
    public static NatalCalculationEligibilityResult Evaluate(
        BirthDataReadModel? birthData)
    {
        if (birthData is null)
        {
            return NatalCalculationEligibilityResult.Blocked(
                NatalCalculationEligibilityStatus.BirthDataMissing);
        }

        switch (birthData.TimePrecision)
        {
            case BirthTimePrecision.Range:
                return NatalCalculationEligibilityResult.Blocked(
                    NatalCalculationEligibilityStatus
                        .BirthTimeRangeRequiresResolution);

            case BirthTimePrecision.DayPeriod:
                return NatalCalculationEligibilityResult.Blocked(
                    NatalCalculationEligibilityStatus
                        .BirthTimeDayPeriodInsufficient);

            case BirthTimePrecision.Unknown:
                return NatalCalculationEligibilityResult.Blocked(
                    NatalCalculationEligibilityStatus
                        .BirthTimeUnknown);

            case BirthTimePrecision.Exact:
            case BirthTimePrecision.Approximate:
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(birthData.TimePrecision));
        }

        if (birthData.LocalTime is null)
        {
            return NatalCalculationEligibilityResult.Blocked(
                NatalCalculationEligibilityStatus
                    .HistoricalInstantMissing);
        }

        switch (birthData.ResolutionState)
        {
            case BirthTemporalResolutionState.Pending:
                return NatalCalculationEligibilityResult.Blocked(
                    NatalCalculationEligibilityStatus
                        .HistoricalTimePending);

            case BirthTemporalResolutionState.Skipped:
                return NatalCalculationEligibilityResult.Blocked(
                    NatalCalculationEligibilityStatus
                        .HistoricalTimeSkipped);

            case BirthTemporalResolutionState.Ambiguous:
                if (birthData.AmbiguousSelectedCandidate is null
                    || birthData.ResolvedInstantUtc is null)
                {
                    return NatalCalculationEligibilityResult.Blocked(
                        NatalCalculationEligibilityStatus
                            .HistoricalTimeAmbiguousUnresolved);
                }

                break;

            case BirthTemporalResolutionState.Resolved:
                break;

            case BirthTemporalResolutionState.NotApplicable:
                return NatalCalculationEligibilityResult.Blocked(
                    NatalCalculationEligibilityStatus
                        .HistoricalInstantMissing);

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(birthData.ResolutionState));
        }

        if (birthData.ResolvedInstantUtc is null)
        {
            return NatalCalculationEligibilityResult.Blocked(
                NatalCalculationEligibilityStatus
                    .HistoricalInstantMissing);
        }

        if (string.IsNullOrWhiteSpace(
            birthData.TzdbVersion))
        {
            return NatalCalculationEligibilityResult.Blocked(
                NatalCalculationEligibilityStatus
                    .HistoricalInstantMissing);
        }

        return birthData.TimePrecision
            == BirthTimePrecision.Approximate
                ? NatalCalculationEligibilityResult
                    .EligibleApproximate()
                : NatalCalculationEligibilityResult
                    .EligibleExact();
    }
}

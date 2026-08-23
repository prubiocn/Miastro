namespace Miastro.Domain.Natal;

public sealed record NatalCalculationEligibilityResult(
    NatalCalculationEligibilityStatus Status,
    bool CanCalculate,
    bool IsApproximate)
{
    public static NatalCalculationEligibilityResult EligibleExact()
        => new(
            NatalCalculationEligibilityStatus.EligibleExact,
            true,
            false);

    public static NatalCalculationEligibilityResult EligibleApproximate()
        => new(
            NatalCalculationEligibilityStatus.EligibleApproximate,
            true,
            true);

    public static NatalCalculationEligibilityResult Blocked(
        NatalCalculationEligibilityStatus status)
        => new(
            status,
            false,
            false);
}

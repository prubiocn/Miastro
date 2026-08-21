namespace Miastro.Domain.People;

public enum BirthTemporalResolutionState
{
    NotApplicable = 0,
    Pending = 1,
    Resolved = 2,
    Ambiguous = 3,
    Skipped = 4
}

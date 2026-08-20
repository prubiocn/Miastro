using Miastro.Domain.Geography;
using NodaTime;

namespace Miastro.Application.Time;

public sealed record HistoricalTimeResolution(
    HistoricalTimeResolutionStatus Status,
    LocalDateTime OriginalLocalDateTime,
    IanaTimeZoneId IanaTimeZoneId,
    string TzdbVersion,
    IReadOnlyList<HistoricalTimeCandidate> Candidates,
    HistoricalTimeTransition? Transition)
{
    public HistoricalTimeCandidate? SingleCandidate =>
        Candidates.Count == 1 ? Candidates[0] : null;
}

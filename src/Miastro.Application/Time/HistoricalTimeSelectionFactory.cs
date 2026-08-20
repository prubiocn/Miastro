using NodaTime;

namespace Miastro.Application.Time;

public static class HistoricalTimeSelectionFactory
{
    public static HistoricalTimeSelectionSnapshot FromResolution(
        HistoricalTimeResolution resolution)
    {
        ArgumentNullException.ThrowIfNull(resolution);

        return resolution.Status switch
        {
            HistoricalTimeResolutionStatus.Resolved =>
                new HistoricalTimeSelectionSnapshot(
                    resolution.OriginalLocalDateTime,
                    resolution.IanaTimeZoneId,
                    resolution.TzdbVersion,
                    HistoricalTimeSelectionState.Resolved,
                    resolution.SingleCandidate!.Offset,
                    resolution.SingleCandidate.Instant,
                    null),

            HistoricalTimeResolutionStatus.Ambiguous =>
                new HistoricalTimeSelectionSnapshot(
                    resolution.OriginalLocalDateTime,
                    resolution.IanaTimeZoneId,
                    resolution.TzdbVersion,
                    HistoricalTimeSelectionState.AmbiguousPendingChoice,
                    null,
                    null,
                    null),

            HistoricalTimeResolutionStatus.Skipped =>
                new HistoricalTimeSelectionSnapshot(
                    resolution.OriginalLocalDateTime,
                    resolution.IanaTimeZoneId,
                    resolution.TzdbVersion,
                    HistoricalTimeSelectionState.Skipped,
                    null,
                    null,
                    null),

            _ => throw new ArgumentOutOfRangeException(
                nameof(resolution))
        };
    }

    public static HistoricalTimeSelectionSnapshot ChooseAmbiguousCandidate(
        HistoricalTimeResolution resolution,
        int candidateIndex,
        string decision)
    {
        ArgumentNullException.ThrowIfNull(resolution);

        if (resolution.Status !=
            HistoricalTimeResolutionStatus.Ambiguous)
        {
            throw new InvalidOperationException(
                "Only ambiguous resolutions can require a choice.");
        }

        if (candidateIndex is < 0 or > 1 ||
            candidateIndex >= resolution.Candidates.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(candidateIndex));
        }

        if (string.IsNullOrWhiteSpace(decision))
        {
            throw new ArgumentException(
                "Ambiguity decision must be recorded.",
                nameof(decision));
        }

        var chosen = resolution.Candidates[candidateIndex];

        return new HistoricalTimeSelectionSnapshot(
            resolution.OriginalLocalDateTime,
            resolution.IanaTimeZoneId,
            resolution.TzdbVersion,
            HistoricalTimeSelectionState.AmbiguousChosen,
            chosen.Offset,
            chosen.Instant,
            decision.Trim());
    }
}

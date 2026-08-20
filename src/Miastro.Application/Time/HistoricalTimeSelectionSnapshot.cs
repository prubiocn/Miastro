using Miastro.Domain.Geography;
using NodaTime;

namespace Miastro.Application.Time;

public enum HistoricalTimeSelectionState
{
    Resolved,
    AmbiguousPendingChoice,
    AmbiguousChosen,
    Skipped
}

public sealed record HistoricalTimeSelectionSnapshot(
    LocalDateTime OriginalLocalDateTime,
    IanaTimeZoneId IanaTimeZoneId,
    string TzdbVersion,
    HistoricalTimeSelectionState State,
    Offset? ChosenOffset,
    Instant? ChosenInstant,
    string? AmbiguityDecision);

namespace Miastro.Domain.People;

public sealed record PersonHistoryEntry(
    PersonHistoryEventType EventType,
    DateTimeOffset OccurredAtUtc,
    string Summary);

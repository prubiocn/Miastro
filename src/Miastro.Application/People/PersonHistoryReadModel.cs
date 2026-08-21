using Miastro.Domain.People;

namespace Miastro.Application.People;

public sealed record PersonHistoryReadModel(
    PersonHistoryEventType EventType,
    DateTimeOffset OccurredAtUtc,
    string Summary);

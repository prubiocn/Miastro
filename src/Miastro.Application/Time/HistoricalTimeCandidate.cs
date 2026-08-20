using NodaTime;

namespace Miastro.Application.Time;

public sealed record HistoricalTimeCandidate(
    Instant Instant,
    Offset Offset,
    ZonedDateTime ZonedDateTime);

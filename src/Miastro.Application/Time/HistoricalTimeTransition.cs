using NodaTime;

namespace Miastro.Application.Time;

public sealed record HistoricalTimeTransition(
    Offset OffsetBefore,
    Offset OffsetAfter,
    Instant TransitionInstant);

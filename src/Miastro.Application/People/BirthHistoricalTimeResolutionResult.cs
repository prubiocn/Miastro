using Miastro.Application.Time;

namespace Miastro.Application.People;

public sealed record BirthHistoricalTimeResolutionResult(
    HistoricalTimeResolution Resolution,
    string UserMessage);

using Miastro.Application.Geography;
using Miastro.Application.Time;

namespace Miastro.Application.GeographyTime;

public sealed record ResolvedLocationTime(
    LocationSearchResult Location,
    HistoricalTimeResolution TimeResolution);

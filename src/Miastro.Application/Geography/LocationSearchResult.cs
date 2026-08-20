using Miastro.Domain.Geography;

namespace Miastro.Application.Geography;

public sealed record LocationSearchResult(
    long Id,
    string Name,
    string Region,
    string? Subregion,
    string Country,
    string CountryCode,
    Latitude Latitude,
    Longitude Longitude,
    IanaTimeZoneId IanaTimeZoneId,
    long? Population,
    string DisplayName);

namespace Miastro.Application.People;

public sealed record CurrentResidenceReadModel(
    string Locality,
    long? GeoNameId,
    string Region,
    string Country,
    double Latitude,
    double Longitude,
    string IanaTimeZoneId,
    DateTimeOffset? UpdatedAtUtc);

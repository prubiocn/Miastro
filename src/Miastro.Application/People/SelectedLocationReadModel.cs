namespace Miastro.Application.People;

public sealed record SelectedLocationReadModel(
    long GeoNameId,
    string Locality,
    string Region,
    string? Subregion,
    string Country,
    string CountryCode,
    double Latitude,
    double Longitude,
    string IanaTimeZoneId,
    string DisplayName);

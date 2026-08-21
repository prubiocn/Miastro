using Miastro.Application.Geography;

namespace Miastro.UI.Avalonia.ViewModels;

public sealed record LocationResultViewModel(
    long GeoNameId,
    string Name,
    string Region,
    string? Subregion,
    string Country,
    double Latitude,
    double Longitude,
    string IanaTimeZoneId,
    string DisplayName)
{
    public static LocationResultViewModel From(
        LocationSearchResult value)
        => new(
            value.Id,
            value.Name,
            value.Region,
            value.Subregion,
            value.Country,
            value.Latitude.Value,
            value.Longitude.Value,
            value.IanaTimeZoneId.Value,
            value.DisplayName);
}

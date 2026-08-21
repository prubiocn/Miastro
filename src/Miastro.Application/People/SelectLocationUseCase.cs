using Miastro.Application.Geography;

namespace Miastro.Application.People;

public sealed class SelectLocationUseCase(
    ILocationSearchService locationSearchService)
{
    public async Task<SelectedLocationReadModel> ExecuteAsync(
        long geoNameId,
        CancellationToken cancellationToken = default)
    {
        if (geoNameId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(geoNameId));
        }

        var location =
            await locationSearchService.GetByGeoNameIdAsync(
                geoNameId,
                cancellationToken)
            ?? throw new KeyNotFoundException(
                "Selected location was not found.");

        return new SelectedLocationReadModel(
            location.Id,
            location.Name,
            location.Region,
            location.Subregion,
            location.Country,
            location.CountryCode,
            location.Latitude.Value,
            location.Longitude.Value,
            location.IanaTimeZoneId.Value,
            location.DisplayName);
    }
}

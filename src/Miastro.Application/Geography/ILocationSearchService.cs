namespace Miastro.Application.Geography;

public interface ILocationSearchService
{
    Task<IReadOnlyList<LocationSearchResult>> SearchAsync(
        LocationSearchQuery query,
        CancellationToken cancellationToken = default);

    Task<LocationSearchResult?> GetByGeoNameIdAsync(
        long geoNameId,
        CancellationToken cancellationToken = default);
}

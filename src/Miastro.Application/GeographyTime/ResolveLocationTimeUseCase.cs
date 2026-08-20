using Miastro.Application.Geography;
using Miastro.Application.Time;
using NodaTime;

namespace Miastro.Application.GeographyTime;

public sealed class ResolveLocationTimeUseCase
{
    private readonly ILocationSearchService _locations;
    private readonly IHistoricalTimeResolver _time;

    public ResolveLocationTimeUseCase(
        ILocationSearchService locations,
        IHistoricalTimeResolver time)
    {
        _locations = locations;
        _time = time;
    }

    public async Task<ResolvedLocationTime> ExecuteAsync(
        long geoNameId,
        LocalDateTime localDateTime,
        CancellationToken cancellationToken = default)
    {
        var location =
            await _locations.GetByGeoNameIdAsync(
                geoNameId,
                cancellationToken);

        if (location is null)
        {
            throw new GeographyException(
                GeographyErrorCode.LocationNotFound,
                $"GeoNameId not found: {geoNameId}");
        }

        var resolution = _time.Resolve(
            localDateTime,
            location.IanaTimeZoneId);

        return new ResolvedLocationTime(
            location,
            resolution);
    }
}

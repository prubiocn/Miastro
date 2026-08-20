using Miastro.Application.Geography;
using Miastro.Application.GeographyTime;
using Miastro.Application.Time;
using Miastro.Infrastructure.Geography.Catalog;
using Miastro.Infrastructure.Time.Historical;
using NodaTime;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase4GeographyTimeE2ETests
{
    private static ResolveLocationTimeUseCase CreateUseCase()
    {
        var catalog = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "../../../../../data/geography/release/geonames.sqlite"));

        return new ResolveLocationTimeUseCase(
            new SqliteLocationSearchService(
                new GeoNamesCatalogOptions(catalog)),
            new NodaTimeHistoricalTimeResolver());
    }

    [TestMethod]
    public async Task Malaga_LocalDateTime_ToCoordinatesZoneAndInstant()
    {
        var catalog = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "../../../../../data/geography/release/geonames.sqlite"));

        var locations =
            new SqliteLocationSearchService(
                new GeoNamesCatalogOptions(catalog));

        var search = await locations.SearchAsync(
            new LocationSearchQuery(
                "Málaga",
                20,
                "ES"));

        var malaga = search.First(x => x.Name == "Málaga");

        var e2e = await CreateUseCase().ExecuteAsync(
            malaga.Id,
            new LocalDateTime(2024, 1, 15, 12, 0));

        Assert.AreEqual("ES", e2e.Location.CountryCode);
        Assert.AreEqual(
            "Europe/Madrid",
            e2e.Location.IanaTimeZoneId.Value);

        Assert.AreEqual(
            HistoricalTimeResolutionStatus.Resolved,
            e2e.TimeResolution.Status);

        Assert.AreEqual(
            Instant.FromUtc(2024, 1, 15, 11, 0),
            e2e.TimeResolution.SingleCandidate!.Instant);
    }

    [TestMethod]
    public async Task Pamplona_HomonymSelection_ChangesCountryAndZone()
    {
        var catalog = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "../../../../../data/geography/release/geonames.sqlite"));

        var locations =
            new SqliteLocationSearchService(
                new GeoNamesCatalogOptions(catalog));

        var results = await locations.SearchAsync(
            new LocationSearchQuery("Pamplona", 50));

        var spain = results.First(x => x.CountryCode == "ES");
        var colombia = results.First(x => x.CountryCode == "CO");

        Assert.AreNotEqual(spain.Id, colombia.Id);

        var es = await CreateUseCase().ExecuteAsync(
            spain.Id,
            new LocalDateTime(2024, 1, 15, 12, 0));

        var co = await CreateUseCase().ExecuteAsync(
            colombia.Id,
            new LocalDateTime(2024, 1, 15, 12, 0));

        Assert.AreEqual(
            "Europe/Madrid",
            es.Location.IanaTimeZoneId.Value);

        Assert.AreEqual(
            "America/Bogota",
            co.Location.IanaTimeZoneId.Value);

        Assert.AreNotEqual(
            es.TimeResolution.SingleCandidate!.Instant,
            co.TimeResolution.SingleCandidate!.Instant);
    }
}

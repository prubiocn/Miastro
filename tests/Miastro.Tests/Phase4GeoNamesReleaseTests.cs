using Miastro.Application.Geography;
using Miastro.Infrastructure.Geography.Catalog;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase4GeoNamesReleaseTests
{
    private static string CatalogPath =>
        Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "../../../../../data/geography/release/geonames.sqlite"));

    private static SqliteLocationSearchService CreateService() =>
        new(new GeoNamesCatalogOptions(CatalogPath));

    [TestMethod]
    public async Task Pamplona_OfficialCatalog_PreservesHomonyms()
    {
        var results = await CreateService().SearchAsync(
            new LocationSearchQuery("Pamplona", 50));

        Assert.IsTrue(results.Count >= 2);
        Assert.IsTrue(results.Any(x => x.CountryCode == "ES"));
        Assert.IsTrue(results.Any(x => x.CountryCode == "CO"));
    }

    [TestMethod]
    public async Task Malaga_OfficialCatalog_IsAccentInsensitive()
    {
        var results = await CreateService().SearchAsync(
            new LocationSearchQuery("Malaga", 30, "ES"));

        Assert.IsTrue(
            results.Any(x => x.Name == "Málaga"));
    }

    [TestMethod]
    public async Task ACoruna_OfficialCatalog_UsesAlternateName()
    {
        var results = await CreateService().SearchAsync(
            new LocationSearchQuery("La Coruna", 30, "ES"));

        Assert.IsTrue(
            results.Any(x => x.Name == "A Coruña"));
    }

    [TestMethod]
    public async Task SearchRanking_IsReproducible()
    {
        var first = await CreateService().SearchAsync(
            new LocationSearchQuery("Pam", 50));

        var second = await CreateService().SearchAsync(
            new LocationSearchQuery("Pam", 50));

        CollectionAssert.AreEqual(
            first.Select(x => x.Id).ToArray(),
            second.Select(x => x.Id).ToArray());
    }

    [TestMethod]
    public async Task GeoNameId_RoundTrip_PreservesCoordinatesAndZone()
    {
        var results = await CreateService().SearchAsync(
            new LocationSearchQuery("Málaga", 30, "ES"));

        var source = results.First(x => x.Name == "Málaga");

        var exact = await CreateService()
            .GetByGeoNameIdAsync(source.Id);

        Assert.IsNotNull(exact);
        Assert.AreEqual(source.Id, exact.Id);
        Assert.AreEqual(source.Latitude, exact.Latitude);
        Assert.AreEqual(source.Longitude, exact.Longitude);
        Assert.AreEqual(
            source.IanaTimeZoneId,
            exact.IanaTimeZoneId);
    }
}

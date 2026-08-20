using Miastro.Application.Geography;
using Miastro.Infrastructure.Geography.Catalog;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase4GeographySearchTests
{
    private static string CatalogPath =>
        Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "../../../../../data/geography/generated/geonames.sqlite"));

    private static SqliteLocationSearchService CreateService() =>
        new(new GeoNamesCatalogOptions(CatalogPath));

    [TestMethod]
    public async Task Pamplona_ReturnsDistinctHomonyms()
    {
        var results = await CreateService().SearchAsync(
            new LocationSearchQuery("Pamplona", 20));

        Assert.IsTrue(results.Count >= 2);
        Assert.IsTrue(results.Any(x => x.CountryCode == "ES"));
        Assert.IsTrue(results.Any(x => x.CountryCode == "CO"));

        var names = results.Select(x => x.DisplayName).ToArray();
        Assert.IsTrue(names.Any(x => x.Contains("Navarra")));
        Assert.IsTrue(names.Any(x => x.Contains("Norte de Santander")));
    }

    [TestMethod]
    public async Task Malaga_WithoutAccent_FindsMalaga()
    {
        var results = await CreateService().SearchAsync(
            new LocationSearchQuery("Malaga", 10, "ES"));

        Assert.AreEqual("Málaga", results[0].Name);
        Assert.AreEqual("Europe/Madrid", results[0].IanaTimeZoneId.Value);
    }

    [TestMethod]
    public async Task AlternateName_FindsACoruna()
    {
        var results = await CreateService().SearchAsync(
            new LocationSearchQuery("La Coruna", 10, "ES"));

        Assert.AreEqual("A Coruña", results[0].Name);
    }

    [TestMethod]
    public async Task Prefix_IsDeterministic()
    {
        var first = await CreateService().SearchAsync(
            new LocationSearchQuery("Pam", 20));
        var second = await CreateService().SearchAsync(
            new LocationSearchQuery("Pam", 20));

        CollectionAssert.AreEqual(
            first.Select(x => x.Id).ToArray(),
            second.Select(x => x.Id).ToArray());
    }

    [TestMethod]
    public async Task CountryFilter_IsApplied()
    {
        var results = await CreateService().SearchAsync(
            new LocationSearchQuery("Pamplona", 20, "CO"));

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual("CO", results[0].CountryCode);
    }

    [TestMethod]
    public async Task MissingCatalog_IsTypedError()
    {
        var service = new SqliteLocationSearchService(
            new GeoNamesCatalogOptions(
                Path.Combine(
                    Path.GetTempPath(),
                    Guid.NewGuid() + ".sqlite")));

        GeographyException? captured = null;

        try
        {
            await service.SearchAsync(
                new LocationSearchQuery("Madrid"));
        }
        catch (GeographyException ex)
        {
            captured = ex;
        }

        Assert.IsNotNull(captured);

        Assert.AreEqual(
            GeographyErrorCode.CatalogMissing,
            captured.Code);
    }
}

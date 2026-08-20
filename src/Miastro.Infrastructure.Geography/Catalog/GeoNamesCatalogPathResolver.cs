namespace Miastro.Infrastructure.Geography.Catalog;

public static class GeoNamesCatalogPathResolver
{
    public static string FromApplicationBaseDirectory(
        string? baseDirectory = null)
    {
        baseDirectory ??= AppContext.BaseDirectory;

        return Path.Combine(
            baseDirectory,
            "geodata",
            "geonames.sqlite");
    }

    public static string DistributionPath =>
        "/usr/share/miastro/geodata/geonames.sqlite";
}

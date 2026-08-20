namespace Miastro.Tests;

internal static class Phase4GeoCatalogTestPaths
{
    public static string Resolve()
    {
        var configured =
            Environment.GetEnvironmentVariable("MIASTRO_GEODATA_DIR");

        if (!string.IsNullOrWhiteSpace(configured))
        {
            var configuredPath = Path.GetFullPath(
                Path.Combine(configured, "geonames.sqlite"));

            if (File.Exists(configuredPath))
            {
                return configuredPath;
            }

            throw new FileNotFoundException(
                "MIASTRO_GEODATA_DIR does not contain geonames.sqlite.",
                configuredPath);
        }

        var root = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../.."));

        var release = Path.Combine(
            root,
            "data/geography/release/geonames.sqlite");

        if (File.Exists(release))
        {
            return release;
        }

        var generated = Path.Combine(
            root,
            "data/geography/generated/geonames.sqlite");

        if (File.Exists(generated))
        {
            return generated;
        }

        throw new FileNotFoundException(
            "No Phase 4 GeoNames test catalog was found.");
    }
}

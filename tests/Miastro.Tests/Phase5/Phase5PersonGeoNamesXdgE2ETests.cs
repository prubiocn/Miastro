using Microsoft.Extensions.DependencyInjection;
using Miastro.Application.People;
using Miastro.Bootstrap;
using Miastro.Domain.People;

namespace Miastro.Tests.Phase5;

[TestClass]
public sealed class Phase5PersonGeoNamesXdgE2ETests
{
    [TestMethod]
    public async Task Person_with_real_geonames_birth_survives_application_reopen()
    {
        var catalog =
            global::Miastro.Tests
                .Phase4GeoCatalogTestPaths
                .Resolve();

        var persistentRoot =
            Environment.GetEnvironmentVariable(
                "MIASTRO_PHASE5_PERSIST_XDG_ROOT");

        var preserveRoot =
            !string.IsNullOrWhiteSpace(
                persistentRoot);

        var root =
            preserveRoot
                ? Path.GetFullPath(
                    persistentRoot!)
                : Path.Combine(
                    Path.GetTempPath(),
                    "miastro-phase5-xdg-"
                    + Guid.NewGuid().ToString("N"));

        var previous =
            CaptureEnvironment();

        try
        {
            ConfigureEnvironment(
                root,
                Path.GetDirectoryName(catalog)!);

            Guid personId;

            await using (
                var provider =
                    BuildProvider())
            {
                await MiastroBootstrap
                    .InitializeAsync(provider);

                using var scope =
                    provider.CreateScope();

                var search =
                    scope.ServiceProvider
                        .GetRequiredService<
                            ResolveBirthLocationUseCase>();

                var locations =
                    await search.ExecuteAsync(
                        "Malaga",
                        30);

                var malaga =
                    locations.First(
                        x =>
                            x.LocalityEquivalent("Málaga")
                            && x.CountryCodeEquivalent("ES"));

                var select =
                    scope.ServiceProvider
                        .GetRequiredService<
                            SelectLocationUseCase>();

                var location =
                    await select.ExecuteAsync(
                        malaga.Id);

                Assert.AreEqual(
                    "Europe/Madrid",
                    location.IanaTimeZoneId);

                var temporal =
                    scope.ServiceProvider
                        .GetRequiredService<
                            ResolveBirthHistoricalTimeUseCase>();

                var resolution =
                    temporal.Execute(
                        new DateOnly(
                            2000,
                            1,
                            15),
                        new TimeOnly(
                            12,
                            0),
                        location.IanaTimeZoneId);

                var pendingBirth =
                    new BirthDataWriteModel(
                        LocalDate:
                            new DateOnly(
                                2000,
                                1,
                                15),
                        TimePrecision:
                            BirthTimePrecision.Exact,
                        LocalTime:
                            new TimeOnly(
                                12,
                                0),
                        RangeStart: null,
                        RangeEnd: null,
                        DayPeriod: null,
                        GeoNameId:
                            location.GeoNameId,
                        Locality:
                            location.Locality,
                        Country:
                            location.Country,
                        Region:
                            location.Region,
                        Subregion:
                            location.Subregion,
                        Latitude:
                            location.Latitude,
                        Longitude:
                            location.Longitude,
                        IanaTimeZoneId:
                            location.IanaTimeZoneId,
                        TzdbVersion: null,
                        ResolutionState:
                            BirthTemporalResolutionState.Pending,
                        HistoricalOffsetSeconds: null,
                        ResolvedInstantUtc: null,
                        AmbiguousEarlierOffsetSeconds: null,
                        AmbiguousEarlierInstantUtc: null,
                        AmbiguousLaterOffsetSeconds: null,
                        AmbiguousLaterInstantUtc: null,
                        AmbiguousSelectedCandidate: null,
                        AmbiguousSelectionRecordedAtUtc: null,
                        ManualCoordinateOverride: false,
                        OriginalGeoNamesLatitude: null,
                        OriginalGeoNamesLongitude: null);

                var birth =
                    BirthHistoricalTimeSnapshotMapper
                        .Apply(
                            pendingBirth,
                            resolution.Resolution);

                var create =
                    scope.ServiceProvider
                        .GetRequiredService<
                            CreatePersonUseCase>();

                personId =
                    await create.ExecuteAsync(
                        new CreatePersonCommand(
                            "Persona",
                            "Persistente",
                            null,
                            "persona@example.invalid",
                            "nota sintética",
                            true,
                            birth,
                            new CurrentResidenceWriteModel(
                                location.Locality,
                                location.GeoNameId,
                                location.Region,
                                location.Country,
                                location.Latitude,
                                location.Longitude,
                                location.IanaTimeZoneId,
                                DateTimeOffset.UtcNow)),
                        new DateTimeOffset(
                            2026,
                            8,
                            21,
                            9,
                            0,
                            0,
                            TimeSpan.Zero));
            }

            var database =
                Path.Combine(
                    root,
                    "data",
                    "miastro",
                    "miastro.db");

            Assert.IsTrue(
                File.Exists(database),
                "XDG database was not created.");

            await using (
                var reopened =
                    BuildProvider())
            {
                await MiastroBootstrap
                    .InitializeAsync(reopened);

                using var scope =
                    reopened.CreateScope();

                var get =
                    scope.ServiceProvider
                        .GetRequiredService<
                            GetPersonUseCase>();

                var person =
                    await get.ExecuteAsync(
                        personId);

                Assert.IsNotNull(person);

                Assert.AreEqual(
                    "Persona",
                    person.FirstName);

                Assert.AreEqual(
                    "Persistente",
                    person.LastName);

                Assert.IsTrue(
                    person.IsFavorite);

                Assert.IsNotNull(
                    person.BirthData);

                Assert.AreEqual(
                    "Málaga",
                    person.BirthData.Locality);

                Assert.AreEqual(
                    "Europe/Madrid",
                    person.BirthData.IanaTimeZoneId);

                Assert.AreEqual(
                    BirthTemporalResolutionState.Resolved,
                    person.BirthData.ResolutionState);

                Assert.IsNotNull(
                    person.BirthData.ResolvedInstantUtc);

                Assert.IsNotNull(
                    person.CurrentResidence);

                Assert.AreEqual(
                    person.BirthData.GeoNameId,
                    person.CurrentResidence.GeoNameId);

                Assert.IsTrue(
                    person.History.Count >= 1);
            }
        }
        finally
        {
            RestoreEnvironment(previous);

            if (!preserveRoot
                && Directory.Exists(root))
            {
                Directory.Delete(
                    root,
                    recursive: true);
            }
        }
    }

    [TestMethod]
    public void Bootstrap_uses_explicit_geography_registration()
    {
        var root =
            FindRepositoryRoot();

        var bootstrap =
            File.ReadAllText(
                Path.Combine(
                    root,
                    "src",
                    "Miastro.Bootstrap",
                    "MiastroBootstrap.cs"));

        StringAssert.Contains(
            bootstrap,
            "GeoNamesCatalogOptions");

        StringAssert.Contains(
            bootstrap,
            "SqliteLocationSearchService");

        Assert.IsFalse(
            File.Exists(
                Path.Combine(
                    root,
                    "src",
                    "Miastro.Bootstrap",
                    "Phase5InfrastructureServiceFactory.cs")));
    }

    private static ServiceProvider BuildProvider()
        => MiastroBootstrap
            .CreateServiceCollection()
            .BuildServiceProvider(
                new ServiceProviderOptions
                {
                    ValidateOnBuild = true,
                    ValidateScopes = true
                });

    private static Dictionary<string, string?>
        CaptureEnvironment()
    {
        var result =
            new Dictionary<string, string?>();

        foreach (var name in Variables)
        {
            result[name] =
                Environment
                    .GetEnvironmentVariable(name);
        }

        return result;
    }

    private static void ConfigureEnvironment(
        string root,
        string geoDataDirectory)
    {
        Environment.SetEnvironmentVariable(
            "XDG_DATA_HOME",
            Path.Combine(root, "data"));

        Environment.SetEnvironmentVariable(
            "XDG_CONFIG_HOME",
            Path.Combine(root, "config"));

        Environment.SetEnvironmentVariable(
            "XDG_CACHE_HOME",
            Path.Combine(root, "cache"));

        Environment.SetEnvironmentVariable(
            "XDG_STATE_HOME",
            Path.Combine(root, "state"));

        Environment.SetEnvironmentVariable(
            "XDG_RUNTIME_DIR",
            Path.Combine(root, "runtime"));

        Environment.SetEnvironmentVariable(
            "MIASTRO_GEODATA_DIR",
            geoDataDirectory);
    }

    private static void RestoreEnvironment(
        IReadOnlyDictionary<string, string?> values)
    {
        foreach (var item in values)
        {
            Environment.SetEnvironmentVariable(
                item.Key,
                item.Value);
        }
    }

    private static string FindRepositoryRoot()
    {
        var directory =
            new DirectoryInfo(
                AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(
                Path.Combine(
                    directory.FullName,
                    "Miastro.sln")))
            {
                return directory.FullName;
            }

            directory =
                directory.Parent;
        }

        throw new DirectoryNotFoundException();
    }

    private static readonly string[] Variables =
    [
        "XDG_DATA_HOME",
        "XDG_CONFIG_HOME",
        "XDG_CACHE_HOME",
        "XDG_STATE_HOME",
        "XDG_RUNTIME_DIR",
        "MIASTRO_GEODATA_DIR"
    ];
}

internal static class Phase5LocationResultTestExtensions
{
    public static bool LocalityEquivalent(
        this Miastro.Application.Geography.LocationSearchResult value,
        string expected)
        => string.Equals(
            value.Name,
            expected,
            StringComparison.Ordinal);

    public static bool CountryCodeEquivalent(
        this Miastro.Application.Geography.LocationSearchResult value,
        string expected)
        => string.Equals(
            value.CountryCode,
            expected,
            StringComparison.Ordinal);
}

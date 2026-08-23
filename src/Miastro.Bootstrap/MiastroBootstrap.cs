using Miastro.Infrastructure.Geography.Catalog;
using Miastro.Application.Geography;
using Miastro.Application.Time;
using Miastro.Infrastructure.Time.Historical;
using Miastro.Application.People;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Miastro.Application.Configuration;
using Miastro.Application.Platform;
using Miastro.Infrastructure.Platform.Linux.Logging;
using Miastro.Infrastructure.Platform.Linux.Xdg;
using Miastro.Infrastructure.Persistence;
using Miastro.Application.Natal;

namespace Miastro.Bootstrap;

public static class MiastroBootstrap
{
    public static IServiceCollection CreateServiceCollection()
    {
        var services = new ServiceCollection();

        var paths = new XdgApplicationPaths();
        var settings = LoadOrCreateSettings(paths);

        services.AddSingleton<IApplicationPaths>(paths);
        services.AddSingleton(settings);

        services.AddMiastroPersistence();
        services.AddPhase6Astronomy();

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddProvider(
                new XdgFileLoggerProvider(paths.LogFilePath));
        });


        services.AddScoped<CreatePersonUseCase>();
        services.AddScoped<UpdatePersonUseCase>();
        services.AddScoped<GetPersonUseCase>();
        services.AddScoped<SearchPeopleUseCase>();
        services.AddScoped<DeletePersonUseCase>();
        services.AddScoped<SetFavoriteUseCase>();
        services.AddScoped<RecordPersonConsultationUseCase>();
        services.AddScoped<PersistNatalChartSnapshotUseCase>();
        services.AddScoped<CalculateNatalChartUseCase>();
        services.AddScoped<RecalculateNatalChartUseCase>();
        services.AddScoped<UpdateResidenceUseCase>();


                services.AddSingleton(
            new GeoNamesCatalogOptions(
                ResolveGeoNamesCatalogPath()));

        services.AddScoped<
            ILocationSearchService,
            SqliteLocationSearchService>();

        services.AddSingleton<
            IHistoricalTimeResolver,
            NodaTimeHistoricalTimeResolver>();
        services.AddScoped<SelectLocationUseCase>();
services.AddScoped<ResolveBirthLocationUseCase>();
        services.AddScoped<ResolveCurrentResidenceLocationUseCase>();
        services.AddScoped<ResolveBirthHistoricalTimeUseCase>();

        return services;
    }

    private static string ResolveGeoNamesCatalogPath()
    {
        var configured =
            Environment.GetEnvironmentVariable(
                "MIASTRO_GEODATA_DIR");

        if (!string.IsNullOrWhiteSpace(configured))
        {
            return Path.GetFullPath(
                Path.Combine(
                    configured,
                    "geonames.sqlite"));
        }

        var published =
            Path.Combine(
                AppContext.BaseDirectory,
                "geodata",
                "geonames.sqlite");

        if (File.Exists(published))
        {
            return Path.GetFullPath(published);
        }

        return
            "/usr/share/miastro/geodata/geonames.sqlite";
    }

    private static ApplicationSettings LoadOrCreateSettings(
        IApplicationPaths paths)
    {
        if (!File.Exists(paths.SettingsPath))
        {
            var defaults = new ApplicationSettings();

            var json = JsonSerializer.Serialize(
                defaults,
                JsonOptions);

            File.WriteAllText(paths.SettingsPath, json);

            RestrictFile(paths.SettingsPath);

            return defaults;
        }

        try
        {
            var json = File.ReadAllText(paths.SettingsPath);

            return JsonSerializer.Deserialize<ApplicationSettings>(
                       json,
                       JsonOptions)
                   ?? new ApplicationSettings();
        }
        catch (JsonException)
        {
            throw new InvalidOperationException(
                "El archivo de configuración de Miastro no es válido.");
        }
    }

    private static void RestrictFile(string path)
    {
        if (OperatingSystem.IsLinux())
        {
            File.SetUnixFileMode(
                path,
                UnixFileMode.UserRead |
                UnixFileMode.UserWrite);
        }
    }

    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

    public static async Task InitializeAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        var initializer = services.GetRequiredService<DatabaseInitializer>();
        await initializer.InitializeAsync(cancellationToken);
    }

}

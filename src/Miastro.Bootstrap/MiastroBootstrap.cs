using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Miastro.Application.Configuration;
using Miastro.Application.Platform;
using Miastro.Infrastructure.Platform.Linux.Logging;
using Miastro.Infrastructure.Platform.Linux.Xdg;
using Miastro.Infrastructure.Persistence;

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

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddProvider(
                new XdgFileLoggerProvider(paths.LogFilePath));
        });

        return services;
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

#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
cd "$ROOT"

# Dependencias necesarias
dotnet add src/Miastro.Infrastructure.Platform.Linux/Miastro.Infrastructure.Platform.Linux.csproj \
  package Microsoft.Extensions.Logging

dotnet add src/Miastro.UI.Avalonia/Miastro.UI.Avalonia.csproj \
  package Microsoft.Extensions.DependencyInjection

# ------------------------------------------------------------
# APPLICATION — contratos técnicos
# ------------------------------------------------------------

mkdir -p src/Miastro.Application/Platform
mkdir -p src/Miastro.Application/Configuration

cat > src/Miastro.Application/Platform/IApplicationPaths.cs <<'EOF'
namespace Miastro.Application.Platform;

public interface IApplicationPaths
{
    string DataDirectory { get; }
    string ConfigDirectory { get; }
    string CacheDirectory { get; }
    string StateDirectory { get; }
    string RuntimeDirectory { get; }

    string DatabasePath { get; }
    string SettingsPath { get; }
    string LogDirectory { get; }
    string LogFilePath { get; }
}
EOF

cat > src/Miastro.Application/Configuration/ApplicationSettings.cs <<'EOF'
namespace Miastro.Application.Configuration;

public sealed record ApplicationSettings
{
    public int SchemaVersion { get; init; } = 1;
    public string Language { get; init; } = "es-ES";
}
EOF

# ------------------------------------------------------------
# PLATFORM LINUX — XDG
# ------------------------------------------------------------

mkdir -p src/Miastro.Infrastructure.Platform.Linux/Xdg
mkdir -p src/Miastro.Infrastructure.Platform.Linux/Logging

cat > src/Miastro.Infrastructure.Platform.Linux/Xdg/XdgApplicationPaths.cs <<'EOF'
using Miastro.Application.Platform;

namespace Miastro.Infrastructure.Platform.Linux.Xdg;

public sealed class XdgApplicationPaths : IApplicationPaths
{
    private const string ApplicationDirectoryName = "miastro";

    public XdgApplicationPaths()
    {
        var home = Environment.GetFolderPath(
            Environment.SpecialFolder.UserProfile,
            Environment.SpecialFolderOption.DoNotVerify);

        if (string.IsNullOrWhiteSpace(home))
        {
            throw new InvalidOperationException(
                "No se pudo determinar el directorio personal del usuario.");
        }

        DataDirectory = Resolve(
            "XDG_DATA_HOME",
            Path.Combine(home, ".local", "share"),
            ApplicationDirectoryName);

        ConfigDirectory = Resolve(
            "XDG_CONFIG_HOME",
            Path.Combine(home, ".config"),
            ApplicationDirectoryName);

        CacheDirectory = Resolve(
            "XDG_CACHE_HOME",
            Path.Combine(home, ".cache"),
            ApplicationDirectoryName);

        StateDirectory = Resolve(
            "XDG_STATE_HOME",
            Path.Combine(home, ".local", "state"),
            ApplicationDirectoryName);

        RuntimeDirectory = ResolveRuntimeDirectory();

        DatabasePath = Path.Combine(DataDirectory, "miastro.db");
        SettingsPath = Path.Combine(ConfigDirectory, "settings.json");
        LogDirectory = Path.Combine(StateDirectory, "logs");
        LogFilePath = Path.Combine(LogDirectory, "miastro.log");

        EnsureDirectories();
    }

    public string DataDirectory { get; }

    public string ConfigDirectory { get; }

    public string CacheDirectory { get; }

    public string StateDirectory { get; }

    public string RuntimeDirectory { get; }

    public string DatabasePath { get; }

    public string SettingsPath { get; }

    public string LogDirectory { get; }

    public string LogFilePath { get; }

    private static string Resolve(
        string environmentVariable,
        string fallbackRoot,
        string applicationDirectory)
    {
        var configured = Environment.GetEnvironmentVariable(environmentVariable);

        var root = !string.IsNullOrWhiteSpace(configured) &&
                   Path.IsPathRooted(configured)
            ? configured
            : fallbackRoot;

        return Path.Combine(root, applicationDirectory);
    }

    private static string ResolveRuntimeDirectory()
    {
        var configured = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");

        if (!string.IsNullOrWhiteSpace(configured) &&
            Path.IsPathRooted(configured))
        {
            return Path.Combine(configured, ApplicationDirectoryName);
        }

        return Path.Combine(
            Path.GetTempPath(),
            $"miastro-{Environment.UserName}");
    }

    private void EnsureDirectories()
    {
        EnsurePrivateDirectory(DataDirectory);
        EnsurePrivateDirectory(ConfigDirectory);
        EnsurePrivateDirectory(CacheDirectory);
        EnsurePrivateDirectory(StateDirectory);
        EnsurePrivateDirectory(RuntimeDirectory);
        EnsurePrivateDirectory(LogDirectory);
    }

    private static void EnsurePrivateDirectory(string path)
    {
        Directory.CreateDirectory(path);

        if (OperatingSystem.IsLinux())
        {
            File.SetUnixFileMode(
                path,
                UnixFileMode.UserRead |
                UnixFileMode.UserWrite |
                UnixFileMode.UserExecute);
        }
    }
}
EOF

# ------------------------------------------------------------
# LOGGING LOCAL
# ------------------------------------------------------------

cat > src/Miastro.Infrastructure.Platform.Linux/Logging/XdgFileLoggerProvider.cs <<'EOF'
using Microsoft.Extensions.Logging;

namespace Miastro.Infrastructure.Platform.Linux.Logging;

public sealed class XdgFileLoggerProvider : ILoggerProvider
{
    private readonly string _logFilePath;
    private readonly object _sync = new();

    public XdgFileLoggerProvider(string logFilePath)
    {
        _logFilePath = logFilePath;

        var directory = Path.GetDirectoryName(_logFilePath)
            ?? throw new InvalidOperationException(
                "La ruta del log no contiene directorio.");

        Directory.CreateDirectory(directory);
    }

    public ILogger CreateLogger(string categoryName) =>
        new XdgFileLogger(categoryName, _logFilePath, _sync);

    public void Dispose()
    {
    }

    private sealed class XdgFileLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _logFilePath;
        private readonly object _sync;

        public XdgFileLogger(
            string categoryName,
            string logFilePath,
            object sync)
        {
            _categoryName = categoryName;
            _logFilePath = logFilePath;
            _sync = sync;
        }

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull =>
            null;

        public bool IsEnabled(LogLevel logLevel) =>
            logLevel >= LogLevel.Information;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);

            var line =
                $"{DateTimeOffset.UtcNow:O} " +
                $"[{logLevel}] {_categoryName}: {message}";

            lock (_sync)
            {
                File.AppendAllText(
                    _logFilePath,
                    line + Environment.NewLine);

                if (OperatingSystem.IsLinux())
                {
                    File.SetUnixFileMode(
                        _logFilePath,
                        UnixFileMode.UserRead |
                        UnixFileMode.UserWrite);
                }
            }
        }
    }
}
EOF

# ------------------------------------------------------------
# BOOTSTRAP — settings + DI central
# ------------------------------------------------------------

cat > src/Miastro.Bootstrap/MiastroBootstrap.cs <<'EOF'
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Miastro.Application.Configuration;
using Miastro.Application.Platform;
using Miastro.Infrastructure.Platform.Linux.Logging;
using Miastro.Infrastructure.Platform.Linux.Xdg;

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
}
EOF

# ------------------------------------------------------------
# UI — MVVM + DI real
# ------------------------------------------------------------

cat > src/Miastro.UI.Avalonia/ViewModels/MainWindowViewModel.cs <<'EOF'
using Miastro.Application.Configuration;

namespace Miastro.UI.Avalonia.ViewModels;

public sealed class MainWindowViewModel
{
    private readonly ApplicationSettings _settings;

    public MainWindowViewModel(ApplicationSettings settings)
    {
        _settings = settings;
    }

    public string Title => "Miastro";

    public string Status => "Base técnica preparada";

    public string Language => _settings.Language;
}
EOF

cat > src/Miastro.UI.Avalonia/Views/MainWindow.axaml <<'EOF'
<Window
    x:Class="Miastro.UI.Avalonia.Views.MainWindow"
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:vm="using:Miastro.UI.Avalonia.ViewModels"
    x:DataType="vm:MainWindowViewModel"
    Width="960"
    Height="640"
    MinWidth="720"
    MinHeight="480"
    Title="{Binding Title}">

    <Grid Margin="32">
        <StackPanel
            HorizontalAlignment="Center"
            VerticalAlignment="Center"
            Spacing="10">

            <TextBlock
                Text="{Binding Title}"
                FontSize="28"
                FontWeight="SemiBold"
                HorizontalAlignment="Center" />

            <TextBlock
                Text="{Binding Status}"
                HorizontalAlignment="Center" />

        </StackPanel>
    </Grid>
</Window>
EOF

cat > src/Miastro.UI.Avalonia/Program.cs <<'EOF'
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Miastro.Bootstrap;
using Miastro.UI.Avalonia.ViewModels;

namespace Miastro.UI.Avalonia;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            var services = MiastroBootstrap.CreateServiceCollection();

            services.AddSingleton<MainWindowViewModel>();

            App.Services = services.BuildServiceProvider(
                new ServiceProviderOptions
                {
                    ValidateOnBuild = true,
                    ValidateScopes = true
                });

            var logger = App.Services
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("Miastro.Startup");

            logger.LogInformation(
                "Inicio de Miastro. Versión {Version}.",
                typeof(Program).Assembly.GetName().Version?.ToString()
                ?? "desconocida");

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(
                $"Miastro no pudo iniciarse: {ex.Message}");

            Environment.ExitCode = 1;
        }
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect();
}
EOF

cat > src/Miastro.UI.Avalonia/App.axaml.cs <<'EOF'
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Miastro.UI.Avalonia.ViewModels;
using Miastro.UI.Avalonia.Views;

namespace Miastro.UI.Avalonia;

public sealed partial class App : Application
{
    public static IServiceProvider Services { get; set; } =
        null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime
            is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext =
                    Services.GetRequiredService<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
EOF

# ------------------------------------------------------------
# Verificación automática
# ------------------------------------------------------------

dotnet restore Miastro.sln

dotnet build Miastro.sln \
  --configuration Release \
  --no-restore

echo
echo "=== VERIFICACIÓN XDG ==="
echo "Configuración esperada: ${XDG_CONFIG_HOME:-$HOME/.config}/miastro"
echo "Datos esperados:        ${XDG_DATA_HOME:-$HOME/.local/share}/miastro"
echo "Cache esperada:         ${XDG_CACHE_HOME:-$HOME/.cache}/miastro"
echo "Estado esperado:        ${XDG_STATE_HOME:-$HOME/.local/state}/miastro"

echo
echo "PATCH FASE 1 XDG/DI COMPLETADO"

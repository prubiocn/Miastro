using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Miastro.Bootstrap;
using Miastro.UI.Avalonia.ViewModels;
using Miastro.UI.Avalonia.Services;

namespace Miastro.UI.Avalonia;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            var services = MiastroBootstrap.CreateServiceCollection();

            services.AddMiastroUi();

            App.Services = services.BuildServiceProvider(
                new ServiceProviderOptions
                {
                    ValidateOnBuild = true,
                    ValidateScopes = true
                });

            MiastroBootstrap.InitializeAsync(App.Services)
                .GetAwaiter()
                .GetResult();

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

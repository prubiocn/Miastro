using Microsoft.Extensions.DependencyInjection;
using Miastro.UI.Avalonia.Navigation;
using Miastro.UI.Avalonia.States;
using Miastro.UI.Avalonia.ViewModels;

namespace Miastro.UI.Avalonia.Services;

public static class UiServiceCollectionExtensions
{
    public static IServiceCollection AddMiastroUi(
        this IServiceCollection services)
    {
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IUserErrorService, UserErrorService>();
        services.AddTransient<OperationState>();
        services.AddSingleton<MainWindowViewModel>();

        return services;
    }
}

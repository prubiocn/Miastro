namespace Miastro.UI.Avalonia.Navigation;

public sealed class NavigationService : INavigationService
{
    public string CurrentRoute { get; private set; } = "home";

    public void NavigateTo(string route)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route);
        CurrentRoute = route;
    }
}

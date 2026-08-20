namespace Miastro.UI.Avalonia.Navigation;

public interface INavigationService
{
    string CurrentRoute { get; }

    void NavigateTo(string route);
}

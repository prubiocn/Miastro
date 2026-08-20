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

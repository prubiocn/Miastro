using Avalonia.Controls;
using Avalonia.Interactivity;
using Miastro.UI.Avalonia.ViewModels;

namespace Miastro.UI.Avalonia.Views;

public sealed partial class MainWindow
    : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Loaded += OnLoaded;
    }

    private async void OnLoaded(
        object? sender,
        RoutedEventArgs e)
    {
        Loaded -= OnLoaded;

        if (DataContext
            is MainWindowViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }
}

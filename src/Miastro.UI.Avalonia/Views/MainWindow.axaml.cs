using Avalonia.Controls;
using Avalonia.Input;
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
    private void OnNatalWheelPointerPressed(
        object? sender,
        PointerPressedEventArgs e)
    {
        if (sender is not Image image
            || DataContext
                is not MainWindowViewModel viewModel)
        {
            return;
        }

        if (image.Bounds.Width <= 0.0
            || image.Bounds.Height <= 0.0)
        {
            return;
        }

        var point =
            e.GetPosition(
                image);

        viewModel.SelectNatalWheelAt(
            point.X,
            point.Y,
            image.Bounds.Width,
            image.Bounds.Height);

        e.Handled =
            true;
    }

}

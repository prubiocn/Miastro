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
    private void OnNatalWheelPointerMoved(
        object? sender,
        PointerEventArgs e)
    {
        if (sender is not Image image
            || DataContext
                is not MainWindowViewModel viewModel
            || image.Bounds.Width <= 0.0
            || image.Bounds.Height <= 0.0)
        {
            return;
        }

        var point =
            e.GetPosition(
                image);

        viewModel.UpdateNatalWheelTooltipAt(
            point.X,
            point.Y,
            image.Bounds.Width,
            image.Bounds.Height);
    }

    private void OnNatalWheelPointerExited(
        object? sender,
        PointerEventArgs e)
    {
        if (DataContext
            is MainWindowViewModel viewModel)
        {
            viewModel.ClearNatalWheelTooltip();
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

        image.Focus();

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

    private void OnNatalWheelViewportHostSizeChanged(
        object? sender,
        SizeChangedEventArgs e)
    {
        if (sender is not Border host
            || DataContext
                is not MainWindowViewModel viewModel)
        {
            return;
        }

        var availableWidth =
            host.Bounds.Width;

        if (!double.IsFinite(
                availableWidth)
            || availableWidth <= 0.0)
        {
            return;
        }

        var side =
            Math.Min(
                720.0,
                availableWidth);

        if (side <= 0.0)
        {
            return;
        }

        NatalWheelImage.Width =
            side;

        NatalWheelImage.Height =
            side;

        var renderScaling =
            TopLevel
                .GetTopLevel(host)
                ?.RenderScaling
            ?? 1.0;

        viewModel.UpdateNatalWheelViewport(
            side,
            side,
            renderScaling);
    }

    private void OnNatalWheelKeyDown(
        object? sender,
        KeyEventArgs e)
    {
        if (DataContext
            is not MainWindowViewModel viewModel)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Right:
            case Key.Down:
                viewModel
                    .MoveNatalWheelSelection(
                        1);
                break;

            case Key.Left:
            case Key.Up:
                viewModel
                    .MoveNatalWheelSelection(
                        -1);
                break;

            case Key.Home:
                viewModel
                    .SelectFirstNatalWheelObject();
                break;

            case Key.End:
                viewModel
                    .SelectLastNatalWheelObject();
                break;

            case Key.Escape:
                viewModel
                    .ClearNatalWheelSelection();
                break;

            default:
                return;
        }

        e.Handled =
            true;
    }

}

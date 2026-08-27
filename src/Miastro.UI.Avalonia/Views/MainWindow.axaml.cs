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

    private const double
        NatalPanelResponsiveCollapseWidth =
            720.0;

    private bool
        _natalPanelAutoCollapsed;

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

    private void OnNatalWheelPanelLayoutSizeChanged(
        object? sender,
        SizeChangedEventArgs e)
    {
        var width =
            e.NewSize.Width;

        if (!double.IsFinite(
                width)
            || width <= 0.0)
        {
            return;
        }

        if (width
            < NatalPanelResponsiveCollapseWidth)
        {
            if (NatalPhase8PanelHost.IsVisible)
            {
                SetNatalPhase8PanelExpanded(
                    false);

                _natalPanelAutoCollapsed =
                    true;
            }

            return;
        }

        if (_natalPanelAutoCollapsed)
        {
            SetNatalPhase8PanelExpanded(
                true);

            _natalPanelAutoCollapsed =
                false;
        }
    }

    private void OnNatalPhase8PanelToggleClick(
        object? sender,
        RoutedEventArgs e)
    {
        var expand =
            !NatalPhase8PanelHost.IsVisible;

        SetNatalPhase8PanelExpanded(
            expand);

        _natalPanelAutoCollapsed =
            false;
    }

    private void SetNatalPhase8PanelExpanded(
        bool expanded)
    {
        NatalPhase8PanelHost.IsVisible =
            expanded;

        NatalPhase8PanelToggleButton.Content =
            expanded
                ? "Ocultar panel"
                : "Mostrar panel";
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
                    .ClearNatalSelection();
                break;

            default:
                return;
        }

        e.Handled =
            true;
    }


    private void OnMainWindowKeyDown(
        object? sender,
        KeyEventArgs e)
    {
        if (e.Key != Key.Escape
            || DataContext
                is not MainWindowViewModel viewModel)
        {
            return;
        }

        viewModel.ClearNatalSelection();

        e.Handled =
            true;
    }


    private void OnNatalAspectMatrixCellClick(
        object? sender,
        RoutedEventArgs e)
    {
        if (sender is not Button button
            || button.DataContext
                is not global::Miastro.Application.Natal.Reading.NatalAspectMatrixCell cell
            || !cell.HasAspect
            || DataContext
                is not MainWindowViewModel viewModel
            || viewModel.NatalPanels is null)
        {
            return;
        }

        viewModel.NatalPanels.SelectedAspectCell =
            cell;
    }

}

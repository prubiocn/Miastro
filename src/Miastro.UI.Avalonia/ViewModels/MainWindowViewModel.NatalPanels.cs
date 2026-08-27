using Miastro.Application.Natal;
using Miastro.UI.Avalonia.ViewModels.NatalPanels;

namespace Miastro.UI.Avalonia.ViewModels;

public sealed partial class MainWindowViewModel
{
    private NatalPanelHostViewModel?
        _natalPanels;

    public NatalPanelHostViewModel?
        NatalPanels =>
            _natalPanels;

    public bool HasNatalPanels =>
        _natalPanels is not null;

    private void BuildNatalPanels(
        NatalChartSnapshotReadModel snapshot)
    {
        ArgumentNullException.ThrowIfNull(
            snapshot);

        if (_natalPanels is not null)
        {
            _natalPanels.ObjectSelectionRequested -=
                OnNatalPanelObjectSelectionRequested;
        }

        _natalPanels =
            NatalPanelHostViewModel.From(
                snapshot);

        _natalPanels.ObjectSelectionRequested +=
            OnNatalPanelObjectSelectionRequested;

        _natalPanels.AspectSelectionRequested +=
            OnNatalPanelAspectSelectionRequested;

        OnPropertyChanged(
            nameof(NatalPanels));

        OnPropertyChanged(
            nameof(HasNatalPanels));
    }

    private void ClearNatalPanels()
    {
        if (_natalPanels is null)
        {
            return;
        }

        _natalPanels.ObjectSelectionRequested -=
            OnNatalPanelObjectSelectionRequested;

        _natalPanels.AspectSelectionRequested -=
            OnNatalPanelAspectSelectionRequested;

        _natalPanels =
            null;

        OnPropertyChanged(
            nameof(NatalPanels));

        OnPropertyChanged(
            nameof(HasNatalPanels));
    }

    private void OnNatalPanelObjectSelectionRequested(
        object? sender,
        NatalObjectSelectionRequestedEventArgs args)
    {
        ApplyNatalWheelSelection(
            args.ObjectId.ToString());
    }

    private void SyncNatalPanelsFromWheelSelection()
    {
        _natalPanels?.SyncSelectedObject(
            _selectedNatalPlacement?.ObjectId,
            openPositions:
                _selectedNatalPlacement is not null);
    }

    private void OnNatalPanelAspectSelectionRequested(
        object? sender,
        NatalAspectSelectionRequestedEventArgs args)
    {
        // ApplyNatalWheelSelection sincroniza primero la selección
        // simple de Fase 7. Después restauramos expresamente el
        // estado dual y renderizamos ambos objetos + el aspecto.
        ApplyNatalWheelSelection(
            args.Cell.ColumnObjectId.ToString());

        _natalPanels?.SyncDualSelection(
            args.Cell);

        RenderNatalWheelSelectionHighlight();
    }

    private void RenderNatalWheelSelectionHighlight()
    {
        if (_natalWheelPresentation is null)
        {
            return;
        }

        var selectedIds =
            new List<string>();

        string? aspectFirstObjectId =
            null;

        string? aspectSecondObjectId =
            null;

        var state =
            _natalPanels?.SelectionState;

        if (state?.PrimaryObjectId is { } primary)
        {
            selectedIds.Add(
                primary.ToString());
        }
        else if (!string.IsNullOrWhiteSpace(
            _selectedNatalObjectId))
        {
            selectedIds.Add(
                _selectedNatalObjectId);
        }

        if (state?.SecondaryObjectId is { } secondary)
        {
            selectedIds.Add(
                secondary.ToString());

            if (state.PrimaryObjectId is { } first)
            {
                aspectFirstObjectId =
                    first.ToString();

                aspectSecondObjectId =
                    secondary.ToString();
            }
        }

        var presentation =
            _natalWheelPresentationService
                .RenderSelection(
                    _natalWheelPresentation,
                    selectedIds,
                    aspectFirstObjectId,
                    aspectSecondObjectId,
                    _natalWheelRenderScaling);

        ReplaceNatalWheelPresentation(
            presentation);
    }

    private void ReplaceNatalWheelPresentation(
        Miastro.UI.Avalonia.Services.NatalWheelPresentation
            presentation)
    {
        using var stream =
            new System.IO.MemoryStream(
                presentation.PngBytes,
                writable: false);

        var bitmap =
            new global::Avalonia.Media.Imaging.Bitmap(
                stream);

        _natalWheelBitmap?.Dispose();

        _natalWheelPresentation =
            presentation;

        _natalWheelBitmap =
            bitmap;

        OnPropertyChanged(
            nameof(NatalWheelBitmap));

        OnPropertyChanged(
            nameof(HasNatalWheel));
    }

}

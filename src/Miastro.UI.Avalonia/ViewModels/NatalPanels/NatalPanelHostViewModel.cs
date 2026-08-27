using System.ComponentModel;
using System.Runtime.CompilerServices;
using Miastro.Application.Natal;
using Miastro.Application.Natal.Reading;
using Miastro.Domain.Objects;

namespace Miastro.UI.Avalonia.ViewModels.NatalPanels;

public sealed class NatalPanelHostViewModel :
    INotifyPropertyChanged
{
    private NatalPanelKind
        _selectedPanel =
            NatalPanelKind.Positions;

    private NatalDataRowReadModel?
        _selectedDataRow;

    private NatalPositionRowReadModel?
        _selectedPositionRow;

    private NatalAspectMatrixCell?
        _selectedAspectCell;

    private NatalSelectionState
        _selectionState =
            NatalSelectionState.Neutral;

    private bool
        _synchronizingObjectSelection;

    public event EventHandler<
        NatalObjectSelectionRequestedEventArgs>?
        ObjectSelectionRequested;

    public event EventHandler<
        NatalAspectSelectionRequestedEventArgs>?
        AspectSelectionRequested;

    public event PropertyChangedEventHandler?
        PropertyChanged;

    public NatalDataPanelViewModel
        Data { get; }

    public NatalPositionsPanelViewModel
        Positions { get; }

    public NatalAspectsPanelViewModel
        Aspects { get; }

    public NatalDistributionPanelViewModel
        Distribution { get; }

    public NatalSummaryPanelViewModel
        Summary { get; }

    public int SelectedIndex
    {
        get =>
            (int)_selectedPanel;

        set
        {
            if (!Enum.IsDefined(
                typeof(NatalPanelKind),
                value))
            {
                return;
            }

            SelectedPanel =
                (NatalPanelKind)value;
        }
    }

    public NatalDataRowReadModel?
        SelectedDataRow
    {
        get =>
            _selectedDataRow;

        set
        {
            if (ReferenceEquals(
                _selectedDataRow,
                value))
            {
                return;
            }

            _selectedDataRow =
                value;

            OnPropertyChanged();

            if (_synchronizingObjectSelection
                || value is null)
            {
                return;
            }

            SyncSelectedObject(
                value.ObjectId,
                openPositions: true);

            RaiseObjectSelectionRequested(
                value.ObjectId);
        }
    }

    public NatalPositionRowReadModel?
        SelectedPositionRow
    {
        get =>
            _selectedPositionRow;

        set
        {
            if (ReferenceEquals(
                _selectedPositionRow,
                value))
            {
                return;
            }

            _selectedPositionRow =
                value;

            OnPropertyChanged();

            if (_synchronizingObjectSelection
                || value is null)
            {
                return;
            }

            SyncSelectedObject(
                value.ObjectId,
                openPositions: true);

            RaiseObjectSelectionRequested(
                value.ObjectId);
        }
    }

    public NatalSelectionState SelectionState =>
        _selectionState;

    public NatalAspectMatrixCell?
        SelectedAspectCell
    {
        get =>
            _selectedAspectCell;

        set
        {
            if (ReferenceEquals(
                _selectedAspectCell,
                value))
            {
                return;
            }

            _selectedAspectCell =
                value;

            OnPropertyChanged();

            if (_synchronizingObjectSelection
                || value is null
                || !value.HasAspect)
            {
                return;
            }

            _selectionState =
                NatalSelectionReducer.SelectAspect(
                    value);

            OnPropertyChanged(
                nameof(SelectionState));

            SyncDualRows(
                _selectionState);

            AspectSelectionRequested?.Invoke(
                this,
                new NatalAspectSelectionRequestedEventArgs(
                    value));
        }
    }

    public NatalPanelKind SelectedPanel
    {
        get =>
            _selectedPanel;

        set
        {
            if (_selectedPanel == value)
            {
                return;
            }

            _selectedPanel =
                value;

            OnPropertyChanged();

            OnPropertyChanged(
                nameof(SelectedIndex));
        }
    }

    public NatalPanelHostViewModel(
        NatalDataPanelViewModel data,
        NatalPositionsPanelViewModel positions,
        NatalAspectsPanelViewModel aspects,
        NatalDistributionPanelViewModel distribution,
        NatalSummaryPanelViewModel summary)
    {
        Data =
            data
            ?? throw new ArgumentNullException(
                nameof(data));

        Positions =
            positions
            ?? throw new ArgumentNullException(
                nameof(positions));

        Aspects =
            aspects
            ?? throw new ArgumentNullException(
                nameof(aspects));

        Distribution =
            distribution
            ?? throw new ArgumentNullException(
                nameof(distribution));

        Summary =
            summary
            ?? throw new ArgumentNullException(
                nameof(summary));
    }

    public static NatalPanelHostViewModel From(
        NatalChartSnapshotReadModel snapshot)
    {
        ArgumentNullException.ThrowIfNull(
            snapshot);

        return new NatalPanelHostViewModel(
            NatalDataPanelViewModel.From(
                snapshot),
            NatalPositionsPanelViewModel.From(
                snapshot),
            NatalAspectsPanelViewModel.From(
                snapshot),
            NatalDistributionPanelViewModel.From(
                snapshot),
            NatalSummaryPanelViewModel.From(
                snapshot));
    }

    public void ClearSelection()
    {
        _synchronizingObjectSelection =
            true;

        try
        {
            _selectionState =
                NatalSelectionReducer.Clear();

            _selectedAspectCell =
                null;

            _selectedDataRow =
                null;

            _selectedPositionRow =
                null;

            OnPropertyChanged(
                nameof(SelectionState));

            OnPropertyChanged(
                nameof(SelectedAspectCell));

            OnPropertyChanged(
                nameof(SelectedDataRow));

            OnPropertyChanged(
                nameof(SelectedPositionRow));
        }
        finally
        {
            _synchronizingObjectSelection =
                false;
        }
    }

    public void SyncDualSelection(
        NatalAspectMatrixCell cell)
    {
        ArgumentNullException.ThrowIfNull(
            cell);

        if (!cell.HasAspect)
        {
            throw new InvalidOperationException(
                "No se puede sincronizar una celda sin aspecto.");
        }

        _synchronizingObjectSelection =
            true;

        try
        {
            _selectedAspectCell =
                cell;

            _selectionState =
                NatalSelectionReducer.SelectAspect(
                    cell);

            SyncDualRows(
                _selectionState);

            OnPropertyChanged(
                nameof(SelectedAspectCell));

            OnPropertyChanged(
                nameof(SelectionState));

            SelectedPanel =
                NatalPanelKind.Aspects;
        }
        finally
        {
            _synchronizingObjectSelection =
                false;
        }
    }

    private void SyncDualRows(
        NatalSelectionState state)
    {
        _selectedDataRow =
            state.PrimaryObjectId is { } primary
                ? Data.Rows.FirstOrDefault(
                    row =>
                        row.ObjectId == primary)
                : null;

        _selectedPositionRow =
            state.SecondaryObjectId is { } secondary
                ? Positions.Rows.FirstOrDefault(
                    row =>
                        row.ObjectId == secondary)
                : state.PrimaryObjectId is { } primaryOnly
                    ? Positions.Rows.FirstOrDefault(
                        row =>
                            row.ObjectId == primaryOnly)
                    : null;

        OnPropertyChanged(
            nameof(SelectedDataRow));

        OnPropertyChanged(
            nameof(SelectedPositionRow));
    }

    public void SyncSelectedObject(
        AstrologicalObjectId? objectId,
        bool openPositions)
    {
        _synchronizingObjectSelection =
            true;

        _selectionState =
            objectId is null
                ? NatalSelectionReducer.Clear()
                : NatalSelectionReducer.SelectObject(
                    objectId.Value);

        _selectedAspectCell =
            null;

        try
        {
            _selectedDataRow =
                objectId is null
                    ? null
                    : Data.Rows.FirstOrDefault(
                        row =>
                            row.ObjectId
                            == objectId.Value);

            _selectedPositionRow =
                objectId is null
                    ? null
                    : Positions.Rows.FirstOrDefault(
                        row =>
                            row.ObjectId
                            == objectId.Value);

            OnPropertyChanged(
                nameof(SelectedDataRow));

            OnPropertyChanged(
                nameof(SelectedPositionRow));

            OnPropertyChanged(
                nameof(SelectionState));

            OnPropertyChanged(
                nameof(SelectedAspectCell));

            if (openPositions
                && objectId is not null)
            {
                SelectedPanel =
                    NatalPanelKind.Positions;
            }
        }
        finally
        {
            _synchronizingObjectSelection =
                false;
        }
    }

    private void RaiseObjectSelectionRequested(
        AstrologicalObjectId objectId)
        =>
            ObjectSelectionRequested?.Invoke(
                this,
                new NatalObjectSelectionRequestedEventArgs(
                    objectId));

    public void OpenPositions()
        =>
            SelectedPanel =
                NatalPanelKind.Positions;

    public void OpenData()
        =>
            SelectedPanel =
                NatalPanelKind.Data;

    public void OpenAspects()
        =>
            SelectedPanel =
                NatalPanelKind.Aspects;

    public void OpenDistribution()
        =>
            SelectedPanel =
                NatalPanelKind.Distribution;

    public void OpenSummary()
        =>
            SelectedPanel =
                NatalPanelKind.Summary;

    private void OnPropertyChanged(
        [CallerMemberName]
        string? propertyName = null)
        =>
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(
                    propertyName));
}

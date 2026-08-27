using Miastro.Application.Natal;
using Miastro.Application.Natal.Reading;

namespace Miastro.UI.Avalonia.ViewModels.NatalPanels;

public sealed class NatalAspectsPanelViewModel
{
    public NatalAspectMatrixReadModel Matrix { get; }

    public IReadOnlyList<
        NatalAspectMatrixParticipant>
        Participants =>
            Matrix.Participants;

    public IReadOnlyList<
        NatalAspectMatrixCell>
        Cells =>
            Matrix.Cells;

    public IReadOnlyList<
        NatalAspectMatrixColumnViewModel>
        Columns { get; }

    public IReadOnlyList<
        NatalAspectMatrixRowViewModel>
        Rows { get; }

    public bool HasParticipants =>
        Participants.Count > 0;

    public bool HasAspects =>
        Cells.Any(
            cell =>
                cell.HasAspect);

    public bool HasMatrixRows =>
        Rows.Count > 0;

    public NatalAspectsPanelViewModel(
        NatalAspectMatrixReadModel matrix)
    {
        ArgumentNullException.ThrowIfNull(
            matrix);

        Matrix =
            matrix;

        Columns =
            BuildColumns(
                matrix);

        Rows =
            BuildRows(
                matrix);
    }

    public static NatalAspectsPanelViewModel From(
        NatalChartSnapshotReadModel snapshot)
    {
        ArgumentNullException.ThrowIfNull(
            snapshot);

        return new NatalAspectsPanelViewModel(
            NatalAspectMatrixReader.Read(
                snapshot));
    }

    private static IReadOnlyList<
        NatalAspectMatrixColumnViewModel>
        BuildColumns(
            NatalAspectMatrixReadModel matrix)
    {
        var columns =
            new List<
                NatalAspectMatrixColumnViewModel>();

        for (
            var index = 0;
            index < matrix.Participants.Count;
            index++)
        {
            var fromColumn =
                matrix.Cells
                    .FirstOrDefault(
                        cell =>
                            cell.ColumnIndex
                                == index);

            var fromRow =
                matrix.Cells
                    .FirstOrDefault(
                        cell =>
                            cell.RowIndex
                                == index);

            var objectName =
                fromColumn?.ColumnObjectName
                ?? fromRow?.RowObjectName
                ?? $"Objeto {index + 1}";

            columns.Add(
                new NatalAspectMatrixColumnViewModel(
                    index,
                    objectName));
        }

        return columns;
    }

    private static IReadOnlyList<
        NatalAspectMatrixRowViewModel>
        BuildRows(
            NatalAspectMatrixReadModel matrix)
    {
        return matrix.Cells
            .GroupBy(
                cell =>
                    cell.RowIndex)
            .OrderBy(
                group =>
                    group.Key)
            .Select(
                group =>
                {
                    var cells =
                        group
                            .OrderBy(
                                cell =>
                                    cell.ColumnIndex)
                            .ToArray();

                    return new NatalAspectMatrixRowViewModel(
                        group.Key,
                        cells[0].RowObjectName,
                        cells);
                })
            .ToArray();
    }
}

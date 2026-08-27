using Miastro.Application.Natal.Reading;

namespace Miastro.UI.Avalonia.ViewModels.NatalPanels;

public sealed record NatalAspectMatrixColumnViewModel(
    int ColumnIndex,
    string ObjectName);

public sealed record NatalAspectMatrixRowViewModel(
    int RowIndex,
    string ObjectName,
    IReadOnlyList<NatalAspectMatrixCell> Cells);

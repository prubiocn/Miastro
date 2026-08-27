namespace Miastro.UI.Avalonia.ViewModels.NatalPanels;

public sealed record NatalDistributionRowViewModel(
    string Label,
    int Count,
    string ObjectsText,
    bool IsPredominant)
{
    public string CountText =>
        Count.ToString(
            System.Globalization.CultureInfo.InvariantCulture);

    public string AccessibleName =>
        string.IsNullOrWhiteSpace(
            ObjectsText)
            ? $"{Label}: {Count}"
            : $"{Label}: {Count}. {ObjectsText}";
}

public sealed record NatalDistributionSectionViewModel(
    string Title,
    IReadOnlyList<NatalDistributionRowViewModel> Rows,
    string StatusText);

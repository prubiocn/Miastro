using Miastro.Application.Natal;
using Miastro.Application.Natal.Reading;

namespace Miastro.UI.Avalonia.ViewModels.NatalPanels;

public sealed class NatalDataPanelViewModel
{
    public IReadOnlyList<NatalDataRowReadModel>
        Rows { get; }

    public bool HasRows =>
        Rows.Count > 0;

    public NatalDataPanelViewModel(
        IReadOnlyList<NatalDataRowReadModel> rows)
    {
        ArgumentNullException.ThrowIfNull(
            rows);

        Rows =
            rows.ToArray();
    }

    public static NatalDataPanelViewModel From(
        NatalChartSnapshotReadModel snapshot)
    {
        ArgumentNullException.ThrowIfNull(
            snapshot);

        return new NatalDataPanelViewModel(
            NatalDataPanelReader.Read(
                snapshot));
    }
}

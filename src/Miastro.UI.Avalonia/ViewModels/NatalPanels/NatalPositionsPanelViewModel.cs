using Miastro.Application.Natal;
using Miastro.Application.Natal.Reading;

namespace Miastro.UI.Avalonia.ViewModels.NatalPanels;

public sealed class NatalPositionsPanelViewModel
{
    public IReadOnlyList<NatalPositionRowReadModel>
        Rows { get; }

    public bool HasRows =>
        Rows.Count > 0;

    public NatalPositionsPanelViewModel(
        IReadOnlyList<NatalPositionRowReadModel> rows)
    {
        ArgumentNullException.ThrowIfNull(
            rows);

        Rows =
            rows.ToArray();
    }

    public static NatalPositionsPanelViewModel From(
        NatalChartSnapshotReadModel snapshot)
    {
        ArgumentNullException.ThrowIfNull(
            snapshot);

        return new NatalPositionsPanelViewModel(
            NatalPositionsPanelReader.Read(
                snapshot));
    }
}

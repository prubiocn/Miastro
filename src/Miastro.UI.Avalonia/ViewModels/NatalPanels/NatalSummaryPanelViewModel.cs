using Miastro.Application.Natal;
using Miastro.Application.Natal.Reading;

namespace Miastro.UI.Avalonia.ViewModels.NatalPanels;

public sealed class NatalSummaryPanelViewModel
{
    public NatalSummaryReadModel
        Summary { get; }

    public IReadOnlyList<string>
        Lines =>
            Summary.Lines;

    public string SunText =>
        Summary.SunText;

    public string MoonText =>
        Summary.MoonText;

    public string AscendantText =>
        Summary.AscendantText;

    public string MidheavenText =>
        Summary.MidheavenText;

    public string ElementText =>
        Summary.ElementText;

    public string ModalityText =>
        Summary.ModalityText;

    public string HouseConcentrationText =>
        Summary.HouseConcentrationText;

    public string RetrogradesText =>
        Summary.RetrogradesText;

    public IReadOnlyList<
        NatalSummaryAspectReadModel>
        MainAspects =>
            Summary.MainAspects;

    public bool HasMainAspects =>
        MainAspects.Count > 0;

    public NatalSummaryPanelViewModel(
        NatalSummaryReadModel summary)
    {
        ArgumentNullException.ThrowIfNull(
            summary);

        Summary =
            summary;
    }

    public static NatalSummaryPanelViewModel From(
        NatalChartSnapshotReadModel snapshot)
    {
        ArgumentNullException.ThrowIfNull(
            snapshot);

        return new NatalSummaryPanelViewModel(
            NatalSummaryBuilder.Build(
                snapshot));
    }
}

using Miastro.Application.Natal;
using Miastro.Application.Natal.Reading;

namespace Miastro.UI.Avalonia.ViewModels.NatalPanels;

public sealed class NatalDistributionPanelViewModel
{
    public NatalDistributionReadModel
        Zodiac { get; }

    public NatalHouseDistributionReadModel
        Houses { get; }

    public NatalDistributionSynthesisReadModel
        Synthesis { get; }

    public IReadOnlyList<string>
        SynthesisLines =>
            Synthesis.Lines;

    public NatalDistributionSectionViewModel
        Elements { get; }

    public NatalDistributionSectionViewModel
        Modalities { get; }

    public NatalDistributionSectionViewModel
        Polarities { get; }

    public NatalDistributionSectionViewModel
        EastWest { get; }

    public NatalDistributionSectionViewModel
        UpperLower { get; }

    public NatalDistributionSectionViewModel
        Quadrants { get; }

    public NatalDistributionSectionViewModel
        HouseModes { get; }

    public IReadOnlyList<
        NatalDistributionSectionViewModel>
        Sections { get; }

    public string ProfileText =>
        $"Perfil {Zodiac.ProfileId} · "
        + $"{Zodiac.CountedObjects.Count} objetos";

    public NatalDistributionPanelViewModel(
        NatalDistributionReadModel zodiac,
        NatalHouseDistributionReadModel houses,
        NatalDistributionSynthesisReadModel synthesis)
    {
        ArgumentNullException.ThrowIfNull(
            zodiac);

        ArgumentNullException.ThrowIfNull(
            houses);

        ArgumentNullException.ThrowIfNull(
            synthesis);

        if (!string.Equals(
                zodiac.ProfileId,
                houses.ProfileId,
                StringComparison.Ordinal)
            || !string.Equals(
                zodiac.ProfileId,
                synthesis.ProfileId,
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Las secciones de Distribución deben usar el mismo perfil.");
        }

        Zodiac =
            zodiac;

        Houses =
            houses;

        Synthesis =
            synthesis;

        Elements =
            BuildSection(
                "Elementos",
                zodiac.Elements);

        Modalities =
            BuildSection(
                "Modalidades",
                zodiac.Modalities);

        Polarities =
            BuildSection(
                "Polaridad",
                zodiac.Polarities);

        EastWest =
            BuildSection(
                "Hemisferio Este / Oeste",
                houses.EastWest);

        UpperLower =
            BuildSection(
                "Hemisferio Superior / Inferior",
                houses.UpperLower);

        Quadrants =
            BuildSection(
                "Cuadrantes",
                houses.Quadrants);

        HouseModes =
            BuildSection(
                "Casas angulares / sucedentes / cadentes",
                houses.HouseModes);

        Sections =
            new[]
            {
                Elements,
                Modalities,
                Polarities,
                EastWest,
                UpperLower,
                Quadrants,
                HouseModes
            };
    }

    public static NatalDistributionPanelViewModel From(
        NatalChartSnapshotReadModel snapshot)
    {
        ArgumentNullException.ThrowIfNull(
            snapshot);

        var zodiac =
            NatalDistributionService.Build(
                snapshot);

        var houses =
            NatalHouseDistributionService.Build(
                snapshot);

        var synthesis =
            NatalDistributionSynthesisBuilder.Build(
                zodiac,
                houses);

        return new NatalDistributionPanelViewModel(
            zodiac,
            houses,
            synthesis);
    }

    private static NatalDistributionSectionViewModel
        BuildSection<TCategory>(
            string title,
            NatalDistributionSection<TCategory> section)
        where TCategory : struct, Enum
    {
        var rows =
            section.Buckets
                .Select(
                    bucket =>
                        new NatalDistributionRowViewModel(
                            bucket.Label,
                            bucket.Count,
                            bucket.ObjectNames.Count == 0
                                ? "Sin objetos"
                                : string.Join(
                                    ", ",
                                    bucket.ObjectNames),
                            section.Predominant is { } predominant
                            && EqualityComparer<TCategory>
                                .Default
                                .Equals(
                                    bucket.Category,
                                    predominant)))
                .ToArray();

        var status =
            section.Predominant is { } predominantCategory
                ? "Predominio: "
                  + section.Buckets
                      .Single(
                          bucket =>
                              EqualityComparer<TCategory>
                                  .Default
                                  .Equals(
                                      bucket.Category,
                                      predominantCategory))
                      .Label
                : section.IsBalanced
                    ? "Equilibrado"
                    : "Sin predominio único";

        return new NatalDistributionSectionViewModel(
            title,
            rows,
            status);
    }
}

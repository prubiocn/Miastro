using Miastro.Application.Natal.Reading;
using Miastro.Domain.Aspects;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;
using Miastro.UI.Avalonia.ViewModels.NatalPanels;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalPanelViewModelTests
{
    [TestMethod]
    public void Host_defaults_to_positions()
    {
        var host =
            Host();

        Assert.AreEqual(
            NatalPanelKind.Positions,
            host.SelectedPanel);
    }

    [TestMethod]
    public void Host_exposes_five_small_panel_viewmodels()
    {
        var host =
            Host();

        Assert.IsNotNull(
            host.Data);

        Assert.IsNotNull(
            host.Positions);

        Assert.IsNotNull(
            host.Aspects);

        Assert.IsNotNull(
            host.Distribution);

        Assert.IsNotNull(
            host.Summary);
    }

    [TestMethod]
    public void Data_panel_preserves_read_model_rows()
    {
        var rows =
            new[]
            {
                new NatalDataRowReadModel(
                    AstrologicalObjectId.Sun,
                    "Sol",
                    "10° 00′",
                    "Aries",
                    "Marte",
                    false)
            };

        var panel =
            new NatalDataPanelViewModel(
                rows);

        Assert.IsTrue(
            panel.HasRows);

        Assert.AreEqual(
            "Sol",
            panel.Rows.Single()
                .ObjectName);
    }

    [TestMethod]
    public void Positions_panel_preserves_angles_as_distinct_facts()
    {
        var row =
            new NatalPositionRowReadModel(
                AstrologicalObjectId.Ascendant,
                "Ascendente",
                "12° 00′ Libra",
                "Libra",
                "Casa 1",
                "—",
                "Venus",
                "Libra",
                "Venus",
                null,
                true);

        var panel =
            new NatalPositionsPanelViewModel(
                new[]
                {
                    row
                });

        Assert.IsTrue(
            panel.Rows.Single()
                .IsAngle);
    }

    [TestMethod]
    public void Aspects_panel_exposes_real_triangular_cells()
    {
        var matrix =
            new NatalAspectMatrixReadModel(
                new[]
                {
                    new NatalAspectMatrixParticipant(
                        AstrologicalObjectId.Sun,
                        "Sol",
                        0),

                    new NatalAspectMatrixParticipant(
                        AstrologicalObjectId.Saturn,
                        "Saturno",
                        6)
                },
                new[]
                {
                    new NatalAspectMatrixCell(
                        1,
                        0,
                        AstrologicalObjectId.Saturn,
                        AstrologicalObjectId.Sun,
                        "Saturno",
                        "Sol",
                        AspectKind.Square,
                        "Cuadratura",
                        "□",
                        92.0,
                        90.0,
                        2.0,
                        7.0,
                        2.0,
                        "92°00′",
                        "2°00′",
                        "2°00′",
                        "Sol — cuadratura — Saturno — orbe 2°00′")
                });

        var panel =
            new NatalAspectsPanelViewModel(
                matrix);

        Assert.AreEqual(
            2,
            panel.Participants.Count);

        Assert.AreEqual(
            1,
            panel.Cells.Count);

        Assert.IsTrue(
            panel.HasAspects);
    }

    [TestMethod]
    public void Distribution_panel_exposes_textual_synthesis()
    {
        var zodiac =
            new NatalDistributionReadModel(
                "MiastroV1",
                Array.Empty<AstrologicalObjectId>(),
                EmptySection<
                    NatalDistributionElement>(),
                EmptySection<
                    NatalDistributionModality>(),
                EmptySection<
                    NatalDistributionPolarity>());

        var houses =
            new NatalHouseDistributionReadModel(
                "MiastroV1",
                EmptySection<
                    NatalEastWestHemisphere>(),
                EmptySection<
                    NatalUpperLowerHemisphere>(),
                EmptySection<
                    NatalHouseQuadrant>(),
                EmptySection<
                    NatalHouseMode>());

        var synthesis =
            new NatalDistributionSynthesisReadModel(
                "MiastroV1",
                new[]
                {
                    "Elemento predominante: Tierra (6/10)."
                });

        var panel =
            new NatalDistributionPanelViewModel(
                zodiac,
                houses,
                synthesis);

        Assert.AreEqual(
            1,
            panel.SynthesisLines.Count);

        Assert.AreEqual(
            "Elemento predominante: Tierra (6/10).",
            panel.SynthesisLines[0]);
    }

    [TestMethod]
    public void Summary_panel_exposes_compact_lines()
    {
        var summary =
            new NatalSummaryReadModel(
                "Sol: Aries, Casa 1.",
                "Luna: Tauro, Casa 2.",
                "ASC: Libra.",
                "MC: Cáncer.",
                "Elemento: Tierra (6).",
                "Modalidad: Fijo (4).",
                "Concentración de casas: Casa 10 (5/10).",
                "Retrógrados: Mercurio.",
                Array.Empty<
                    NatalSummaryAspectReadModel>());

        var panel =
            new NatalSummaryPanelViewModel(
                summary);

        Assert.AreEqual(
            8,
            panel.Lines.Count);
    }

    [TestMethod]
    public void Host_panel_navigation_is_deterministic()
    {
        var host =
            Host();

        host.OpenData();

        Assert.AreEqual(
            NatalPanelKind.Data,
            host.SelectedPanel);

        host.OpenAspects();

        Assert.AreEqual(
            NatalPanelKind.Aspects,
            host.SelectedPanel);

        host.OpenDistribution();

        Assert.AreEqual(
            NatalPanelKind.Distribution,
            host.SelectedPanel);

        host.OpenSummary();

        Assert.AreEqual(
            NatalPanelKind.Summary,
            host.SelectedPanel);

        host.OpenPositions();

        Assert.AreEqual(
            NatalPanelKind.Positions,
            host.SelectedPanel);
    }

    [TestMethod]
    public void Host_notifies_selected_panel_changes()
    {
        var host =
            Host();

        var notifications =
            new List<string?>();

        host.PropertyChanged +=
            (_, args) =>
                notifications.Add(
                    args.PropertyName);

        host.OpenAspects();

        CollectionAssert.Contains(
            notifications.ToArray(),
            nameof(
                NatalPanelHostViewModel
                    .SelectedPanel));
    }

    [TestMethod]
    public void Changing_to_same_panel_does_not_duplicate_notification()
    {
        var host =
            Host();

        var count =
            0;

        host.PropertyChanged +=
            (_, args) =>
            {
                if (args.PropertyName
                    == nameof(
                        NatalPanelHostViewModel
                            .SelectedPanel))
                {
                    count++;
                }
            };

        host.OpenPositions();

        Assert.AreEqual(
            0,
            count);
    }

    [TestMethod]
    public void Panel_viewmodels_do_not_modify_source_collections()
    {
        var source =
            new List<NatalDataRowReadModel>
            {
                new(
                    AstrologicalObjectId.Sun,
                    "Sol",
                    "00° 00′",
                    "Aries",
                    "Marte",
                    false)
            };

        var panel =
            new NatalDataPanelViewModel(
                source);

        source.Add(
            new NatalDataRowReadModel(
                AstrologicalObjectId.Moon,
                "Luna",
                "00° 00′",
                "Tauro",
                "Venus",
                false));

        Assert.AreEqual(
            1,
            panel.Rows.Count);
    }

    private static NatalPanelHostViewModel Host()
        => new(
            new NatalDataPanelViewModel(
                Array.Empty<
                    NatalDataRowReadModel>()),
            new NatalPositionsPanelViewModel(
                Array.Empty<
                    NatalPositionRowReadModel>()),
            new NatalAspectsPanelViewModel(
                new NatalAspectMatrixReadModel(
                    Array.Empty<
                        NatalAspectMatrixParticipant>(),
                    Array.Empty<
                        NatalAspectMatrixCell>())),
            new NatalDistributionPanelViewModel(
                EmptyDistribution(),
                EmptyHouseDistribution(),
                new NatalDistributionSynthesisReadModel(
                    "MiastroV1",
                    Array.Empty<string>())),
            new NatalSummaryPanelViewModel(
                new NatalSummaryReadModel(
                    "Sol: —.",
                    "Luna: —.",
                    "ASC: —.",
                    "MC: —.",
                    "Elemento: —.",
                    "Modalidad: —.",
                    "Concentración de casas: —.",
                    "Retrógrados: ninguno.",
                    Array.Empty<
                        NatalSummaryAspectReadModel>())));

    private static NatalDistributionReadModel
        EmptyDistribution()
        => new(
            "MiastroV1",
            Array.Empty<
                AstrologicalObjectId>(),
            EmptySection<
                NatalDistributionElement>(),
            EmptySection<
                NatalDistributionModality>(),
            EmptySection<
                NatalDistributionPolarity>());

    private static NatalHouseDistributionReadModel
        EmptyHouseDistribution()
        => new(
            "MiastroV1",
            EmptySection<
                NatalEastWestHemisphere>(),
            EmptySection<
                NatalUpperLowerHemisphere>(),
            EmptySection<
                NatalHouseQuadrant>(),
            EmptySection<
                NatalHouseMode>());

    private static NatalDistributionSection<T>
        EmptySection<T>()
        where T : struct, Enum
        => new(
            Array.Empty<
                NatalDistributionBucket<T>>(),
            null,
            true);
}

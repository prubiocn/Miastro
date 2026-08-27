using Miastro.Application.Natal.Reading;
using Miastro.Domain.Aspects;
using Miastro.Domain.Objects;
using Miastro.UI.Avalonia.ViewModels.NatalPanels;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalDualAspectSelectionTests
{
    [TestMethod]
    public void Selecting_aspect_cell_creates_dual_selection_state()
    {
        var host =
            Host();

        host.SelectedAspectCell =
            AspectCell();

        Assert.IsTrue(
            host.SelectionState
                .IsDualSelection);

        Assert.AreEqual(
            AstrologicalObjectId.Sun,
            host.SelectionState
                .PrimaryObjectId);

        Assert.AreEqual(
            AstrologicalObjectId.Saturn,
            host.SelectionState
                .SecondaryObjectId);

        Assert.AreEqual(
            AspectKind.Square,
            host.SelectionState
                .ActiveAspect?
                .Kind);
    }

    [TestMethod]
    public void Selecting_aspect_cell_raises_typed_event_once()
    {
        var host =
            Host();

        var count =
            0;

        NatalAspectMatrixCell? received =
            null;

        host.AspectSelectionRequested +=
            (_, args) =>
            {
                count++;
                received =
                    args.Cell;
            };

        host.SelectedAspectCell =
            AspectCell();

        Assert.AreEqual(
            1,
            count);

        Assert.IsNotNull(
            received);

        Assert.AreEqual(
            AspectKind.Square,
            received!.AspectKind);
    }

    [TestMethod]
    public void Dual_selection_synchronizes_primary_and_secondary_rows()
    {
        var host =
            Host();

        host.SelectedAspectCell =
            AspectCell();

        Assert.AreEqual(
            AstrologicalObjectId.Sun,
            host.SelectedDataRow?
                .ObjectId);

        Assert.AreEqual(
            AstrologicalObjectId.Saturn,
            host.SelectedPositionRow?
                .ObjectId);
    }

    [TestMethod]
    public void Programmatic_dual_sync_does_not_raise_feedback_event()
    {
        var host =
            Host();

        var count =
            0;

        host.AspectSelectionRequested +=
            (_, _) =>
                count++;

        host.SyncDualSelection(
            AspectCell());

        Assert.AreEqual(
            0,
            count);

        Assert.IsTrue(
            host.SelectionState
                .IsDualSelection);

        Assert.AreEqual(
            NatalPanelKind.Aspects,
            host.SelectedPanel);
    }

    [TestMethod]
    public void Clear_selection_returns_fully_neutral_state()
    {
        var host =
            Host();

        host.SyncDualSelection(
            AspectCell());

        host.ClearSelection();

        Assert.IsTrue(
            host.SelectionState
                .IsNeutral);

        Assert.IsNull(
            host.SelectedAspectCell);

        Assert.IsNull(
            host.SelectedDataRow);

        Assert.IsNull(
            host.SelectedPositionRow);
    }

    [TestMethod]
    public void Simple_selection_after_dual_removes_active_aspect()
    {
        var host =
            Host();

        host.SyncDualSelection(
            AspectCell());

        host.SyncSelectedObject(
            AstrologicalObjectId.Mercury,
            true);

        Assert.IsFalse(
            host.SelectionState
                .IsDualSelection);

        Assert.AreEqual(
            AstrologicalObjectId.Mercury,
            host.SelectionState
                .PrimaryObjectId);

        Assert.IsNull(
            host.SelectionState
                .ActiveAspect);

        Assert.IsNull(
            host.SelectedAspectCell);
    }

    [TestMethod]
    public void Xaml_binds_selected_aspect_cell()
    {
        var xaml =
            File.ReadAllText(
                Path.Combine(
                    FindRepoRoot(),
                    "src",
                    "Miastro.UI.Avalonia",
                    "Views",
                    "MainWindow.axaml"));

        StringAssert.Contains(
            xaml,
            "NatalPanels.SelectedAspectCell");
    }

    [TestMethod]
    public void Main_window_subscribes_and_unsubscribes_dual_event()
    {
        var source =
            File.ReadAllText(
                Path.Combine(
                    FindRepoRoot(),
                    "src",
                    "Miastro.UI.Avalonia",
                    "ViewModels",
                    "MainWindowViewModel.NatalPanels.cs"));

        StringAssert.Contains(
            source,
            "AspectSelectionRequested +=");

        StringAssert.Contains(
            source,
            "AspectSelectionRequested -=");

        StringAssert.Contains(
            source,
            "OnNatalPanelAspectSelectionRequested");
    }

    private static NatalPanelHostViewModel Host()
        => new(
            new NatalDataPanelViewModel(
                new[]
                {
                    Data(
                        AstrologicalObjectId.Sun,
                        "Sol"),

                    Data(
                        AstrologicalObjectId.Saturn,
                        "Saturno"),

                    Data(
                        AstrologicalObjectId.Mercury,
                        "Mercurio")
                }),
            new NatalPositionsPanelViewModel(
                new[]
                {
                    Position(
                        AstrologicalObjectId.Sun,
                        "Sol"),

                    Position(
                        AstrologicalObjectId.Saturn,
                        "Saturno"),

                    Position(
                        AstrologicalObjectId.Mercury,
                        "Mercurio")
                }),
            new NatalAspectsPanelViewModel(
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
                        AspectCell()
                    })),
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

    private static NatalAspectMatrixCell AspectCell()
        => new(
            RowIndex:
                1,
            ColumnIndex:
                0,
            RowObjectId:
                AstrologicalObjectId.Saturn,
            ColumnObjectId:
                AstrologicalObjectId.Sun,
            RowObjectName:
                "Saturno",
            ColumnObjectName:
                "Sol",
            AspectKind:
                AspectKind.Square,
            AspectName:
                "Cuadratura",
            AspectSymbol:
                "□",
            SeparationDegrees:
                92.0,
            ExactAngleDegrees:
                90.0,
            DeviationDegrees:
                2.0,
            AllowedOrbDegrees:
                7.0,
            UsedOrbDegrees:
                2.0,
            SeparationText:
                "92°00′",
            OrbText:
                "2°00′",
            DeviationText:
                "2°00′",
            AccessibleName:
                "Sol — cuadratura — Saturno — orbe 2°00′");

    private static NatalDataRowReadModel Data(
        AstrologicalObjectId id,
        string name)
        => new(
            id,
            name,
            "00° 00′",
            "Aries",
            "Marte",
            false);

    private static NatalPositionRowReadModel Position(
        AstrologicalObjectId id,
        string name)
        => new(
            id,
            name,
            "00° 00′ Aries",
            "Aries",
            "Casa 1",
            "Directo",
            "Marte",
            "Aries",
            "Marte",
            null,
            false);

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

    private static string FindRepoRoot()
    {
        var current =
            new DirectoryInfo(
                AppContext.BaseDirectory);

        while (current is not null)
        {
            if (File.Exists(
                Path.Combine(
                    current.FullName,
                    "Miastro.sln")))
            {
                return current.FullName;
            }

            current =
                current.Parent;
        }

        throw new InvalidOperationException(
            "No se encontró la raíz del repositorio.");
    }
}

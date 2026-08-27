using Miastro.Application.Natal.Reading;
using Miastro.Domain.Objects;
using Miastro.UI.Avalonia.ViewModels.NatalPanels;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalSimpleSelectionSynchronizationTests
{
    [TestMethod]
    public void Selecting_data_row_requests_same_object_and_opens_positions()
    {
        var host =
            Host();

        AstrologicalObjectId? requested =
            null;

        host.ObjectSelectionRequested +=
            (_, args) =>
                requested =
                    args.ObjectId;

        host.OpenData();

        host.SelectedDataRow =
            host.Data.Rows.Single(
                x =>
                    x.ObjectId
                    == AstrologicalObjectId.Sun);

        Assert.AreEqual(
            AstrologicalObjectId.Sun,
            requested);

        Assert.AreEqual(
            NatalPanelKind.Positions,
            host.SelectedPanel);

        Assert.AreEqual(
            AstrologicalObjectId.Sun,
            host.SelectedPositionRow?
                .ObjectId);
    }

    [TestMethod]
    public void Selecting_position_row_requests_same_object()
    {
        var host =
            Host();

        AstrologicalObjectId? requested =
            null;

        host.ObjectSelectionRequested +=
            (_, args) =>
                requested =
                    args.ObjectId;

        host.SelectedPositionRow =
            host.Positions.Rows.Single(
                x =>
                    x.ObjectId
                    == AstrologicalObjectId.Moon);

        Assert.AreEqual(
            AstrologicalObjectId.Moon,
            requested);

        Assert.AreEqual(
            AstrologicalObjectId.Moon,
            host.SelectedDataRow?
                .ObjectId);
    }

    [TestMethod]
    public void Wheel_sync_selects_corresponding_rows_without_feedback_event()
    {
        var host =
            Host();

        var requests =
            0;

        host.ObjectSelectionRequested +=
            (_, _) =>
                requests++;

        host.SyncSelectedObject(
            AstrologicalObjectId.Mars,
            openPositions: true);

        Assert.AreEqual(
            0,
            requests);

        Assert.AreEqual(
            AstrologicalObjectId.Mars,
            host.SelectedDataRow?
                .ObjectId);

        Assert.AreEqual(
            AstrologicalObjectId.Mars,
            host.SelectedPositionRow?
                .ObjectId);

        Assert.AreEqual(
            NatalPanelKind.Positions,
            host.SelectedPanel);
    }

    [TestMethod]
    public void Clearing_wheel_selection_clears_panel_rows_without_event()
    {
        var host =
            Host();

        host.SyncSelectedObject(
            AstrologicalObjectId.Sun,
            true);

        var requests =
            0;

        host.ObjectSelectionRequested +=
            (_, _) =>
                requests++;

        host.SyncSelectedObject(
            null,
            false);

        Assert.IsNull(
            host.SelectedDataRow);

        Assert.IsNull(
            host.SelectedPositionRow);

        Assert.AreEqual(
            0,
            requests);
    }

    [TestMethod]
    public void Selection_uses_object_id_not_row_index()
    {
        var host =
            Host();

        host.SyncSelectedObject(
            AstrologicalObjectId.Mars,
            false);

        Assert.AreEqual(
            "Marte",
            host.SelectedPositionRow?
                .ObjectName);
    }

    [TestMethod]
    public void Xaml_binds_data_and_positions_selected_items()
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
            "NatalPanels.SelectedDataRow");

        StringAssert.Contains(
            xaml,
            "NatalPanels.SelectedPositionRow");
    }

    [TestMethod]
    public void Main_window_wheel_selection_has_panel_sync_hook()
    {
        var root =
            FindRepoRoot();

        var natal =
            File.ReadAllText(
                Path.Combine(
                    root,
                    "src",
                    "Miastro.UI.Avalonia",
                    "ViewModels",
                    "MainWindowViewModel.Natal.cs"));

        var bridge =
            File.ReadAllText(
                Path.Combine(
                    root,
                    "src",
                    "Miastro.UI.Avalonia",
                    "ViewModels",
                    "MainWindowViewModel.NatalPanels.cs"));

        StringAssert.Contains(
            natal,
            "SyncNatalPanelsFromWheelSelection();");

        StringAssert.Contains(
            bridge,
            "OnNatalPanelObjectSelectionRequested");

        StringAssert.Contains(
            bridge,
            "ApplyNatalWheelSelection(");
    }

    private static NatalPanelHostViewModel Host()
    {
        var dataRows =
            new[]
            {
                Data(
                    AstrologicalObjectId.Mars,
                    "Marte"),

                Data(
                    AstrologicalObjectId.Sun,
                    "Sol"),

                Data(
                    AstrologicalObjectId.Moon,
                    "Luna")
            };

        var positionRows =
            new[]
            {
                Position(
                    AstrologicalObjectId.Moon,
                    "Luna"),

                Position(
                    AstrologicalObjectId.Mars,
                    "Marte"),

                Position(
                    AstrologicalObjectId.Sun,
                    "Sol")
            };

        return new NatalPanelHostViewModel(
            new NatalDataPanelViewModel(
                dataRows),
            new NatalPositionsPanelViewModel(
                positionRows),
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
    }

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

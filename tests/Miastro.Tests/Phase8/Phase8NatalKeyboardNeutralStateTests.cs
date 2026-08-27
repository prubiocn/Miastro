using Miastro.Application.Natal.Reading;
using Miastro.Domain.Aspects;
using Miastro.Domain.Objects;
using Miastro.UI.Avalonia.ViewModels.NatalPanels;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalKeyboardNeutralStateTests
{
    [TestMethod]
    public void Host_clear_returns_dual_selection_to_neutral()
    {
        var host =
            Host();

        host.SyncDualSelection(
            AspectCell());

        Assert.IsTrue(
            host.SelectionState.IsDualSelection);

        host.ClearSelection();

        Assert.IsTrue(
            host.SelectionState.IsNeutral);

        Assert.IsNull(
            host.SelectedDataRow);

        Assert.IsNull(
            host.SelectedPositionRow);

        Assert.IsNull(
            host.SelectedAspectCell);
    }

    [TestMethod]
    public void Neutral_clear_is_idempotent()
    {
        var host =
            Host();

        host.ClearSelection();
        host.ClearSelection();

        Assert.IsTrue(
            host.SelectionState.IsNeutral);

        Assert.IsNull(
            host.SelectedDataRow);

        Assert.IsNull(
            host.SelectedPositionRow);

        Assert.IsNull(
            host.SelectedAspectCell);
    }

    [TestMethod]
    public void Window_routes_escape_globally()
    {
        var xaml =
            Read(
                "src/Miastro.UI.Avalonia/Views/"
                + "MainWindow.axaml");

        StringAssert.Contains(
            xaml,
            "KeyDown=\"OnMainWindowKeyDown\"");
    }

    [TestMethod]
    public void Global_handler_processes_only_escape()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/Views/"
                + "MainWindow.axaml.cs");

        var handler =
            Extract(
                source,
                "private void OnMainWindowKeyDown(",
                "\n    }",
                includeEnd: true);

        StringAssert.Contains(
            handler,
            "e.Key != Key.Escape");

        StringAssert.Contains(
            handler,
            "ClearNatalSelection()");

        Assert.IsFalse(
            handler.Contains(
                "Key.Right",
                StringComparison.Ordinal));

        Assert.IsFalse(
            handler.Contains(
                "Key.Left",
                StringComparison.Ordinal));

        Assert.IsFalse(
            handler.Contains(
                "Key.Tab",
                StringComparison.Ordinal));
    }

    [TestMethod]
    public void Wheel_escape_uses_same_neutral_endpoint()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/Views/"
                + "MainWindow.axaml.cs");

        var start =
            source.IndexOf(
                "private void OnNatalWheelKeyDown(",
                StringComparison.Ordinal);

        var end =
            source.IndexOf(
                "private void OnMainWindowKeyDown(",
                start,
                StringComparison.Ordinal);

        Assert.IsTrue(
            start >= 0);

        Assert.IsTrue(
            end > start);

        var handler =
            source[start..end];

        StringAssert.Contains(
            handler,
            "case Key.Escape:");

        StringAssert.Contains(
            handler,
            ".ClearNatalSelection();");
    }

    [TestMethod]
    public void Unified_clear_delegates_to_existing_selection_pipeline()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/ViewModels/"
                + "MainWindowViewModel.Natal.cs");

        var start =
            source.IndexOf(
                "public void ClearNatalSelection()",
                StringComparison.Ordinal);

        var end =
            source.IndexOf(
                "public void ClearNatalWheelSelection()",
                start,
                StringComparison.Ordinal);

        Assert.IsTrue(
            start >= 0);

        Assert.IsTrue(
            end > start);

        var method =
            source[start..end];

        StringAssert.Contains(
            method,
            "ApplyNatalWheelSelection(");

        StringAssert.Contains(
            method,
            "null");
    }

    [TestMethod]
    public void Legacy_clear_remains_compatible()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/ViewModels/"
                + "MainWindowViewModel.Natal.cs");

        StringAssert.Contains(
            source,
            "public void ClearNatalWheelSelection()");

        StringAssert.Contains(
            source,
            "ClearNatalSelection();");
    }

    [TestMethod]
    public void Standard_wheel_keyboard_navigation_is_preserved()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/Views/"
                + "MainWindow.axaml.cs");

        foreach (var key in new[]
        {
            "Key.Right",
            "Key.Down",
            "Key.Left",
            "Key.Up",
            "Key.Home",
            "Key.End"
        })
        {
            StringAssert.Contains(
                source,
                key);
        }
    }

    [TestMethod]
    public void Tab_and_listbox_navigation_are_not_overridden()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/Views/"
                + "MainWindow.axaml.cs");

        Assert.IsFalse(
            source.Contains(
                "case Key.Tab:",
                StringComparison.Ordinal));

        var xaml =
            Read(
                "src/Miastro.UI.Avalonia/Views/"
                + "MainWindow.axaml");

        StringAssert.Contains(
            xaml,
            "Name=\"NatalPhase8PanelTabs\"");

        StringAssert.Contains(
            xaml,
            "NatalPanels.SelectedDataRow");

        StringAssert.Contains(
            xaml,
            "NatalPanels.SelectedPositionRow");

        StringAssert.Contains(
            xaml,
            "NatalPanels.SelectedAspectCell");
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
                        "Saturno")
                }),
            new NatalPositionsPanelViewModel(
                new[]
                {
                    Position(
                        AstrologicalObjectId.Sun,
                        "Sol"),

                    Position(
                        AstrologicalObjectId.Saturn,
                        "Saturno")
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

    private static string Read(
        string relativePath)
        => File.ReadAllText(
            Path.Combine(
                FindRepoRoot(),
                relativePath));

    private static string Extract(
        string source,
        string startMarker,
        string endMarker,
        bool includeEnd)
    {
        var start =
            source.IndexOf(
                startMarker,
                StringComparison.Ordinal);

        if (start < 0)
        {
            throw new InvalidOperationException(
                $"No se encontró {startMarker}.");
        }

        var end =
            source.IndexOf(
                endMarker,
                start,
                StringComparison.Ordinal);

        if (end < 0)
        {
            throw new InvalidOperationException(
                $"No se encontró {endMarker}.");
        }

        return source[
            start..
            (
                includeEnd
                    ? end + endMarker.Length
                    : end
            )];
    }

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

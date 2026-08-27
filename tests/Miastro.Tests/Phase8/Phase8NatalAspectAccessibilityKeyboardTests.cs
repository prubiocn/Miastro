using Miastro.Application.Natal.Reading;
using Miastro.Domain.Aspects;
using Miastro.Domain.Objects;
using Miastro.UI.Avalonia.ViewModels.NatalPanels;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalAspectAccessibilityKeyboardTests
{
    [TestMethod]
    public void Aspect_cell_accessible_name_contains_both_objects_aspect_and_orb()
    {
        var cell =
            Cell();

        StringAssert.Contains(
            cell.AccessibleName,
            "Sol");

        StringAssert.Contains(
            cell.AccessibleName,
            "cuadratura");

        StringAssert.Contains(
            cell.AccessibleName,
            "Saturno");

        StringAssert.Contains(
            cell.AccessibleName,
            "2°14");
    }

    [TestMethod]
    public void Aspect_matrix_list_has_explicit_accessible_name()
    {
        var xaml =
            ReadXaml();

        StringAssert.Contains(
            xaml,
            "Name=\"NatalAspectMatrixList\"");

        StringAssert.Contains(
            xaml,
            "AutomationProperties.Name=\"Matriz de aspectos natales\"");
    }

    [TestMethod]
    public void Aspect_matrix_exposes_keyboard_help()
    {
        var xaml =
            ReadXaml();

        StringAssert.Contains(
            xaml,
            "Usa las flechas para recorrer los aspectos.");

        StringAssert.Contains(
            xaml,
            "Escape limpia la selección.");
    }

    [TestMethod]
    public void Aspect_cell_exposes_accessible_name_and_tooltip()
    {
        var xaml =
            ReadXaml();

        StringAssert.Contains(
            xaml,
            "AutomationProperties.Name=\"{Binding AccessibleName}\"");

        StringAssert.Contains(
            xaml,
            "AutomationProperties.HelpText=\"{Binding AccessibleName}\"");

        StringAssert.Contains(
            xaml,
            "ToolTip.Tip=\"{Binding AccessibleName}\"");
    }

    [TestMethod]
    public void Aspect_is_not_expressed_by_color_only()
    {
        var xaml =
            ReadXaml();

        StringAssert.Contains(
            xaml,
            "Text=\"{Binding AspectSymbol}\"");

        StringAssert.Contains(
            xaml,
            "Text=\"{Binding AspectName}\"");

        StringAssert.Contains(
            xaml,
            "Text=\"{Binding OrbText}\"");

        StringAssert.Contains(
            xaml,
            "Text=\"{Binding ColumnObjectName}\"");

        StringAssert.Contains(
            xaml,
            "Text=\"{Binding RowObjectName}\"");
    }

    [TestMethod]
    public void Aspect_list_preserves_standard_single_selection_keyboard_behavior()
    {
        var xaml =
            ReadXaml();

        StringAssert.Contains(
            xaml,
            "SelectionMode=\"Single\"");

        StringAssert.Contains(
            xaml,
            "SelectedItem=\"{Binding NatalPanels.SelectedAspectCell, Mode=TwoWay}\"");
    }

    [TestMethod]
    public void Window_does_not_intercept_arrow_keys_globally()
    {
        var source =
            File.ReadAllText(
                Path.Combine(
                    FindRepoRoot(),
                    "src",
                    "Miastro.UI.Avalonia",
                    "Views",
                    "MainWindow.axaml.cs"));

        var start =
            source.IndexOf(
                "private void OnMainWindowKeyDown(",
                StringComparison.Ordinal);

        Assert.IsTrue(
            start >= 0);

        var handler =
            source[start..];

        StringAssert.Contains(
            handler,
            "e.Key != Key.Escape");

        Assert.IsFalse(
            handler.Contains(
                "case Key.Right:",
                StringComparison.Ordinal));

        Assert.IsFalse(
            handler.Contains(
                "case Key.Left:",
                StringComparison.Ordinal));

        Assert.IsFalse(
            handler.Contains(
                "case Key.Up:",
                StringComparison.Ordinal));

        Assert.IsFalse(
            handler.Contains(
                "case Key.Down:",
                StringComparison.Ordinal));
    }

    [TestMethod]
    public void Programmatic_matrix_selection_creates_dual_state()
    {
        var host =
            Host();

        host.SelectedAspectCell =
            Cell();

        Assert.IsTrue(
            host.SelectionState.IsDualSelection);

        Assert.AreEqual(
            AstrologicalObjectId.Sun,
            host.SelectionState.PrimaryObjectId);

        Assert.AreEqual(
            AstrologicalObjectId.Saturn,
            host.SelectionState.SecondaryObjectId);

        Assert.AreEqual(
            AspectKind.Square,
            host.SelectionState.ActiveAspect?.Kind);
    }

    [TestMethod]
    public void Escape_endpoint_remains_global_and_neutral()
    {
        var source =
            File.ReadAllText(
                Path.Combine(
                    FindRepoRoot(),
                    "src",
                    "Miastro.UI.Avalonia",
                    "ViewModels",
                    "MainWindowViewModel.Natal.cs"));

        StringAssert.Contains(
            source,
            "public void ClearNatalSelection()");

        StringAssert.Contains(
            source,
            "ApplyNatalWheelSelection(");
    }

    private static NatalAspectMatrixCell Cell()
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
            92.2333333333,
            90.0,
            2.2333333333,
            7.0,
            2.2333333333,
            "92°14′",
            "2°14′",
            "2°14′",
            "Sol — cuadratura — Saturno — orbe 2°14′");

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
                        Cell()
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

    private static string ReadXaml()
        => File.ReadAllText(
            Path.Combine(
                FindRepoRoot(),
                "src",
                "Miastro.UI.Avalonia",
                "Views",
                "MainWindow.axaml"));

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

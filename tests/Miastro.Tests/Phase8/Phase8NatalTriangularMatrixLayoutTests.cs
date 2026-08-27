using Miastro.Application.Natal.Reading;
using Miastro.Domain.Aspects;
using Miastro.Domain.Objects;
using Miastro.UI.Avalonia.ViewModels.NatalPanels;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalTriangularMatrixLayoutTests
{
    [TestMethod]
    public void Layout_has_one_row_per_participant_after_first()
    {
        var vm =
            Panel();

        Assert.AreEqual(
            4,
            vm.Columns.Count);

        Assert.AreEqual(
            3,
            vm.Rows.Count);
    }

    [TestMethod]
    public void Row_lengths_form_strict_lower_triangle()
    {
        var vm =
            Panel();

        CollectionAssert.AreEqual(
            new[]
            {
                1,
                2,
                3
            },
            vm.Rows
                .Select(
                    row =>
                        row.Cells.Count)
                .ToArray());
    }

    [TestMethod]
    public void Every_visual_cell_is_below_diagonal()
    {
        var vm =
            Panel();

        foreach (
            var row
            in vm.Rows)
        {
            foreach (
                var cell
                in row.Cells)
            {
                Assert.IsTrue(
                    cell.ColumnIndex
                        < cell.RowIndex);
            }
        }
    }

    [TestMethod]
    public void Triangle_has_no_ab_ba_duplicates()
    {
        var vm =
            Panel();

        var pairs =
            vm.Rows
                .SelectMany(
                    row =>
                        row.Cells)
                .Select(
                    cell =>
                        Normalize(
                            cell.RowObjectId,
                            cell.ColumnObjectId))
                .ToArray();

        Assert.AreEqual(
            pairs.Length,
            pairs.Distinct().Count());
    }

    [TestMethod]
    public void Matrix_contains_explicit_no_aspect_cells()
    {
        var vm =
            Panel();

        Assert.IsTrue(
            vm.Cells.Any(
                cell =>
                    !cell.HasAspect));

        var missing =
            vm.Cells.Single(
                cell =>
                    cell.RowObjectId
                        == AstrologicalObjectId.Mars
                    && cell.ColumnObjectId
                        == AstrologicalObjectId.Moon);

        Assert.IsFalse(
            missing.HasAspect);
    }

    [TestMethod]
    public void Xaml_contains_real_triangular_matrix()
    {
        var xaml =
            Read(
                "src/Miastro.UI.Avalonia/Views/"
                + "MainWindow.axaml");

        StringAssert.Contains(
            xaml,
            "Name=\"NatalAspectTriangularMatrix\"");

        StringAssert.Contains(
            xaml,
            "NatalPanels.Aspects.Rows");

        StringAssert.Contains(
            xaml,
            "ItemsSource=\"{Binding Cells}\"");
    }

    [TestMethod]
    public void Matrix_supports_horizontal_scroll_on_narrow_width()
    {
        var xaml =
            Read(
                "src/Miastro.UI.Avalonia/Views/"
                + "MainWindow.axaml");

        var aspectStart =
            xaml.IndexOf(
                "Header=\"Aspectos\"",
                StringComparison.Ordinal);

        var distributionStart =
            xaml.IndexOf(
                "Header=\"Distribución\"",
                aspectStart,
                StringComparison.Ordinal);

        Assert.IsTrue(
            aspectStart >= 0);

        Assert.IsTrue(
            distributionStart > aspectStart);

        var block =
            xaml[
                aspectStart..
                distributionStart];

        StringAssert.Contains(
            block,
            "HorizontalScrollBarVisibility=\"Auto\"");
    }

    [TestMethod]
    public void Aspect_cells_are_standard_keyboard_activatable_buttons()
    {
        var xaml =
            Read(
                "src/Miastro.UI.Avalonia/Views/"
                + "MainWindow.axaml");

        StringAssert.Contains(
            xaml,
            "Click=\"OnNatalAspectMatrixCellClick\"");

        StringAssert.Contains(
            xaml,
            "AutomationProperties.Name=\"{Binding AccessibleName}\"");

        StringAssert.Contains(
            xaml,
            "IsEnabled=\"{Binding HasAspect}\"");
    }

    [TestMethod]
    public void Codebehind_routes_matrix_activation_to_existing_selected_cell()
    {
        var source =
            Read(
                "src/Miastro.UI.Avalonia/Views/"
                + "MainWindow.axaml.cs");

        StringAssert.Contains(
            source,
            "OnNatalAspectMatrixCellClick");

        StringAssert.Contains(
            source,
            "NatalPanels.SelectedAspectCell");

        StringAssert.Contains(
            source,
            "viewModel.NatalPanels.SelectedAspectCell");

        StringAssert.Contains(
            source,
            "cell;");
    }

    [TestMethod]
    public void Compact_accessible_list_remains_available()
    {
        var xaml =
            Read(
                "src/Miastro.UI.Avalonia/Views/"
                + "MainWindow.axaml");

        StringAssert.Contains(
            xaml,
            "Header=\"Lista accesible de aspectos\"");

        StringAssert.Contains(
            xaml,
            "Name=\"NatalAspectMatrixList\"");
    }

    [TestMethod]
    public void Layout_projection_preserves_canonical_cell_order()
    {
        var vm =
            Panel();

        var actual =
            vm.Rows
                .SelectMany(
                    row =>
                        row.Cells)
                .Select(
                    cell =>
                        (
                            cell.RowIndex,
                            cell.ColumnIndex
                        ))
                .ToArray();

        var expected =
            actual
                .OrderBy(
                    pair =>
                        pair.RowIndex)
                .ThenBy(
                    pair =>
                        pair.ColumnIndex)
                .ToArray();

        CollectionAssert.AreEqual(
            expected,
            actual);
    }

    private static NatalAspectsPanelViewModel Panel()
    {
        var participants =
            new[]
            {
                Participant(
                    AstrologicalObjectId.Sun,
                    "Sol",
                    0),

                Participant(
                    AstrologicalObjectId.Moon,
                    "Luna",
                    1),

                Participant(
                    AstrologicalObjectId.Mars,
                    "Marte",
                    4),

                Participant(
                    AstrologicalObjectId.Saturn,
                    "Saturno",
                    6)
            };

        var cells =
            new[]
            {
                Cell(
                    1,
                    0,
                    AstrologicalObjectId.Moon,
                    AstrologicalObjectId.Sun,
                    "Luna",
                    "Sol",
                    AspectKind.Trine),

                Cell(
                    2,
                    0,
                    AstrologicalObjectId.Mars,
                    AstrologicalObjectId.Sun,
                    "Marte",
                    "Sol",
                    AspectKind.Square),

                Cell(
                    2,
                    1,
                    AstrologicalObjectId.Mars,
                    AstrologicalObjectId.Moon,
                    "Marte",
                    "Luna",
                    null),

                Cell(
                    3,
                    0,
                    AstrologicalObjectId.Saturn,
                    AstrologicalObjectId.Sun,
                    "Saturno",
                    "Sol",
                    AspectKind.Opposition),

                Cell(
                    3,
                    1,
                    AstrologicalObjectId.Saturn,
                    AstrologicalObjectId.Moon,
                    "Saturno",
                    "Luna",
                    AspectKind.Sextile),

                Cell(
                    3,
                    2,
                    AstrologicalObjectId.Saturn,
                    AstrologicalObjectId.Mars,
                    "Saturno",
                    "Marte",
                    null)
            };

        return new NatalAspectsPanelViewModel(
            new NatalAspectMatrixReadModel(
                participants,
                cells));
    }

    private static NatalAspectMatrixParticipant Participant(
        AstrologicalObjectId id,
        string name,
        int order)
        => new(
            id,
            name,
            order);

    private static NatalAspectMatrixCell Cell(
        int row,
        int column,
        AstrologicalObjectId rowId,
        AstrologicalObjectId columnId,
        string rowName,
        string columnName,
        AspectKind? kind)
    {
        var has =
            kind is not null;

        return new NatalAspectMatrixCell(
            row,
            column,
            rowId,
            columnId,
            rowName,
            columnName,
            kind,
            has
                ? kind!.Value.ToString()
                : "Sin aspecto",
            has
                ? "•"
                : "—",
            90.0,
            has
                ? 90.0
                : 0.0,
            has
                ? 0.0
                : 90.0,
            has
                ? 7.0
                : 0.0,
            has
                ? 0.0
                : 0.0,
            "90°00′",
            has
                ? "0°00′"
                : "—",
            has
                ? "0°00′"
                : "—",
            has
                ? $"{columnName} — {kind} — {rowName} — orbe 0°00′"
                : $"{columnName} — {rowName} — sin aspecto");
    }

    private static string Normalize(
        AstrologicalObjectId first,
        AstrologicalObjectId second)
    {
        var values =
            new[]
            {
                first.ToString(),
                second.ToString()
            };

        Array.Sort(
            values,
            StringComparer.Ordinal);

        return string.Join(
            "|",
            values);
    }

    private static string Read(
        string relativePath)
        => File.ReadAllText(
            Path.Combine(
                FindRepoRoot(),
                relativePath));

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

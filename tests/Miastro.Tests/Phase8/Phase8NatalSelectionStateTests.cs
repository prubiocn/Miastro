using Miastro.Application.Natal.Reading;
using Miastro.Domain.Aspects;
using Miastro.Domain.Objects;

namespace Miastro.Tests.Phase8;

[TestClass]
public sealed class Phase8NatalSelectionStateTests
{
    [TestMethod]
    public void Neutral_state_has_no_selected_objects()
    {
        var state =
            NatalSelectionState.Neutral;

        Assert.IsTrue(
            state.IsNeutral);

        Assert.IsFalse(
            state.HasPrimaryObject);

        Assert.IsFalse(
            state.IsDualSelection);

        Assert.AreEqual(
            0,
            state.SelectedObjectIds.Count);
    }

    [TestMethod]
    public void Selecting_one_object_creates_simple_selection()
    {
        var state =
            NatalSelectionReducer.SelectObject(
                AstrologicalObjectId.Mars);

        Assert.IsFalse(
            state.IsNeutral);

        Assert.IsTrue(
            state.HasPrimaryObject);

        Assert.IsFalse(
            state.IsDualSelection);

        Assert.AreEqual(
            AstrologicalObjectId.Mars,
            state.PrimaryObjectId);

        Assert.IsNull(
            state.SecondaryObjectId);

        Assert.IsNull(
            state.ActiveAspect);
    }

    [TestMethod]
    public void Selecting_aspect_creates_dual_selection()
    {
        var state =
            NatalSelectionReducer.SelectAspect(
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.Saturn,
                AspectKind.Square);

        Assert.IsTrue(
            state.IsDualSelection);

        Assert.AreEqual(
            AstrologicalObjectId.Sun,
            state.PrimaryObjectId);

        Assert.AreEqual(
            AstrologicalObjectId.Saturn,
            state.SecondaryObjectId);

        Assert.IsNotNull(
            state.ActiveAspect);

        Assert.AreEqual(
            AspectKind.Square,
            state.ActiveAspect!.Kind);
    }

    [TestMethod]
    public void Dual_selection_is_normalized_by_canonical_object_order()
    {
        var state =
            NatalSelectionReducer.SelectAspect(
                AstrologicalObjectId.Saturn,
                AstrologicalObjectId.Sun,
                AspectKind.Trine);

        Assert.AreEqual(
            AstrologicalObjectId.Sun,
            state.PrimaryObjectId);

        Assert.AreEqual(
            AstrologicalObjectId.Saturn,
            state.SecondaryObjectId);

        Assert.AreEqual(
            AstrologicalObjectId.Sun,
            state.ActiveAspect!.FirstObjectId);

        Assert.AreEqual(
            AstrologicalObjectId.Saturn,
            state.ActiveAspect.SecondObjectId);
    }

    [TestMethod]
    public void Selecting_matrix_cell_uses_its_persisted_aspect_identity()
    {
        var cell =
            new NatalAspectMatrixCell(
                RowIndex: 1,
                ColumnIndex: 0,
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

        var state =
            NatalSelectionReducer.SelectAspect(
                cell);

        Assert.IsTrue(
            state.IsDualSelection);

        Assert.AreEqual(
            AstrologicalObjectId.Sun,
            state.PrimaryObjectId);

        Assert.AreEqual(
            AstrologicalObjectId.Saturn,
            state.SecondaryObjectId);

        Assert.AreEqual(
            AspectKind.Square,
            state.ActiveAspect!.Kind);
    }

    [TestMethod]
    public void Cell_without_aspect_cannot_create_dual_selection()
    {
        var cell =
            new NatalAspectMatrixCell(
                RowIndex: 1,
                ColumnIndex: 0,
                RowObjectId:
                    AstrologicalObjectId.Saturn,
                ColumnObjectId:
                    AstrologicalObjectId.Sun,
                RowObjectName:
                    "Saturno",
                ColumnObjectName:
                    "Sol",
                AspectKind:
                    null,
                AspectName:
                    "Sin aspecto",
                AspectSymbol:
                    string.Empty,
                SeparationDegrees:
                    null,
                ExactAngleDegrees:
                    null,
                DeviationDegrees:
                    null,
                AllowedOrbDegrees:
                    null,
                UsedOrbDegrees:
                    null,
                SeparationText:
                    string.Empty,
                OrbText:
                    string.Empty,
                DeviationText:
                    string.Empty,
                AccessibleName:
                    "Sol — Saturno — sin aspecto");

        var rejected =
            false;

        try
        {
            _ =
                NatalSelectionReducer
                    .SelectAspect(
                        cell);
        }
        catch (InvalidOperationException)
        {
            rejected =
                true;
        }

        Assert.IsTrue(
            rejected);
    }

    [TestMethod]
    public void Same_object_cannot_form_dual_selection()
    {
        var rejected =
            false;

        try
        {
            _ =
                NatalSelectionReducer
                    .SelectAspect(
                        AstrologicalObjectId.Sun,
                        AstrologicalObjectId.Sun,
                        AspectKind.Conjunction);
        }
        catch (InvalidOperationException)
        {
            rejected =
                true;
        }

        Assert.IsTrue(
            rejected);
    }

    [TestMethod]
    public void ContainsObject_matches_both_members_of_dual_selection()
    {
        var state =
            NatalSelectionReducer.SelectAspect(
                AstrologicalObjectId.Venus,
                AstrologicalObjectId.Mars,
                AspectKind.Sextile);

        Assert.IsTrue(
            NatalSelectionReducer.ContainsObject(
                state,
                AstrologicalObjectId.Venus));

        Assert.IsTrue(
            NatalSelectionReducer.ContainsObject(
                state,
                AstrologicalObjectId.Mars));

        Assert.IsFalse(
            NatalSelectionReducer.ContainsObject(
                state,
                AstrologicalObjectId.Jupiter));
    }

    [TestMethod]
    public void Clear_returns_neutral_state()
    {
        var selected =
            NatalSelectionReducer.SelectAspect(
                AstrologicalObjectId.Moon,
                AstrologicalObjectId.Pluto,
                AspectKind.Opposition);

        Assert.IsTrue(
            selected.IsDualSelection);

        var cleared =
            NatalSelectionReducer.Clear();

        Assert.IsTrue(
            cleared.IsNeutral);

        Assert.AreEqual(
            0,
            cleared.SelectedObjectIds.Count);
    }

    [TestMethod]
    public void Selecting_single_object_after_aspect_removes_secondary_and_active_aspect()
    {
        var dual =
            NatalSelectionReducer.SelectAspect(
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.Saturn,
                AspectKind.Square);

        Assert.IsTrue(
            dual.IsDualSelection);

        var single =
            NatalSelectionReducer.SelectObject(
                AstrologicalObjectId.Mercury);

        Assert.AreEqual(
            AstrologicalObjectId.Mercury,
            single.PrimaryObjectId);

        Assert.IsNull(
            single.SecondaryObjectId);

        Assert.IsNull(
            single.ActiveAspect);

        Assert.IsFalse(
            single.IsDualSelection);
    }
}

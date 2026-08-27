using Miastro.Domain.Natal;
using Miastro.Domain.Objects;

namespace Miastro.Application.Natal.Reading;

public static class NatalSelectionReducer
{
    public static NatalSelectionState Clear()
        => NatalSelectionState.Neutral;

    public static NatalSelectionState SelectObject(
        AstrologicalObjectId objectId)
        => new(
            objectId,
            null,
            null);

    public static NatalSelectionState SelectAspect(
        NatalAspectMatrixCell cell)
    {
        ArgumentNullException.ThrowIfNull(
            cell);

        if (!cell.HasAspect
            || cell.AspectKind is null)
        {
            throw new InvalidOperationException(
                "No se puede seleccionar una celda sin aspecto.");
        }

        return BuildAspectSelection(
            cell.ColumnObjectId,
            cell.RowObjectId,
            cell.AspectKind.Value);
    }

    public static NatalSelectionState SelectAspect(
        AstrologicalObjectId firstObjectId,
        AstrologicalObjectId secondObjectId,
        Domain.Aspects.AspectKind kind)
        => BuildAspectSelection(
            firstObjectId,
            secondObjectId,
            kind);

    public static bool ContainsObject(
        NatalSelectionState state,
        AstrologicalObjectId objectId)
    {
        ArgumentNullException.ThrowIfNull(
            state);

        return state.PrimaryObjectId == objectId
            || state.SecondaryObjectId == objectId;
    }

    private static NatalSelectionState
        BuildAspectSelection(
            AstrologicalObjectId firstObjectId,
            AstrologicalObjectId secondObjectId,
            Domain.Aspects.AspectKind kind)
    {
        if (firstObjectId == secondObjectId)
        {
            throw new InvalidOperationException(
                "Una selección dual requiere dos objetos distintos.");
        }

        var firstIndex =
            NatalObjectOrder.GetIndex(
                firstObjectId);

        var secondIndex =
            NatalObjectOrder.GetIndex(
                secondObjectId);

        var primary =
            firstIndex <= secondIndex
                ? firstObjectId
                : secondObjectId;

        var secondary =
            firstIndex <= secondIndex
                ? secondObjectId
                : firstObjectId;

        return new NatalSelectionState(
            primary,
            secondary,
            new NatalAspectSelection(
                primary,
                secondary,
                kind));
    }
}

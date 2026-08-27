using Miastro.Domain.Aspects;
using Miastro.Domain.Objects;

namespace Miastro.Application.Natal.Reading;

public sealed record NatalAspectSelection(
    AstrologicalObjectId FirstObjectId,
    AstrologicalObjectId SecondObjectId,
    AspectKind Kind);

public sealed record NatalSelectionState(
    AstrologicalObjectId? PrimaryObjectId,
    AstrologicalObjectId? SecondaryObjectId,
    NatalAspectSelection? ActiveAspect)
{
    public static NatalSelectionState Neutral { get; } =
        new(
            null,
            null,
            null);

    public bool IsNeutral =>
        PrimaryObjectId is null
        && SecondaryObjectId is null
        && ActiveAspect is null;

    public bool HasPrimaryObject =>
        PrimaryObjectId is not null;

    public bool IsDualSelection =>
        PrimaryObjectId is not null
        && SecondaryObjectId is not null
        && ActiveAspect is not null;

    public IReadOnlyList<AstrologicalObjectId>
        SelectedObjectIds
        =>
            PrimaryObjectId is null
                ? Array.Empty<AstrologicalObjectId>()
                : SecondaryObjectId is null
                    ? new[]
                    {
                        PrimaryObjectId.Value
                    }
                    : new[]
                    {
                        PrimaryObjectId.Value,
                        SecondaryObjectId.Value
                    };
}

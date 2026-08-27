using Miastro.Domain.Aspects;
using Miastro.Domain.Objects;

namespace Miastro.Application.Natal.Reading;

public sealed record NatalAspectMatrixParticipant(
    AstrologicalObjectId ObjectId,
    string ObjectName,
    int CanonicalIndex);

public sealed record NatalAspectMatrixCell(
    int RowIndex,
    int ColumnIndex,
    AstrologicalObjectId RowObjectId,
    AstrologicalObjectId ColumnObjectId,
    string RowObjectName,
    string ColumnObjectName,
    AspectKind? AspectKind,
    string AspectName,
    string AspectSymbol,
    double? SeparationDegrees,
    double? ExactAngleDegrees,
    double? DeviationDegrees,
    double? AllowedOrbDegrees,
    double? UsedOrbDegrees,
    string SeparationText,
    string OrbText,
    string DeviationText,
    string AccessibleName)
{
    public bool HasAspect =>
        AspectKind is not null;
}

public sealed record NatalAspectMatrixReadModel(
    IReadOnlyList<NatalAspectMatrixParticipant> Participants,
    IReadOnlyList<NatalAspectMatrixCell> Cells);

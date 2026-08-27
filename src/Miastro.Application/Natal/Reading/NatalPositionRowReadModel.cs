using Miastro.Domain.Objects;
using Miastro.Domain.Placements;

namespace Miastro.Application.Natal.Reading;

public sealed record NatalPositionRowReadModel(
    AstrologicalObjectId ObjectId,
    string ObjectName,
    string ExactPositionText,
    string SignName,
    string HouseText,
    string MotionText,
    string SignRulersText,
    string HouseCuspSignText,
    string HouseRulersText,
    MotionState? Motion,
    bool IsAngle)
{
    public string GlyphText =>
        NatalFactsPresentationCatalog
            .ObjectGlyphText(
                ObjectId);

    public string AccessibleName =>
        $"{ObjectName}, "
        + $"{ExactPositionText}, "
        + $"{HouseText}, "
        + $"{MotionText}";

    public string AngleKindText =>
        IsAngle
            ? "Ángulo"
            : string.Empty;
}

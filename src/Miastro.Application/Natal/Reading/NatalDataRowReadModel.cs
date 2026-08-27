using Miastro.Domain.Objects;

namespace Miastro.Application.Natal.Reading;

public sealed record NatalDataRowReadModel(
    AstrologicalObjectId ObjectId,
    string ObjectName,
    string DegreeText,
    string SignName,
    string SignRulersText,
    bool IsAngle)
{
    public string GlyphText =>
        NatalFactsPresentationCatalog
            .ObjectGlyphText(
                ObjectId);

    public string AccessibleName =>
        $"{ObjectName}, "
        + $"{DegreeText} {SignName}, "
        + $"regente {SignRulersText}";
}

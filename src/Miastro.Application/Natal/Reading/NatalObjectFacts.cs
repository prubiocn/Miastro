using Miastro.Domain.Objects;
using Miastro.Domain.Placements;
using Miastro.Domain.Zodiac;

namespace Miastro.Application.Natal.Reading;

public sealed record NatalObjectFacts(
    AstrologicalObjectId ObjectId,
    double LongitudeDegrees,
    ZodiacSign Sign,
    double DegreeInSign,
    int? HouseNumber,
    MotionState? Motion,
    IReadOnlyList<AstrologicalObjectId> SignRulers,
    ZodiacSign? HouseCuspSign,
    IReadOnlyList<AstrologicalObjectId> HouseRulers);

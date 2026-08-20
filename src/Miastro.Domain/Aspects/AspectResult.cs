using Miastro.Domain.Angles;
using Miastro.Domain.Objects;

namespace Miastro.Domain.Aspects;

public sealed record AspectResult(
    AstrologicalObjectId FirstObject,
    AstrologicalObjectId SecondObject,
    AspectDefinition Definition,
    AngularSeparation Separation,
    double ExactAngleDegrees,
    double DeviationDegrees,
    double AllowedOrbDegrees,
    double UsedOrbDegrees);

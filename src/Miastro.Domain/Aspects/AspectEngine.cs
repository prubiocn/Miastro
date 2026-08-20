using Miastro.Domain.Angles;
using Miastro.Domain.Objects;

namespace Miastro.Domain.Aspects;

public static class AspectEngine
{
    public static AspectResult? Detect(
        AstrologicalObjectId firstObject,
        EclipticLongitude firstLongitude,
        AstrologicalObjectId secondObject,
        EclipticLongitude secondLongitude,
        AspectProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        if (!profile.IsParticipant(firstObject) ||
            !profile.IsParticipant(secondObject))
        {
            return null;
        }

        var separation =
            AngularSeparation.Between(
                firstLongitude,
                secondLongitude);

        var candidates =
            profile.Aspects
                .Select(definition =>
                {
                    var deviation = Math.Abs(
                        separation.Degrees -
                        definition.ExactAngleDegrees);

                    var allowedOrb =
                        profile.GetAllowedOrb(
                            definition,
                            firstObject,
                            secondObject);

                    return new
                    {
                        Definition = definition,
                        Deviation = deviation,
                        AllowedOrb = allowedOrb
                    };
                })
                .Where(candidate =>
                    candidate.Deviation <=
                    candidate.AllowedOrb)
                .OrderBy(candidate =>
                    candidate.Deviation)
                .ThenBy(candidate =>
                    candidate.Definition.Priority)
                .ToArray();

        if (candidates.Length == 0)
        {
            return null;
        }

        var selected = candidates[0];

        return new AspectResult(
            firstObject,
            secondObject,
            selected.Definition,
            separation,
            selected.Definition.ExactAngleDegrees,
            selected.Deviation,
            selected.AllowedOrb,
            selected.Deviation);
    }
}

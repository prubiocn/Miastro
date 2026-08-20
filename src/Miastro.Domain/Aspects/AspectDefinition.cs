namespace Miastro.Domain.Aspects;

public sealed record AspectDefinition
{
    public AspectKind Kind { get; }

    public double ExactAngleDegrees { get; }

    public double BaseOrbDegrees { get; }

    public int Priority { get; }

    public AspectDefinition(
        AspectKind kind,
        double exactAngleDegrees,
        double baseOrbDegrees,
        int priority)
    {
        if (!double.IsFinite(exactAngleDegrees) ||
            exactAngleDegrees is < 0.0 or > 180.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(exactAngleDegrees));
        }

        if (!double.IsFinite(baseOrbDegrees) ||
            baseOrbDegrees < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(baseOrbDegrees));
        }

        if (priority < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(priority));
        }

        Kind = kind;
        ExactAngleDegrees = exactAngleDegrees;
        BaseOrbDegrees = baseOrbDegrees;
        Priority = priority;
    }
}

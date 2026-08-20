namespace Miastro.Domain.Placements;

public static class MotionStateResolver
{
    public static MotionState FromSpeed(
        double speedDegreesPerDay)
    {
        if (!double.IsFinite(speedDegreesPerDay))
        {
            throw new ArgumentOutOfRangeException(
                nameof(speedDegreesPerDay));
        }

        if (speedDegreesPerDay > 0.0)
        {
            return MotionState.Direct;
        }

        if (speedDegreesPerDay < 0.0)
        {
            return MotionState.Retrograde;
        }

        return MotionState.Stationary;
    }
}

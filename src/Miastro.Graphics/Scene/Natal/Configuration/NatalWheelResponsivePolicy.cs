namespace Miastro.Graphics.Scene.Natal.Configuration;

public static class NatalWheelResponsivePolicy
{
    public const double FullThreshold =
        720.0;

    public const double CompactThreshold =
        480.0;

    public static NatalWheelDetailLevel Resolve(
        double width,
        double height)
    {
        if (!double.IsFinite(width)
            || !double.IsFinite(height)
            || width <= 0.0
            || height <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(width));
        }

        var available =
            Math.Min(
                width,
                height);

        if (available >= FullThreshold)
        {
            return NatalWheelDetailLevel.Full;
        }

        if (available >= CompactThreshold)
        {
            return NatalWheelDetailLevel.Compact;
        }

        return NatalWheelDetailLevel.Minimal;
    }
}

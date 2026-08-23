using Miastro.Graphics.Geometry;

namespace Miastro.Graphics.Layout;

public sealed record NatalWheelMetrics(
    double Width,
    double Height,
    double Scale,
    ChartPoint Center,
    double OuterRadius,
    double ZodiacInnerRadius,
    double DegreeInnerRadius,
    double HouseOuterRadius,
    double HouseInnerRadius,
    double AspectRadius)
{
    public const double ReferenceSize = 800.0;

    public const double MinimumUsableSize = 360.0;

    public static NatalWheelMetrics Create(
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

        var size =
            Math.Min(
                width,
                height);

        var effectiveSize =
            size;

        var scale =
            effectiveSize
            / ReferenceSize;

        var outerRadius =
            effectiveSize
            * 0.46;

        return new NatalWheelMetrics(
            width,
            height,
            scale,
            new ChartPoint(
                width / 2.0,
                height / 2.0),
            outerRadius,
            outerRadius * 0.88,
            outerRadius * 0.81,
            outerRadius * 0.73,
            outerRadius * 0.44,
            outerRadius * 0.40);
    }
}

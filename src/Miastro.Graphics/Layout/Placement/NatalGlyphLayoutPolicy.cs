using Miastro.Graphics.Layout;

namespace Miastro.Graphics.Layout.Placement;

public sealed record NatalGlyphLayoutPolicy(
    double GlyphSize,
    double MinimumGap,
    double RadialStep,
    double LeaderLineThreshold,
    int MaximumRadialLevel)
{
    public static NatalGlyphLayoutPolicy
        FromMetrics(
            NatalWheelMetrics metrics)
    {
        ArgumentNullException.ThrowIfNull(
            metrics);

        var glyphSize =
            28.0 * metrics.Scale;

        var minimumGap =
            4.0 * metrics.Scale;

        return new NatalGlyphLayoutPolicy(
            glyphSize,
            minimumGap,
            glyphSize * 1.60
                + minimumGap,
            glyphSize * 0.75,
            12);
    }
}

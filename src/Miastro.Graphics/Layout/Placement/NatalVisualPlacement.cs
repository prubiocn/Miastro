using Miastro.Graphics.Geometry;

namespace Miastro.Graphics.Layout.Placement;

public sealed record NatalVisualPlacement(
    string Id,
    double RealLongitudeDegrees,
    double RealScreenAngleDegrees,
    double VisualScreenAngleDegrees,
    int RadialLevel,
    double VisualRadius,
    ChartPoint RealAnchor,
    ChartPoint VisualCenter,
    ChartRect Bounds,
    bool HasLeaderLine,
    ChartPoint? LeaderLineStart,
    ChartPoint? LeaderLineEnd)
{
    public double AngularOffsetDegrees
    {
        get
        {
            var normalized =
                NatalWheelCoordinates.NormalizeDegrees(
                    VisualScreenAngleDegrees
                    - RealScreenAngleDegrees);

            return normalized > 180.0
                ? normalized - 360.0
                : normalized;
        }
    }
}

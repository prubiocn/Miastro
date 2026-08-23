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
    public double AngularOffsetDegrees =>
        NatalWheelCoordinates.NormalizeDegrees(
            VisualScreenAngleDegrees
            - RealScreenAngleDegrees);
}

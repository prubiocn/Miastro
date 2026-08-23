using Miastro.Graphics.Geometry;

namespace Miastro.Graphics.Layout;

public sealed record AngleAxisLayout(
    NatalAngleKind Kind,
    double RealLongitudeDegrees,
    double ScreenAngleDegrees,
    ChartPoint OuterPoint,
    ChartPoint InnerPoint);

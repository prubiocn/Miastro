using Miastro.Graphics.Geometry;

namespace Miastro.Graphics.Layout;

public sealed record DegreeTickLayout(
    int ZodiacDegree,
    DegreeTickKind Kind,
    double ScreenAngleDegrees,
    ChartPoint OuterPoint,
    ChartPoint InnerPoint);

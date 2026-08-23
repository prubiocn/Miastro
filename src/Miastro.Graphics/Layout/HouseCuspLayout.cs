using Miastro.Graphics.Geometry;

namespace Miastro.Graphics.Layout;

public sealed record HouseCuspLayout(
    int HouseNumber,
    double RealLongitudeDegrees,
    double ScreenAngleDegrees,
    ChartPoint OuterPoint,
    ChartPoint InnerPoint,
    double HouseCenterLongitudeDegrees,
    double HouseCenterScreenAngleDegrees,
    ChartPoint HouseNumberPosition);

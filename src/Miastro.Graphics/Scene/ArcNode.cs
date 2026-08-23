using Miastro.Graphics.Geometry;

namespace Miastro.Graphics.Scene;

public sealed record ArcNode : SceneNode
{
    public ArcNode(
        string id,
        SceneLayer layer,
        ChartPoint center,
        double radius,
        double startAngleDegrees,
        double sweepAngleDegrees)
        : base(id, layer)
    {
        Center = center;
        Radius = radius;
        StartAngleDegrees = startAngleDegrees;
        SweepAngleDegrees = sweepAngleDegrees;
    }

    public ChartPoint Center { get; }

    public double Radius { get; }

    public double StartAngleDegrees { get; }

    public double SweepAngleDegrees { get; }
}

using Miastro.Graphics.Geometry;

namespace Miastro.Graphics.Scene;

public sealed record CircleNode : SceneNode
{
    public CircleNode(
        string id,
        SceneLayer layer,
        ChartPoint center,
        double radius)
        : base(id, layer)
    {
        Center = center;
        Radius = radius;
    }

    public ChartPoint Center { get; }

    public double Radius { get; }
}

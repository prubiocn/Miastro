using Miastro.Graphics.Geometry;

namespace Miastro.Graphics.Scene;

public sealed record PathNode : SceneNode
{
    public PathNode(
        string id,
        SceneLayer layer,
        IReadOnlyList<ChartPoint> points,
        bool closed)
        : base(id, layer)
    {
        Points = points;
        Closed = closed;
    }

    public IReadOnlyList<ChartPoint> Points { get; }

    public bool Closed { get; }
}

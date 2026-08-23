using Miastro.Graphics.Geometry;

namespace Miastro.Graphics.Scene;

public sealed record LineNode : SceneNode
{
    public LineNode(
        string id,
        SceneLayer layer,
        ChartPoint start,
        ChartPoint end)
        : base(id, layer)
    {
        Start = start;
        End = end;
    }

    public ChartPoint Start { get; }

    public ChartPoint End { get; }
}

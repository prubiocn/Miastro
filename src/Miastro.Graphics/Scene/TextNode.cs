using Miastro.Graphics.Geometry;

namespace Miastro.Graphics.Scene;

public sealed record TextNode : SceneNode
{
    public TextNode(
        string id,
        SceneLayer layer,
        string text,
        ChartPoint position,
        double size,
        ChartRect bounds)
        : base(id, layer)
    {
        Text = text;
        Position = position;
        Size = size;
        Bounds = bounds;
    }

    public string Text { get; }

    public ChartPoint Position { get; }

    public double Size { get; }

    public ChartRect Bounds { get; }
}

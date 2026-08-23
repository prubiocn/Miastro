using Miastro.Graphics.Geometry;

namespace Miastro.Graphics.Scene;

public sealed record GlyphNode : SceneNode
{
    public GlyphNode(
        string id,
        SceneLayer layer,
        string glyphKey,
        ChartPoint position,
        double size,
        ChartRect bounds)
        : base(id, layer)
    {
        GlyphKey = glyphKey;
        Position = position;
        Size = size;
        Bounds = bounds;
    }

    public string GlyphKey { get; }

    public ChartPoint Position { get; }

    public double Size { get; }

    public ChartRect Bounds { get; }
}

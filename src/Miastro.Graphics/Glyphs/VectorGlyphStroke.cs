using Miastro.Graphics.Geometry;

namespace Miastro.Graphics.Glyphs;

public sealed record VectorGlyphStroke(
    IReadOnlyList<ChartPoint> Points,
    bool Closed = false);

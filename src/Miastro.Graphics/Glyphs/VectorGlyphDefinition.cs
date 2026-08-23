namespace Miastro.Graphics.Glyphs;

public sealed record VectorGlyphDefinition(
    string Key,
    IReadOnlyList<VectorGlyphStroke> Strokes,
    IReadOnlyList<VectorGlyphCircle> Circles)
{
    public bool IsEmpty =>
        Strokes.Count == 0
        && Circles.Count == 0;
}

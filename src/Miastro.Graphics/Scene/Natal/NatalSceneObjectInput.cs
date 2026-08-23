using Miastro.Graphics.Scene;

namespace Miastro.Graphics.Scene.Natal;

public sealed record NatalSceneObjectInput
{
    public NatalSceneObjectInput(
        string id,
        string glyphKey,
        SceneLayer layer)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException(
                "Object Id is required.",
                nameof(id));
        }

        if (string.IsNullOrWhiteSpace(glyphKey))
        {
            throw new ArgumentException(
                "Glyph key is required.",
                nameof(glyphKey));
        }

        if (layer is not SceneLayer.BodyLayer
            and not SceneLayer.PointLayer)
        {
            throw new ArgumentOutOfRangeException(
                nameof(layer),
                "Natal objects must use BodyLayer or PointLayer.");
        }

        Id = id.Trim();
        GlyphKey = glyphKey.Trim();
        Layer = layer;
    }

    public string Id { get; }

    public string GlyphKey { get; }

    public SceneLayer Layer { get; }
}

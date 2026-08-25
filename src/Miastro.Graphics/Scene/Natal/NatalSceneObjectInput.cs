using Miastro.Graphics.Scene;

namespace Miastro.Graphics.Scene.Natal;

public sealed record NatalSceneObjectInput
{
    public NatalSceneObjectInput(
        string id,
        string glyphKey,
        SceneLayer layer,
        string? labelText = null,
        bool isRetrograde = false)
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

        Id =
            id.Trim();

        GlyphKey =
            glyphKey.Trim();

        Layer =
            layer;

        LabelText =
            string.IsNullOrWhiteSpace(
                labelText)
                ? Id
                : labelText.Trim();

        IsRetrograde =
            isRetrograde;
    }

    public string Id { get; }

    public string GlyphKey { get; }

    public SceneLayer Layer { get; }

    public string LabelText { get; }

    public bool IsRetrograde { get; }
}

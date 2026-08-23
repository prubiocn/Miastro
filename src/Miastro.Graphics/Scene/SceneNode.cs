namespace Miastro.Graphics.Scene;

public abstract record SceneNode
{
    protected SceneNode(
        string id,
        SceneLayer layer)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException(
                "Scene node Id is required.",
                nameof(id));
        }

        Id = id.Trim();
        Layer = layer;
    }

    public string Id { get; }

    public SceneLayer Layer { get; }

    public string StyleKey { get; init; } =
        string.Empty;
}

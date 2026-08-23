namespace Miastro.Graphics.Scene;

public sealed record GroupNode : SceneNode
{
    public GroupNode(
        string id,
        SceneLayer layer,
        IReadOnlyList<SceneNode> children)
        : base(id, layer)
    {
        Children = children;
    }

    public IReadOnlyList<SceneNode> Children { get; }
}

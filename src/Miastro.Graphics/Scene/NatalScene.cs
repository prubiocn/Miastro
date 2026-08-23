namespace Miastro.Graphics.Scene;

public sealed record NatalScene(
    double Width,
    double Height,
    IReadOnlyList<SceneNode> Nodes)
{
    public IReadOnlyList<SceneNode> OrderedNodes =>
        Nodes
            .OrderBy(x => (int)x.Layer)
            .ThenBy(
                x => x.Id,
                StringComparer.Ordinal)
            .ToArray();
}

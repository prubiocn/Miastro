namespace Miastro.Graphics.Scene.Natal.Aspects;

/// <summary>
/// Renderer-facing aspect input.
///
/// This type does not determine whether an aspect exists.
/// It only represents an aspect already supplied by upstream data.
/// </summary>
public sealed record NatalAspectSceneInput
{
    public NatalAspectSceneInput(
        string id,
        string firstObjectId,
        string secondObjectId,
        NatalAspectVisualClass visualClass)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException(
                "Aspect Id is required.",
                nameof(id));
        }

        if (string.IsNullOrWhiteSpace(firstObjectId))
        {
            throw new ArgumentException(
                "First object Id is required.",
                nameof(firstObjectId));
        }

        if (string.IsNullOrWhiteSpace(secondObjectId))
        {
            throw new ArgumentException(
                "Second object Id is required.",
                nameof(secondObjectId));
        }

        if (string.Equals(
            firstObjectId,
            secondObjectId,
            StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "An aspect requires two different objects.");
        }

        Id = id.Trim();
        FirstObjectId = firstObjectId.Trim();
        SecondObjectId = secondObjectId.Trim();
        VisualClass = visualClass;
    }

    public string Id { get; }

    public string FirstObjectId { get; }

    public string SecondObjectId { get; }

    public NatalAspectVisualClass VisualClass { get; }
}

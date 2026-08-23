namespace Miastro.Graphics.Layout.Placement;

public sealed record NatalObjectLayoutInput(
    string Id,
    double RealLongitudeDegrees)
{
    public string Id { get; } =
        string.IsNullOrWhiteSpace(Id)
            ? throw new ArgumentException(
                "Object Id is required.",
                nameof(Id))
            : Id.Trim();
}

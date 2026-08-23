using System.Globalization;

namespace Miastro.Graphics.Layout.Placement;

public sealed record NatalObjectPlacementSnapshot(
    IReadOnlyList<NatalVisualPlacement> Placements)
{
    public string ToDiagnosticText()
    {
        var culture =
            CultureInfo.InvariantCulture;

        return string.Join(
            "\n",
            Placements.Select(
                placement =>
                    string.Format(
                        culture,
                        "OBJECT|{0}|REAL={1:F9}|REALANGLE={2:F9}|VISUALANGLE={3:F9}|LEVEL={4}|RADIUS={5:F9}|X={6:F9}|Y={7:F9}|LEADER={8}",
                        placement.Id,
                        placement.RealLongitudeDegrees,
                        placement.RealScreenAngleDegrees,
                        placement.VisualScreenAngleDegrees,
                        placement.RadialLevel,
                        placement.VisualRadius,
                        placement.VisualCenter.X,
                        placement.VisualCenter.Y,
                        placement.HasLeaderLine
                            ? "1"
                            : "0")));
    }
}

using System.Globalization;

namespace Miastro.Graphics.Layout;

public sealed record NatalWheelLayoutSnapshot(
    NatalWheelMetrics Metrics,
    double AscendantLongitudeDegrees,
    double MidheavenLongitudeDegrees,
    IReadOnlyList<ZodiacSectorLayout> ZodiacSectors,
    IReadOnlyList<DegreeTickLayout> DegreeTicks,
    IReadOnlyList<HouseCuspLayout> HouseCusps,
    IReadOnlyList<AngleAxisLayout> AngleAxes)
{
    public string ToDiagnosticText()
    {
        var culture =
            CultureInfo.InvariantCulture;

        var lines =
            new List<string>
            {
                FormattableString.Invariant(
                    $"SIZE|{Metrics.Width:F6}|{Metrics.Height:F6}|{Metrics.Scale:F9}"),

                FormattableString.Invariant(
                    $"ASC|{AscendantLongitudeDegrees:F9}"),

                FormattableString.Invariant(
                    $"MC|{MidheavenLongitudeDegrees:F9}")
            };

        foreach (var sector in ZodiacSectors)
        {
            lines.Add(
                string.Format(
                    culture,
                    "SIGN|{0}|{1:F9}|{2:F9}|{3:F9}|{4:F9}",
                    sector.SignIndex,
                    sector.StartLongitudeDegrees,
                    sector.CenterLongitudeDegrees,
                    sector.StartScreenAngleDegrees,
                    sector.SweepAngleDegrees));
        }

        foreach (var cusp in HouseCusps)
        {
            lines.Add(
                string.Format(
                    culture,
                    "HOUSE|{0}|{1:F9}|{2:F9}|{3:F9}",
                    cusp.HouseNumber,
                    cusp.RealLongitudeDegrees,
                    cusp.ScreenAngleDegrees,
                    cusp.HouseCenterLongitudeDegrees));
        }

        foreach (var axis in AngleAxes)
        {
            lines.Add(
                string.Format(
                    culture,
                    "AXIS|{0}|{1:F9}|{2:F9}",
                    axis.Kind,
                    axis.RealLongitudeDegrees,
                    axis.ScreenAngleDegrees));
        }

        return string.Join(
            "\n",
            lines);
    }
}

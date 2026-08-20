using Miastro.Domain.Angles;

namespace Miastro.Domain.DerivedPoints;

public static class PartOfFortuneCalculator
{
    public static EclipticLongitude Calculate(
        EclipticLongitude ascendant,
        EclipticLongitude sun,
        EclipticLongitude moon,
        ChartSect sect)
    {
        var value = sect switch
        {
            ChartSect.Day =>
                ascendant.Degrees
                + moon.Degrees
                - sun.Degrees,

            ChartSect.Night =>
                ascendant.Degrees
                + sun.Degrees
                - moon.Degrees,

            _ =>
                throw new ArgumentOutOfRangeException(nameof(sect))
        };

        return EclipticLongitude.FromDegrees(value);
    }
}

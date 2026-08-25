using Miastro.Graphics.Geometry;

namespace Miastro.Graphics.Layout;

public sealed record NatalWheelMetrics(
    double Width,
    double Height,
    double Scale,
    ChartPoint Center,
    double OuterRadius,
    double ZodiacInnerRadius,
    double DegreeInnerRadius,
    double HouseOuterRadius,
    double HouseInnerRadius,
    double AspectRadius)
{
    // Primera circunferencia:
    // borde exterior del zodiaco.
    public double ZodiacOuterRingRadius =>
        OuterRadius;

    // Segunda circunferencia:
    // graduación zodiacal.
    public double DegreeRingRadius =>
        ZodiacInnerRadius;

    // Banda planetaria:
    // espacio abierto entre grados y aspectos.
    public double PlanetBandOuterRadius =>
        DegreeRingRadius;

    public double PlanetBandInnerRadius =>
        AspectRadius;

    // Radio base de colocación de los glifos.
    public double PlanetOrbitRadius =>
        OuterRadius
        * 0.70;

    // Cuarta circunferencia:
    // alma.
    public double SoulRadius =>
        OuterRadius
        * 0.12;

    public const double ReferenceSize = 800.0;

    public const double MinimumUsableSize = 360.0;

    public static NatalWheelMetrics Create(
        double width,
        double height)
    {
        if (!double.IsFinite(width)
            || !double.IsFinite(height)
            || width <= 0.0
            || height <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(width));
        }

        var size =
            Math.Min(
                width,
                height);

        var effectiveSize =
            size;

        var scale =
            effectiveSize
            / ReferenceSize;

        var outerRadius =
            effectiveSize
            * 0.46;

        return new NatalWheelMetrics(
            width,
            height,
            scale,
            new ChartPoint(
                width / 2.0,
                height / 2.0),
            outerRadius,
            // Métricas auxiliares del zodiaco/grados.
            outerRadius * 0.88,
            outerRadius * 0.81,

            // Segunda circunferencia: planetas/casas.
            outerRadius * 0.70,

            // Tercera circunferencia: aspectos.
            outerRadius * 0.48,
            outerRadius * 0.48);
    }
}

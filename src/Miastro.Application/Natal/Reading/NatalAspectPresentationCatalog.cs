using Miastro.Domain.Aspects;

namespace Miastro.Application.Natal.Reading;

public static class NatalAspectPresentationCatalog
{
    public static string Name(
        AspectKind kind)
        => kind switch
        {
            AspectKind.Conjunction =>
                "Conjunción",

            AspectKind.Opposition =>
                "Oposición",

            AspectKind.Square =>
                "Cuadratura",

            AspectKind.Trine =>
                "Trígono",

            AspectKind.Sextile =>
                "Sextil",

            AspectKind.Quincunx =>
                "Quincuncio",

            AspectKind.Semisextile =>
                "Semisextil",

            AspectKind.Quintile =>
                "Quintil",

            AspectKind.Biquintile =>
                "Biquintil",

            _ =>
                kind.ToString()
        };

    public static string Symbol(
        AspectKind kind)
        => kind switch
        {
            AspectKind.Conjunction =>
                "☌",

            AspectKind.Opposition =>
                "☍",

            AspectKind.Square =>
                "□",

            AspectKind.Trine =>
                "△",

            AspectKind.Sextile =>
                "✶",

            AspectKind.Quincunx =>
                "⚻",

            AspectKind.Semisextile =>
                "⚺",

            AspectKind.Quintile =>
                "Q",

            AspectKind.Biquintile =>
                "bQ",

            _ =>
                "·"
        };

    public static int Order(
        AspectKind kind)
        => kind switch
        {
            AspectKind.Conjunction => 0,
            AspectKind.Opposition => 1,
            AspectKind.Square => 2,
            AspectKind.Trine => 3,
            AspectKind.Sextile => 4,
            AspectKind.Quincunx => 5,
            AspectKind.Semisextile => 6,
            AspectKind.Quintile => 7,
            AspectKind.Biquintile => 8,
            _ => int.MaxValue
        };
}

using Miastro.Domain.Objects;
using Miastro.Domain.Zodiac;

namespace Miastro.Domain.Rulerships;

public sealed record Rulership
{
    public ZodiacSign Sign { get; }

    public AstrologicalObjectId Traditional { get; }

    public AstrologicalObjectId? Modern { get; }

    public Rulership(
        ZodiacSign sign,
        AstrologicalObjectId traditional,
        AstrologicalObjectId? modern = null)
    {
        ValidateRuler(traditional);

        if (modern is not null)
        {
            ValidateRuler(modern.Value);
        }

        Sign = sign;
        Traditional = traditional;
        Modern = modern;
    }

    public IReadOnlyList<AstrologicalObjectId> Both =>
        Modern is null
            ? [Traditional]
            : [Traditional, Modern.Value];

    private static void ValidateRuler(
        AstrologicalObjectId ruler)
    {
        var category =
            AstrologicalObjectCatalog.GetCategory(ruler);

        if (category is not
            AstrologicalObjectCategory.Planet
            and not AstrologicalObjectCategory.Luminary)
        {
            throw new ArgumentException(
                "Una regencia V1 debe utilizar un planeta o luminar.",
                nameof(ruler));
        }
    }
}

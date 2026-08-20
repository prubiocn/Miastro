using Miastro.Domain.Aspects;
using Miastro.Domain.Calculation;
using Miastro.Domain.Houses;
using Miastro.Domain.Placements;

namespace Miastro.Domain.Charts;

public sealed class AstrologicalChart
{
    private readonly IReadOnlyList<AstrologicalPlacement> _placements;
    private readonly IReadOnlyList<HouseCusp> _houseCusps;

    public Guid Id { get; }

    public ChartType Type { get; }

    public IReadOnlyList<AstrologicalPlacement> Placements =>
        _placements;

    public IReadOnlyList<HouseCusp> HouseCusps =>
        _houseCusps;

    public HouseSystem? HouseSystem { get; }

    public CalculationMetadata Metadata { get; }

    public CalculationProfile CalculationProfile { get; }

    public AspectProfile AspectProfile { get; }

    public AstrologicalChart(
        Guid id,
        ChartType type,
        IEnumerable<AstrologicalPlacement> placements,
        CalculationProfile calculationProfile,
        AspectProfile aspectProfile,
        CalculationMetadata metadata,
        IEnumerable<HouseCusp>? houseCusps = null,
        HouseSystem? houseSystem = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "El identificador de carta no puede estar vacío.",
                nameof(id));
        }

        ArgumentNullException.ThrowIfNull(placements);
        ArgumentNullException.ThrowIfNull(calculationProfile);
        ArgumentNullException.ThrowIfNull(aspectProfile);
        ArgumentNullException.ThrowIfNull(metadata);

        var placementArray = placements.ToArray();

        if (placementArray
            .GroupBy(x => x.ObjectId)
            .Any(group => group.Count() > 1))
        {
            throw new ArgumentException(
                "Una carta no puede contener el mismo objeto dos veces.",
                nameof(placements));
        }

        var cuspArray =
            houseCusps?.ToArray() ??
            [];

        if (cuspArray.Length > 0)
        {
            if (houseSystem is null)
            {
                throw new ArgumentException(
                    "Las cúspides requieren un sistema de casas.",
                    nameof(houseSystem));
            }

            if (cuspArray.Length != 12)
            {
                throw new ArgumentException(
                    "Un conjunto de cúspides debe contener exactamente 12 casas.",
                    nameof(houseCusps));
            }

            var houseNumbers = cuspArray
                .Select(x => x.House.Number)
                .Order()
                .ToArray();

            if (!houseNumbers.SequenceEqual(
                Enumerable.Range(1, 12)))
            {
                throw new ArgumentException(
                    "Las cúspides deben contener las casas 1 a 12 exactamente una vez.",
                    nameof(houseCusps));
            }
        }

        Id = id;
        Type = type;
        _placements = Array.AsReadOnly(placementArray);
        _houseCusps = Array.AsReadOnly(cuspArray);
        HouseSystem = houseSystem;
        Metadata = metadata;
        CalculationProfile = calculationProfile;
        AspectProfile = aspectProfile;
    }
}

using Miastro.Astronomy.Abstractions.Errors;
using Miastro.Domain.Angles;
using Miastro.Domain.Charts;
using Miastro.Domain.Houses;

namespace Miastro.Astronomy.Abstractions.Models;

public sealed record HouseCalculationResult
{
    public bool Success { get; }

    public HouseSystem HouseSystem { get; }

    public IReadOnlyList<HouseCusp> Cusps { get; }

    public EclipticLongitude? Ascendant { get; }

    public EclipticLongitude? Midheaven { get; }

    public EclipticLongitude? Descendant =>
        Ascendant is null
            ? null
            : Ascendant.Value +
              Angle.FromDegrees(180.0);

    public EclipticLongitude? ImumCoeli =>
        Midheaven is null
            ? null
            : Midheaven.Value +
              Angle.FromDegrees(180.0);

    public GeographicLocation Location { get; }

    public AstronomicalInstant Instant { get; }

    public AstronomyEngineMetadata? EngineMetadata { get; }

    public AstronomyError? Error { get; }

    private HouseCalculationResult(
        bool success,
        HouseSystem houseSystem,
        IReadOnlyList<HouseCusp> cusps,
        EclipticLongitude? ascendant,
        EclipticLongitude? midheaven,
        GeographicLocation location,
        AstronomicalInstant instant,
        AstronomyEngineMetadata? engineMetadata,
        AstronomyError? error)
    {
        Success = success;
        HouseSystem = houseSystem;
        Cusps = cusps;
        Ascendant = ascendant;
        Midheaven = midheaven;
        Location = location;
        Instant = instant;
        EngineMetadata = engineMetadata;
        Error = error;
    }

    public static HouseCalculationResult Succeeded(
        HouseSystem houseSystem,
        IReadOnlyList<HouseCusp> cusps,
        EclipticLongitude ascendant,
        EclipticLongitude midheaven,
        GeographicLocation location,
        AstronomicalInstant instant,
        AstronomyEngineMetadata engineMetadata)
    {
        ArgumentNullException.ThrowIfNull(cusps);
        ArgumentNullException.ThrowIfNull(engineMetadata);

        if (cusps.Count != 12)
        {
            throw new ArgumentException(
                "El resultado correcto debe contener 12 cúspides.",
                nameof(cusps));
        }

        return new(
            true,
            houseSystem,
            cusps,
            ascendant,
            midheaven,
            location,
            instant,
            engineMetadata,
            null);
    }

    public static HouseCalculationResult Failed(
        HouseSystem houseSystem,
        GeographicLocation location,
        AstronomicalInstant instant,
        AstronomyError error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new(
            false,
            houseSystem,
            [],
            null,
            null,
            location,
            instant,
            null,
            error);
    }
}

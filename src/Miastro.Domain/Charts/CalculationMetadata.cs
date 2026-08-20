using Miastro.Domain.Houses;

namespace Miastro.Domain.Charts;

public sealed record CalculationMetadata
{
    public string? MiastroVersion { get; }
    public string? CalculationProfileId { get; }
    public string? Engine { get; }
    public string? EngineVersion { get; }
    public string? EphemerisVersion { get; }
    public string? TzdbVersion { get; }
    public HouseSystem? HouseSystem { get; }

    public CalculationMetadata(
        string? miastroVersion = null,
        string? calculationProfileId = null,
        string? engine = null,
        string? engineVersion = null,
        string? ephemerisVersion = null,
        string? tzdbVersion = null,
        HouseSystem? houseSystem = null)
    {
        MiastroVersion = Normalize(miastroVersion, nameof(miastroVersion));
        CalculationProfileId = Normalize(
            calculationProfileId,
            nameof(calculationProfileId));
        Engine = Normalize(engine, nameof(engine));
        EngineVersion = Normalize(
            engineVersion,
            nameof(engineVersion));
        EphemerisVersion = Normalize(
            ephemerisVersion,
            nameof(ephemerisVersion));
        TzdbVersion = Normalize(tzdbVersion, nameof(tzdbVersion));
        HouseSystem = houseSystem;
    }

    private static string? Normalize(
        string? value,
        string parameterName)
    {
        if (value is null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "El valor no puede estar vacío.",
                parameterName);
        }

        return value.Trim();
    }
}

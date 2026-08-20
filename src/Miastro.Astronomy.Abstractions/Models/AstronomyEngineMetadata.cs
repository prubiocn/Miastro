namespace Miastro.Astronomy.Abstractions.Models;

public sealed record AstronomyEngineMetadata
{
    public string Engine { get; }

    public string EngineVersion { get; }

    public string AdapterVersion { get; }

    public string Architecture { get; }

    public AstronomyEngineMetadata(
        string engine,
        string engineVersion,
        string adapterVersion,
        string architecture)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(engine);
        ArgumentException.ThrowIfNullOrWhiteSpace(engineVersion);
        ArgumentException.ThrowIfNullOrWhiteSpace(adapterVersion);
        ArgumentException.ThrowIfNullOrWhiteSpace(architecture);

        Engine = engine;
        EngineVersion = engineVersion;
        AdapterVersion = adapterVersion;
        Architecture = architecture;
    }
}

namespace Miastro.Application.Natal;

public interface INatalCalculationMetadataProvider
{
    NatalCalculationEnvironment Get();
}

public sealed record NatalCalculationEnvironment(
    string MiastroVersion,
    string Engine,
    string EngineVersion,
    string AdapterVersion,
    string EphemerisVersion);

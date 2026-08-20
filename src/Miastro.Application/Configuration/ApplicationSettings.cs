namespace Miastro.Application.Configuration;

public sealed record ApplicationSettings
{
    public int SchemaVersion { get; init; } = 1;
    public string Language { get; init; } = "es-ES";
}

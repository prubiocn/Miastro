namespace Miastro.Infrastructure.Persistence.Entities;

internal sealed class TechnicalProbe
{
    public int Id { get; set; }

    public string Value { get; set; } = string.Empty;

    public DateTimeOffset CreatedUtc { get; set; }
}

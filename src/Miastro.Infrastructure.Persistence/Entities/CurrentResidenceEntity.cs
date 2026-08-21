namespace Miastro.Infrastructure.Persistence.Entities;

public sealed class CurrentResidenceEntity
{
    public Guid PersonId { get; set; }
    public PersonEntity Person { get; set; } = null!;

    public string Locality { get; set; } = string.Empty;
    public long? GeoNameId { get; set; }
    public string Region { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string IanaTimeZoneId { get; set; } = string.Empty;

    public DateTimeOffset? UpdatedAtUtc { get; set; }
}

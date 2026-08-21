using Miastro.Domain.Geography;

namespace Miastro.Domain.People;

public sealed class CurrentResidence
{
    public string Locality { get; }
    public long? GeoNameId { get; }
    public string Region { get; }
    public string Country { get; }
    public Latitude Latitude { get; }
    public Longitude Longitude { get; }
    public IanaTimeZoneId TimeZoneId { get; }
    public DateTimeOffset? UpdatedAtUtc { get; }

    public CurrentResidence(
        string locality,
        long? geoNameId,
        string region,
        string country,
        Latitude latitude,
        Longitude longitude,
        IanaTimeZoneId timeZoneId,
        DateTimeOffset? updatedAtUtc)
    {
        Locality = Require(locality, nameof(locality), 200);
        GeoNameId = geoNameId;
        Region = Require(region, nameof(region), 160);
        Country = Require(country, nameof(country), 120);
        Latitude = latitude;
        Longitude = longitude;
        TimeZoneId = timeZoneId;
        UpdatedAtUtc = updatedAtUtc?.ToUniversalTime();
    }

    private static string Require(
        string value,
        string parameter,
        int maxLength)
    {
        var normalized = value?.Trim()
            ?? throw new ArgumentNullException(parameter);

        if (normalized.Length == 0 || normalized.Length > maxLength)
        {
            throw new ArgumentException(
                "Invalid value.",
                parameter);
        }

        return normalized;
    }
}

namespace Miastro.Astronomy.Abstractions.Models;

public readonly record struct AstronomicalInstant
{
    public DateTimeOffset Utc { get; }

    private AstronomicalInstant(DateTimeOffset utc)
    {
        Utc = utc.ToUniversalTime();
    }

    public static AstronomicalInstant FromUtc(
        DateTimeOffset instant) =>
        new(instant);
}

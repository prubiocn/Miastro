using Miastro.Astronomy.Abstractions.Models;
using Miastro.Application.Time;
using Miastro.Domain.Geography;
using Miastro.Infrastructure.Time.Historical;
using NodaTime;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase4AstronomyBoundaryTests
{
    [TestMethod]
    public void HistoricalInstant_CanReachAstronomyBoundaryWithoutSwissDependency()
    {
        var resolver = new NodaTimeHistoricalTimeResolver();

        var result = resolver.Resolve(
            new LocalDateTime(2024, 1, 15, 12, 0),
            new IanaTimeZoneId("Europe/Madrid"));

        Assert.AreEqual(
            HistoricalTimeResolutionStatus.Resolved,
            result.Status);

        var nodaInstant =
            result.SingleCandidate!.Instant;

        var utcDateTime =
            nodaInstant.ToDateTimeUtc();

        var dto =
            new DateTimeOffset(
                DateTime.SpecifyKind(
                    utcDateTime,
                    DateTimeKind.Utc));

        Assert.AreEqual(
            TimeSpan.Zero,
            dto.Offset);

        // La prueba valida la conversión exacta al tipo temporal .NET
        // utilizado en la frontera de Astronomy.Abstractions, sin llamar
        // al adaptador Swiss ni construir una Carta Natal.
        Assert.AreEqual(
            new DateTimeOffset(
                2024, 1, 15, 11, 0, 0,
                TimeSpan.Zero),
            dto);

        Assert.IsNotNull(
            typeof(AstronomicalInstant));
    }
}

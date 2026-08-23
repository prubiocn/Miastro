using Microsoft.Extensions.DependencyInjection;
using Miastro.Astronomy.Abstractions.Contracts;
using Miastro.Astronomy.Abstractions.Models;
using Miastro.Bootstrap;
using Miastro.Domain.Calculation;
using Miastro.Domain.Objects;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6SwissNatalBodiesTests
{
    [TestMethod]
    public void Swiss_calculates_all_required_natal_bodies()
    {
        var services =
            MiastroBootstrap
                .CreateServiceCollection();

        using var provider =
            services.BuildServiceProvider();

        var calculator =
            provider.GetRequiredService<
                IEclipticPositionCalculator>();

        var instant =
            AstronomicalInstant.FromUtc(
                new DateTimeOffset(
                    2000, 1, 1,
                    11, 0, 0,
                    TimeSpan.Zero));

        var objects =
            new[]
            {
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.Moon,
                AstrologicalObjectId.Mercury,
                AstrologicalObjectId.Venus,
                AstrologicalObjectId.Mars,
                AstrologicalObjectId.Jupiter,
                AstrologicalObjectId.Saturn,
                AstrologicalObjectId.Uranus,
                AstrologicalObjectId.Neptune,
                AstrologicalObjectId.Pluto,
                AstrologicalObjectId.NorthTrueNode,
                AstrologicalObjectId.MeanLilith,
                AstrologicalObjectId.Chiron,
                AstrologicalObjectId.Ceres,
                AstrologicalObjectId.Pallas,
                AstrologicalObjectId.Juno,
                AstrologicalObjectId.Vesta
            };

        foreach (var objectId in objects)
        {
            var position =
                calculator.Calculate(
                    objectId,
                    instant,
                    CalculationProfile.MiastroV1);

            Assert.AreEqual(
                objectId,
                position.ObjectId);

            Assert.IsTrue(
                double.IsFinite(
                    position.Longitude.Degrees));

            Assert.IsTrue(
                double.IsFinite(
                    position.LatitudeDegrees));

            Assert.IsTrue(
                double.IsFinite(
                    position.DistanceAu));

            Assert.IsTrue(
                double.IsFinite(
                    position.LongitudeSpeedDegreesPerDay));

            Assert.IsTrue(
                double.IsFinite(
                    position.LatitudeSpeedDegreesPerDay));

            Assert.IsTrue(
                double.IsFinite(
                    position.DistanceSpeedAuPerDay));
        }
    }
}

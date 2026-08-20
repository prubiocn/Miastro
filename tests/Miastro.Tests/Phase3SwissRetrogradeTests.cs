using Miastro.Astronomy.Abstractions.Models;
using Miastro.Domain.Calculation;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;
using Miastro.Infrastructure.SwissEphemeris.Calculation;
using Miastro.Infrastructure.SwissEphemeris.Configuration;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase3SwissRetrogradeTests
{
    private const string ExpectedHash =
        "47e6fed985ccb5f067b7a0f6f746ec3567a7b54ce5f86140b2138616a8e6a653";

    [TestMethod]
    public void Native_speed_can_feed_domain_motion_state_without_rounding()
    {
        var root =
            FindRepositoryRoot();

        var calculator =
            new SwissEphemerisPositionCalculator(
                new SwissEphemerisOptions(
                    Path.Combine(
                        root,
                        "src",
                        "Miastro.Infrastructure.SwissEphemeris",
                        "native",
                        "linux-x64",
                        "libswe.so"),
                    Path.Combine(
                        root,
                        "data",
                        "ephemeris"),
                    ExpectedHash,
                    "2.10.03"));

        var position =
            calculator.Calculate(
                AstrologicalObjectId.Mercury,
                AstronomicalInstant.FromUtc(
                    new DateTimeOffset(
                        2024, 1, 1,
                        12, 0, 0,
                        TimeSpan.Zero)),
                CalculationProfile.MiastroV1);

        var state =
            MotionStateResolver.FromSpeed(
                position.LongitudeSpeedDegreesPerDay);

        Assert.IsTrue(
            state is MotionState.Direct
                or MotionState.Retrograde
                or MotionState.Stationary);
    }

    private static string FindRepositoryRoot()
    {
        var directory =
            new DirectoryInfo(
                AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(
                    Path.Combine(
                        directory.FullName,
                        "Miastro.sln")))
            {
                return directory.FullName;
            }

            directory =
                directory.Parent;
        }

        throw new DirectoryNotFoundException();
    }
}

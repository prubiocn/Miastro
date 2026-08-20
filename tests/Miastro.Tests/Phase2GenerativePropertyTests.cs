using Miastro.Domain.Angles;
using Miastro.Domain.Aspects;
using Miastro.Domain.DerivedPoints;
using Miastro.Domain.Objects;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase2GenerativePropertyTests
{
    private const int Seed = 20260820;

    [TestMethod]
    public void Generated_longitudes_always_normalize_to_valid_range()
    {
        var random = new Random(Seed);

        for (var i = 0; i < 10_000; i++)
        {
            var raw =
                (random.NextDouble() - 0.5) *
                2_000_000.0;

            var longitude =
                EclipticLongitude.FromDegrees(raw);

            Assert.IsGreaterThanOrEqualTo(
                0.0,
                longitude.Degrees);

            Assert.IsLessThan(
                360.0,
                longitude.Degrees);
        }
    }

    [TestMethod]
    public void Angular_separation_is_symmetric_and_bounded()
    {
        var random = new Random(Seed);

        for (var i = 0; i < 10_000; i++)
        {
            var first =
                EclipticLongitude.FromDegrees(
                    random.NextDouble() * 360.0);

            var second =
                EclipticLongitude.FromDegrees(
                    random.NextDouble() * 360.0);

            var ab =
                AngularSeparation.Between(
                    first,
                    second);

            var ba =
                AngularSeparation.Between(
                    second,
                    first);

            Assert.AreEqual(
                ab.Degrees,
                ba.Degrees,
                1e-12);

            Assert.IsGreaterThanOrEqualTo(
                0.0,
                ab.Degrees);

            Assert.IsLessThanOrEqualTo(
                180.0,
                ab.Degrees);
        }
    }

    [TestMethod]
    public void South_node_is_always_exactly_opposite()
    {
        var random = new Random(Seed);

        for (var i = 0; i < 10_000; i++)
        {
            var north =
                EclipticLongitude.FromDegrees(
                    random.NextDouble() * 360.0);

            var south =
                LunarNodeCalculator.CalculateSouthNode(
                    north);

            var separation =
                AngularSeparation.Between(
                    north,
                    south);

            Assert.AreEqual(
                180.0,
                separation.Degrees,
                1e-10);
        }
    }

    [TestMethod]
    public void Aspect_detection_is_stable_for_generated_inputs()
    {
        var random = new Random(Seed);
        var profile =
            MiastroV1AspectProfile.Instance;

        for (var i = 0; i < 5_000; i++)
        {
            var first =
                EclipticLongitude.FromDegrees(
                    random.NextDouble() * 360.0);

            var second =
                EclipticLongitude.FromDegrees(
                    random.NextDouble() * 360.0);

            var a = AspectEngine.Detect(
                AstrologicalObjectId.Mars,
                first,
                AstrologicalObjectId.Jupiter,
                second,
                profile);

            var b = AspectEngine.Detect(
                AstrologicalObjectId.Mars,
                first,
                AstrologicalObjectId.Jupiter,
                second,
                profile);

            Assert.AreEqual(a, b);
        }
    }
}

using Miastro.Domain.Angles;
using Miastro.Domain.Aspects;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6NatalAspectCalculationTests
{
    [TestMethod]
    public void V1_excludes_nodes_lilith_and_fortune()
    {
        var placements =
            new[]
            {
                Placement(
                    AstrologicalObjectId.Sun,
                    0.0),

                Placement(
                    AstrologicalObjectId.Moon,
                    60.0),

                Placement(
                    AstrologicalObjectId.NorthTrueNode,
                    0.0),

                Placement(
                    AstrologicalObjectId.SouthNode,
                    180.0),

                Placement(
                    AstrologicalObjectId.MeanLilith,
                    90.0),

                Placement(
                    AstrologicalObjectId.PartOfFortune,
                    120.0)
            };

        var aspects =
            NatalAspectCalculator.Calculate(
                placements);

        Assert.AreEqual(
            1,
            aspects.Count);

        Assert.AreEqual(
            AstrologicalObjectId.Sun,
            aspects[0].FirstObject);

        Assert.AreEqual(
            AstrologicalObjectId.Moon,
            aspects[0].SecondObject);

        Assert.AreEqual(
            AspectKind.Sextile,
            aspects[0].Definition.Kind);
    }

    [TestMethod]
    public void Aspect_order_is_canonical()
    {
        var placements =
            new[]
            {
                Placement(
                    AstrologicalObjectId.Mars,
                    90.0),

                Placement(
                    AstrologicalObjectId.Sun,
                    0.0),

                Placement(
                    AstrologicalObjectId.Moon,
                    180.0)
            };

        var aspects =
            NatalAspectCalculator.Calculate(
                placements);

        Assert.IsTrue(
            aspects.Count >= 2);

        Assert.AreEqual(
            AstrologicalObjectId.Sun,
            aspects[0].FirstObject);
    }

    private static AstrologicalPlacement Placement(
        AstrologicalObjectId objectId,
        double longitude)
        => new(
            objectId,
            EclipticLongitude
                .FromDegrees(longitude),
            speedDegreesPerDay: 1.0);
}

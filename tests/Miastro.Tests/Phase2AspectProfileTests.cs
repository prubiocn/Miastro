using Miastro.Domain.Aspects;
using Miastro.Domain.Objects;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase2AspectProfileTests
{
    private static AspectProfile Profile =>
        MiastroV1AspectProfile.Instance;

    [TestMethod]
    [DataRow(AspectKind.Conjunction, 0.0, 8.0)]
    [DataRow(AspectKind.Semisextile, 30.0, 2.0)]
    [DataRow(AspectKind.Sextile, 60.0, 4.0)]
    [DataRow(AspectKind.Square, 90.0, 6.0)]
    [DataRow(AspectKind.Trine, 120.0, 6.0)]
    [DataRow(AspectKind.Quincunx, 150.0, 3.0)]
    [DataRow(AspectKind.Opposition, 180.0, 8.0)]
    [DataRow(AspectKind.Quintile, 72.0, 2.0)]
    [DataRow(AspectKind.Biquintile, 144.0, 2.0)]
    public void Canonical_aspect_definitions_are_correct(
        AspectKind kind,
        double exact,
        double baseOrb)
    {
        var definition =
            Profile.Aspects.Single(x => x.Kind == kind);

        Assert.AreEqual(
            exact,
            definition.ExactAngleDegrees,
            1e-12);

        Assert.AreEqual(
            baseOrb,
            definition.BaseOrbDegrees,
            1e-12);
    }

    [TestMethod]
    public void Luminary_bonus_is_added_only_once()
    {
        var conjunction =
            Profile.Aspects.Single(
                x => x.Kind == AspectKind.Conjunction);

        Assert.AreEqual(
            8.0,
            Profile.GetAllowedOrb(
                conjunction,
                AstrologicalObjectId.Mercury,
                AstrologicalObjectId.Venus),
            1e-12);

        Assert.AreEqual(
            9.0,
            Profile.GetAllowedOrb(
                conjunction,
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.Venus),
            1e-12);

        Assert.AreEqual(
            9.0,
            Profile.GetAllowedOrb(
                conjunction,
                AstrologicalObjectId.Moon,
                AstrologicalObjectId.Venus),
            1e-12);

        Assert.AreEqual(
            9.0,
            Profile.GetAllowedOrb(
                conjunction,
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.Moon),
            1e-12);
    }

    [TestMethod]
    [DataRow(AstrologicalObjectId.Mercury, true)]
    [DataRow(AstrologicalObjectId.Chiron, true)]
    [DataRow(AstrologicalObjectId.Ascendant, true)]
    [DataRow(AstrologicalObjectId.Midheaven, true)]
    [DataRow(AstrologicalObjectId.NorthTrueNode, false)]
    [DataRow(AstrologicalObjectId.SouthNode, false)]
    [DataRow(AstrologicalObjectId.MeanLilith, false)]
    [DataRow(AstrologicalObjectId.PartOfFortune, false)]
    public void V1_participants_are_correct(
        AstrologicalObjectId objectId,
        bool expected)
    {
        Assert.AreEqual(
            expected,
            Profile.IsParticipant(objectId));
    }
}

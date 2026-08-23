using Miastro.Application.Natal;
using Miastro.Domain.Aspects;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6NatalSemanticIntegrityTests
{
    [TestMethod]
    public void Reversed_aspect_pair_is_rejected_as_duplicate()
    {
        var snapshot =
            Phase6NatalTestSnapshotFactory.Create(
                Guid.NewGuid());

        var first =
            new NatalAspectSnapshot(
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.Moon,
                AspectKind.Sextile,
                60.0,
                60.0,
                0.0,
                5.0,
                0.0);

        var reversed =
            new NatalAspectSnapshot(
                AstrologicalObjectId.Moon,
                AstrologicalObjectId.Sun,
                AspectKind.Sextile,
                60.0,
                60.0,
                0.0,
                5.0,
                0.0);

        Assert.ThrowsExactly<ArgumentException>(
            () =>
                NatalSnapshotValidator.Validate(
                    snapshot with
                    {
                        Aspects =
                        [
                            first,
                            reversed
                        ]
                    }));
    }

    [TestMethod]
    public void Non_v1_aspect_participant_is_rejected()
    {
        var snapshot =
            Phase6NatalTestSnapshotFactory.Create(
                Guid.NewGuid());

        var invalid =
            new NatalAspectSnapshot(
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.NorthTrueNode,
                AspectKind.Conjunction,
                0.0,
                0.0,
                0.0,
                9.0,
                0.0);

        Assert.ThrowsExactly<ArgumentException>(
            () =>
                NatalSnapshotValidator.Validate(
                    snapshot with
                    {
                        Aspects =
                        [
                            invalid
                        ]
                    }));
    }

    [TestMethod]
    public void Aspect_outside_allowed_orb_is_rejected()
    {
        var snapshot =
            Phase6NatalTestSnapshotFactory.Create(
                Guid.NewGuid());

        var invalid =
            new NatalAspectSnapshot(
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.Moon,
                AspectKind.Sextile,
                70.0,
                60.0,
                10.0,
                5.0,
                10.0);

        Assert.ThrowsExactly<ArgumentException>(
            () =>
                NatalSnapshotValidator.Validate(
                    snapshot with
                    {
                        Aspects =
                        [
                            invalid
                        ]
                    }));
    }

    [TestMethod]
    [DataRow(
        0.0,
        MotionState.Stationary)]
    [DataRow(
        0.000000000001,
        MotionState.Direct)]
    [DataRow(
        -0.000000000001,
        MotionState.Retrograde)]
    public void Stationary_policy_uses_exact_zero_only(
        double speed,
        MotionState expected)
    {
        Assert.AreEqual(
            expected,
            MotionStateResolver.FromSpeed(
                speed));
    }
}

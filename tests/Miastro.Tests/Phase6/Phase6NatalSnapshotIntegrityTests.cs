using Miastro.Application.Natal;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;

namespace Miastro.Tests.Phase6;

[TestClass]
public sealed class Phase6NatalSnapshotIntegrityTests
{
    [TestMethod]
    public void Complete_canonical_snapshot_is_valid()
    {
        NatalSnapshotValidator.Validate(
            Phase6NatalTestSnapshotFactory.Create(
                Guid.NewGuid()));
    }

    [TestMethod]
    public void Missing_required_object_is_rejected()
    {
        var snapshot =
            Phase6NatalTestSnapshotFactory.Create(
                Guid.NewGuid());

        var placements =
            snapshot.Placements
                .Where(x =>
                    x.ObjectId !=
                    AstrologicalObjectId.Midheaven)
                .ToArray();

        Assert.ThrowsExactly<
            ArgumentException>(
            () =>
                NatalSnapshotValidator.Validate(
                    snapshot with
                    {
                        Placements =
                            placements
                    }));
    }

    [TestMethod]
    public void Wrong_south_node_is_rejected()
    {
        var snapshot =
            Phase6NatalTestSnapshotFactory.Create(
                Guid.NewGuid());

        var placements =
            snapshot.Placements
                .Select(x =>
                    x.ObjectId ==
                    AstrologicalObjectId.SouthNode
                        ? x with
                        {
                            LongitudeDegrees =
                                Normalize(
                                    x.LongitudeDegrees
                                    + 1.0)
                        }
                        : x)
                .ToArray();

        Assert.ThrowsExactly<
            ArgumentException>(
            () =>
                NatalSnapshotValidator.Validate(
                    snapshot with
                    {
                        Placements =
                            placements
                    }));
    }

    [TestMethod]
    public void Duplicate_house_cusp_is_rejected()
    {
        var snapshot =
            Phase6NatalTestSnapshotFactory.Create(
                Guid.NewGuid());

        var cusps =
            snapshot.HouseCusps
                .ToArray();

        cusps[1] =
            cusps[1] with
            {
                HouseNumber = 1
            };

        Assert.ThrowsExactly<
            ArgumentException>(
            () =>
                NatalSnapshotValidator.Validate(
                    snapshot with
                    {
                        HouseCusps =
                            cusps
                    }));
    }

    [TestMethod]
    public void Wrong_birth_hash_is_rejected()
    {
        var snapshot =
            Phase6NatalTestSnapshotFactory.Create(
                Guid.NewGuid())
            with
            {
                BirthDataHash =
                    new string('0', 64)
            };

        Assert.ThrowsExactly<
            ArgumentException>(
            () =>
                NatalSnapshotValidator.Validate(
                    snapshot));
    }

    private static double Normalize(
        double value)
    {
        var normalized =
            value % 360.0;

        return normalized < 0.0
            ? normalized + 360.0
            : normalized;
    }
}

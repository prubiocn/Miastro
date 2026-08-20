using Miastro.Domain.Angles;
using Miastro.Domain.Aspects;
using Miastro.Domain.Objects;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase2AspectEngineTests
{
    private static AspectProfile Profile =>
        MiastroV1AspectProfile.Instance;

    [TestMethod]
    [DataRow(AspectKind.Conjunction, 0.0)]
    [DataRow(AspectKind.Semisextile, 30.0)]
    [DataRow(AspectKind.Sextile, 60.0)]
    [DataRow(AspectKind.Square, 90.0)]
    [DataRow(AspectKind.Trine, 120.0)]
    [DataRow(AspectKind.Quincunx, 150.0)]
    [DataRow(AspectKind.Opposition, 180.0)]
    [DataRow(AspectKind.Quintile, 72.0)]
    [DataRow(AspectKind.Biquintile, 144.0)]
    public void Detects_all_exact_aspects(
        AspectKind expectedKind,
        double secondLongitude)
    {
        var result = AspectEngine.Detect(
            AstrologicalObjectId.Mercury,
            EclipticLongitude.FromDegrees(0.0),
            AstrologicalObjectId.Venus,
            EclipticLongitude.FromDegrees(secondLongitude),
            Profile);

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedKind, result.Definition.Kind);
        Assert.AreEqual(0.0, result.DeviationDegrees, 1e-12);
        Assert.AreEqual(0.0, result.UsedOrbDegrees, 1e-12);
    }

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
    public void Base_orb_boundary_is_inclusive(
        AspectKind kind,
        double exact,
        double orb)
    {
        var result = AspectEngine.Detect(
            AstrologicalObjectId.Mercury,
            EclipticLongitude.FromDegrees(0.0),
            AstrologicalObjectId.Venus,
            EclipticLongitude.FromDegrees(
                exact == 180.0
                    ? exact - orb
                    : exact + orb),
            Profile);

        Assert.IsNotNull(result);
        Assert.AreEqual(kind, result.Definition.Kind);
        Assert.AreEqual(
            orb,
            result.DeviationDegrees,
            1e-9);
    }

    [TestMethod]
    public void Just_outside_orb_returns_none()
    {
        var result = AspectEngine.Detect(
            AstrologicalObjectId.Mercury,
            EclipticLongitude.FromDegrees(0.0),
            AstrologicalObjectId.Venus,
            EclipticLongitude.FromDegrees(39.0001),
            Profile);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Luminary_extended_orb_is_used()
    {
        var result = AspectEngine.Detect(
            AstrologicalObjectId.Sun,
            EclipticLongitude.FromDegrees(0.0),
            AstrologicalObjectId.Venus,
            EclipticLongitude.FromDegrees(9.0),
            Profile);

        Assert.IsNotNull(result);
        Assert.AreEqual(
            AspectKind.Conjunction,
            result.Definition.Kind);

        Assert.AreEqual(
            9.0,
            result.AllowedOrbDegrees,
            1e-12);
    }

    [TestMethod]
    public void Sun_and_Moon_do_not_receive_double_bonus()
    {
        var result = AspectEngine.Detect(
            AstrologicalObjectId.Sun,
            EclipticLongitude.FromDegrees(0.0),
            AstrologicalObjectId.Moon,
            EclipticLongitude.FromDegrees(9.5),
            Profile);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Excluded_participant_returns_none()
    {
        var result = AspectEngine.Detect(
            AstrologicalObjectId.NorthTrueNode,
            EclipticLongitude.FromDegrees(0.0),
            AstrologicalObjectId.Mars,
            EclipticLongitude.FromDegrees(90.0),
            Profile);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Aspect_detection_crosses_zero_correctly()
    {
        var result = AspectEngine.Detect(
            AstrologicalObjectId.Mercury,
            EclipticLongitude.FromDegrees(359.0),
            AstrologicalObjectId.Venus,
            EclipticLongitude.FromDegrees(1.0),
            Profile);

        Assert.IsNotNull(result);
        Assert.AreEqual(
            AspectKind.Conjunction,
            result.Definition.Kind);

        Assert.AreEqual(
            2.0,
            result.Separation.Degrees,
            1e-12);

        Assert.AreEqual(
            2.0,
            result.DeviationDegrees,
            1e-12);
    }

    [TestMethod]
    public void Same_input_is_deterministic()
    {
        AspectResult? first = null;

        for (var i = 0; i < 100; i++)
        {
            var current = AspectEngine.Detect(
                AstrologicalObjectId.Mars,
                EclipticLongitude.FromDegrees(10.0),
                AstrologicalObjectId.Jupiter,
                EclipticLongitude.FromDegrees(100.5),
                Profile);

            Assert.IsNotNull(current);

            if (first is null)
            {
                first = current;
                continue;
            }

            Assert.AreEqual(first, current);
        }
    }

    [TestMethod]
    public void Lowest_deviation_wins_when_candidates_overlap()
    {
        var custom = new AspectProfile(
            "overlap-test",
            [
                new AspectDefinition(
                    AspectKind.Sextile,
                    60.0,
                    20.0,
                    1),

                new AspectDefinition(
                    AspectKind.Quintile,
                    72.0,
                    20.0,
                    0)
            ],
            [
                AstrologicalObjectId.Mars,
                AstrologicalObjectId.Jupiter
            ],
            0.0);

        var result = AspectEngine.Detect(
            AstrologicalObjectId.Mars,
            EclipticLongitude.FromDegrees(0.0),
            AstrologicalObjectId.Jupiter,
            EclipticLongitude.FromDegrees(70.0),
            custom);

        Assert.IsNotNull(result);
        Assert.AreEqual(
            AspectKind.Quintile,
            result.Definition.Kind);
    }

    [TestMethod]
    public void Stable_priority_breaks_equal_deviation_tie()
    {
        var custom = new AspectProfile(
            "priority-test",
            [
                new AspectDefinition(
                    AspectKind.Sextile,
                    60.0,
                    20.0,
                    5),

                new AspectDefinition(
                    AspectKind.Quintile,
                    72.0,
                    20.0,
                    1)
            ],
            [
                AstrologicalObjectId.Mars,
                AstrologicalObjectId.Jupiter
            ],
            0.0);

        var result = AspectEngine.Detect(
            AstrologicalObjectId.Mars,
            EclipticLongitude.FromDegrees(0.0),
            AstrologicalObjectId.Jupiter,
            EclipticLongitude.FromDegrees(66.0),
            custom);

        Assert.IsNotNull(result);

        Assert.AreEqual(
            AspectKind.Quintile,
            result.Definition.Kind);
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Glyphs;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7CanonicalGlyphShapeTests
{
    [TestMethod]
    public void Moon_is_waxing_D_shaped_and_visually_closed()
    {
        var glyph =
            new NatalVectorGlyphCatalog()
                .GetRequired(
                    "planet-moon");

        Assert.AreEqual(
            1,
            glyph.Strokes.Count);

        Assert.AreEqual(
            0,
            glyph.Circles.Count);

        var contour =
            glyph.Strokes.Single();

        Assert.IsTrue(
            contour.Points.Count
            >= 16);

        Assert.AreEqual(
            contour.Points[0],
            contour.Points[^1]);

        Assert.IsTrue(
            contour.Points.Max(
                point => point.X)
            >= 0.35);

        Assert.IsTrue(
            contour.Points.Min(
                point => point.X)
            < 0.0);

        Assert.IsTrue(
            contour.Points.Any(
                point =>
                    point.Y < -0.40));

        Assert.IsTrue(
            contour.Points.Any(
                point =>
                    point.Y > 0.40));

        // El contorno contiene borde exterior e interior curvos:
        // creciente cerrada y no una simple C abierta.
        var distinctX =
            contour.Points
                .Select(
                    point => point.X)
                .Distinct()
                .Count();

        Assert.IsTrue(
            distinctX
            >= 8);

        // El borde interior no es una recta:
        // contiene varios niveles horizontales propios.
        var innerCurvePoints =
            contour.Points
                .Where(
                    point =>
                        point.X
                        >= 0.0
                        && point.X
                        <= 0.18)
                .ToArray();

        Assert.IsTrue(
            innerCurvePoints.Length
            >= 6);

        Assert.IsTrue(
            innerCurvePoints
                .Select(
                    point => point.X)
                .Distinct()
                .Count()
            >= 4);
    }


    [TestMethod]
    public void Saturn_has_standard_cross_stem_and_attached_curve()
    {
        var glyph =
            new NatalVectorGlyphCatalog()
                .GetRequired(
                    "planet-saturn");

        Assert.AreEqual(
            3,
            glyph.Strokes.Count);

        var stem =
            glyph.Strokes.Single(
                x =>
                    x.Points.Count == 2
                    && Math.Abs(
                        x.Points[0].X
                        - x.Points[1].X)
                        < 1e-12);

        var curve =
            glyph.Strokes.Single(
                x =>
                    x.Points.Count > 2);

        Assert.AreEqual(
            stem.Points[^1],
            curve.Points[0]);
    }


    [TestMethod]
    public void Chiron_uses_clean_key_over_circle_geometry()
    {
        var glyph =
            new NatalVectorGlyphCatalog()
                .GetRequired(
                    "point-chiron");

        Assert.AreEqual(
            3,
            glyph.Strokes.Count);

        Assert.AreEqual(
            1,
            glyph.Circles.Count);

        Assert.IsTrue(
            glyph.Circles.Single()
                .Center.Y
            > 0.0);
    }

    [TestMethod]
    public void Four_main_asteroid_glyphs_are_available()
    {
        var catalog =
            new NatalVectorGlyphCatalog();

        foreach (
            var key
            in new[]
            {
                "asteroid-ceres",
                "asteroid-pallas",
                "asteroid-juno",
                "asteroid-vesta"
            })
        {
            Assert.IsFalse(
                catalog
                    .GetRequired(key)
                    .IsEmpty,
                key);
        }
    }
}

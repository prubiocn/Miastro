using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Glyphs;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7SaturnPlutoGlyphTests
{
    [TestMethod]
    public void Saturn_has_recognisable_standard_geometry()
    {
        var glyph =
            new NatalVectorGlyphCatalog()
                .GetRequired(
                    "planet-saturn");

        Assert.AreEqual(
            3,
            glyph.Strokes.Count);

        Assert.AreEqual(
            0,
            glyph.Circles.Count);

        var curve =
            glyph.Strokes.Single(
                x =>
                    x.Points.Count > 2);

        Assert.AreEqual(
            0.00,
            curve.Points[0].X,
            1e-12);

        Assert.AreEqual(
            0.04,
            curve.Points[0].Y,
            1e-12);

        Assert.IsTrue(
            curve.Points.Max(
                x => x.X)
            >= 0.32);
    }


    [TestMethod]
    public void Pluto_uses_circle_cup_vertical_and_lower_crossbar()
    {
        var glyph =
            new NatalVectorGlyphCatalog()
                .GetRequired(
                    "planet-pluto");

        Assert.AreEqual(
            1,
            glyph.Circles.Count);

        Assert.AreEqual(
            3,
            glyph.Strokes.Count);

        var circle =
            glyph.Circles.Single();

        Assert.IsTrue(
            circle.Center.Y < 0);

        var vertical =
            glyph.Strokes.Single(
                x =>
                    x.Points.Count == 2
                    && Math.Abs(
                        x.Points[0].X
                        - x.Points[1].X)
                        < 1e-12);

        Assert.AreEqual(
            0.0,
            vertical.Points[0].X,
            1e-12);

        var horizontal =
            glyph.Strokes.Single(
                x =>
                    x.Points.Count == 2
                    && Math.Abs(
                        x.Points[0].Y
                        - x.Points[1].Y)
                        < 1e-12);

        Assert.IsTrue(
            horizontal.Points[0].X < 0);

        Assert.IsTrue(
            horizontal.Points[1].X > 0);

        Assert.IsTrue(
            horizontal.Points[0].Y > 0);
    }
}

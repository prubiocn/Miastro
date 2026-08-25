using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miastro.Graphics.Glyphs;

namespace Miastro.Tests.Phase7;

[TestClass]
public sealed class Phase7MoonSaturnReferenceTests
{
    [TestMethod]
    public void Moon_is_closed_waxing_D_contour()
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

        var stroke =
            glyph.Strokes.Single();

        Assert.IsTrue(
            stroke.Points.Count
            >= 16);

        Assert.AreEqual(
            stroke.Points[0],
            stroke.Points[^1]);

        Assert.IsTrue(
            stroke.Points.Max(
                x => x.X)
            >= 0.35);

        Assert.IsTrue(
            stroke.Points.Min(
                x => x.X)
            < 0.0);
    }

    [TestMethod]
    public void Saturn_is_standard_cross_with_attached_lower_curve()
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

        var vertical =
            glyph.Strokes.Single(
                stroke =>
                    stroke.Points.Count == 2
                    && Math.Abs(
                        stroke.Points[0].X
                        - stroke.Points[1].X)
                        < 1e-12);

        var cross =
            glyph.Strokes.Single(
                stroke =>
                    stroke.Points.Count == 2
                    && Math.Abs(
                        stroke.Points[0].Y
                        - stroke.Points[1].Y)
                        < 1e-12);

        var curve =
            glyph.Strokes.Single(
                stroke =>
                    stroke.Points.Count > 2);

        Assert.AreEqual(
            vertical.Points[^1],
            curve.Points[0]);

        Assert.IsTrue(
            cross.Points[0].X < 0.0
            && cross.Points[1].X > 0.0);

        Assert.IsTrue(
            curve.Points.Max(
                p => p.X)
            > 0.30);

        Assert.IsTrue(
            curve.Points.Max(
                p => p.Y)
            > 0.40);
    }


}

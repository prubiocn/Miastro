using Miastro.Graphics.Geometry;

namespace Miastro.Graphics.Glyphs;

public sealed class NatalVectorGlyphCatalog
{
    private readonly IReadOnlyDictionary<
        string,
        VectorGlyphDefinition> _glyphs;

    public NatalVectorGlyphCatalog()
    {
        _glyphs =
            Build()
                .ToDictionary(
                    x => x.Key,
                    StringComparer.Ordinal);
    }

    public IReadOnlyCollection<string> Keys =>
        _glyphs.Keys
            .OrderBy(
                x => x,
                StringComparer.Ordinal)
            .ToArray();

    public bool TryGet(
        string key,
        out VectorGlyphDefinition definition)
        =>
            _glyphs.TryGetValue(
                key,
                out definition!);

    public VectorGlyphDefinition GetRequired(
        string key)
        =>
            _glyphs.TryGetValue(
                key,
                out var definition)
                ? definition
                : throw new KeyNotFoundException(
                    $"Unknown vector glyph '{key}'.");

    private static IEnumerable<VectorGlyphDefinition>
        Build()
    {
        yield return Glyph(
            "planet-sun",
            circles:
            [
                C(0, 0, 0.34),
                C(0, 0, 0.07)
            ]);

        yield return Glyph(
            "planet-moon",
            strokes:
            [
                // Creciente aprobada:
                // contorno cerrado, convexo y orientado tipo D.
                S(
                    -0.12, -0.43,

                     0.02, -0.42,
                     0.16, -0.37,
                     0.27, -0.28,
                     0.34, -0.15,
                     0.37,  0.00,
                     0.34,  0.15,
                     0.27,  0.28,
                     0.16,  0.37,
                     0.02,  0.42,

                    -0.12,  0.43,

                     0.02,  0.33,
                     0.11,  0.22,
                     0.16,  0.10,
                     0.17,  0.00,
                     0.16, -0.10,
                     0.11, -0.22,
                     0.02, -0.33,

                    -0.12, -0.43)
            ]);

        yield return Glyph(
            "planet-mercury",
            strokes:
            [
                S(
                    -0.24, -0.39,
                    -0.12, -0.27,
                     0.00, -0.23,
                     0.12, -0.27,
                     0.24, -0.39),

                S(
                     0.00,  0.20,
                     0.00,  0.43),

                S(
                    -0.14,  0.32,
                     0.14,  0.32)
            ],
            circles:
            [
                C(0, -0.02, 0.22)
            ]);

        yield return Glyph(
            "planet-venus",
            strokes:
            [
                S(
                     0.00,  0.20,
                     0.00,  0.43),

                S(
                    -0.14,  0.32,
                     0.14,  0.32)
            ],
            circles:
            [
                C(0, -0.08, 0.25)
            ]);

        yield return Glyph(
            "planet-mars",
            strokes:
            [
                S(
                     0.18, -0.18,
                     0.40, -0.40),

                S(
                     0.20, -0.40,
                     0.40, -0.40,
                     0.40, -0.20)
            ],
            circles:
            [
                C(-0.08, 0.08, 0.27)
            ]);

        yield return Glyph(
            "planet-jupiter",
            strokes:
            [
                S(
                    -0.30, -0.20,
                    -0.10, -0.32,
                     0.08, -0.24,
                     0.05, -0.05,
                    -0.22,  0.18,
                     0.20,  0.18),

                S(
                     0.13, -0.40,
                     0.13,  0.40)
            ]);

        yield return Glyph(
            "planet-saturn",
            strokes:
            [
                // Asta vertical.
                S(
                     0.00, -0.43,
                     0.00,  0.04),

                // Cruz superior.
                S(
                    -0.25, -0.24,
                     0.20, -0.24),

                // Semicírculo inferior unido a la base del asta.
                S(
                     0.00,  0.04,
                     0.10,  0.02,
                     0.20,  0.05,
                     0.28,  0.12,
                     0.32,  0.21,
                     0.32,  0.30,
                     0.28,  0.38,
                     0.20,  0.43,
                     0.10,  0.44,
                     0.01,  0.41,
                    -0.06,  0.35)
            ]);

        yield return Glyph(
            "planet-uranus",
            strokes:
            [
                S(
                    -0.30, -0.36,
                    -0.30,  0.20),

                S(
                     0.30, -0.36,
                     0.30,  0.20),

                S(
                    -0.30, -0.02,
                     0.30, -0.02),

                S(
                     0.00, -0.34,
                     0.00,  0.24)
            ],
            circles:
            [
                C(0, 0.32, 0.10)
            ]);

        yield return Glyph(
            "planet-neptune",
            strokes:
            [
                S(
                    -0.30, -0.30,
                    -0.20, -0.12,
                     0.00,  0.00,
                     0.20, -0.12,
                     0.30, -0.30),

                S(
                     0.00, -0.36,
                     0.00,  0.40),

                S(
                    -0.17,  0.27,
                     0.17,  0.27),

                S(
                    -0.30, -0.30,
                    -0.20, -0.34),

                S(
                     0.30, -0.30,
                     0.20, -0.34)
            ]);

        yield return Glyph(
            "planet-pluto",
            strokes:
            [
                // Semicircunferencia inferior abierta hacia arriba.
                S(
                    -0.30, -0.01,
                    -0.28,  0.08,
                    -0.23,  0.15,
                    -0.16,  0.20,
                    -0.08,  0.23,
                     0.00,  0.24,
                     0.08,  0.23,
                     0.16,  0.20,
                     0.23,  0.15,
                     0.28,  0.08,
                     0.30, -0.01),

                // Eje vertical.
                S(
                     0.00,  0.24,
                     0.00,  0.45),

                // Cruz inferior.
                S(
                    -0.18,  0.36,
                     0.18,  0.36)
            ],
            circles:
            [
                // Orbe superior principal.
                C(
                     0.00,
                    -0.25,
                     0.18)
            ]);

        yield return Glyph(
            "point-north-node",
            strokes:
            [
                S(
                    -0.34,  0.12,
                    -0.24, -0.10,
                     0.00, -0.22,
                     0.24, -0.10,
                     0.34,  0.12)
            ],
            circles:
            [
                C(-0.28, 0.18, 0.08),
                C( 0.28, 0.18, 0.08)
            ]);

        yield return Glyph(
            "point-south-node",
            strokes:
            [
                S(
                    -0.34, -0.12,
                    -0.24,  0.10,
                     0.00,  0.22,
                     0.24,  0.10,
                     0.34, -0.12)
            ],
            circles:
            [
                C(-0.28, -0.18, 0.08),
                C( 0.28, -0.18, 0.08)
            ]);

        yield return Glyph(
            "point-lilith",
            strokes:
            [
                S(
                    -0.10, -0.36,
                    -0.25, -0.22,
                    -0.27,  0.00,
                    -0.16,  0.18,
                     0.02,  0.25),

                S(
                     0.02,  0.25,
                     0.02,  0.44),

                S(
                    -0.11,  0.35,
                     0.15,  0.35)
            ]);

        yield return Glyph(
            "point-fortuna",
            strokes:
            [
                S(
                    -0.31, 0,
                     0.31, 0),

                S(
                     0, -0.31,
                     0,  0.31)
            ],
            circles:
            [
                C(0, 0, 0.36)
            ]);

        yield return Glyph(
            "point-chiron",
            strokes:
            [
                // Asta desde el círculo hacia la cabeza.
                S(
                     0.00,  0.16,
                     0.00, -0.30),

                // Brazo superior.
                S(
                     0.00, -0.24,
                     0.23, -0.43),

                // Brazo inferior.
                S(
                     0.00, -0.24,
                     0.23, -0.08)
            ],
            circles:
            [
                C(
                     0.00,
                     0.29,
                     0.13)
            ]);

        yield return Glyph(
            "asteroid-ceres",
            strokes:
            [
                S(
                     0.00, -0.10,
                     0.00,  0.42),

                S(
                    -0.14,  0.30,
                     0.14,  0.30),

                S(
                     0.20, -0.35,
                     0.02, -0.32,
                    -0.10, -0.18,
                    -0.05,  0.00,
                     0.10,  0.08)
            ]);

        yield return Glyph(
            "asteroid-pallas",
            strokes:
            [
                S(
                     0.00, -0.40,
                     0.24, -0.14,
                     0.00,  0.10,
                    -0.24, -0.14,
                     0.00, -0.40,
                     0.00,  0.42),

                S(
                    -0.13,  0.31,
                     0.13,  0.31)
            ]);

        yield return Glyph(
            "asteroid-juno",
            strokes:
            [
                S(
                     0.00, -0.40,
                     0.09, -0.14,
                     0.34, -0.14,
                     0.14,  0.03,
                     0.22,  0.30,
                     0.00,  0.13,
                    -0.22,  0.30,
                    -0.14,  0.03,
                    -0.34, -0.14,
                    -0.09, -0.14,
                     0.00, -0.40)
            ]);

        yield return Glyph(
            "asteroid-vesta",
            strokes:
            [
                S(
                    -0.23,  0.26,
                     0.23,  0.26),

                S(
                    -0.14,  0.26,
                    -0.08,  0.42),

                S(
                     0.14,  0.26,
                     0.08,  0.42),

                S(
                     0.00, -0.42,
                    -0.16, -0.15,
                     0.00,  0.08,
                     0.16, -0.15,
                     0.00, -0.42)
            ]);

        yield return Glyph(
            "angle-asc",
            strokes:
            [
                S(
                    -0.32,  0.36,
                     0.00, -0.38,
                     0.32,  0.36),

                S(
                    -0.18,  0.05,
                     0.18,  0.05)
            ]);

        yield return Glyph(
            "angle-mc",
            strokes:
            [
                S(
                    -0.36,  0.36,
                    -0.36, -0.36,
                     0.00,  0.04,
                     0.36, -0.36,
                     0.36,  0.36)
            ]);

        foreach (
            var zodiac
            in BuildZodiac())
        {
            yield return zodiac;
        }

        foreach (
            var aspect
            in BuildAspects())
        {
            yield return aspect;
        }
    }

    private static IEnumerable<VectorGlyphDefinition>
        BuildZodiac()
    {
        // Aries ♈
        yield return Glyph(
            "zodiac-00",
            strokes:
            [
                S(
                     0.00,  0.38,
                     0.00,  0.02,
                    -0.08, -0.20,
                    -0.22, -0.36,
                    -0.36, -0.30,
                    -0.42, -0.14),

                S(
                     0.00,  0.38,
                     0.00,  0.02,
                     0.08, -0.20,
                     0.22, -0.36,
                     0.36, -0.30,
                     0.42, -0.14)
            ]);

        // Tauro ♉
        yield return Glyph(
            "zodiac-01",
            strokes:
            [
                S(
                    -0.34, -0.36,
                    -0.26, -0.18,
                    -0.12, -0.10),

                S(
                     0.34, -0.36,
                     0.26, -0.18,
                     0.12, -0.10)
            ],
            circles:
            [
                C(0.00, 0.10, 0.27)
            ]);

        // Géminis ♊
        yield return Glyph(
            "zodiac-02",
            strokes:
            [
                S(
                    -0.22, -0.30,
                    -0.22,  0.30),

                S(
                     0.22, -0.30,
                     0.22,  0.30),

                S(
                    -0.34, -0.34,
                     0.00, -0.28,
                     0.34, -0.34),

                S(
                    -0.34,  0.34,
                     0.00,  0.28,
                     0.34,  0.34)
            ]);

        // Cáncer ♋
        yield return Glyph(
            "zodiac-03",
            strokes:
            [
                S(
                    -0.34, -0.10,
                    -0.20, -0.26,
                     0.04, -0.28,
                     0.28, -0.16),

                S(
                     0.34,  0.10,
                     0.20,  0.26,
                    -0.04,  0.28,
                    -0.28,  0.16)
            ],
            circles:
            [
                C(-0.22, -0.10, 0.10),
                C( 0.22,  0.10, 0.10)
            ]);

        // Leo ♌
        yield return Glyph(
            "zodiac-04",
            strokes:
            [
                S(
                    -0.18,  0.10,
                    -0.04, -0.12,
                     0.08, -0.30,
                     0.24, -0.34,
                     0.34, -0.20,
                     0.30, -0.02,
                     0.16,  0.16,
                     0.14,  0.32,
                     0.26,  0.38)
            ],
            circles:
            [
                C(-0.22, 0.14, 0.13)
            ]);

        // Virgo ♍
        yield return Glyph(
            "zodiac-05",
            strokes:
            [
                S(
                    -0.34, -0.28,
                    -0.34,  0.28),

                S(
                    -0.34, -0.08,
                    -0.20, -0.24,
                    -0.08, -0.08,
                    -0.08,  0.26),

                S(
                    -0.08, -0.08,
                     0.06, -0.24,
                     0.18, -0.08,
                     0.18,  0.20),

                S(
                     0.18, -0.08,
                     0.30, -0.20,
                     0.36, -0.06,
                     0.30,  0.14,
                     0.14,  0.34),

                S(
                     0.12,  0.16,
                     0.38,  0.34)
            ]);

        // Libra ♎
        yield return Glyph(
            "zodiac-06",
            strokes:
            [
                S(
                    -0.40,  0.28,
                     0.40,  0.28),

                S(
                    -0.40,  0.08,
                    -0.18,  0.08,
                    -0.14, -0.12,
                     0.00, -0.24,
                     0.14, -0.12,
                     0.18,  0.08,
                     0.40,  0.08)
            ]);

        // Escorpio ♏
        yield return Glyph(
            "zodiac-07",
            strokes:
            [
                S(
                    -0.34, -0.28,
                    -0.34,  0.24),

                S(
                    -0.34, -0.06,
                    -0.20, -0.24,
                    -0.08, -0.06,
                    -0.08,  0.24),

                S(
                    -0.08, -0.06,
                     0.06, -0.24,
                     0.18, -0.06,
                     0.18,  0.18,
                     0.34,  0.28),

                S(
                     0.34,  0.28,
                     0.24,  0.28),

                S(
                     0.34,  0.28,
                     0.32,  0.16)
            ]);

        // Sagitario ♐
        yield return Glyph(
            "zodiac-08",
            strokes:
            [
                S(
                    -0.32,  0.32,
                     0.30, -0.30),

                S(
                     0.08, -0.30,
                     0.30, -0.30,
                     0.30, -0.08),

                S(
                    -0.18, -0.02,
                     0.14,  0.30)
            ]);

        // Capricornio ♑
        yield return Glyph(
            "zodiac-09",
            strokes:
            [
                S(
                    -0.36, -0.28,
                    -0.36,  0.20),

                S(
                    -0.36, -0.08,
                    -0.20, -0.24,
                    -0.06, -0.08,
                    -0.06,  0.24),

                S(
                    -0.06, -0.04,
                     0.10, -0.18,
                     0.26, -0.08,
                     0.28,  0.10,
                     0.18,  0.24,
                     0.04,  0.20,
                     0.12,  0.06,
                     0.34,  0.30)
            ]);

        // Acuario ♒
        yield return Glyph(
            "zodiac-10",
            strokes:
            [
                S(
                    -0.40, -0.18,
                    -0.26, -0.30,
                    -0.12, -0.18,
                     0.02, -0.30,
                     0.16, -0.18,
                     0.30, -0.30,
                     0.40, -0.22),

                S(
                    -0.40,  0.14,
                    -0.26,  0.02,
                    -0.12,  0.14,
                     0.02,  0.02,
                     0.16,  0.14,
                     0.30,  0.02,
                     0.40,  0.10)
            ]);

        // Piscis ♓
        yield return Glyph(
            "zodiac-11",
            strokes:
            [
                S(
                    -0.30, -0.34,
                    -0.18, -0.18,
                    -0.14,  0.00,
                    -0.18,  0.18,
                    -0.30,  0.34),

                S(
                     0.30, -0.34,
                     0.18, -0.18,
                     0.14,  0.00,
                     0.18,  0.18,
                     0.30,  0.34),

                S(
                    -0.28, 0.00,
                     0.28, 0.00)
            ]);
    }

    private static IEnumerable<VectorGlyphDefinition>
        BuildAspects()
    {
        yield return Glyph(
            "aspect-conjunction",
            circles:
            [
                C(0, 0, 0.28)
            ]);

        yield return Glyph(
            "aspect-opposition",
            strokes:
            [
                S(
                    -0.20,  0.20,
                     0.20, -0.20)
            ],
            circles:
            [
                C(-0.25,  0.25, 0.09),
                C( 0.25, -0.25, 0.09)
            ]);

        yield return Glyph(
            "aspect-trine",
            strokes:
            [
                S(
                     0.00, -0.34,
                     0.34,  0.26,
                    -0.34,  0.26,
                     0.00, -0.34)
            ]);

        yield return Glyph(
            "aspect-square",
            strokes:
            [
                S(
                    -0.28, -0.28,
                     0.28, -0.28,
                     0.28,  0.28,
                    -0.28,  0.28,
                    -0.28, -0.28)
            ]);

        yield return Glyph(
            "aspect-sextile",
            strokes:
            [
                S(
                     0.00, -0.34,
                     0.00,  0.34),

                S(
                    -0.30, -0.17,
                     0.30,  0.17),

                S(
                    -0.30,  0.17,
                     0.30, -0.17)
            ]);

        yield return Glyph(
            "aspect-quincunx",
            strokes:
            [
                S(
                    -0.28, -0.28,
                     0.28,  0.28),

                S(
                    -0.28,  0.28,
                     0.28, -0.28)
            ],
            circles:
            [
                C(0, 0, 0.06)
            ]);
    }

    private static VectorGlyphDefinition Glyph(
        string key,
        IReadOnlyList<VectorGlyphStroke>? strokes = null,
        IReadOnlyList<VectorGlyphCircle>? circles = null)
        =>
            new(
                key,
                strokes
                    ?? Array.Empty<VectorGlyphStroke>(),
                circles
                    ?? Array.Empty<VectorGlyphCircle>());

    private static VectorGlyphCircle C(
        double x,
        double y,
        double radius)
        =>
            new(
                new ChartPoint(
                    x,
                    y),
                radius);

    private static VectorGlyphStroke S(
        params double[] coordinates)
    {
        if (coordinates.Length < 4
            || coordinates.Length % 2 != 0)
        {
            throw new ArgumentException(
                "A stroke requires coordinate pairs.",
                nameof(coordinates));
        }

        var points =
            new ChartPoint[
                coordinates.Length / 2];

        for (
            var i = 0;
            i < coordinates.Length;
            i += 2)
        {
            points[i / 2] =
                new ChartPoint(
                    coordinates[i],
                    coordinates[i + 1]);
        }

        return new VectorGlyphStroke(
            points);
    }
}

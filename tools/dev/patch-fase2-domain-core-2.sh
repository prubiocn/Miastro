#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
DOMAIN="$ROOT/src/Miastro.Domain"
TESTS="$ROOT/tests/Miastro.Tests"

cd "$ROOT"

mkdir -p \
  "$DOMAIN/DerivedPoints" \
  "$DOMAIN/Rulerships" \
  "$DOMAIN/Placements"

# ------------------------------------------------------------
# NODO SUR DERIVADO
# ------------------------------------------------------------

cat > "$DOMAIN/DerivedPoints/LunarNodeCalculator.cs" <<'EOF'
using Miastro.Domain.Angles;

namespace Miastro.Domain.DerivedPoints;

public static class LunarNodeCalculator
{
    public static EclipticLongitude CalculateSouthNode(
        EclipticLongitude northTrueNode) =>
        northTrueNode + Angle.FromDegrees(180.0);
}
EOF

# ------------------------------------------------------------
# PARTE DE LA FORTUNA
# ------------------------------------------------------------

cat > "$DOMAIN/DerivedPoints/ChartSect.cs" <<'EOF'
namespace Miastro.Domain.DerivedPoints;

public enum ChartSect
{
    Day,
    Night
}
EOF

cat > "$DOMAIN/DerivedPoints/PartOfFortuneCalculator.cs" <<'EOF'
using Miastro.Domain.Angles;

namespace Miastro.Domain.DerivedPoints;

public static class PartOfFortuneCalculator
{
    public static EclipticLongitude Calculate(
        EclipticLongitude ascendant,
        EclipticLongitude sun,
        EclipticLongitude moon,
        ChartSect sect)
    {
        var value = sect switch
        {
            ChartSect.Day =>
                ascendant.Degrees
                + moon.Degrees
                - sun.Degrees,

            ChartSect.Night =>
                ascendant.Degrees
                + sun.Degrees
                - moon.Degrees,

            _ =>
                throw new ArgumentOutOfRangeException(nameof(sect))
        };

        return EclipticLongitude.FromDegrees(value);
    }
}
EOF

# ------------------------------------------------------------
# REGENCIAS
# ------------------------------------------------------------

cat > "$DOMAIN/Rulerships/Rulership.cs" <<'EOF'
using Miastro.Domain.Objects;
using Miastro.Domain.Zodiac;

namespace Miastro.Domain.Rulerships;

public sealed record Rulership
{
    public ZodiacSign Sign { get; }

    public AstrologicalObjectId Traditional { get; }

    public AstrologicalObjectId? Modern { get; }

    public Rulership(
        ZodiacSign sign,
        AstrologicalObjectId traditional,
        AstrologicalObjectId? modern = null)
    {
        ValidateRuler(traditional);

        if (modern is not null)
        {
            ValidateRuler(modern.Value);
        }

        Sign = sign;
        Traditional = traditional;
        Modern = modern;
    }

    public IReadOnlyList<AstrologicalObjectId> Both =>
        Modern is null
            ? [Traditional]
            : [Traditional, Modern.Value];

    private static void ValidateRuler(
        AstrologicalObjectId ruler)
    {
        var category =
            AstrologicalObjectCatalog.GetCategory(ruler);

        if (category is not
            AstrologicalObjectCategory.Planet
            and not AstrologicalObjectCategory.Luminary)
        {
            throw new ArgumentException(
                "Una regencia V1 debe utilizar un planeta o luminar.",
                nameof(ruler));
        }
    }
}
EOF

cat > "$DOMAIN/Rulerships/RulershipCatalog.cs" <<'EOF'
using Miastro.Domain.Objects;
using Miastro.Domain.Zodiac;

namespace Miastro.Domain.Rulerships;

public static class RulershipCatalog
{
    private static readonly IReadOnlyDictionary<ZodiacSign, Rulership>
        Rulerships =
        new Dictionary<ZodiacSign, Rulership>
        {
            [ZodiacSign.Aries] =
                new(
                    ZodiacSign.Aries,
                    AstrologicalObjectId.Mars),

            [ZodiacSign.Taurus] =
                new(
                    ZodiacSign.Taurus,
                    AstrologicalObjectId.Venus),

            [ZodiacSign.Gemini] =
                new(
                    ZodiacSign.Gemini,
                    AstrologicalObjectId.Mercury),

            [ZodiacSign.Cancer] =
                new(
                    ZodiacSign.Cancer,
                    AstrologicalObjectId.Moon),

            [ZodiacSign.Leo] =
                new(
                    ZodiacSign.Leo,
                    AstrologicalObjectId.Sun),

            [ZodiacSign.Virgo] =
                new(
                    ZodiacSign.Virgo,
                    AstrologicalObjectId.Mercury),

            [ZodiacSign.Libra] =
                new(
                    ZodiacSign.Libra,
                    AstrologicalObjectId.Venus),

            [ZodiacSign.Scorpio] =
                new(
                    ZodiacSign.Scorpio,
                    AstrologicalObjectId.Mars,
                    AstrologicalObjectId.Pluto),

            [ZodiacSign.Sagittarius] =
                new(
                    ZodiacSign.Sagittarius,
                    AstrologicalObjectId.Jupiter),

            [ZodiacSign.Capricorn] =
                new(
                    ZodiacSign.Capricorn,
                    AstrologicalObjectId.Saturn),

            [ZodiacSign.Aquarius] =
                new(
                    ZodiacSign.Aquarius,
                    AstrologicalObjectId.Saturn,
                    AstrologicalObjectId.Uranus),

            [ZodiacSign.Pisces] =
                new(
                    ZodiacSign.Pisces,
                    AstrologicalObjectId.Jupiter,
                    AstrologicalObjectId.Neptune)
        };

    public static Rulership Get(
        ZodiacSign sign)
    {
        if (!Rulerships.TryGetValue(sign, out var rulership))
        {
            throw new ArgumentOutOfRangeException(nameof(sign));
        }

        return rulership;
    }

    public static IReadOnlyCollection<Rulership> All =>
        Rulerships.Values;
}
EOF

# ------------------------------------------------------------
# RETROGRADACIÓN
# ------------------------------------------------------------

cat > "$DOMAIN/Placements/MotionState.cs" <<'EOF'
namespace Miastro.Domain.Placements;

public enum MotionState
{
    Direct,
    Retrograde,
    Stationary
}
EOF

cat > "$DOMAIN/Placements/MotionStateResolver.cs" <<'EOF'
namespace Miastro.Domain.Placements;

public static class MotionStateResolver
{
    public static MotionState FromSpeed(
        double speedDegreesPerDay)
    {
        if (!double.IsFinite(speedDegreesPerDay))
        {
            throw new ArgumentOutOfRangeException(
                nameof(speedDegreesPerDay));
        }

        if (speedDegreesPerDay > 0.0)
        {
            return MotionState.Direct;
        }

        if (speedDegreesPerDay < 0.0)
        {
            return MotionState.Retrograde;
        }

        return MotionState.Stationary;
    }
}
EOF

# ------------------------------------------------------------
# POSICIÓN ASTROLÓGICA
# ------------------------------------------------------------

cat > "$DOMAIN/Placements/ZodiacPosition.cs" <<'EOF'
using Miastro.Domain.Angles;
using Miastro.Domain.Zodiac;

namespace Miastro.Domain.Placements;

public readonly record struct ZodiacPosition
{
    public ZodiacSign Sign { get; }

    public double DegreeInSign { get; }

    private ZodiacPosition(
        ZodiacSign sign,
        double degreeInSign)
    {
        if (degreeInSign is < 0.0 or >= 30.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(degreeInSign));
        }

        Sign = sign;
        DegreeInSign = degreeInSign;
    }

    public static ZodiacPosition FromLongitude(
        EclipticLongitude longitude) =>
        new(
            ZodiacSignInfo.FromLongitude(longitude),
            ZodiacSignInfo.GetDegreeInSign(longitude));
}
EOF

cat > "$DOMAIN/Placements/AstrologicalPlacement.cs" <<'EOF'
using Miastro.Domain.Angles;
using Miastro.Domain.Houses;
using Miastro.Domain.Objects;
using Miastro.Domain.Zodiac;

namespace Miastro.Domain.Placements;

public sealed record AstrologicalPlacement
{
    public AstrologicalObjectId ObjectId { get; }

    public EclipticLongitude Longitude { get; }

    public ZodiacSign Sign { get; }

    public double DegreeInSign { get; }

    public AstrologicalHouse? House { get; }

    public double? SpeedDegreesPerDay { get; }

    public MotionState? Motion { get; }

    public bool? IsRetrograde =>
        Motion switch
        {
            MotionState.Retrograde => true,
            MotionState.Direct => false,
            MotionState.Stationary => false,
            null => null,
            _ => null
        };

    public AstrologicalPlacement(
        AstrologicalObjectId objectId,
        EclipticLongitude longitude,
        AstrologicalHouse? house = null,
        double? speedDegreesPerDay = null)
    {
        _ = AstrologicalObjectCatalog.GetCategory(objectId);

        if (speedDegreesPerDay is not null &&
            !double.IsFinite(speedDegreesPerDay.Value))
        {
            throw new ArgumentOutOfRangeException(
                nameof(speedDegreesPerDay));
        }

        var zodiacPosition =
            ZodiacPosition.FromLongitude(longitude);

        ObjectId = objectId;
        Longitude = longitude;
        Sign = zodiacPosition.Sign;
        DegreeInSign = zodiacPosition.DegreeInSign;
        House = house;
        SpeedDegreesPerDay = speedDegreesPerDay;
        Motion = speedDegreesPerDay is null
            ? null
            : MotionStateResolver.FromSpeed(
                speedDegreesPerDay.Value);
    }
}
EOF

# ------------------------------------------------------------
# DOCUMENTACIÓN
# ------------------------------------------------------------

cat > "$ROOT/docs/domain/derived-points.md" <<'EOF'
# Puntos derivados V1

## Nodo Sur

Miastro V1 utiliza Nodo Norte Verdadero.

El Nodo Sur no se calcula de forma astronómica independiente en el dominio.

Se deriva siempre como:

`Nodo Sur = Nodo Norte Verdadero + 180°`

normalizado a `[0°, 360°)`.

## Parte de la Fortuna

Carta diurna:

`ASC + Luna - Sol`

Carta nocturna:

`ASC + Sol - Luna`

La determinación astronómica de carta diurna o nocturna queda fuera de Fase 2.
EOF

cat > "$ROOT/docs/domain/rulerships.md" <<'EOF'
# Regencias V1

Regencias tradicionales y modernas:

- Aries → Marte
- Tauro → Venus
- Géminis → Mercurio
- Cáncer → Luna
- Leo → Sol
- Virgo → Mercurio
- Libra → Venus
- Escorpio → Marte / Plutón
- Sagitario → Júpiter
- Capricornio → Saturno
- Acuario → Saturno / Urano
- Piscis → Júpiter / Neptuno

No se incluyen regencias esotéricas en V1.
EOF

cat > "$ROOT/docs/domain/placements.md" <<'EOF'
# Posiciones astrológicas

`AstrologicalPlacement` representa una posición ya calculada por una capa externa.

Contiene:

- objeto;
- longitud eclíptica;
- signo derivado;
- grado dentro del signo;
- casa opcional;
- velocidad opcional;
- estado de movimiento opcional.

No contiene tipos ni dependencias de Swiss Ephemeris.

La retrogradación se deriva del signo de la velocidad sin redondeo:

- velocidad > 0 → Direct
- velocidad < 0 → Retrograde
- velocidad = 0 → Stationary
EOF

# ------------------------------------------------------------
# TESTS
# ------------------------------------------------------------

cat > "$TESTS/Phase2AngularAndDerivedTests.cs" <<'EOF'
using Miastro.Domain.Angles;
using Miastro.Domain.DerivedPoints;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase2AngularAndDerivedTests
{
    [DataTestMethod]
    [DataRow(360.0, 0.0)]
    [DataRow(361.0, 1.0)]
    [DataRow(-1.0, 359.0)]
    [DataRow(720.0, 0.0)]
    public void Longitude_normalizes(
        double input,
        double expected)
    {
        var longitude =
            EclipticLongitude.FromDegrees(input);

        Assert.AreEqual(
            expected,
            longitude.Degrees,
            1e-12);
    }

    [TestMethod]
    public void Separation_across_zero_is_two_degrees()
    {
        var a = EclipticLongitude.FromDegrees(359.0);
        var b = EclipticLongitude.FromDegrees(1.0);

        var separation =
            AngularSeparation.Between(a, b);

        Assert.AreEqual(
            2.0,
            separation.Degrees,
            1e-12);
    }

    [TestMethod]
    public void Separation_can_reach_180()
    {
        var a = EclipticLongitude.FromDegrees(0.0);
        var b = EclipticLongitude.FromDegrees(180.0);

        Assert.AreEqual(
            180.0,
            AngularSeparation.Between(a, b).Degrees,
            1e-12);
    }

    [DataTestMethod]
    [DataRow(0.0, 180.0)]
    [DataRow(180.0, 0.0)]
    [DataRow(359.0, 179.0)]
    public void South_node_is_opposite_true_north_node(
        double north,
        double expectedSouth)
    {
        var result =
            LunarNodeCalculator.CalculateSouthNode(
                EclipticLongitude.FromDegrees(north));

        Assert.AreEqual(
            expectedSouth,
            result.Degrees,
            1e-12);
    }

    [TestMethod]
    public void Part_of_fortune_day_formula_is_correct()
    {
        var result =
            PartOfFortuneCalculator.Calculate(
                EclipticLongitude.FromDegrees(100.0),
                EclipticLongitude.FromDegrees(20.0),
                EclipticLongitude.FromDegrees(50.0),
                ChartSect.Day);

        Assert.AreEqual(
            130.0,
            result.Degrees,
            1e-12);
    }

    [TestMethod]
    public void Part_of_fortune_night_formula_is_correct()
    {
        var result =
            PartOfFortuneCalculator.Calculate(
                EclipticLongitude.FromDegrees(100.0),
                EclipticLongitude.FromDegrees(20.0),
                EclipticLongitude.FromDegrees(50.0),
                ChartSect.Night);

        Assert.AreEqual(
            70.0,
            result.Degrees,
            1e-12);
    }

    [TestMethod]
    public void Part_of_fortune_normalizes_negative_result()
    {
        var result =
            PartOfFortuneCalculator.Calculate(
                EclipticLongitude.FromDegrees(10.0),
                EclipticLongitude.FromDegrees(300.0),
                EclipticLongitude.FromDegrees(20.0),
                ChartSect.Day);

        Assert.AreEqual(
            90.0,
            result.Degrees,
            1e-12);
    }

    [TestMethod]
    public void Part_of_fortune_normalizes_result_above_360()
    {
        var result =
            PartOfFortuneCalculator.Calculate(
                EclipticLongitude.FromDegrees(350.0),
                EclipticLongitude.FromDegrees(10.0),
                EclipticLongitude.FromDegrees(100.0),
                ChartSect.Day);

        Assert.AreEqual(
            80.0,
            result.Degrees,
            1e-12);
    }
}
EOF

cat > "$TESTS/Phase2ZodiacHousePlacementTests.cs" <<'EOF'
using Miastro.Domain.Angles;
using Miastro.Domain.Houses;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;
using Miastro.Domain.Zodiac;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase2ZodiacHousePlacementTests
{
    [DataTestMethod]
    [DataRow(0.0, ZodiacSign.Aries, 0.0)]
    [DataRow(29.999, ZodiacSign.Aries, 29.999)]
    [DataRow(30.0, ZodiacSign.Taurus, 0.0)]
    [DataRow(45.0, ZodiacSign.Taurus, 15.0)]
    [DataRow(359.0, ZodiacSign.Pisces, 29.0)]
    [DataRow(359.999, ZodiacSign.Pisces, 29.999)]
    public void Zodiac_position_is_derived_correctly(
        double longitude,
        ZodiacSign expectedSign,
        double expectedDegree)
    {
        var position =
            ZodiacPosition.FromLongitude(
                EclipticLongitude.FromDegrees(longitude));

        Assert.AreEqual(expectedSign, position.Sign);
        Assert.AreEqual(
            expectedDegree,
            position.DegreeInSign,
            1e-9);
    }

    [TestMethod]
    public void Zodiac_properties_are_correct()
    {
        Assert.AreEqual(
            ZodiacElement.Fire,
            ZodiacSignInfo.GetElement(ZodiacSign.Aries));

        Assert.AreEqual(
            ZodiacModality.Cardinal,
            ZodiacSignInfo.GetModality(ZodiacSign.Aries));

        Assert.AreEqual(
            ZodiacPolarity.Masculine,
            ZodiacSignInfo.GetPolarity(ZodiacSign.Aries));

        Assert.AreEqual(
            ZodiacSign.Libra,
            ZodiacSignInfo.GetOpposite(ZodiacSign.Aries));
    }

    [TestMethod]
    public void House_range_is_enforced()
    {
        for (var number = 1; number <= 12; number++)
        {
            Assert.AreEqual(
                number,
                AstrologicalHouse.FromNumber(number).Number);
        }

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => AstrologicalHouse.FromNumber(0));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => AstrologicalHouse.FromNumber(13));
    }

    [DataTestMethod]
    [DataRow(1, 7)]
    [DataRow(2, 8)]
    [DataRow(3, 9)]
    [DataRow(4, 10)]
    [DataRow(5, 11)]
    [DataRow(6, 12)]
    public void House_opposite_is_correct(
        int number,
        int expected)
    {
        var house =
            AstrologicalHouse.FromNumber(number);

        Assert.AreEqual(
            expected,
            house.Opposite.Number);
    }

    [DataTestMethod]
    [DataRow(1.0, MotionState.Direct)]
    [DataRow(-0.01, MotionState.Retrograde)]
    [DataRow(0.0, MotionState.Stationary)]
    public void Motion_state_is_derived_without_rounding(
        double speed,
        MotionState expected)
    {
        Assert.AreEqual(
            expected,
            MotionStateResolver.FromSpeed(speed));
    }

    [TestMethod]
    public void Placement_derives_sign_degree_and_motion()
    {
        var placement =
            new AstrologicalPlacement(
                AstrologicalObjectId.Mercury,
                EclipticLongitude.FromDegrees(45.0),
                AstrologicalHouse.FromNumber(2),
                -0.5);

        Assert.AreEqual(
            ZodiacSign.Taurus,
            placement.Sign);

        Assert.AreEqual(
            15.0,
            placement.DegreeInSign,
            1e-12);

        Assert.AreEqual(
            2,
            placement.House!.Value.Number);

        Assert.AreEqual(
            MotionState.Retrograde,
            placement.Motion);

        Assert.AreEqual(
            true,
            placement.IsRetrograde);
    }
}
EOF

cat > "$TESTS/Phase2RulershipTests.cs" <<'EOF'
using Miastro.Domain.Objects;
using Miastro.Domain.Rulerships;
using Miastro.Domain.Zodiac;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase2RulershipTests
{
    [DataTestMethod]
    [DataRow(ZodiacSign.Aries, AstrologicalObjectId.Mars)]
    [DataRow(ZodiacSign.Taurus, AstrologicalObjectId.Venus)]
    [DataRow(ZodiacSign.Gemini, AstrologicalObjectId.Mercury)]
    [DataRow(ZodiacSign.Cancer, AstrologicalObjectId.Moon)]
    [DataRow(ZodiacSign.Leo, AstrologicalObjectId.Sun)]
    [DataRow(ZodiacSign.Virgo, AstrologicalObjectId.Mercury)]
    [DataRow(ZodiacSign.Libra, AstrologicalObjectId.Venus)]
    [DataRow(ZodiacSign.Sagittarius, AstrologicalObjectId.Jupiter)]
    [DataRow(ZodiacSign.Capricorn, AstrologicalObjectId.Saturn)]
    public void Traditional_rulerships_are_correct(
        ZodiacSign sign,
        AstrologicalObjectId ruler)
    {
        Assert.AreEqual(
            ruler,
            RulershipCatalog.Get(sign).Traditional);
    }

    [TestMethod]
    public void Scorpio_has_Mars_and_Pluto()
    {
        var result =
            RulershipCatalog.Get(ZodiacSign.Scorpio);

        Assert.AreEqual(
            AstrologicalObjectId.Mars,
            result.Traditional);

        Assert.AreEqual(
            AstrologicalObjectId.Pluto,
            result.Modern);
    }

    [TestMethod]
    public void Aquarius_has_Saturn_and_Uranus()
    {
        var result =
            RulershipCatalog.Get(ZodiacSign.Aquarius);

        Assert.AreEqual(
            AstrologicalObjectId.Saturn,
            result.Traditional);

        Assert.AreEqual(
            AstrologicalObjectId.Uranus,
            result.Modern);
    }

    [TestMethod]
    public void Pisces_has_Jupiter_and_Neptune()
    {
        var result =
            RulershipCatalog.Get(ZodiacSign.Pisces);

        Assert.AreEqual(
            AstrologicalObjectId.Jupiter,
            result.Traditional);

        Assert.AreEqual(
            AstrologicalObjectId.Neptune,
            result.Modern);
    }
}
EOF

# ------------------------------------------------------------
# BUILD + TESTS
# ------------------------------------------------------------

dotnet restore Miastro.sln

dotnet build Miastro.sln \
  --configuration Release \
  --no-restore

dotnet test tests/Miastro.Tests/Miastro.Tests.csproj \
  --configuration Release \
  --no-build \
  --logger "console;verbosity=minimal"

echo
echo "=== FASE 2 — BLOQUE 2 ==="
echo "Nodo Sur derivado: OK"
echo "Parte de la Fortuna: OK"
echo "Regencias tradicionales/modernas: OK"
echo "AstrologicalPlacement: OK"
echo "Retrogradación: OK"
echo "Tests nuevos de dominio: OK"
echo "Tests Fase 1 preservados: OK"

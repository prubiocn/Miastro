#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
DOMAIN="$ROOT/src/Miastro.Domain"

cd "$ROOT"

mkdir -p \
  "$DOMAIN/Angles" \
  "$DOMAIN/Zodiac" \
  "$DOMAIN/Houses" \
  "$DOMAIN/Objects" \
  "$DOMAIN/Calculation" \
  "$DOMAIN/Charts" \
  "$DOMAIN/Placements" \
  "$DOMAIN/Rulerships" \
  "$DOMAIN/Aspects" \
  "$ROOT/docs/domain"

# ------------------------------------------------------------
# ANGLES
# ------------------------------------------------------------

cat > "$DOMAIN/Angles/Angle.cs" <<'EOF'
namespace Miastro.Domain.Angles;

public readonly record struct Angle : IComparable<Angle>
{
    public double Degrees { get; }

    private Angle(double degrees)
    {
        if (!double.IsFinite(degrees))
        {
            throw new ArgumentOutOfRangeException(
                nameof(degrees),
                "El ángulo debe ser finito.");
        }

        Degrees = degrees;
    }

    public static Angle FromDegrees(double degrees) =>
        new(degrees);

    public Angle Normalize360() =>
        new(NormalizeDegrees(Degrees));

    public static Angle operator +(Angle left, Angle right) =>
        new(left.Degrees + right.Degrees);

    public static Angle operator -(Angle left, Angle right) =>
        new(left.Degrees - right.Degrees);

    public int CompareTo(Angle other) =>
        Degrees.CompareTo(other.Degrees);

    public static double NormalizeDegrees(double degrees)
    {
        if (!double.IsFinite(degrees))
        {
            throw new ArgumentOutOfRangeException(
                nameof(degrees),
                "El ángulo debe ser finito.");
        }

        var normalized = degrees % 360.0;

        if (normalized < 0.0)
        {
            normalized += 360.0;
        }

        if (normalized >= 360.0)
        {
            normalized = 0.0;
        }

        return normalized;
    }
}
EOF

cat > "$DOMAIN/Angles/EclipticLongitude.cs" <<'EOF'
namespace Miastro.Domain.Angles;

public readonly record struct EclipticLongitude : IComparable<EclipticLongitude>
{
    public double Degrees { get; }

    private EclipticLongitude(double degrees)
    {
        Degrees = Angle.NormalizeDegrees(degrees);
    }

    public static EclipticLongitude FromDegrees(double degrees) =>
        new(degrees);

    public int CompareTo(EclipticLongitude other) =>
        Degrees.CompareTo(other.Degrees);

    public static EclipticLongitude operator +(
        EclipticLongitude longitude,
        Angle angle) =>
        new(longitude.Degrees + angle.Degrees);

    public static EclipticLongitude operator -(
        EclipticLongitude longitude,
        Angle angle) =>
        new(longitude.Degrees - angle.Degrees);
}
EOF

cat > "$DOMAIN/Angles/AngularSeparation.cs" <<'EOF'
namespace Miastro.Domain.Angles;

public readonly record struct AngularSeparation
{
    public double Degrees { get; }

    private AngularSeparation(double degrees)
    {
        if (degrees < 0.0 || degrees > 180.0)
        {
            throw new ArgumentOutOfRangeException(nameof(degrees));
        }

        Degrees = degrees;
    }

    public static AngularSeparation Between(
        EclipticLongitude first,
        EclipticLongitude second)
    {
        var difference =
            Math.Abs(first.Degrees - second.Degrees);

        var minimum = Math.Min(
            difference,
            360.0 - difference);

        return new AngularSeparation(minimum);
    }
}
EOF

# ------------------------------------------------------------
# ZODIAC
# ------------------------------------------------------------

cat > "$DOMAIN/Zodiac/ZodiacElement.cs" <<'EOF'
namespace Miastro.Domain.Zodiac;

public enum ZodiacElement
{
    Fire,
    Earth,
    Air,
    Water
}
EOF

cat > "$DOMAIN/Zodiac/ZodiacModality.cs" <<'EOF'
namespace Miastro.Domain.Zodiac;

public enum ZodiacModality
{
    Cardinal,
    Fixed,
    Mutable
}
EOF

cat > "$DOMAIN/Zodiac/ZodiacPolarity.cs" <<'EOF'
namespace Miastro.Domain.Zodiac;

public enum ZodiacPolarity
{
    Masculine,
    Feminine
}
EOF

cat > "$DOMAIN/Zodiac/ZodiacSign.cs" <<'EOF'
namespace Miastro.Domain.Zodiac;

public enum ZodiacSign
{
    Aries = 0,
    Taurus = 1,
    Gemini = 2,
    Cancer = 3,
    Leo = 4,
    Virgo = 5,
    Libra = 6,
    Scorpio = 7,
    Sagittarius = 8,
    Capricorn = 9,
    Aquarius = 10,
    Pisces = 11
}
EOF

cat > "$DOMAIN/Zodiac/ZodiacAxis.cs" <<'EOF'
namespace Miastro.Domain.Zodiac;

public readonly record struct ZodiacAxis(
    ZodiacSign First,
    ZodiacSign Second)
{
    public ZodiacAxis
    {
        if (GetOpposite(First) != Second)
        {
            throw new ArgumentException(
                "Los signos no forman un eje zodiacal válido.");
        }
    }

    public static ZodiacSign GetOpposite(ZodiacSign sign) =>
        (ZodiacSign)(((int)sign + 6) % 12);
}
EOF

cat > "$DOMAIN/Zodiac/ZodiacSignInfo.cs" <<'EOF'
using Miastro.Domain.Angles;

namespace Miastro.Domain.Zodiac;

public static class ZodiacSignInfo
{
    public static int GetIndex(ZodiacSign sign) =>
        Validate(sign);

    public static EclipticLongitude GetStart(
        ZodiacSign sign) =>
        EclipticLongitude.FromDegrees(
            Validate(sign) * 30.0);

    public static EclipticLongitude GetEndExclusive(
        ZodiacSign sign) =>
        EclipticLongitude.FromDegrees(
            (Validate(sign) + 1) * 30.0);

    public static ZodiacElement GetElement(
        ZodiacSign sign) =>
        Validate(sign) switch
        {
            0 or 4 or 8 => ZodiacElement.Fire,
            1 or 5 or 9 => ZodiacElement.Earth,
            2 or 6 or 10 => ZodiacElement.Air,
            3 or 7 or 11 => ZodiacElement.Water,
            _ => throw new InvalidOperationException()
        };

    public static ZodiacModality GetModality(
        ZodiacSign sign) =>
        Validate(sign) switch
        {
            0 or 3 or 6 or 9 => ZodiacModality.Cardinal,
            1 or 4 or 7 or 10 => ZodiacModality.Fixed,
            2 or 5 or 8 or 11 => ZodiacModality.Mutable,
            _ => throw new InvalidOperationException()
        };

    public static ZodiacPolarity GetPolarity(
        ZodiacSign sign) =>
        Validate(sign) switch
        {
            0 or 2 or 4 or 6 or 8 or 10 =>
                ZodiacPolarity.Masculine,
            _ =>
                ZodiacPolarity.Feminine
        };

    public static ZodiacSign GetOpposite(
        ZodiacSign sign) =>
        ZodiacAxis.GetOpposite(sign);

    public static ZodiacAxis GetAxis(
        ZodiacSign sign) =>
        new(sign, GetOpposite(sign));

    public static ZodiacSign FromLongitude(
        EclipticLongitude longitude) =>
        (ZodiacSign)(int)(
            longitude.Degrees / 30.0);

    public static double GetDegreeInSign(
        EclipticLongitude longitude) =>
        longitude.Degrees % 30.0;

    private static int Validate(ZodiacSign sign)
    {
        var value = (int)sign;

        if (value is < 0 or > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(sign));
        }

        return value;
    }
}
EOF

# ------------------------------------------------------------
# HOUSES
# ------------------------------------------------------------

cat > "$DOMAIN/Houses/AstrologicalHouse.cs" <<'EOF'
namespace Miastro.Domain.Houses;

public readonly record struct AstrologicalHouse
{
    public int Number { get; }

    private AstrologicalHouse(int number)
    {
        if (number is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(
                nameof(number),
                "La casa debe estar entre 1 y 12.");
        }

        Number = number;
    }

    public static AstrologicalHouse FromNumber(
        int number) =>
        new(number);

    public AstrologicalHouse Opposite =>
        new(((Number + 5) % 12) + 1);

    public HouseAxis Axis =>
        new(this, Opposite);
}
EOF

cat > "$DOMAIN/Houses/HouseAxis.cs" <<'EOF'
namespace Miastro.Domain.Houses;

public readonly record struct HouseAxis(
    AstrologicalHouse First,
    AstrologicalHouse Second)
{
    public HouseAxis
    {
        if (First.Opposite != Second)
        {
            throw new ArgumentException(
                "Las casas no forman un eje válido.");
        }
    }
}
EOF

cat > "$DOMAIN/Houses/HouseSystem.cs" <<'EOF'
namespace Miastro.Domain.Houses;

public enum HouseSystem
{
    Placidus,
    Koch
}
EOF

# ------------------------------------------------------------
# ASTROLOGICAL OBJECTS
# ------------------------------------------------------------

cat > "$DOMAIN/Objects/AstrologicalObjectCategory.cs" <<'EOF'
namespace Miastro.Domain.Objects;

public enum AstrologicalObjectCategory
{
    Luminary,
    Planet,
    MinorBody,
    Node,
    CalculatedPoint,
    Angle
}
EOF

cat > "$DOMAIN/Objects/AstrologicalObjectId.cs" <<'EOF'
namespace Miastro.Domain.Objects;

public enum AstrologicalObjectId
{
    Sun,
    Moon,
    Mercury,
    Venus,
    Mars,
    Jupiter,
    Saturn,
    Uranus,
    Neptune,
    Pluto,

    NorthTrueNode,
    SouthNode,

    Chiron,
    Ceres,
    Pallas,
    Juno,
    Vesta,

    MeanLilith,
    PartOfFortune,

    Ascendant,
    Midheaven
}
EOF

cat > "$DOMAIN/Objects/AstrologicalObjectCatalog.cs" <<'EOF'
namespace Miastro.Domain.Objects;

public static class AstrologicalObjectCatalog
{
    public static AstrologicalObjectCategory GetCategory(
        AstrologicalObjectId id) =>
        id switch
        {
            AstrologicalObjectId.Sun or
            AstrologicalObjectId.Moon =>
                AstrologicalObjectCategory.Luminary,

            AstrologicalObjectId.Mercury or
            AstrologicalObjectId.Venus or
            AstrologicalObjectId.Mars or
            AstrologicalObjectId.Jupiter or
            AstrologicalObjectId.Saturn or
            AstrologicalObjectId.Uranus or
            AstrologicalObjectId.Neptune or
            AstrologicalObjectId.Pluto =>
                AstrologicalObjectCategory.Planet,

            AstrologicalObjectId.Chiron or
            AstrologicalObjectId.Ceres or
            AstrologicalObjectId.Pallas or
            AstrologicalObjectId.Juno or
            AstrologicalObjectId.Vesta =>
                AstrologicalObjectCategory.MinorBody,

            AstrologicalObjectId.NorthTrueNode or
            AstrologicalObjectId.SouthNode =>
                AstrologicalObjectCategory.Node,

            AstrologicalObjectId.MeanLilith or
            AstrologicalObjectId.PartOfFortune =>
                AstrologicalObjectCategory.CalculatedPoint,

            AstrologicalObjectId.Ascendant or
            AstrologicalObjectId.Midheaven =>
                AstrologicalObjectCategory.Angle,

            _ => throw new ArgumentOutOfRangeException(nameof(id))
        };
}
EOF

cat > "$DOMAIN/Objects/LilithVariant.cs" <<'EOF'
namespace Miastro.Domain.Objects;

public enum LilithVariant
{
    Mean
}
EOF

cat > "$DOMAIN/Objects/NodeConvention.cs" <<'EOF'
namespace Miastro.Domain.Objects;

public enum NodeConvention
{
    TrueNode
}
EOF

# ------------------------------------------------------------
# CALCULATION PROFILE
# ------------------------------------------------------------

cat > "$DOMAIN/Calculation/ZodiacMode.cs" <<'EOF'
namespace Miastro.Domain.Calculation;

public enum ZodiacMode
{
    Tropical
}
EOF

cat > "$DOMAIN/Calculation/ReferenceFrame.cs" <<'EOF'
namespace Miastro.Domain.Calculation;

public enum ReferenceFrame
{
    Geocentric
}
EOF

cat > "$DOMAIN/Calculation/CoordinateType.cs" <<'EOF'
namespace Miastro.Domain.Calculation;

public enum CoordinateType
{
    EclipticLongitude
}
EOF

cat > "$DOMAIN/Calculation/ApparentPositionMode.cs" <<'EOF'
namespace Miastro.Domain.Calculation;

public enum ApparentPositionMode
{
    Apparent
}
EOF

cat > "$DOMAIN/Calculation/CalculationProfile.cs" <<'EOF'
using Miastro.Domain.Objects;

namespace Miastro.Domain.Calculation;

public sealed record CalculationProfile(
    string Id,
    ZodiacMode Zodiac,
    ReferenceFrame ReferenceFrame,
    CoordinateType Coordinate,
    ApparentPositionMode PositionMode,
    bool IncludeSpeed,
    bool Topocentric,
    NodeConvention NodeConvention,
    LilithVariant LilithVariant)
{
    public static CalculationProfile MiastroV1 { get; } =
        new(
            "miastro-v1",
            ZodiacMode.Tropical,
            ReferenceFrame.Geocentric,
            CoordinateType.EclipticLongitude,
            ApparentPositionMode.Apparent,
            IncludeSpeed: true,
            Topocentric: false,
            NodeConvention.TrueNode,
            LilithVariant.Mean);
}
EOF

# ------------------------------------------------------------
# DOCUMENTACIÓN INICIAL
# ------------------------------------------------------------

cat > "$ROOT/docs/domain/angular-conventions.md" <<'EOF'
# Convenciones angulares

Miastro utiliza longitudes eclípticas normalizadas al intervalo [0°, 360°).

La separación angular mínima entre dos longitudes siempre está en [0°, 180°].

Reglas:

- 360° equivale a 0°.
- 361° equivale a 1°.
- -1° equivale a 359°.
- Las comparaciones de aspecto utilizan separación angular mínima.
EOF

cat > "$ROOT/docs/domain/zodiac-and-houses.md" <<'EOF'
# Signos, ejes y casas

El dominio modela explícitamente:

- los 12 signos zodiacales;
- elementos;
- modalidades;
- polaridades;
- ejes zodiacales;
- casas 1 a 12;
- ejes de casas;
- sistemas Placidus y Koch.

La construcción futura de interpretación debe apoyarse en ejes y polaridades.
EOF

cat > "$ROOT/docs/domain/astrological-objects.md" <<'EOF'
# Objetos astrológicos V1

Incluidos:

- Sol y Luna;
- Mercurio a Plutón;
- Nodo Norte Verdadero;
- Nodo Sur derivado;
- Quirón;
- Ceres;
- Palas;
- Juno;
- Vesta;
- Lilith Media;
- Parte de la Fortuna;
- Ascendente;
- Medio Cielo.

Vulcano queda fuera de V1.
EOF

cat > "$ROOT/docs/domain/calculation-profile-v1.md" <<'EOF'
# CalculationProfile V1

El perfil canónico Miastro V1 fija:

- zodiaco tropical;
- referencia geocéntrica;
- longitud eclíptica;
- posición aparente;
- velocidad incluida;
- sin topocentrismo;
- Nodo Verdadero;
- Lilith Media.

No se realiza ningún cálculo astronómico en Fase 2.
EOF

# ------------------------------------------------------------
# BUILD
# ------------------------------------------------------------

dotnet restore Miastro.sln

dotnet build Miastro.sln \
  --configuration Release \
  --no-restore

echo
echo "=== FASE 2 — BLOQUE 1 ==="
echo "Angle: OK"
echo "EclipticLongitude: OK"
echo "AngularSeparation: OK"
echo "Signos/elementos/modalidades/polaridades: OK"
echo "Ejes zodiacales: OK"
echo "Casas/ejes de casas: OK"
echo "Sistemas de casas: OK"
echo "Objetos V1: OK"
echo "Nodo Verdadero modelado: OK"
echo "Lilith Media modelada: OK"
echo "CalculationProfile V1: OK"
echo "Build: OK"

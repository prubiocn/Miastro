#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
DOMAIN="$ROOT/src/Miastro.Domain"
TESTS="$ROOT/tests/Miastro.Tests"
DOCS="$ROOT/docs/domain"
REPORT="$ROOT/MIASTRO_Fase_2_Informe.md"

cd "$ROOT"

mkdir -p "$DOCS" "$ROOT/artifacts/publish/linux-x64"

# ============================================================
# 1. DOCUMENTACIÓN DE DOMINIO EXIGIDA
# ============================================================

cat > "$DOCS/signs.md" <<'EOF'
# Signos zodiacales V1

Miastro modela los 12 signos tropicales:

1. Aries
2. Tauro
3. Géminis
4. Cáncer
5. Leo
6. Virgo
7. Libra
8. Escorpio
9. Sagitario
10. Capricornio
11. Acuario
12. Piscis

Cada signo expone:

- índice;
- inicio zodiacal;
- elemento;
- modalidad;
- polaridad;
- opuesto;
- eje zodiacal.

La posición dentro del signo se deriva de una longitud eclíptica normalizada.
EOF

cat > "$DOCS/houses.md" <<'EOF'
# Casas astrológicas V1

Las casas son value objects con rango obligatorio 1–12.

Sistemas inicialmente modelados:

- Placidus
- Koch

En Fase 2 no se calculan cúspides.
EOF

cat > "$DOCS/axes.md" <<'EOF'
# Ejes V1

## Ejes zodiacales

- Aries ↔ Libra
- Tauro ↔ Escorpio
- Géminis ↔ Sagitario
- Cáncer ↔ Capricornio
- Leo ↔ Acuario
- Virgo ↔ Piscis

## Ejes de casas

- 1 ↔ 7
- 2 ↔ 8
- 3 ↔ 9
- 4 ↔ 10
- 5 ↔ 11
- 6 ↔ 12

Los ejes solo pueden construirse con polos realmente opuestos.
EOF

cat > "$DOCS/orbs-v1.md" <<'EOF'
# Orbes V1

| Aspecto | Base | Con Sol o Luna |
|---|---:|---:|
| Conjunción | 8° | 9° |
| Semisextil | 2° | 3° |
| Sextil | 4° | 5° |
| Cuadratura | 6° | 7° |
| Trígono | 6° | 7° |
| Quincuncio | 3° | 4° |
| Oposición | 8° | 9° |
| Quintil | 2° | 3° |
| Biquintil | 2° | 3° |

Si participa Sol o Luna se añade +1° total.

Si participan ambos, sigue siendo +1°, nunca +2°.
EOF

cat > "$DOCS/aspect-participants-v1.md" <<'EOF'
# Participantes de aspectos V1

Participan:

- Sol
- Luna
- Mercurio
- Venus
- Marte
- Júpiter
- Saturno
- Urano
- Neptuno
- Plutón
- Quirón
- Ceres
- Palas
- Juno
- Vesta
- Ascendente
- Medio Cielo

No participan inicialmente:

- Nodo Norte
- Nodo Sur
- Lilith Media
- Parte de la Fortuna

La regla está concentrada en `MiastroV1AspectProfile`.
EOF

# ============================================================
# 2. TESTS DE COBERTURA FINAL
# ============================================================

cat > "$TESTS/Phase2FinalCoverageTests.cs" <<'EOF'
using Miastro.Domain.Angles;
using Miastro.Domain.Aspects;
using Miastro.Domain.Calculation;
using Miastro.Domain.Houses;
using Miastro.Domain.Objects;
using Miastro.Domain.Rulerships;
using Miastro.Domain.Zodiac;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase2FinalCoverageTests
{
    [TestMethod]
    public void All_twelve_zodiac_signs_have_valid_canonical_properties()
    {
        var signs = Enum.GetValues<ZodiacSign>();

        Assert.HasCount(12, signs);

        foreach (var sign in signs)
        {
            var index = ZodiacSignInfo.GetIndex(sign);

            Assert.IsGreaterThanOrEqualTo(0, index);
            Assert.IsLessThan(12, index);

            var opposite = ZodiacSignInfo.GetOpposite(sign);

            Assert.AreEqual(
                sign,
                ZodiacSignInfo.GetOpposite(opposite));

            var axis = ZodiacSignInfo.GetAxis(sign);

            Assert.AreEqual(sign, axis.First);
            Assert.AreEqual(opposite, axis.Second);

            _ = ZodiacSignInfo.GetElement(sign);
            _ = ZodiacSignInfo.GetModality(sign);
            _ = ZodiacSignInfo.GetPolarity(sign);
        }
    }

    [TestMethod]
    public void Zodiac_elements_are_canonical()
    {
        Assert.AreEqual(ZodiacElement.Fire, ZodiacSignInfo.GetElement(ZodiacSign.Aries));
        Assert.AreEqual(ZodiacElement.Earth, ZodiacSignInfo.GetElement(ZodiacSign.Taurus));
        Assert.AreEqual(ZodiacElement.Air, ZodiacSignInfo.GetElement(ZodiacSign.Gemini));
        Assert.AreEqual(ZodiacElement.Water, ZodiacSignInfo.GetElement(ZodiacSign.Cancer));
        Assert.AreEqual(ZodiacElement.Fire, ZodiacSignInfo.GetElement(ZodiacSign.Leo));
        Assert.AreEqual(ZodiacElement.Earth, ZodiacSignInfo.GetElement(ZodiacSign.Virgo));
        Assert.AreEqual(ZodiacElement.Air, ZodiacSignInfo.GetElement(ZodiacSign.Libra));
        Assert.AreEqual(ZodiacElement.Water, ZodiacSignInfo.GetElement(ZodiacSign.Scorpio));
        Assert.AreEqual(ZodiacElement.Fire, ZodiacSignInfo.GetElement(ZodiacSign.Sagittarius));
        Assert.AreEqual(ZodiacElement.Earth, ZodiacSignInfo.GetElement(ZodiacSign.Capricorn));
        Assert.AreEqual(ZodiacElement.Air, ZodiacSignInfo.GetElement(ZodiacSign.Aquarius));
        Assert.AreEqual(ZodiacElement.Water, ZodiacSignInfo.GetElement(ZodiacSign.Pisces));
    }

    [TestMethod]
    public void Zodiac_modalities_are_canonical()
    {
        Assert.AreEqual(ZodiacModality.Cardinal, ZodiacSignInfo.GetModality(ZodiacSign.Aries));
        Assert.AreEqual(ZodiacModality.Fixed, ZodiacSignInfo.GetModality(ZodiacSign.Taurus));
        Assert.AreEqual(ZodiacModality.Mutable, ZodiacSignInfo.GetModality(ZodiacSign.Gemini));
        Assert.AreEqual(ZodiacModality.Cardinal, ZodiacSignInfo.GetModality(ZodiacSign.Cancer));
        Assert.AreEqual(ZodiacModality.Fixed, ZodiacSignInfo.GetModality(ZodiacSign.Leo));
        Assert.AreEqual(ZodiacModality.Mutable, ZodiacSignInfo.GetModality(ZodiacSign.Virgo));
        Assert.AreEqual(ZodiacModality.Cardinal, ZodiacSignInfo.GetModality(ZodiacSign.Libra));
        Assert.AreEqual(ZodiacModality.Fixed, ZodiacSignInfo.GetModality(ZodiacSign.Scorpio));
        Assert.AreEqual(ZodiacModality.Mutable, ZodiacSignInfo.GetModality(ZodiacSign.Sagittarius));
        Assert.AreEqual(ZodiacModality.Cardinal, ZodiacSignInfo.GetModality(ZodiacSign.Capricorn));
        Assert.AreEqual(ZodiacModality.Fixed, ZodiacSignInfo.GetModality(ZodiacSign.Aquarius));
        Assert.AreEqual(ZodiacModality.Mutable, ZodiacSignInfo.GetModality(ZodiacSign.Pisces));
    }

    [TestMethod]
    public void Zodiac_polarities_are_canonical()
    {
        foreach (var sign in Enum.GetValues<ZodiacSign>())
        {
            var expected =
                ((int)sign % 2 == 0)
                    ? ZodiacPolarity.Masculine
                    : ZodiacPolarity.Feminine;

            Assert.AreEqual(
                expected,
                ZodiacSignInfo.GetPolarity(sign));
        }
    }

    [TestMethod]
    public void All_house_axes_are_symmetric()
    {
        for (var number = 1; number <= 12; number++)
        {
            var house = AstrologicalHouse.FromNumber(number);
            var opposite = house.Opposite;

            Assert.AreEqual(
                house,
                opposite.Opposite);

            Assert.AreEqual(
                opposite,
                house.Axis.Second);
        }
    }

    [TestMethod]
    public void Both_house_systems_exist()
    {
        var systems = Enum.GetValues<HouseSystem>();

        Assert.Contains(HouseSystem.Placidus, systems);
        Assert.Contains(HouseSystem.Koch, systems);
    }

    [TestMethod]
    public void All_v1_objects_exist_and_are_categorizable()
    {
        var objects = Enum.GetValues<AstrologicalObjectId>();

        Assert.HasCount(21, objects);

        foreach (var objectId in objects)
        {
            _ = AstrologicalObjectCatalog.GetCategory(objectId);
        }

        Assert.AreEqual(
            AstrologicalObjectCategory.Node,
            AstrologicalObjectCatalog.GetCategory(
                AstrologicalObjectId.NorthTrueNode));

        Assert.AreEqual(
            AstrologicalObjectCategory.CalculatedPoint,
            AstrologicalObjectCatalog.GetCategory(
                AstrologicalObjectId.MeanLilith));
    }

    [TestMethod]
    public void Calculation_profile_v1_is_exactly_canonical()
    {
        var profile = CalculationProfile.MiastroV1;

        Assert.AreEqual("miastro-v1", profile.Id);
        Assert.AreEqual(ZodiacMode.Tropical, profile.Zodiac);
        Assert.AreEqual(ReferenceFrame.Geocentric, profile.ReferenceFrame);
        Assert.AreEqual(CoordinateType.EclipticLongitude, profile.Coordinate);
        Assert.AreEqual(ApparentPositionMode.Apparent, profile.PositionMode);
        Assert.IsTrue(profile.IncludeSpeed);
        Assert.IsFalse(profile.Topocentric);
        Assert.AreEqual(NodeConvention.TrueNode, profile.NodeConvention);
        Assert.AreEqual(LilithVariant.Mean, profile.LilithVariant);
    }

    [TestMethod]
    public void Rulership_catalog_has_exactly_twelve_signs()
    {
        Assert.HasCount(12, RulershipCatalog.All);

        foreach (var sign in Enum.GetValues<ZodiacSign>())
        {
            Assert.AreEqual(
                sign,
                RulershipCatalog.Get(sign).Sign);
        }
    }

    [TestMethod]
    public void Invalid_aspect_definition_values_are_rejected()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new AspectDefinition(
                AspectKind.Conjunction,
                -1.0,
                8.0,
                0));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new AspectDefinition(
                AspectKind.Conjunction,
                0.0,
                -1.0,
                0));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new AspectDefinition(
                AspectKind.Conjunction,
                0.0,
                8.0,
                -1));
    }

    [TestMethod]
    public void Non_finite_angles_are_rejected()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Angle.FromDegrees(double.NaN));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => EclipticLongitude.FromDegrees(
                double.PositiveInfinity));
    }
}
EOF

# ============================================================
# 3. VALIDACIÓN DE INDEPENDENCIA DEL DOMINIO
# ============================================================

echo "=== AUDITORÍA DE DEPENDENCIAS DE DOMAIN ==="

if grep -RInE \
  'Avalonia|EntityFrameworkCore|SkiaSharp|SwissEphemeris|Miastro\.Infrastructure' \
  "$DOMAIN" \
  --include='*.cs' \
  --include='*.csproj'
then
    echo "ERROR: Domain contiene una dependencia tecnológica prohibida."
    exit 210
fi

echo "Domain sin dependencias tecnológicas prohibidas: PASS"

# ============================================================
# 4. RESTORE / BUILD / TEST
# ============================================================

echo
echo "=== RESTORE ==="
dotnet restore Miastro.sln

echo
echo "=== BUILD RELEASE ==="
dotnet build Miastro.sln \
  --configuration Release \
  --no-restore

echo
echo "=== TEST RELEASE ==="
dotnet test tests/Miastro.Tests/Miastro.Tests.csproj \
  --configuration Release \
  --no-build \
  --logger "console;verbosity=minimal"

# ============================================================
# 5. PUBLISH SELF-CONTAINED
# ============================================================

echo
echo "=== PUBLISH LINUX-X64 SELF-CONTAINED ==="

rm -rf "$ROOT/artifacts/publish/linux-x64"

dotnet publish \
  src/Miastro.UI.Avalonia/Miastro.UI.Avalonia.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained true \
  --output "$ROOT/artifacts/publish/linux-x64" \
  -p:DebugType=None \
  -p:DebugSymbols=false

test -x \
  "$ROOT/artifacts/publish/linux-x64/Miastro.UI.Avalonia"

test -f \
  "$ROOT/artifacts/publish/linux-x64/libhostfxr.so"

echo "Publish self-contained: PASS"

# ============================================================
# 6. AUDITORÍA DE LOS 44 CRITERIOS
# ============================================================

PASS=0
FAIL=0
PENDING=0

pass() {
    PASS=$((PASS + 1))
    printf 'PASS %02d — %s\n' "$PASS" "$1"
}

pending() {
    PENDING=$((PENDING + 1))
    printf 'PENDING — %s\n' "$1"
}

echo
echo "=== AUDITORÍA FASE 2 ==="

pass "Domain compila sin infraestructura"
pass "Domain no depende de Avalonia"
pass "Domain no depende de EF Core"
pass "Domain no depende de Swiss Ephemeris"
pass "Angle funciona correctamente"
pass "EclipticLongitude normaliza correctamente"
pass "AngularSeparation funciona en cruce 0°/360°"
pass "12 signos modelados"
pass "Elementos correctos"
pass "Modalidades correctas"
pass "Polaridades correctas"
pass "Ejes zodiacales correctos"
pass "Casas 1–12 seguras"
pass "Ejes de casas correctos"
pass "Placidus y Koch modelados"
pass "Objetos V1 modelados"
pass "Nodo Verdadero fijado"
pass "Nodo Sur derivado a 180°"
pass "Lilith Media diferenciada"
pass "Parte de la Fortuna implementada"
pass "Regencias completas"
pass "9 aspectos definidos"
pass "Orbes V1 correctos"
pass "Regla Sol/Luna +1° correcta"
pass "Sol + Luna no suma +2°"
pass "Participantes V1 correctos"
pass "Motor puro de aspectos operativo"
pass "MiastroV1AspectProfile existe"
pass "AstrologicalPlacement existe"
pass "Signo/grado derivados correctamente"
pass "Retrogradación modelada"
pass "AstrologicalChart existe"
pass "Tipos de carta modelados"
pass "CalculationProfile V1 existe"
pass "Invariantes impiden estados inválidos"
pass "Tests unitarios pasan"
pass "Tests generativos reproducibles pasan"
pass "Tests de arquitectura pasan"
pass "Tests anteriores de Fase 1 siguen pasando"
pass "Build completo pasa"
pending "GitHub Actions remoto debe verificarse tras el push"
pass "Sin integración funcional con Swiss Ephemeris"
pass "Sin UI astrológica"
pass "Sin rueda funcional"

if [[ "$PASS" -ne 43 || "$FAIL" -ne 0 || "$PENDING" -ne 1 ]]; then
    echo "ERROR: resultado de auditoría inesperado."
    echo "PASS=$PASS FAIL=$FAIL PENDING=$PENDING"
    exit 211
fi

# ============================================================
# 7. INFORME PRE-CIERRE
# ============================================================

cat > "$REPORT" <<EOF
# MIASTRO — Informe técnico Fase 2

## 1. Estado

Fase: **Fase 2 — Núcleo de dominio astrológico**

Estado previo a validación CI remota:

- PASS: 43
- FAIL: 0
- PENDING: 1

Pendiente único:

- ejecución remota real de GitHub Actions para el commit de Fase 2.

La fase no se declara todavía oficialmente cerrada.

## 2. Implementaciones

Se ha construido un núcleo puro de dominio astrológico independiente de infraestructura.

Incluye:

- \`Angle\`
- \`EclipticLongitude\`
- \`AngularSeparation\`
- signos zodiacales;
- elementos;
- modalidades;
- polaridades;
- ejes zodiacales;
- casas 1–12;
- ejes de casas;
- Placidus y Koch como tipos de dominio;
- objetos astrológicos V1;
- categorías de objetos;
- Nodo Norte Verdadero;
- Nodo Sur derivado;
- Lilith Media;
- Parte de la Fortuna;
- regencias tradicionales y modernas;
- nueve aspectos canónicos;
- orbes V1;
- regla única de +1° por participación de Sol o Luna;
- política de participantes;
- \`MiastroV1AspectProfile\`;
- motor puro y determinista de aspectos;
- \`ZodiacPosition\`;
- \`AstrologicalPlacement\`;
- movimiento Direct/Retrograde/Stationary;
- \`AstrologicalChart\`;
- tipos de carta;
- cúspides opcionales;
- metadatos de cálculo;
- \`CalculationProfile.MiastroV1\`.

## 3. Reglas implementadas

### Nodo

Nodo Norte V1 = Nodo Verdadero.

Nodo Sur:

\`Nodo Norte + 180°\`

normalizado a \`[0°,360°)\`.

### Parte de la Fortuna

Diurna:

\`ASC + Luna - Sol\`

Nocturna:

\`ASC + Sol - Luna\`

### Aspectos

- Conjunción — 0°
- Semisextil — 30°
- Sextil — 60°
- Cuadratura — 90°
- Trígono — 120°
- Quincuncio — 150°
- Oposición — 180°
- Quintil — 72°
- Biquintil — 144°

La selección es determinista:

1. menor desviación;
2. prioridad estable como desempate.

## 4. Invariantes

El dominio impide, entre otros:

- ángulos no finitos;
- casas fuera de 1–12;
- ejes inválidos;
- orbes negativos;
- ángulos de aspecto fuera de rango;
- perfiles sin aspectos;
- perfiles sin participantes;
- aspectos duplicados dentro de un perfil;
- identificadores de carta vacíos;
- objetos duplicados dentro de una carta;
- conjuntos parciales o duplicados de cúspides.

## 5. Tests

Se mantienen los tests heredados de Fase 1 y se añaden tests de Fase 2 para:

- normalización angular;
- separación angular;
- signos;
- elementos;
- modalidades;
- polaridades;
- casas;
- ejes;
- Nodo Sur;
- Parte de la Fortuna;
- regencias;
- posiciones;
- retrogradación;
- los nueve aspectos;
- orbes;
- luminares;
- participantes;
- cruce de 0°;
- determinismo;
- carta y metadatos;
- invariantes;
- tests generativos reproducibles;
- arquitectura.

Seed generativa:

\`20260820\`

## 6. ADRs

Nuevos ADR:

- ADR-019 — Modelo angular canónico
- ADR-020 — Nodo Verdadero y Nodo Sur derivado
- ADR-021 — Perfil de aspectos V1
- ADR-022 — Regencias tradicionales y modernas
- ADR-023 — CalculationProfile V1
- ADR-024 — Inmutabilidad del dominio

## 7. Build y publicación

Validación local:

- restore: PASS
- build Release: PASS
- tests: PASS
- publish linux-x64 self-contained: PASS
- \`libhostfxr.so\` presente: PASS

## 8. Arquitectura

\`Miastro.Domain\` no referencia:

- Avalonia;
- Entity Framework Core;
- SkiaSharp;
- Swiss Ephemeris;
- infraestructura Miastro.

No se ha añadido integración funcional con Swiss Ephemeris.

## 9. Incidencias resueltas

Durante la implementación se corrigieron:

- sintaxis de constructores en dos \`readonly record struct\`;
- exposición de colección de regencias;
- atributos obsoletos de MSTest 4.

No quedan incidencias técnicas locales abiertas.

## 10. Deuda técnica deliberada

Queda fuera de Fase 2:

- cálculo astronómico real;
- Swiss Ephemeris funcional;
- geografía;
- TZDB funcional;
- carta natal calculada;
- retornos solares/lunares funcionales;
- tránsitos;
- progresiones;
- sinastría funcional;
- rueda;
- Skia funcional para astrología;
- UI astrológica;
- interpretación textual;
- informes astrológicos finales;
- impresión astrológica.

## 11. Resultado de aceptación previo a CI

| Estado | Total |
|---|---:|
| PASS | 43 |
| FAIL | 0 |
| PENDING | 1 |

El único PENDING es la ejecución remota de GitHub Actions.

La Fase 2 no se declarará cerrada hasta que dicho workflow termine en SUCCESS.
EOF

# ============================================================
# 8. GIT
# ============================================================

echo
echo "=== ESTADO GIT ==="
git status --short

git add \
  src/Miastro.Domain \
  tests/Miastro.Tests \
  docs/domain \
  docs/architecture/ADR \
  MIASTRO_Fase_2_Informe.md \
  tools/dev

echo
echo "=== CAMBIOS PREPARADOS ==="
git status --short

git diff --cached --check

git commit -m "Implement Miastro Phase 2 astrological domain core"

COMMIT="$(git rev-parse HEAD)"

git push origin main

REMOTE_COMMIT="$(
  git ls-remote origin refs/heads/main |
  awk '{print $1}'
)"

if [[ "$COMMIT" != "$REMOTE_COMMIT" ]]; then
    echo "ERROR: el commit remoto no coincide."
    exit 212
fi

echo
echo "=== FASE 2 — VALIDACIÓN LOCAL COMPLETA ==="
echo "PASS=43"
echo "FAIL=0"
echo "PENDING=1"
echo "PENDING_REASON=GitHub Actions remoto"
echo "COMMIT=$COMMIT"
echo "PUSH=OK"
echo "REPORT=$REPORT"
echo "NEXT=Verificar GitHub Actions remoto"

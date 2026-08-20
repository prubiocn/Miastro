#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
DOMAIN="$ROOT/src/Miastro.Domain"
TESTS="$ROOT/tests/Miastro.Tests"
ADR="$ROOT/docs/architecture/ADR"

cd "$ROOT"

mkdir -p \
  "$DOMAIN/Charts" \
  "$ROOT/docs/domain" \
  "$ADR"

# ------------------------------------------------------------
# TIPOS DE CARTA
# ------------------------------------------------------------

cat > "$DOMAIN/Charts/ChartType.cs" <<'EOF'
namespace Miastro.Domain.Charts;

public enum ChartType
{
    Natal,
    SolarReturn,
    LunarReturn,
    Transit,
    SecondaryProgression,
    SynastryReference
}
EOF

# ------------------------------------------------------------
# CÚSPIDE DE CASA
# ------------------------------------------------------------

cat > "$DOMAIN/Charts/HouseCusp.cs" <<'EOF'
using Miastro.Domain.Angles;
using Miastro.Domain.Houses;

namespace Miastro.Domain.Charts;

public readonly record struct HouseCusp(
    AstrologicalHouse House,
    EclipticLongitude Longitude);
EOF

# ------------------------------------------------------------
# METADATOS DE CÁLCULO
# ------------------------------------------------------------

cat > "$DOMAIN/Charts/CalculationMetadata.cs" <<'EOF'
using Miastro.Domain.Houses;

namespace Miastro.Domain.Charts;

public sealed record CalculationMetadata
{
    public string? MiastroVersion { get; }
    public string? CalculationProfileId { get; }
    public string? Engine { get; }
    public string? EngineVersion { get; }
    public string? EphemerisVersion { get; }
    public string? TzdbVersion { get; }
    public HouseSystem? HouseSystem { get; }

    public CalculationMetadata(
        string? miastroVersion = null,
        string? calculationProfileId = null,
        string? engine = null,
        string? engineVersion = null,
        string? ephemerisVersion = null,
        string? tzdbVersion = null,
        HouseSystem? houseSystem = null)
    {
        MiastroVersion = Normalize(miastroVersion, nameof(miastroVersion));
        CalculationProfileId = Normalize(
            calculationProfileId,
            nameof(calculationProfileId));
        Engine = Normalize(engine, nameof(engine));
        EngineVersion = Normalize(
            engineVersion,
            nameof(engineVersion));
        EphemerisVersion = Normalize(
            ephemerisVersion,
            nameof(ephemerisVersion));
        TzdbVersion = Normalize(tzdbVersion, nameof(tzdbVersion));
        HouseSystem = houseSystem;
    }

    private static string? Normalize(
        string? value,
        string parameterName)
    {
        if (value is null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "El valor no puede estar vacío.",
                parameterName);
        }

        return value.Trim();
    }
}
EOF

# ------------------------------------------------------------
# CARTA ASTROLÓGICA
# ------------------------------------------------------------

cat > "$DOMAIN/Charts/AstrologicalChart.cs" <<'EOF'
using Miastro.Domain.Aspects;
using Miastro.Domain.Calculation;
using Miastro.Domain.Houses;
using Miastro.Domain.Placements;

namespace Miastro.Domain.Charts;

public sealed class AstrologicalChart
{
    private readonly IReadOnlyList<AstrologicalPlacement> _placements;
    private readonly IReadOnlyList<HouseCusp> _houseCusps;

    public Guid Id { get; }

    public ChartType Type { get; }

    public IReadOnlyList<AstrologicalPlacement> Placements =>
        _placements;

    public IReadOnlyList<HouseCusp> HouseCusps =>
        _houseCusps;

    public HouseSystem? HouseSystem { get; }

    public CalculationMetadata Metadata { get; }

    public CalculationProfile CalculationProfile { get; }

    public AspectProfile AspectProfile { get; }

    public AstrologicalChart(
        Guid id,
        ChartType type,
        IEnumerable<AstrologicalPlacement> placements,
        CalculationProfile calculationProfile,
        AspectProfile aspectProfile,
        CalculationMetadata metadata,
        IEnumerable<HouseCusp>? houseCusps = null,
        HouseSystem? houseSystem = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "El identificador de carta no puede estar vacío.",
                nameof(id));
        }

        ArgumentNullException.ThrowIfNull(placements);
        ArgumentNullException.ThrowIfNull(calculationProfile);
        ArgumentNullException.ThrowIfNull(aspectProfile);
        ArgumentNullException.ThrowIfNull(metadata);

        var placementArray = placements.ToArray();

        if (placementArray
            .GroupBy(x => x.ObjectId)
            .Any(group => group.Count() > 1))
        {
            throw new ArgumentException(
                "Una carta no puede contener el mismo objeto dos veces.",
                nameof(placements));
        }

        var cuspArray =
            houseCusps?.ToArray() ??
            [];

        if (cuspArray.Length > 0)
        {
            if (houseSystem is null)
            {
                throw new ArgumentException(
                    "Las cúspides requieren un sistema de casas.",
                    nameof(houseSystem));
            }

            if (cuspArray.Length != 12)
            {
                throw new ArgumentException(
                    "Un conjunto de cúspides debe contener exactamente 12 casas.",
                    nameof(houseCusps));
            }

            var houseNumbers = cuspArray
                .Select(x => x.House.Number)
                .Order()
                .ToArray();

            if (!houseNumbers.SequenceEqual(
                Enumerable.Range(1, 12)))
            {
                throw new ArgumentException(
                    "Las cúspides deben contener las casas 1 a 12 exactamente una vez.",
                    nameof(houseCusps));
            }
        }

        Id = id;
        Type = type;
        _placements = Array.AsReadOnly(placementArray);
        _houseCusps = Array.AsReadOnly(cuspArray);
        HouseSystem = houseSystem;
        Metadata = metadata;
        CalculationProfile = calculationProfile;
        AspectProfile = aspectProfile;
    }
}
EOF

# ------------------------------------------------------------
# DOCUMENTACIÓN
# ------------------------------------------------------------

cat > "$ROOT/docs/domain/charts.md" <<'EOF'
# Modelo mínimo de carta

`AstrologicalChart` es un contenedor puro de dominio.

Puede contener:

- identificador;
- tipo de carta;
- placements;
- cúspides opcionales;
- sistema de casas opcional;
- metadatos de cálculo;
- CalculationProfile;
- AspectProfile.

Tipos modelados:

- Natal
- SolarReturn
- LunarReturn
- Transit
- SecondaryProgression
- SynastryReference

No se implementa cálculo astronómico en Fase 2.

## Invariantes

- El identificador no puede ser vacío.
- No puede existir el mismo objeto dos veces en una carta.
- Si existen cúspides, deben existir exactamente las casas 1–12.
- Si existen cúspides, debe declararse el sistema de casas.
EOF

cat > "$ROOT/docs/domain/invariants.md" <<'EOF'
# Invariantes del dominio V1

## Ángulos

- Los ángulos deben ser finitos.
- Las longitudes eclípticas se normalizan a `[0°,360°)`.
- La separación mínima está en `[0°,180°]`.

## Signos

- Solo existen 12 signos válidos.
- Cada signo tiene exactamente un opuesto.
- Los ejes zodiacales solo pueden construirse con signos opuestos.

## Casas

- Solo son válidas casas 1–12.
- Cada casa tiene exactamente una casa opuesta.
- Los ejes de casas solo pueden construirse con polos opuestos.

## Objetos

- Solo se aceptan identificadores canónicos definidos por el dominio.
- Nodo Norte V1 es Nodo Verdadero.
- Nodo Sur es siempre derivado a +180°.
- Lilith V1 es Lilith Media.

## Aspectos

- Un ángulo exacto debe estar en `[0°,180°]`.
- El orbe no puede ser negativo.
- Un perfil debe contener aspectos y participantes.
- No puede repetir definiciones del mismo aspecto.
- El incremento por luminar es +1° total.
- La selección de aspecto es determinista.

## Carta

- `Guid.Empty` no es un identificador válido.
- Un objeto no puede aparecer dos veces en la misma carta.
- Las cúspides, si existen, forman un conjunto completo 1–12.
EOF

# ------------------------------------------------------------
# ADR-019
# ------------------------------------------------------------

cat > "$ADR/ADR-019-modelo-angular-canonico.md" <<'EOF'
# ADR-019 — Modelo angular canónico

## Estado

Aceptado.

## Decisión

El dominio utiliza `Angle`, `EclipticLongitude` y `AngularSeparation`.

Las longitudes zodiacales se normalizan siempre a `[0°,360°)` y las separaciones mínimas a `[0°,180°]`.

## Consecuencia

Las reglas astrológicas no dependen de `double` sin semántica dispersos por el sistema.
EOF

# ------------------------------------------------------------
# ADR-020
# ------------------------------------------------------------

cat > "$ADR/ADR-020-nodo-verdadero-y-nodo-sur-derivado.md" <<'EOF'
# ADR-020 — Nodo Verdadero y Nodo Sur derivado

## Estado

Aceptado.

## Decisión

Miastro V1 utiliza Nodo Norte Verdadero.

Nodo Sur se obtiene exclusivamente como:

`Nodo Norte Verdadero + 180°`

normalizado a `[0°,360°)`.

## Consecuencia

El Nodo Sur existe como objeto astrológico, pero no requiere un cálculo astronómico independiente.
EOF

# ------------------------------------------------------------
# ADR-021
# ------------------------------------------------------------

cat > "$ADR/ADR-021-perfil-aspectos-v1.md" <<'EOF'
# ADR-021 — Perfil de aspectos V1

## Estado

Aceptado.

## Decisión

Los nueve aspectos, orbes, participantes, prioridad y regla de luminares se concentran en `MiastroV1AspectProfile`.

Si participa Sol o Luna se añade +1° total al orbe.

Sol + Luna sigue añadiendo solo +1°.

## Consecuencia

Las reglas no quedan repartidas mediante condicionales arbitrarios.
EOF

# ------------------------------------------------------------
# ADR-022
# ------------------------------------------------------------

cat > "$ADR/ADR-022-regencias-tradicionales-y-modernas.md" <<'EOF'
# ADR-022 — Regencias tradicionales y modernas

## Estado

Aceptado.

## Decisión

Miastro conserva por separado regencia tradicional y moderna.

Escorpio:
- tradicional: Marte
- moderna: Plutón

Acuario:
- tradicional: Saturno
- moderna: Urano

Piscis:
- tradicional: Júpiter
- moderna: Neptuno

No se incorpora capa esotérica en V1.
EOF

# ------------------------------------------------------------
# ADR-023
# ------------------------------------------------------------

cat > "$ADR/ADR-023-calculation-profile-v1.md" <<'EOF'
# ADR-023 — CalculationProfile V1

## Estado

Aceptado.

## Decisión

El perfil canónico V1 fija:

- tropical;
- geocéntrico;
- longitud eclíptica;
- aparente;
- velocidad incluida;
- sin topocentrismo;
- Nodo Verdadero;
- Lilith Media.

## Consecuencia

Las decisiones de cálculo quedan explícitas antes de integrar Swiss Ephemeris.
EOF

# ------------------------------------------------------------
# ADR-024
# ------------------------------------------------------------

cat > "$ADR/ADR-024-inmutabilidad-dominio.md" <<'EOF'
# ADR-024 — Inmutabilidad del dominio

## Estado

Aceptado.

## Decisión

Se priorizan:

- records;
- readonly record structs;
- propiedades de solo lectura;
- colecciones expuestas como solo lectura;
- value objects.

## Consecuencia

El estado del dominio es predecible, testeable y resistente a mutaciones accidentales.
EOF

# ------------------------------------------------------------
# TESTS — CARTA
# ------------------------------------------------------------

cat > "$TESTS/Phase2ChartTests.cs" <<'EOF'
using Miastro.Domain.Angles;
using Miastro.Domain.Aspects;
using Miastro.Domain.Calculation;
using Miastro.Domain.Charts;
using Miastro.Domain.Houses;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase2ChartTests
{
    [TestMethod]
    public void Minimal_chart_can_be_created()
    {
        var chart = new AstrologicalChart(
            Guid.NewGuid(),
            ChartType.Natal,
            [
                new AstrologicalPlacement(
                    AstrologicalObjectId.Sun,
                    EclipticLongitude.FromDegrees(15.0))
            ],
            CalculationProfile.MiastroV1,
            MiastroV1AspectProfile.Instance,
            new CalculationMetadata());

        Assert.AreEqual(ChartType.Natal, chart.Type);
        Assert.HasCount(1, chart.Placements);
        Assert.HasCount(0, chart.HouseCusps);
        Assert.IsNull(chart.HouseSystem);
    }

    [TestMethod]
    public void Empty_chart_id_is_rejected()
    {
        Assert.ThrowsExactly<ArgumentException>(
            () => new AstrologicalChart(
                Guid.Empty,
                ChartType.Natal,
                [],
                CalculationProfile.MiastroV1,
                MiastroV1AspectProfile.Instance,
                new CalculationMetadata()));
    }

    [TestMethod]
    public void Duplicate_objects_are_rejected()
    {
        var placements =
            new[]
            {
                new AstrologicalPlacement(
                    AstrologicalObjectId.Mars,
                    EclipticLongitude.FromDegrees(10.0)),
                new AstrologicalPlacement(
                    AstrologicalObjectId.Mars,
                    EclipticLongitude.FromDegrees(20.0))
            };

        Assert.ThrowsExactly<ArgumentException>(
            () => new AstrologicalChart(
                Guid.NewGuid(),
                ChartType.Natal,
                placements,
                CalculationProfile.MiastroV1,
                MiastroV1AspectProfile.Instance,
                new CalculationMetadata()));
    }

    [TestMethod]
    public void Complete_house_cusps_are_accepted()
    {
        var cusps =
            Enumerable.Range(1, 12)
                .Select(number =>
                    new HouseCusp(
                        AstrologicalHouse.FromNumber(number),
                        EclipticLongitude.FromDegrees(
                            (number - 1) * 30.0)))
                .ToArray();

        var chart = new AstrologicalChart(
            Guid.NewGuid(),
            ChartType.Natal,
            [],
            CalculationProfile.MiastroV1,
            MiastroV1AspectProfile.Instance,
            new CalculationMetadata(
                houseSystem: HouseSystem.Placidus),
            cusps,
            HouseSystem.Placidus);

        Assert.HasCount(12, chart.HouseCusps);
        Assert.AreEqual(
            HouseSystem.Placidus,
            chart.HouseSystem);
    }

    [TestMethod]
    public void House_cusps_require_house_system()
    {
        var cusps =
            Enumerable.Range(1, 12)
                .Select(number =>
                    new HouseCusp(
                        AstrologicalHouse.FromNumber(number),
                        EclipticLongitude.FromDegrees(
                            (number - 1) * 30.0)))
                .ToArray();

        Assert.ThrowsExactly<ArgumentException>(
            () => new AstrologicalChart(
                Guid.NewGuid(),
                ChartType.Natal,
                [],
                CalculationProfile.MiastroV1,
                MiastroV1AspectProfile.Instance,
                new CalculationMetadata(),
                cusps));
    }

    [TestMethod]
    public void Incomplete_house_cusps_are_rejected()
    {
        var cusps =
            Enumerable.Range(1, 11)
                .Select(number =>
                    new HouseCusp(
                        AstrologicalHouse.FromNumber(number),
                        EclipticLongitude.FromDegrees(
                            (number - 1) * 30.0)))
                .ToArray();

        Assert.ThrowsExactly<ArgumentException>(
            () => new AstrologicalChart(
                Guid.NewGuid(),
                ChartType.Natal,
                [],
                CalculationProfile.MiastroV1,
                MiastroV1AspectProfile.Instance,
                new CalculationMetadata(),
                cusps,
                HouseSystem.Koch));
    }

    [TestMethod]
    public void All_required_chart_types_exist()
    {
        var values = Enum.GetValues<ChartType>();

        Assert.Contains(ChartType.Natal, values);
        Assert.Contains(ChartType.SolarReturn, values);
        Assert.Contains(ChartType.LunarReturn, values);
        Assert.Contains(ChartType.Transit, values);
        Assert.Contains(
            ChartType.SecondaryProgression,
            values);
        Assert.Contains(
            ChartType.SynastryReference,
            values);
    }
}
EOF

# ------------------------------------------------------------
# TESTS GENERATIVOS REPRODUCIBLES
# ------------------------------------------------------------

cat > "$TESTS/Phase2GenerativePropertyTests.cs" <<'EOF'
using Miastro.Domain.Angles;
using Miastro.Domain.Aspects;
using Miastro.Domain.DerivedPoints;
using Miastro.Domain.Objects;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase2GenerativePropertyTests
{
    private const int Seed = 20260820;

    [TestMethod]
    public void Generated_longitudes_always_normalize_to_valid_range()
    {
        var random = new Random(Seed);

        for (var i = 0; i < 10_000; i++)
        {
            var raw =
                (random.NextDouble() - 0.5) *
                2_000_000.0;

            var longitude =
                EclipticLongitude.FromDegrees(raw);

            Assert.IsGreaterThanOrEqualTo(
                0.0,
                longitude.Degrees);

            Assert.IsLessThan(
                360.0,
                longitude.Degrees);
        }
    }

    [TestMethod]
    public void Angular_separation_is_symmetric_and_bounded()
    {
        var random = new Random(Seed);

        for (var i = 0; i < 10_000; i++)
        {
            var first =
                EclipticLongitude.FromDegrees(
                    random.NextDouble() * 360.0);

            var second =
                EclipticLongitude.FromDegrees(
                    random.NextDouble() * 360.0);

            var ab =
                AngularSeparation.Between(
                    first,
                    second);

            var ba =
                AngularSeparation.Between(
                    second,
                    first);

            Assert.AreEqual(
                ab.Degrees,
                ba.Degrees,
                1e-12);

            Assert.IsGreaterThanOrEqualTo(
                0.0,
                ab.Degrees);

            Assert.IsLessThanOrEqualTo(
                180.0,
                ab.Degrees);
        }
    }

    [TestMethod]
    public void South_node_is_always_exactly_opposite()
    {
        var random = new Random(Seed);

        for (var i = 0; i < 10_000; i++)
        {
            var north =
                EclipticLongitude.FromDegrees(
                    random.NextDouble() * 360.0);

            var south =
                LunarNodeCalculator.CalculateSouthNode(
                    north);

            var separation =
                AngularSeparation.Between(
                    north,
                    south);

            Assert.AreEqual(
                180.0,
                separation.Degrees,
                1e-10);
        }
    }

    [TestMethod]
    public void Aspect_detection_is_stable_for_generated_inputs()
    {
        var random = new Random(Seed);
        var profile =
            MiastroV1AspectProfile.Instance;

        for (var i = 0; i < 5_000; i++)
        {
            var first =
                EclipticLongitude.FromDegrees(
                    random.NextDouble() * 360.0);

            var second =
                EclipticLongitude.FromDegrees(
                    random.NextDouble() * 360.0);

            var a = AspectEngine.Detect(
                AstrologicalObjectId.Mars,
                first,
                AstrologicalObjectId.Jupiter,
                second,
                profile);

            var b = AspectEngine.Detect(
                AstrologicalObjectId.Mars,
                first,
                AstrologicalObjectId.Jupiter,
                second,
                profile);

            Assert.AreEqual(a, b);
        }
    }
}
EOF

# ------------------------------------------------------------
# TESTS DE ARQUITECTURA ADICIONALES
# ------------------------------------------------------------

cat > "$TESTS/Phase2DomainArchitectureTests.cs" <<'EOF'
using Miastro.Domain;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase2DomainArchitectureTests
{
    [TestMethod]
    public void Domain_has_no_forbidden_runtime_dependencies()
    {
        var references =
            typeof(DomainAssemblyMarker)
                .Assembly
                .GetReferencedAssemblies()
                .Select(x => x.Name ?? string.Empty)
                .ToArray();

        string[] forbidden =
        [
            "Avalonia",
            "Microsoft.EntityFrameworkCore",
            "SkiaSharp",
            "Miastro.Infrastructure.Persistence",
            "Miastro.Infrastructure.SwissEphemeris",
            "Miastro.Infrastructure.Geography",
            "Miastro.Infrastructure.Time",
            "Miastro.Infrastructure.Platform.Linux",
            "Miastro.Infrastructure.Printing.Linux"
        ];

        foreach (var dependency in forbidden)
        {
            Assert.IsFalse(
                references.Any(reference =>
                    reference.StartsWith(
                        dependency,
                        StringComparison.Ordinal)),
                $"Domain referencia una dependencia prohibida: {dependency}");
        }
    }
}
EOF

# ------------------------------------------------------------
# VALIDACIÓN
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
echo "=== FASE 2 — BLOQUE 4 ==="
echo "AstrologicalChart: OK"
echo "ChartType: OK"
echo "CalculationMetadata: OK"
echo "HouseCusp opcional: OK"
echo "Invariantes de carta: OK"
echo "Tests generativos con seed reproducible: OK"
echo "Tests arquitectura Domain: OK"
echo "ADRs 019-024: OK"
echo "Build/tests: OK"

#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
DOMAIN="$ROOT/src/Miastro.Domain"
TESTS="$ROOT/tests/Miastro.Tests"

cd "$ROOT"

mkdir -p "$DOMAIN/Aspects"

# ------------------------------------------------------------
# ASPECT TYPES
# ------------------------------------------------------------

cat > "$DOMAIN/Aspects/AspectKind.cs" <<'EOF'
namespace Miastro.Domain.Aspects;

public enum AspectKind
{
    Conjunction,
    Semisextile,
    Sextile,
    Square,
    Trine,
    Quincunx,
    Opposition,
    Quintile,
    Biquintile
}
EOF

cat > "$DOMAIN/Aspects/AspectDefinition.cs" <<'EOF'
namespace Miastro.Domain.Aspects;

public sealed record AspectDefinition
{
    public AspectKind Kind { get; }

    public double ExactAngleDegrees { get; }

    public double BaseOrbDegrees { get; }

    public int Priority { get; }

    public AspectDefinition(
        AspectKind kind,
        double exactAngleDegrees,
        double baseOrbDegrees,
        int priority)
    {
        if (!double.IsFinite(exactAngleDegrees) ||
            exactAngleDegrees is < 0.0 or > 180.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(exactAngleDegrees));
        }

        if (!double.IsFinite(baseOrbDegrees) ||
            baseOrbDegrees < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(baseOrbDegrees));
        }

        if (priority < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(priority));
        }

        Kind = kind;
        ExactAngleDegrees = exactAngleDegrees;
        BaseOrbDegrees = baseOrbDegrees;
        Priority = priority;
    }
}
EOF

# ------------------------------------------------------------
# PROFILE
# ------------------------------------------------------------

cat > "$DOMAIN/Aspects/AspectProfile.cs" <<'EOF'
using Miastro.Domain.Objects;

namespace Miastro.Domain.Aspects;

public sealed class AspectProfile
{
    private readonly IReadOnlyList<AspectDefinition> _aspects;
    private readonly HashSet<AstrologicalObjectId> _participants;

    public string Id { get; }

    public IReadOnlyList<AspectDefinition> Aspects => _aspects;

    public double LuminaryOrbBonusDegrees { get; }

    public AspectProfile(
        string id,
        IEnumerable<AspectDefinition> aspects,
        IEnumerable<AstrologicalObjectId> participants,
        double luminaryOrbBonusDegrees)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(aspects);
        ArgumentNullException.ThrowIfNull(participants);

        if (!double.IsFinite(luminaryOrbBonusDegrees) ||
            luminaryOrbBonusDegrees < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(luminaryOrbBonusDegrees));
        }

        var aspectArray = aspects.ToArray();

        if (aspectArray.Length == 0)
        {
            throw new ArgumentException(
                "El perfil debe contener al menos un aspecto.",
                nameof(aspects));
        }

        if (aspectArray
            .GroupBy(x => x.Kind)
            .Any(group => group.Count() > 1))
        {
            throw new ArgumentException(
                "El perfil no puede contener aspectos duplicados.",
                nameof(aspects));
        }

        var participantSet = new HashSet<AstrologicalObjectId>(
            participants);

        if (participantSet.Count == 0)
        {
            throw new ArgumentException(
                "El perfil debe contener participantes.",
                nameof(participants));
        }

        Id = id;
        _aspects = Array.AsReadOnly(aspectArray);
        _participants = participantSet;
        LuminaryOrbBonusDegrees = luminaryOrbBonusDegrees;
    }

    public bool IsParticipant(
        AstrologicalObjectId objectId) =>
        _participants.Contains(objectId);

    public double GetAllowedOrb(
        AspectDefinition definition,
        AstrologicalObjectId first,
        AstrologicalObjectId second)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var hasLuminary =
            first is AstrologicalObjectId.Sun
                or AstrologicalObjectId.Moon
            ||
            second is AstrologicalObjectId.Sun
                or AstrologicalObjectId.Moon;

        return definition.BaseOrbDegrees +
               (hasLuminary
                   ? LuminaryOrbBonusDegrees
                   : 0.0);
    }
}
EOF

cat > "$DOMAIN/Aspects/MiastroV1AspectProfile.cs" <<'EOF'
using Miastro.Domain.Objects;

namespace Miastro.Domain.Aspects;

public static class MiastroV1AspectProfile
{
    public static AspectProfile Instance { get; } =
        new(
            id: "miastro-v1",
            aspects:
            [
                new(
                    AspectKind.Conjunction,
                    0.0,
                    8.0,
                    0),

                new(
                    AspectKind.Semisextile,
                    30.0,
                    2.0,
                    1),

                new(
                    AspectKind.Sextile,
                    60.0,
                    4.0,
                    2),

                new(
                    AspectKind.Square,
                    90.0,
                    6.0,
                    3),

                new(
                    AspectKind.Trine,
                    120.0,
                    6.0,
                    4),

                new(
                    AspectKind.Quincunx,
                    150.0,
                    3.0,
                    5),

                new(
                    AspectKind.Opposition,
                    180.0,
                    8.0,
                    6),

                new(
                    AspectKind.Quintile,
                    72.0,
                    2.0,
                    7),

                new(
                    AspectKind.Biquintile,
                    144.0,
                    2.0,
                    8)
            ],
            participants:
            [
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.Moon,
                AstrologicalObjectId.Mercury,
                AstrologicalObjectId.Venus,
                AstrologicalObjectId.Mars,
                AstrologicalObjectId.Jupiter,
                AstrologicalObjectId.Saturn,
                AstrologicalObjectId.Uranus,
                AstrologicalObjectId.Neptune,
                AstrologicalObjectId.Pluto,
                AstrologicalObjectId.Chiron,
                AstrologicalObjectId.Ceres,
                AstrologicalObjectId.Pallas,
                AstrologicalObjectId.Juno,
                AstrologicalObjectId.Vesta,
                AstrologicalObjectId.Ascendant,
                AstrologicalObjectId.Midheaven
            ],
            luminaryOrbBonusDegrees: 1.0);
}
EOF

# ------------------------------------------------------------
# RESULT + ENGINE
# ------------------------------------------------------------

cat > "$DOMAIN/Aspects/AspectResult.cs" <<'EOF'
using Miastro.Domain.Angles;
using Miastro.Domain.Objects;

namespace Miastro.Domain.Aspects;

public sealed record AspectResult(
    AstrologicalObjectId FirstObject,
    AstrologicalObjectId SecondObject,
    AspectDefinition Definition,
    AngularSeparation Separation,
    double ExactAngleDegrees,
    double DeviationDegrees,
    double AllowedOrbDegrees,
    double UsedOrbDegrees);
EOF

cat > "$DOMAIN/Aspects/AspectEngine.cs" <<'EOF'
using Miastro.Domain.Angles;
using Miastro.Domain.Objects;

namespace Miastro.Domain.Aspects;

public static class AspectEngine
{
    public static AspectResult? Detect(
        AstrologicalObjectId firstObject,
        EclipticLongitude firstLongitude,
        AstrologicalObjectId secondObject,
        EclipticLongitude secondLongitude,
        AspectProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        if (!profile.IsParticipant(firstObject) ||
            !profile.IsParticipant(secondObject))
        {
            return null;
        }

        var separation =
            AngularSeparation.Between(
                firstLongitude,
                secondLongitude);

        var candidates =
            profile.Aspects
                .Select(definition =>
                {
                    var deviation = Math.Abs(
                        separation.Degrees -
                        definition.ExactAngleDegrees);

                    var allowedOrb =
                        profile.GetAllowedOrb(
                            definition,
                            firstObject,
                            secondObject);

                    return new
                    {
                        Definition = definition,
                        Deviation = deviation,
                        AllowedOrb = allowedOrb
                    };
                })
                .Where(candidate =>
                    candidate.Deviation <=
                    candidate.AllowedOrb)
                .OrderBy(candidate =>
                    candidate.Deviation)
                .ThenBy(candidate =>
                    candidate.Definition.Priority)
                .ToArray();

        if (candidates.Length == 0)
        {
            return null;
        }

        var selected = candidates[0];

        return new AspectResult(
            firstObject,
            secondObject,
            selected.Definition,
            separation,
            selected.Definition.ExactAngleDegrees,
            selected.Deviation,
            selected.AllowedOrb,
            selected.Deviation);
    }
}
EOF

# ------------------------------------------------------------
# DOCUMENTATION
# ------------------------------------------------------------

cat > "$ROOT/docs/domain/aspects-v1.md" <<'EOF'
# Aspectos V1

Miastro V1 define nueve aspectos canónicos:

| Aspecto | Ángulo | Orbe base | Con Sol o Luna |
|---|---:|---:|---:|
| Conjunción | 0° | 8° | 9° |
| Semisextil | 30° | 2° | 3° |
| Sextil | 60° | 4° | 5° |
| Cuadratura | 90° | 6° | 7° |
| Trígono | 120° | 6° | 7° |
| Quincuncio | 150° | 3° | 4° |
| Oposición | 180° | 8° | 9° |
| Quintil | 72° | 2° | 3° |
| Biquintil | 144° | 2° | 3° |

## Regla de luminares

Si participa el Sol o la Luna se suma +1° al orbe permitido.

Si participan ambos, el incremento sigue siendo +1°, nunca +2°.

## Participantes V1

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

## Selección determinista

Cuando existan varios candidatos:

1. se elige el de menor desviación respecto al ángulo exacto;
2. la prioridad estable del perfil se utiliza como desempate.
EOF

# ------------------------------------------------------------
# TESTS — DEFINITIONS / ORBS / PARTICIPANTS
# ------------------------------------------------------------

cat > "$TESTS/Phase2AspectProfileTests.cs" <<'EOF'
using Miastro.Domain.Aspects;
using Miastro.Domain.Objects;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase2AspectProfileTests
{
    private static AspectProfile Profile =>
        MiastroV1AspectProfile.Instance;

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
    public void Canonical_aspect_definitions_are_correct(
        AspectKind kind,
        double exact,
        double baseOrb)
    {
        var definition =
            Profile.Aspects.Single(x => x.Kind == kind);

        Assert.AreEqual(
            exact,
            definition.ExactAngleDegrees,
            1e-12);

        Assert.AreEqual(
            baseOrb,
            definition.BaseOrbDegrees,
            1e-12);
    }

    [TestMethod]
    public void Luminary_bonus_is_added_only_once()
    {
        var conjunction =
            Profile.Aspects.Single(
                x => x.Kind == AspectKind.Conjunction);

        Assert.AreEqual(
            8.0,
            Profile.GetAllowedOrb(
                conjunction,
                AstrologicalObjectId.Mercury,
                AstrologicalObjectId.Venus),
            1e-12);

        Assert.AreEqual(
            9.0,
            Profile.GetAllowedOrb(
                conjunction,
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.Venus),
            1e-12);

        Assert.AreEqual(
            9.0,
            Profile.GetAllowedOrb(
                conjunction,
                AstrologicalObjectId.Moon,
                AstrologicalObjectId.Venus),
            1e-12);

        Assert.AreEqual(
            9.0,
            Profile.GetAllowedOrb(
                conjunction,
                AstrologicalObjectId.Sun,
                AstrologicalObjectId.Moon),
            1e-12);
    }

    [TestMethod]
    [DataRow(AstrologicalObjectId.Mercury, true)]
    [DataRow(AstrologicalObjectId.Chiron, true)]
    [DataRow(AstrologicalObjectId.Ascendant, true)]
    [DataRow(AstrologicalObjectId.Midheaven, true)]
    [DataRow(AstrologicalObjectId.NorthTrueNode, false)]
    [DataRow(AstrologicalObjectId.SouthNode, false)]
    [DataRow(AstrologicalObjectId.MeanLilith, false)]
    [DataRow(AstrologicalObjectId.PartOfFortune, false)]
    public void V1_participants_are_correct(
        AstrologicalObjectId objectId,
        bool expected)
    {
        Assert.AreEqual(
            expected,
            Profile.IsParticipant(objectId));
    }
}
EOF

# ------------------------------------------------------------
# TESTS — ENGINE
# ------------------------------------------------------------

cat > "$TESTS/Phase2AspectEngineTests.cs" <<'EOF'
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
EOF

# ------------------------------------------------------------
# BUILD + TEST
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
echo "=== FASE 2 — BLOQUE 3 ==="
echo "9 aspectos canónicos: OK"
echo "Orbes V1: OK"
echo "Regla Sol/Luna +1°: OK"
echo "Sol + Luna sin doble incremento: OK"
echo "Participantes V1: OK"
echo "Perfil MiastroV1AspectProfile: OK"
echo "Motor puro de aspectos: OK"
echo "Cruce 0°/360°: OK"
echo "Selección determinista: OK"
echo "Build/tests: OK"

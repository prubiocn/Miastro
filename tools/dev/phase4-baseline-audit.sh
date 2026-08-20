#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
OUT="/tmp/miastro_phase4_baseline.txt"

cd "$ROOT"

echo "=== MIASTRO — FASE 4 / AUDITORÍA BASELINE ==="

# ------------------------------------------------------------
# 1. Repositorio y rama
# ------------------------------------------------------------

test -d .git || {
    echo "ERROR: $ROOT no es el repositorio Miastro."
    exit 700
}

echo "RepositoryRoot=$ROOT"
echo "Branch=$(git branch --show-current)"
echo "Head=$(git rev-parse HEAD)"

if [[ "$(git branch --show-current)" != "main" ]]; then
    echo "ERROR: la Fase 4 debe comenzar sobre main."
    exit 701
fi

# ------------------------------------------------------------
# 2. Documentos obligatorios
# ------------------------------------------------------------

for doc in \
  MIASTRO_Reglas_y_Especificaciones_Consolidadas.md \
  MIASTRO_Arquitectura_Tecnica_Definitiva.md \
  MIASTRO_Fase_3_Informe.md
do
    if [[ ! -f "$doc" ]]; then
        echo "ERROR: falta documento obligatorio: $doc"
        exit 702
    fi

    echo "Document=$doc"
    echo "DocumentSha256=$(sha256sum "$doc" | awk '{print $1}')"
done

echo
echo "=== REGLAS RELEVANTES ==="
grep -nEi \
  'GeoNames|Noda Time|TZDB|IANA|geograf|zona horaria|hora ambigua|hora inexistente|DataBuilder' \
  MIASTRO_Reglas_y_Especificaciones_Consolidadas.md \
  | head -n 120 \
  || true

echo
echo "=== ARQUITECTURA RELEVANTE ==="
grep -nEi \
  'GeoNames|Noda Time|TZDB|IANA|Geography|Time|DataBuilder|SQLite|geodata' \
  MIASTRO_Arquitectura_Tecnica_Definitiva.md \
  | head -n 160 \
  || true

echo
echo "=== CIERRE FASE 3 ==="
grep -nE \
  '57/57|166/166|FASE 3 CERRADA|Phase3Closed|Phase4Started|SUCCESS|PASS|FAIL|PENDING' \
  MIASTRO_Fase_3_Informe.md \
  | tail -n 80 \
  || true

# ------------------------------------------------------------
# 3. Asegurar baseline de Fase 3
# ------------------------------------------------------------

grep -Fq '57/57 PASS' MIASTRO_Fase_3_Informe.md || {
    echo "ERROR: el informe no acredita 57/57 PASS."
    exit 703
}

grep -Fq 'FASE 3 CERRADA' MIASTRO_Fase_3_Informe.md || {
    echo "ERROR: Fase 3 no figura como cerrada."
    exit 704
}

echo "Phase3Baseline=PASS"

# ------------------------------------------------------------
# 4. Estado Git limpio antes de Fase 4
# ------------------------------------------------------------

echo
echo "=== GIT STATUS ==="
git status --short

if ! git diff --quiet || ! git diff --cached --quiet; then
    echo "ERROR: existen cambios versionados sin consolidar."
    exit 705
fi

echo "TrackedWorkingTree=PASS"

# ------------------------------------------------------------
# 5. Proyectos afectados
# ------------------------------------------------------------

echo
echo "=== PROYECTOS FASE 4 ==="

for project in \
  src/Miastro.Infrastructure.Geography/Miastro.Infrastructure.Geography.csproj \
  src/Miastro.Infrastructure.Time/Miastro.Infrastructure.Time.csproj \
  src/Miastro.Application/Miastro.Application.csproj \
  src/Miastro.Domain/Miastro.Domain.csproj \
  src/Miastro.Infrastructure.Persistence/Miastro.Infrastructure.Persistence.csproj \
  tests/Miastro.Tests/Miastro.Tests.csproj
do
    test -f "$project" || {
        echo "ERROR: falta proyecto esperado: $project"
        exit 706
    }

    echo "--- $project ---"
    sed -n '1,220p' "$project"
done

# ------------------------------------------------------------
# 6. DataBuilder existente
# ------------------------------------------------------------

echo
echo "=== DATABUILDER ACTUAL ==="

if [[ -d tools/DataBuilder ]]; then
    find tools/DataBuilder \
      -maxdepth 3 \
      -type f \
      -print \
      | sort

    while IFS= read -r file; do
        case "$file" in
            *.cs|*.csproj|*.md|*.json)
                echo
                echo "--- $file ---"
                sed -n '1,260p' "$file"
                ;;
        esac
    done < <(
        find tools/DataBuilder \
          -maxdepth 3 \
          -type f \
          -print \
          | sort
    )
else
    echo "DataBuilder=ABSENT"
fi

# ------------------------------------------------------------
# 7. Implementación Geography/Time actual
# ------------------------------------------------------------

echo
echo "=== GEOGRAPHY ACTUAL ==="
find src/Miastro.Infrastructure.Geography \
  -type f \
  \( -name '*.cs' -o -name '*.json' \) \
  -print \
  | sort

echo
echo "=== TIME ACTUAL ==="
find src/Miastro.Infrastructure.Time \
  -type f \
  \( -name '*.cs' -o -name '*.json' \) \
  -print \
  | sort

# ------------------------------------------------------------
# 8. Dependencias actuales relacionadas
# ------------------------------------------------------------

echo
echo "=== DEPENDENCIAS NUGET RELEVANTES ==="

grep -RInE \
  'NodaTime|Microsoft.Data.Sqlite|EntityFrameworkCore.Sqlite|SQLitePCLRaw|Geography|Time' \
  --include='*.csproj' \
  --include='Directory.Packages.props' \
  . \
  || true

# ------------------------------------------------------------
# 9. ADRs: proteger numeración
# ------------------------------------------------------------

echo
echo "=== ADRS EXISTENTES ==="

ADR_DIR="docs/architecture/ADR"

test -d "$ADR_DIR" || {
    echo "ERROR: no existe $ADR_DIR"
    exit 707
}

find "$ADR_DIR" \
  -maxdepth 1 \
  -type f \
  -name 'ADR-*.md' \
  -printf '%f\n' \
  | sort

for n in 005 006; do
    existing="$(
      find "$ADR_DIR" \
        -maxdepth 1 \
        -type f \
        -iname "ADR-${n}*.md" \
        -print \
        -quit
    )"

    if [[ -n "$existing" ]]; then
        echo
        echo "--- EXISTING ADR-$n: $existing ---"
        sed -n '1,240p' "$existing"
    else
        echo "ADR-$n=AVAILABLE"
    fi
done

# ------------------------------------------------------------
# 10. Empaquetado actual
# ------------------------------------------------------------

echo
echo "=== EMPAQUETADO ACTUAL ==="

for file in \
  tools/dev/build-fase3-deb.sh \
  tools/dev/verify-fase3-installed.sh \
  Directory.Build.targets
do
    if [[ -f "$file" ]]; then
        echo
        echo "--- $file ---"
        sed -n '1,320p' "$file"
    fi
done

# ------------------------------------------------------------
# 11. CI actual
# ------------------------------------------------------------

echo
echo "=== CI ACTUAL ==="
sed -n '1,420p' .github/workflows/ci.yml

# ------------------------------------------------------------
# 12. Arquitectura: referencias prohibidas actuales
# ------------------------------------------------------------

echo
echo "=== AUDITORÍA ARQUITECTURA PRE-FASE4 ==="

DOMAIN_NODA="$(
  grep -RIl 'NodaTime' \
    src/Miastro.Domain \
    --include='*.cs' \
    --include='*.csproj' \
    || true
)"

UI_NODA="$(
  grep -RIl 'NodaTime' \
    src/Miastro.UI.Avalonia \
    --include='*.cs' \
    --include='*.csproj' \
    || true
)"

UI_SQLITE="$(
  grep -RIlE \
    'Microsoft\.Data\.Sqlite|SqliteConnection|DbContext' \
    src/Miastro.UI.Avalonia \
    --include='*.cs' \
    --include='*.csproj' \
    || true
)"

echo "DomainNodaTimeRefs=${DOMAIN_NODA:-NONE}"
echo "UiNodaTimeRefs=${UI_NODA:-NONE}"
echo "UiSqliteRefs=${UI_SQLITE:-NONE}"

# ------------------------------------------------------------
# 13. Tests heredados
# ------------------------------------------------------------

echo
echo "=== BASELINE TESTS ==="

dotnet restore Miastro.sln

dotnet build \
  Miastro.sln \
  --configuration Release \
  --no-restore

dotnet test \
  tests/Miastro.Tests/Miastro.Tests.csproj \
  --configuration Release \
  --no-build \
  --logger "console;verbosity=minimal"

# ------------------------------------------------------------
# 14. Crear scaffold documental de Fase 4
#     (sin declarar nada implementado)
# ------------------------------------------------------------

mkdir -p \
  docs/geography \
  docs/time \
  tools/DataBuilder \
  data/geography/source \
  data/geography/fixtures \
  data/geography/generated

cat > MIASTRO_Fase_4_Informe.md <<'EOF'
# MIASTRO — Informe técnico Fase 4

## Estado

Fase 4 — Geografía offline y tiempo histórico.

Estado inicial:

- PASS: 0
- FAIL: 0
- PENDING: 57

**FASE 4 INICIADA — NO CERRADA**

## Objetivo

Implementar:

- GeoNames offline;
- búsqueda y resolución de localidades;
- coordenadas seguras;
- IANA Time Zone IDs;
- Noda Time + TZDB;
- resolución histórica de fecha/hora local;
- ambigüedades;
- horas inexistentes;
- conversión reproducible a UTC.

## Exclusiones

No se implementa todavía:

- Carta Natal funcional;
- UI astrológica final;
- rueda;
- revolución solar;
- revolución lunar;
- tránsitos;
- progresiones;
- sinastría;
- interpretación;
- informes;
- impresión astrológica;
- Fase 5.

## Aceptación

| Estado | Total |
|---|---:|
| PASS | 0 |
| FAIL | 0 |
| PENDING | 57 |

La fase no podrá cerrarse mientras exista cualquier FAIL relevante o
PENDING técnico.
EOF

cat > docs/geography/README.md <<'EOF'
# Geografía — Fase 4

Infraestructura offline de localidades basada en GeoNames.

Este directorio documentará:

- datasets;
- hashes;
- licencia;
- DataBuilder;
- esquema SQLite;
- índices;
- normalización;
- ranking;
- homónimos;
- rendimiento;
- errores;
- empaquetado.

Estado: en implementación.
EOF

cat > docs/time/README.md <<'EOF'
# Tiempo histórico — Fase 4

Infraestructura basada en Noda Time + IANA TZDB.

Este directorio documentará:

- versión Noda Time;
- versión TZDB;
- resolución normal;
- ambigüedad;
- horas inexistentes;
- offsets;
- reproducibilidad;
- golden cases;
- errores.

Estado: en implementación.
EOF

# No versionamos aún datasets descargados en source.
cat > data/geography/source/.gitignore <<'EOF'
*
!.gitignore
EOF

# generated será construido por DataBuilder.
cat > data/geography/generated/.gitignore <<'EOF'
*
!.gitignore
EOF

git diff --check

echo
echo "=== FASE 4 — BASELINE COMPLETADO ==="
echo "Phase3Baseline=PASS"
echo "LegacyBuild=PASS"
echo "LegacyTestsExpected=166"
echo "Phase4Started=YES"
echo "Phase4Closed=NO"
echo "AcceptancePASS=0"
echo "AcceptanceFAIL=0"
echo "AcceptancePENDING=57"
echo "Phase5Started=NO"
echo
git status --short

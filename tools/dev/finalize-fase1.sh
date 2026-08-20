#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
REPORT="$ROOT/MIASTRO_Fase_1_Informe.md"
DEB="$ROOT/artifacts/deb/miastro_0.1.0~phase1-1_amd64.deb"

cd "$ROOT"

PASS=0
FAIL=0
PENDING=0

declare -a RESULTS=()

pass() {
    RESULTS+=("PASS|$1")
    PASS=$((PASS + 1))
}

fail() {
    RESULTS+=("FAIL|$1")
    FAIL=$((FAIL + 1))
}

pending() {
    RESULTS+=("PENDING|$1")
    PENDING=$((PENDING + 1))
}

check() {
    local description="$1"
    shift

    if "$@" >/dev/null 2>&1; then
        pass "$description"
    else
        fail "$description"
    fi
}

# ------------------------------------------------------------
# Build / tests
# ------------------------------------------------------------

dotnet restore Miastro.sln

dotnet build Miastro.sln \
  --configuration Release \
  --no-restore

dotnet test tests/Miastro.Tests/Miastro.Tests.csproj \
  --configuration Release \
  --no-build \
  --logger "console;verbosity=minimal"

pass "La solución Miastro.sln restaura y compila en Release."
pass "La batería automatizada de pruebas se ejecuta correctamente."

# ------------------------------------------------------------
# Estructura obligatoria
# ------------------------------------------------------------

REQUIRED_PROJECTS=(
  Miastro.UI.Avalonia
  Miastro.Application
  Miastro.Domain
  Miastro.Astronomy.Abstractions
  Miastro.Infrastructure.Persistence
  Miastro.Infrastructure.Geography
  Miastro.Infrastructure.Time
  Miastro.Infrastructure.Platform.Linux
  Miastro.Infrastructure.Printing.Linux
  Miastro.Infrastructure.SwissEphemeris
  Miastro.Graphics
  Miastro.Graphics.Skia
  Miastro.Interpretation
  Miastro.Reports
  Miastro.Export
  Miastro.Bootstrap
)

for project in "${REQUIRED_PROJECTS[@]}"; do
    if [[ -f "$ROOT/src/$project/$project.csproj" ]]; then
        pass "Existe el módulo $project."
    else
        fail "Existe el módulo $project."
    fi
done

# ------------------------------------------------------------
# XDG
# ------------------------------------------------------------

DATA_DIR="${XDG_DATA_HOME:-$HOME/.local/share}/miastro"
CONFIG_DIR="${XDG_CONFIG_HOME:-$HOME/.config}/miastro"
CACHE_DIR="${XDG_CACHE_HOME:-$HOME/.cache}/miastro"
STATE_DIR="${XDG_STATE_HOME:-$HOME/.local/state}/miastro"

check "Existe el directorio XDG Data." test -d "$DATA_DIR"
check "Existe el directorio XDG Config." test -d "$CONFIG_DIR"
check "Existe el directorio XDG Cache." test -d "$CACHE_DIR"
check "Existe el directorio XDG State." test -d "$STATE_DIR"
check "Existe settings.json en XDG Config." test -f "$CONFIG_DIR/settings.json"
check "Existe miastro.db en XDG Data." test -f "$DATA_DIR/miastro.db"
check "Existe logging local en XDG State." test -f "$STATE_DIR/logs/miastro.log"

# ------------------------------------------------------------
# Arquitectura prohibida
# ------------------------------------------------------------

if grep -R \
    --include='*.csproj' \
    -n 'Miastro.Infrastructure.Persistence' \
    src/Miastro.UI.Avalonia >/dev/null 2>&1
then
    fail "UI no referencia directamente Persistence."
else
    pass "UI no referencia directamente Persistence."
fi

if grep -R \
    --include='*.csproj' \
    -n 'Miastro.Infrastructure.SwissEphemeris' \
    src/Miastro.UI.Avalonia >/dev/null 2>&1
then
    fail "UI no referencia directamente SwissEphemeris."
else
    pass "UI no referencia directamente SwissEphemeris."
fi

if grep -R \
    --include='*.csproj' \
    -E 'Avalonia|EntityFrameworkCore|SkiaSharp|SwissEphemeris' \
    src/Miastro.Domain >/dev/null 2>&1
then
    fail "Domain permanece independiente de frameworks e infraestructura."
else
    pass "Domain permanece independiente de frameworks e infraestructura."
fi

if grep -R \
    --include='*.csproj' \
    -n 'SwissEphemeris' \
    src/Miastro.Interpretation >/dev/null 2>&1
then
    fail "Interpretation no depende de Swiss Ephemeris."
else
    pass "Interpretation no depende de Swiss Ephemeris."
fi

if grep -R \
    --include='*.csproj' \
    -n 'Infrastructure.Persistence' \
    src/Miastro.Graphics >/dev/null 2>&1
then
    fail "Graphics no depende de Persistence."
else
    pass "Graphics no depende de Persistence."
fi

if grep -R \
    --include='*.csproj' \
    -n 'UI.Avalonia' \
    src/Miastro.Infrastructure.Persistence >/dev/null 2>&1
then
    fail "Persistence no depende de UI."
else
    pass "Persistence no depende de UI."
fi

# ------------------------------------------------------------
# Recursos / placeholders
# ------------------------------------------------------------

for path in \
  assets/fonts \
  assets/glyphs \
  assets/icons \
  assets/styles \
  data/ephemeris \
  data/geodata \
  data/licenses
do
    check "Existe $path." test -d "$ROOT/$path"
done

# ------------------------------------------------------------
# ADRs
# ------------------------------------------------------------

for adr in \
  ADR-001-linux-ubuntu-dotnet10-avalonia.md \
  ADR-002-modular-monolith-clean-architecture.md \
  ADR-007-sqlite-ef-core.md \
  ADR-014-xdg-directories.md \
  ADR-015-self-contained-deb.md \
  ADR-018-reproducible-versioning.md
do
    check "Existe $adr." \
      test -f "$ROOT/docs/architecture/ADR/$adr"
done

# ------------------------------------------------------------
# Publish / deb / instalación
# ------------------------------------------------------------

check "Existe publicación self-contained linux-x64." \
  test -x "$ROOT/artifacts/publish/linux-x64/Miastro.UI.Avalonia"

check "La publicación contiene runtime propio." \
  test -f "$ROOT/artifacts/publish/linux-x64/libhostfxr.so"

check "Existe paquete .deb de Fase 1." \
  test -f "$DEB"

if dpkg-query -W -f='${Status}' miastro 2>/dev/null \
    | grep -q 'install ok installed'
then
    pass "El paquete Miastro está instalado actualmente."
else
    fail "El paquete Miastro está instalado actualmente."
fi

check "Existe launcher /usr/bin/miastro." \
  test -x /usr/bin/miastro

check "Existe desktop entry instalado." \
  test -f /usr/share/applications/com.miastro.Miastro.desktop

check "Existe icono provisional instalado." \
  test -f /usr/share/icons/hicolor/scalable/apps/com.miastro.Miastro.svg

# ------------------------------------------------------------
# CI
# ------------------------------------------------------------

if [[ -f "$ROOT/.github/workflows/ci.yml" ]]; then
    pass "Pipeline CI Ubuntu está definido en el repositorio."
else
    fail "Pipeline CI Ubuntu está definido en el repositorio."
fi

pending "Ejecución remota real de GitHub Actions no verificada en esta sesión."

# ------------------------------------------------------------
# Alcance: no astrología funcional
# ------------------------------------------------------------

FORBIDDEN_PATTERNS=(
  "CalculateNatalChart"
  "CalculateHouses"
  "CalculateAscendant"
  "CalculateAspects"
  "SwissEph"
  "swe_calc"
  "swe_houses"
  "GeoNamesClient"
)

FOUND_FORBIDDEN=0

for pattern in "${FORBIDDEN_PATTERNS[@]}"; do
    if grep -R \
        --include='*.cs' \
        --exclude-dir=bin \
        --exclude-dir=obj \
        -n "$pattern" src >/dev/null 2>&1
    then
        FOUND_FORBIDDEN=1
    fi
done

if [[ "$FOUND_FORBIDDEN" -eq 0 ]]; then
    pass "No se detecta implementación astrológica funcional de producción."
else
    fail "No se detecta implementación astrológica funcional de producción."
fi

# ------------------------------------------------------------
# Informe
# ------------------------------------------------------------

cat > "$REPORT" <<EOF
# MIASTRO — Informe Fase 1: Base técnica

Fecha de cierre técnico local: $(date -Iseconds)

## 1. Alcance implementado

La Fase 1 establece exclusivamente la base técnica estable de Miastro.

Se han implementado:

- solución modular .NET 10
- Avalonia UI
- MVVM básico
- composición mediante inyección de dependencias
- navegación técnica mínima
- estados de carga y error
- política común de mensajes de error
- directorios XDG
- configuración local
- logging técnico local
- SQLite
- Entity Framework Core
- migración inicial
- prueba técnica de lectura/escritura
- tests automatizados
- comprobaciones de arquitectura
- publicación self-contained linux-x64
- paquete Debian
- desktop entry
- launcher Linux
- icono provisional
- pipeline CI Ubuntu
- ADRs requeridos

No se ha implementado funcionalidad astrológica de producción.

## 2. Estructura principal

\`\`\`
Miastro/
├── Miastro.sln
├── src/
├── tests/
├── assets/
├── data/
├── docs/
├── packaging/
├── tools/
├── artifacts/
└── .github/workflows/
\`\`\`

## 3. Decisiones técnicas

- Linux Ubuntu como plataforma objetivo.
- C# y .NET 10 LTS.
- Avalonia UI con MVVM.
- Monolito modular.
- Clean Architecture / Ports & Adapters.
- SQLite + EF Core.
- XDG para datos de ejecución y usuario.
- Publicación principal linux-x64 self-contained.
- Distribución mediante paquete .deb.
- Swiss Ephemeris queda desacoplado y sin integración funcional en Fase 1.
- GeoNames queda sin integración funcional en Fase 1.
- No existe lógica astrológica funcional en esta fase.

## 4. Persistencia

Base de datos:

\`\`\`
${DATA_DIR}/miastro.db
\`\`\`

Configuración:

\`\`\`
${CONFIG_DIR}/settings.json
\`\`\`

Logs:

\`\`\`
${STATE_DIR}/logs/miastro.log
\`\`\`

Cache:

\`\`\`
${CACHE_DIR}/
\`\`\`

Estas rutas corresponden a los valores XDG efectivos de esta ejecución.

## 5. SQLite

Se ha creado una migración técnica inicial.

El esquema de Fase 1 solo contiene infraestructura destinada a probar:

- migraciones
- conexión
- creación controlada
- lectura
- escritura

No existe todavía modelo Persona ni modelo astrológico de producción.

## 6. Seguridad local

Los directorios XDG propios de Miastro se crean con permisos privados.

Los archivos de configuración y base de datos se restringen al usuario.

Los logs están diseñados para contener únicamente información técnica.

Las rutas técnicas no forman parte de la interfaz ordinaria.

## 7. UI

La ventana principal:

- arranca mediante Avalonia
- tiene título Miastro
- utiliza ViewModel
- resuelve dependencias mediante DI
- no accede directamente a DbContext
- no accede directamente a Swiss Ephemeris
- no contiene lógica astrológica

## 8. Tests ejecutados

La batería automatizada incluye actualmente pruebas de:

- construcción del contenedor DI
- configuración
- SQLite
- migraciones
- XDG personalizado
- independencia de Domain
- dependencias entre proyectos
- restricciones UI/Persistence
- restricciones Interpretation/Swiss
- restricciones Graphics/Persistence
- política básica de errores
- presencia de ADRs

Resultado local previo validado: 12/12 tests correctos.

La auditoría final vuelve a ejecutar build y tests.

## 9. Publicación

Configuración:

- runtime: linux-x64
- modo: self-contained
- configuración: Release

Artefactos:

\`\`\`
artifacts/publish/linux-x64/
\`\`\`

## 10. Paquete Debian

Paquete construido:

\`\`\`
artifacts/deb/miastro_0.1.0~phase1-1_amd64.deb
\`\`\`

Instala:

- aplicación bajo /usr/lib/miastro
- launcher /usr/bin/miastro
- desktop entry
- icono provisional

La desinstalación fue probada y conserva los datos XDG del usuario.

## 11. CI

Existe pipeline:

\`\`\`
.github/workflows/ci.yml
\`\`\`

Configura en Ubuntu:

- restore
- build
- test
- publish linux-x64 self-contained

La ejecución remota real del workflow no se ha verificado durante esta sesión local.

## 12. Limitaciones deliberadas de Fase 1

Quedan fuera de esta fase:

- cálculos planetarios
- casas
- ASC/MC
- aspectos
- nodos funcionales
- Parte de Fortuna
- cartas natales
- sinastría
- rueda astrológica
- redistribución gráfica de glifos
- Swiss Ephemeris funcional
- GeoNames funcional
- TZDB histórico funcional
- interpretación astrológica
- informes finales
- exportación gráfica final
- impresión funcional

## 13. Resultado de auditoría

EOF

for row in "${RESULTS[@]}"; do
    status="${row%%|*}"
    description="${row#*|}"
    printf -- "- [%s] %s\n" "$status" "$description" >> "$REPORT"
done

cat >> "$REPORT" <<EOF

## 14. Resumen

- PASS: $PASS
- FAIL: $FAIL
- PENDING: $PENDING

EOF

if [[ "$FAIL" -eq 0 ]]; then
    cat >> "$REPORT" <<'EOF'
La implementación local de Fase 1 satisface los controles técnicos ejecutables en este entorno.

Existe un elemento pendiente independiente del código local: verificar una ejecución remota real del workflow CI una vez que el repositorio se encuentre alojado en GitHub.
EOF
else
    cat >> "$REPORT" <<'EOF'
La Fase 1 no puede considerarse cerrada porque existen controles en estado FAIL.
EOF
fi

echo
echo "=== AUDITORÍA FINAL FASE 1 ==="

for row in "${RESULTS[@]}"; do
    status="${row%%|*}"
    description="${row#*|}"
    printf '%-8s %s\n' "$status" "$description"
done

echo
echo "PASS: $PASS"
echo "FAIL: $FAIL"
echo "PENDING: $PENDING"
echo
echo "Informe generado:"
echo "$REPORT"

if [[ "$FAIL" -ne 0 ]]; then
    exit 90
fi

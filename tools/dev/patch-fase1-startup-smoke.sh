#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
cd "$ROOT"

# ------------------------------------------------------------
# Quitar dependencia directa UI -> Persistence
# La inicialización técnica queda en el composition root.
# ------------------------------------------------------------

python3 - <<'PY'
from pathlib import Path

bootstrap = Path("/home/pablo/Aplicaciones/Miastro/src/Miastro.Bootstrap/MiastroBootstrap.cs")
text = bootstrap.read_text()

marker = """
    public static async Task InitializeAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        var initializer = services.GetRequiredService<DatabaseInitializer>();
        await initializer.InitializeAsync(cancellationToken);
    }
"""

if "public static async Task InitializeAsync(" not in text:
    insert_at = text.rfind("}")
    text = text[:insert_at] + marker + "\n" + text[insert_at:]

bootstrap.write_text(text)

program = Path("/home/pablo/Aplicaciones/Miastro/src/Miastro.UI.Avalonia/Program.cs")
text = program.read_text()

text = text.replace(
    "using Miastro.Infrastructure.Persistence;\n",
    ""
)

old = """            var initializer = App.Services
                .GetRequiredService<DatabaseInitializer>();

            initializer.InitializeAsync()
                .GetAwaiter()
                .GetResult();

            var logger = App.Services
"""

new = """            MiastroBootstrap.InitializeAsync(App.Services)
                .GetAwaiter()
                .GetResult();

            var logger = App.Services
"""

text = text.replace(old, new)
program.write_text(text)
PY

# ------------------------------------------------------------
# Compilar
# ------------------------------------------------------------

dotnet restore Miastro.sln
dotnet build Miastro.sln -c Release --no-restore

# ------------------------------------------------------------
# Verificar que UI no depende directamente de Persistence
# ------------------------------------------------------------

if grep -R \
    --include='*.cs' \
    --include='*.csproj' \
    -nE 'Miastro\.Infrastructure\.Persistence|MiastroDbContext|DbContext' \
    src/Miastro.UI.Avalonia
then
    echo "ERROR: dependencia prohibida UI -> Persistence."
    exit 40
fi

# ------------------------------------------------------------
# Arranque real breve de Avalonia
# ------------------------------------------------------------

EXE="$ROOT/src/Miastro.UI.Avalonia/bin/Release/net10.0/Miastro.UI.Avalonia"

test -x "$EXE"

"$EXE" >/tmp/miastro-gui-smoke.log 2>&1 &
APP_PID=$!

sleep 4

if ! kill -0 "$APP_PID" 2>/dev/null; then
    echo "ERROR: Miastro terminó inesperadamente durante el arranque."
    cat /tmp/miastro-gui-smoke.log
    exit 41
fi

kill "$APP_PID"
wait "$APP_PID" 2>/dev/null || true

# ------------------------------------------------------------
# Validación XDG
# ------------------------------------------------------------

DATA_DIR="${XDG_DATA_HOME:-$HOME/.local/share}/miastro"
CONFIG_DIR="${XDG_CONFIG_HOME:-$HOME/.config}/miastro"
CACHE_DIR="${XDG_CACHE_HOME:-$HOME/.cache}/miastro"
STATE_DIR="${XDG_STATE_HOME:-$HOME/.local/state}/miastro"

DB="$DATA_DIR/miastro.db"
SETTINGS="$CONFIG_DIR/settings.json"
LOG="$STATE_DIR/logs/miastro.log"

test -d "$DATA_DIR"
test -d "$CONFIG_DIR"
test -d "$CACHE_DIR"
test -d "$STATE_DIR"

test -f "$DB"
test -f "$SETTINGS"
test -f "$LOG"

# Directorios privados: 700
for dir in \
    "$DATA_DIR" \
    "$CONFIG_DIR" \
    "$CACHE_DIR" \
    "$STATE_DIR"
do
    MODE="$(stat -c '%a' "$dir")"
    if [[ "$MODE" != "700" ]]; then
        echo "ERROR: permisos incorrectos en directorio XDG: $MODE"
        exit 42
    fi
done

# Config y DB no accesibles para grupo/otros
SETTINGS_MODE="$(stat -c '%a' "$SETTINGS")"
DB_MODE="$(stat -c '%a' "$DB")"

[[ "$SETTINGS_MODE" == "600" ]] || {
    echo "ERROR: settings.json no tiene permisos 600."
    exit 43
}

[[ "$DB_MODE" == "600" ]] || {
    echo "ERROR: miastro.db no tiene permisos 600."
    exit 44
}

# ------------------------------------------------------------
# Verificación SQLite sin depender del ejecutable sqlite3
# ------------------------------------------------------------

python3 - "$DB" <<'PY'
import sqlite3
import sys

db = sys.argv[1]

con = sqlite3.connect(db)
cur = con.cursor()

tables = {
    row[0]
    for row in cur.execute(
        "SELECT name FROM sqlite_master WHERE type='table'"
    )
}

required = {
    "__EFMigrationsHistory",
    "TechnicalProbes",
}

missing = required - tables

if missing:
    raise SystemExit(
        "ERROR: faltan tablas SQLite: " + ", ".join(sorted(missing))
    )

migration_count = cur.execute(
    "SELECT COUNT(*) FROM __EFMigrationsHistory"
).fetchone()[0]

probe_count = cur.execute(
    "SELECT COUNT(*) FROM TechnicalProbes"
).fetchone()[0]

if migration_count < 1:
    raise SystemExit("ERROR: no hay migraciones aplicadas.")

if probe_count < 1:
    raise SystemExit("ERROR: falla la prueba técnica de escritura SQLite.")

con.close()

print("SQLite: OK")
print(f"Migraciones aplicadas: {migration_count}")
print(f"Pruebas técnicas persistidas: {probe_count}")
PY

echo
echo "=== FASE 1 — SMOKE TEST ==="
echo "Build: OK"
echo "Avalonia startup: OK"
echo "MVVM/DI startup: OK"
echo "UI -> Persistence: no existe"
echo "XDG: OK"
echo "settings.json: OK"
echo "SQLite migration/read-write: OK"
echo "Logging local: OK"
echo "Permisos privados: OK"

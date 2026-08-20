#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
DEB="$ROOT/artifacts/deb/miastro_0.1.0~phase1-1_amd64.deb"

DATA_DIR="${XDG_DATA_HOME:-$HOME/.local/share}/miastro"
CONFIG_DIR="${XDG_CONFIG_HOME:-$HOME/.config}/miastro"
STATE_DIR="${XDG_STATE_HOME:-$HOME/.local/state}/miastro"

DB="$DATA_DIR/miastro.db"
SETTINGS="$CONFIG_DIR/settings.json"
LOG="$STATE_DIR/logs/miastro.log"

cd "$ROOT"

# Normalizar permisos del árbol de empaquetado y reconstruir.
chmod 644 \
  artifacts/deb/root/usr/share/applications/com.miastro.Miastro.desktop \
  artifacts/deb/root/usr/share/icons/hicolor/scalable/apps/com.miastro.Miastro.svg \
  artifacts/deb/root/DEBIAN/control

chmod 755 \
  artifacts/deb/root/usr/bin/miastro \
  artifacts/deb/root/usr/lib/miastro/Miastro.UI.Avalonia \
  artifacts/deb/root/DEBIAN/postinst \
  artifacts/deb/root/DEBIAN/postrm

dpkg-deb \
  --root-owner-group \
  --build artifacts/deb/root "$DEB"

# Datos que deben sobrevivir a una desinstalación.
test -f "$DB"
test -f "$SETTINGS"
test -f "$LOG"

DB_BEFORE="$(stat -c '%s' "$DB")"
SETTINGS_BEFORE="$(sha256sum "$SETTINGS" | cut -d' ' -f1)"

# ------------------------------------------------------------
# INSTALACIÓN REAL
# ------------------------------------------------------------

sudo -n dpkg -i "$DEB"

dpkg-query -W \
  -f='${Status} ${Version}\n' \
  miastro

test -x /usr/bin/miastro
test -x /usr/lib/miastro/Miastro.UI.Avalonia
test -f /usr/share/applications/com.miastro.Miastro.desktop
test -f /usr/share/icons/hicolor/scalable/apps/com.miastro.Miastro.svg

# ------------------------------------------------------------
# ARRANQUE DESDE EL LAUNCHER INSTALADO
# ------------------------------------------------------------

/usr/bin/miastro >/tmp/miastro-package-smoke.log 2>&1 &
PID=$!

sleep 4

if ! kill -0 "$PID" 2>/dev/null; then
    echo "ERROR: Miastro instalado terminó durante el arranque."
    cat /tmp/miastro-package-smoke.log
    exit 81
fi

kill "$PID"
wait "$PID" 2>/dev/null || true

# ------------------------------------------------------------
# DESINSTALACIÓN
# ------------------------------------------------------------

sudo -n dpkg -r miastro

if dpkg-query -W miastro >/dev/null 2>&1; then
    STATUS="$(dpkg-query -W -f='${Status}' miastro 2>/dev/null || true)"
    if [[ "$STATUS" == *"installed"* ]]; then
        echo "ERROR: el paquete continúa instalado."
        exit 82
    fi
fi

test ! -e /usr/bin/miastro
test ! -e /usr/lib/miastro/Miastro.UI.Avalonia
test ! -e /usr/share/applications/com.miastro.Miastro.desktop

# ------------------------------------------------------------
# XDG DEBE CONSERVARSE
# ------------------------------------------------------------

test -f "$DB"
test -f "$SETTINGS"
test -f "$LOG"

DB_AFTER="$(stat -c '%s' "$DB")"
SETTINGS_AFTER="$(sha256sum "$SETTINGS" | cut -d' ' -f1)"

[[ "$DB_AFTER" -ge "$DB_BEFORE" ]] || {
    echo "ERROR: la base XDG no se conservó correctamente."
    exit 83
}

[[ "$SETTINGS_AFTER" == "$SETTINGS_BEFORE" ]] || {
    echo "ERROR: settings.json fue modificado durante la desinstalación."
    exit 84
}

# ------------------------------------------------------------
# REINSTALAR PARA DEJAR MIASTRO INSTALADO
# ------------------------------------------------------------

sudo -n dpkg -i "$DEB"

dpkg-query -W \
  -f='${Status} ${Version}\n' \
  miastro

echo
echo "=== VALIDACIÓN PAQUETE FASE 1 ==="
echo "Instalación .deb: OK"
echo "Launcher /usr/bin/miastro: OK"
echo "Arranque desde paquete: OK"
echo "Desktop entry: OK"
echo "Desinstalación: OK"
echo "Datos XDG preservados: OK"
echo "Reinstalación final: OK"

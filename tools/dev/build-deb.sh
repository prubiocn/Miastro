#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
VERSION="0.1.0~phase1-1"
PUBLISH="$ROOT/artifacts/publish/linux-x64"
PKGROOT="$ROOT/artifacts/deb/root"
OUTPUT="$ROOT/artifacts/deb/miastro_${VERSION}_amd64.deb"

command -v dpkg-deb >/dev/null 2>&1 || {
    echo "ERROR: falta dpkg-deb."
    echo "Instala el paquete dpkg antes de continuar."
    exit 70
}

rm -rf "$PKGROOT"

mkdir -p \
  "$PKGROOT/DEBIAN" \
  "$PKGROOT/usr/lib/miastro" \
  "$PKGROOT/usr/bin" \
  "$PKGROOT/usr/share/applications" \
  "$PKGROOT/usr/share/icons/hicolor/scalable/apps"

cp -a "$PUBLISH/." \
  "$PKGROOT/usr/lib/miastro/"

cp "$ROOT/packaging/debian/miastro-launcher.sh" \
  "$PKGROOT/usr/bin/miastro"

cp "$ROOT/packaging/debian/com.miastro.Miastro.desktop" \
  "$PKGROOT/usr/share/applications/com.miastro.Miastro.desktop"

cp "$ROOT/assets/icons/com.miastro.Miastro.svg" \
  "$PKGROOT/usr/share/icons/hicolor/scalable/apps/com.miastro.Miastro.svg"

chmod 755 \
  "$PKGROOT/usr/bin/miastro" \
  "$PKGROOT/usr/lib/miastro/Miastro.UI.Avalonia"

cat > "$PKGROOT/DEBIAN/control" <<CONTROL
Package: miastro
Version: $VERSION
Section: utils
Priority: optional
Architecture: amd64
Maintainer: Miastro
Depends: libx11-6, libice6, libsm6, libfontconfig1
Description: Miastro
 Base técnica de la aplicación de escritorio Miastro.
CONTROL

cat > "$PKGROOT/DEBIAN/postinst" <<'EOF2'
#!/bin/sh
set -e

if command -v update-desktop-database >/dev/null 2>&1; then
    update-desktop-database /usr/share/applications || true
fi

if command -v gtk-update-icon-cache >/dev/null 2>&1; then
    gtk-update-icon-cache -q /usr/share/icons/hicolor || true
fi

exit 0
EOF2

cat > "$PKGROOT/DEBIAN/postrm" <<'EOF2'
#!/bin/sh
set -e

if command -v update-desktop-database >/dev/null 2>&1; then
    update-desktop-database /usr/share/applications || true
fi

if command -v gtk-update-icon-cache >/dev/null 2>&1; then
    gtk-update-icon-cache -q /usr/share/icons/hicolor || true
fi

# Deliberadamente NO se eliminan datos XDG del usuario.
exit 0
EOF2

chmod 755 \
  "$PKGROOT/DEBIAN/postinst" \
  "$PKGROOT/DEBIAN/postrm"

dpkg-deb \
  --root-owner-group \
  --build "$PKGROOT" "$OUTPUT"

test -f "$OUTPUT"

echo "Paquete .deb: $OUTPUT"

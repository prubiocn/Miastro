#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
cd "$ROOT"

VERSION="0.1.0"
DEB_VERSION="0.1.0~phase1-1"
PUBLISH="$ROOT/artifacts/publish/linux-x64"
PKGROOT="$ROOT/artifacts/deb/root"
DEB_OUT="$ROOT/artifacts/deb/miastro_${DEB_VERSION}_amd64.deb"

mkdir -p \
  "$ROOT/artifacts/publish" \
  "$ROOT/artifacts/deb" \
  "$ROOT/packaging/debian" \
  "$ROOT/assets/icons" \
  "$ROOT/.github/workflows"

# ------------------------------------------------------------
# Versionado
# ------------------------------------------------------------

printf '%s\n' "$VERSION" > "$ROOT/VERSION"

# ------------------------------------------------------------
# Icono provisional
# ------------------------------------------------------------

cat > "$ROOT/assets/icons/com.miastro.Miastro.svg" <<'EOF'
<svg xmlns="http://www.w3.org/2000/svg"
     width="256"
     height="256"
     viewBox="0 0 256 256">
  <rect x="16" y="16" width="224" height="224" rx="48"
        fill="#20242b"/>
  <circle cx="128" cy="128" r="72"
          fill="none"
          stroke="#f2f2f2"
          stroke-width="8"/>
  <circle cx="128" cy="128" r="10"
          fill="#f2f2f2"/>
  <path d="M128 42 L138 74 L128 66 L118 74 Z"
        fill="#f2f2f2"/>
</svg>
EOF

# ------------------------------------------------------------
# Desktop entry
# ------------------------------------------------------------

cat > "$ROOT/packaging/debian/com.miastro.Miastro.desktop" <<'EOF'
[Desktop Entry]
Type=Application
Name=Miastro
Comment=Aplicación profesional de astrología
Exec=/usr/bin/miastro
Icon=com.miastro.Miastro
Terminal=false
Categories=Office;Utility;
StartupNotify=true
EOF

# ------------------------------------------------------------
# Launcher
# ------------------------------------------------------------

cat > "$ROOT/packaging/debian/miastro-launcher.sh" <<'EOF'
#!/usr/bin/env bash
exec /usr/lib/miastro/Miastro.UI.Avalonia "$@"
EOF

chmod 755 "$ROOT/packaging/debian/miastro-launcher.sh"

# ------------------------------------------------------------
# Script reproducible de publicación
# ------------------------------------------------------------

cat > "$ROOT/tools/dev/publish-linux-x64.sh" <<'EOF'
#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
OUT="$ROOT/artifacts/publish/linux-x64"

cd "$ROOT"

rm -rf "$OUT"
mkdir -p "$OUT"

dotnet publish \
  src/Miastro.UI.Avalonia/Miastro.UI.Avalonia.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained true \
  --output "$OUT" \
  -p:DebugType=None \
  -p:DebugSymbols=false

test -x "$OUT/Miastro.UI.Avalonia"

echo "Publish linux-x64 self-contained: OK"
EOF

chmod 755 "$ROOT/tools/dev/publish-linux-x64.sh"

# ------------------------------------------------------------
# Script reproducible de .deb
# ------------------------------------------------------------

cat > "$ROOT/tools/dev/build-deb.sh" <<'EOF'
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
EOF

chmod 755 "$ROOT/tools/dev/build-deb.sh"

# ------------------------------------------------------------
# CI Ubuntu
# ------------------------------------------------------------

cat > "$ROOT/.github/workflows/ci.yml" <<'EOF'
name: Miastro CI

on:
  push:
  pull_request:

jobs:
  build-test-publish:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup .NET 10
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Install native Avalonia dependencies
        run: |
          sudo apt-get update
          sudo apt-get install -y \
            libx11-6 \
            libice6 \
            libsm6 \
            libfontconfig1

      - name: Restore local tools
        run: dotnet tool restore

      - name: Restore
        run: dotnet restore Miastro.sln

      - name: Build
        run: dotnet build Miastro.sln -c Release --no-restore

      - name: Test
        run: >
          dotnet test
          tests/Miastro.Tests/Miastro.Tests.csproj
          -c Release
          --no-build

      - name: Publish linux-x64 self-contained
        run: >
          dotnet publish
          src/Miastro.UI.Avalonia/Miastro.UI.Avalonia.csproj
          -c Release
          -r linux-x64
          --self-contained true
          -o artifacts/publish/linux-x64
          -p:DebugType=None
          -p:DebugSymbols=false
EOF

# ------------------------------------------------------------
# Build + test
# ------------------------------------------------------------

dotnet restore Miastro.sln

dotnet build Miastro.sln \
  --configuration Release \
  --no-restore

dotnet test \
  tests/Miastro.Tests/Miastro.Tests.csproj \
  --configuration Release \
  --no-build \
  --logger "console;verbosity=minimal"

# ------------------------------------------------------------
# Publish
# ------------------------------------------------------------

"$ROOT/tools/dev/publish-linux-x64.sh"

test -x "$PUBLISH/Miastro.UI.Avalonia"

# Confirma que el runtime está incluido
test -f "$PUBLISH/libhostfxr.so" || {
    echo "ERROR: la publicación no parece self-contained."
    exit 71
}

# ------------------------------------------------------------
# Construcción .deb
# ------------------------------------------------------------

"$ROOT/tools/dev/build-deb.sh"

# ------------------------------------------------------------
# Inspección del paquete
# ------------------------------------------------------------

echo
echo "=== CONTENIDO PRINCIPAL DEL .DEB ==="

dpkg-deb -c "$DEB_OUT" | grep -E \
  'usr/bin/miastro$|Miastro.UI.Avalonia$|com.miastro.Miastro.desktop$|com.miastro.Miastro.svg$'

echo
echo "=== METADATOS DEL .DEB ==="

dpkg-deb -f "$DEB_OUT" \
  Package Version Architecture Depends

echo
echo "=== RESULTADO ==="
echo "CI Ubuntu: configurada"
echo "Build: OK"
echo "Tests: OK"
echo "Publish linux-x64 self-contained: OK"
echo "Desktop entry: OK"
echo "Icono provisional: OK"
echo "Launcher /usr/bin/miastro: OK"
echo "Paquete .deb: OK"
echo "Datos XDG no incluidos en paquete: OK"

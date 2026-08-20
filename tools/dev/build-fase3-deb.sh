#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
ROOT="$(cd -- "$SCRIPT_DIR/../.." && pwd)"
PUBLISH="${MIASTRO_PUBLISH_DIR:-$ROOT/artifacts/publish/fase3-linux-x64}"
OUT="$ROOT/artifacts/deb"
STAGE="$ROOT/artifacts/package/fase3-deb"
VERSION="0.3.0~phase3-1"
PACKAGE="miastro"
ARCH="amd64"

cd "$ROOT"

command -v dpkg-deb >/dev/null 2>&1 || {
    echo "ERROR: dpkg-deb no disponible."
    exit 420
}

echo "PublishInput=$PUBLISH"

if [[ ! -x "$PUBLISH/Miastro.UI.Avalonia" ]]; then
    echo "ERROR: falta ejecutable publish: $PUBLISH/Miastro.UI.Avalonia"
    exit 520
fi

if [[ ! -f "$PUBLISH/native/linux-x64/libswe.so" ]]; then
    echo "ERROR: falta libswe.so: $PUBLISH/native/linux-x64/libswe.so"
    find "$PUBLISH" -maxdepth 4 -type f -print || true
    exit 521
fi

if [[ ! -f "$PUBLISH/ephemeris/manifest.json" ]]; then
    echo "ERROR: falta manifiesto de efemérides: $PUBLISH/ephemeris/manifest.json"
    find "$PUBLISH" -maxdepth 4 -type f -print || true
    exit 522
fi

rm -rf "$STAGE"
mkdir -p \
  "$STAGE/DEBIAN" \
  "$STAGE/usr/bin" \
  "$STAGE/usr/lib/miastro/app" \
  "$STAGE/usr/lib/miastro/native" \
  "$STAGE/usr/share/miastro/ephemeris" \
  "$STAGE/usr/share/doc/miastro/swiss-ephemeris" \
  "$OUT"

cp -a \
  "$PUBLISH/." \
  "$STAGE/usr/lib/miastro/app/"

rm -rf \
  "$STAGE/usr/lib/miastro/app/native" \
  "$STAGE/usr/lib/miastro/app/ephemeris"

install -m 0755 \
  "$PUBLISH/native/linux-x64/libswe.so" \
  "$STAGE/usr/lib/miastro/native/libswe.so"

install -m 0644 \
  "$PUBLISH/native/linux-x64/native-manifest.json" \
  "$STAGE/usr/lib/miastro/native/native-manifest.json"

cp -a \
  "$PUBLISH/ephemeris/." \
  "$STAGE/usr/share/miastro/ephemeris/"

for license in \
  LICENSE \
  LICENSE.TXT \
  agpl-3.0.txt
do
    install -m 0644 \
      "$ROOT/licenses/SwissEphemeris/$license" \
      "$STAGE/usr/share/doc/miastro/swiss-ephemeris/$license"
done

cat > "$STAGE/usr/bin/miastro" <<'LAUNCH'
#!/usr/bin/env bash
set -e
exec /usr/lib/miastro/app/Miastro.UI.Avalonia "$@"
LAUNCH

chmod 0755 \
  "$STAGE/usr/bin/miastro"

INSTALLED_SIZE="$(
  du -sk "$STAGE/usr" |
  awk '{print $1}'
)"

cat > "$STAGE/DEBIAN/control" <<EOF_CONTROL
Package: $PACKAGE
Version: $VERSION
Section: science
Priority: optional
Architecture: $ARCH
Installed-Size: $INSTALLED_SIZE
Maintainer: Miastro Project
Depends: libc6, libx11-6, libfontconfig1, libfreetype6
Description: Miastro astrology application
 Miastro desktop application with private Swiss Ephemeris integration.
EOF_CONTROL

DEB="$OUT/${PACKAGE}_${VERSION}_${ARCH}.deb"

rm -f "$DEB"

dpkg-deb \
  --build \
  --root-owner-group \
  "$STAGE" \
  "$DEB"

dpkg-deb \
  --info \
  "$DEB"

dpkg-deb \
  --contents \
  "$DEB" \
  > "$OUT/contents.txt"

for expected in \
  './usr/bin/miastro' \
  './usr/lib/miastro/native/libswe.so' \
  './usr/share/miastro/ephemeris/sepl_18.se1' \
  './usr/share/miastro/ephemeris/semo_18.se1' \
  './usr/share/miastro/ephemeris/seas_18.se1'
do
    grep -Fq "$expected" \
      "$OUT/contents.txt" || {
        echo "ERROR: falta en .deb: $expected"
        exit 421
    }
done

echo "DEB=$DEB"
echo "DebBuild=PASS"

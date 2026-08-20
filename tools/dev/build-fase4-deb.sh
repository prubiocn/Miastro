#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
ROOT="$(cd -- "$SCRIPT_DIR/../.." && pwd)"

PUBLISH="${MIASTRO_PUBLISH_DIR:-$ROOT/artifacts/publish/fase4-linux-x64}"
OUT="$ROOT/artifacts/deb"
STAGE="$ROOT/artifacts/package/fase4-deb"

VERSION="${MIASTRO_DEB_VERSION:-0.4.0~phase4-1}"
PACKAGE="miastro"
ARCH="amd64"

cd "$ROOT"

command -v dpkg-deb >/dev/null 2>&1 || {
    echo "ERROR: dpkg-deb no disponible."
    exit 920
}

for f in \
  "$PUBLISH/Miastro.UI.Avalonia" \
  "$PUBLISH/native/linux-x64/libswe.so" \
  "$PUBLISH/ephemeris/manifest.json" \
  "$PUBLISH/geodata/geonames.sqlite" \
  "$PUBLISH/geodata/manifest.json"
do
    test -f "$f" || {
        echo "ERROR: falta recurso publish: $f"
        exit 921
    }
done

rm -rf "$STAGE"

mkdir -p \
  "$STAGE/DEBIAN" \
  "$STAGE/usr/bin" \
  "$STAGE/usr/lib/miastro/app" \
  "$STAGE/usr/lib/miastro/native" \
  "$STAGE/usr/share/miastro/ephemeris" \
  "$STAGE/usr/share/miastro/geodata" \
  "$STAGE/usr/share/doc/miastro/swiss-ephemeris" \
  "$STAGE/usr/share/doc/miastro/geonames" \
  "$OUT"

cp -a "$PUBLISH/." "$STAGE/usr/lib/miastro/app/"

rm -rf \
  "$STAGE/usr/lib/miastro/app/native" \
  "$STAGE/usr/lib/miastro/app/ephemeris" \
  "$STAGE/usr/lib/miastro/app/geodata"

install -m 0755 \
  "$PUBLISH/native/linux-x64/libswe.so" \
  "$STAGE/usr/lib/miastro/native/libswe.so"

install -m 0644 \
  "$PUBLISH/native/linux-x64/native-manifest.json" \
  "$STAGE/usr/lib/miastro/native/native-manifest.json"

cp -a \
  "$PUBLISH/ephemeris/." \
  "$STAGE/usr/share/miastro/ephemeris/"

install -m 0644 \
  "$PUBLISH/geodata/geonames.sqlite" \
  "$STAGE/usr/share/miastro/geodata/geonames.sqlite"

install -m 0644 \
  "$PUBLISH/geodata/manifest.json" \
  "$STAGE/usr/share/miastro/geodata/manifest.json"

for license in \
  LICENSE \
  LICENSE.TXT \
  agpl-3.0.txt
do
    install -m 0644 \
      "$ROOT/licenses/SwissEphemeris/$license" \
      "$STAGE/usr/share/doc/miastro/swiss-ephemeris/$license"
done

install -m 0644 \
  "$ROOT/docs/licenses/GeoNames/ATTRIBUTION.md" \
  "$STAGE/usr/share/doc/miastro/geonames/ATTRIBUTION.md"

install -m 0644 \
  "$ROOT/docs/licenses/GeoNames/CC-BY-4.0.txt" \
  "$STAGE/usr/share/doc/miastro/geonames/CC-BY-4.0.txt"

install -m 0644 \
  "$ROOT/docs/licenses/GeoNames/GeoNames-readme.txt" \
  "$STAGE/usr/share/doc/miastro/geonames/GeoNames-readme.txt"

cat > "$STAGE/usr/bin/miastro" <<'LAUNCH'
#!/usr/bin/env bash
set -e
exec /usr/lib/miastro/app/Miastro.UI.Avalonia "$@"
LAUNCH

chmod 0755 "$STAGE/usr/bin/miastro"

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
 Miastro desktop application with private Swiss Ephemeris,
 offline GeoNames catalog and historical IANA/TZDB time support.
EOF_CONTROL

DEB="$OUT/${PACKAGE}_${VERSION}_${ARCH}.deb"

rm -f "$DEB"

dpkg-deb \
  --build \
  --root-owner-group \
  "$STAGE" \
  "$DEB"

dpkg-deb --info "$DEB"

dpkg-deb \
  --contents \
  "$DEB" \
  > "$OUT/fase4-contents.txt"

for expected in \
  './usr/bin/miastro' \
  './usr/lib/miastro/native/libswe.so' \
  './usr/share/miastro/ephemeris/sepl_18.se1' \
  './usr/share/miastro/geodata/geonames.sqlite' \
  './usr/share/miastro/geodata/manifest.json' \
  './usr/share/doc/miastro/geonames/ATTRIBUTION.md' \
  './usr/share/doc/miastro/geonames/CC-BY-4.0.txt'
do
    grep -Fq "$expected" \
      "$OUT/fase4-contents.txt" || {
        echo "ERROR: falta en .deb: $expected"
        exit 922
    }
done

echo "DEB=$DEB"
echo "DebBuild=PASS"

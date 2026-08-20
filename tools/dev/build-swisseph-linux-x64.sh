#!/usr/bin/env bash
set -euo pipefail

export LC_ALL=C

ROOT="/home/pablo/Aplicaciones/Miastro"
BUILD="$ROOT/artifacts/vendor/swisseph"
SOURCE="$BUILD/source"
NATIVE="$ROOT/src/Miastro.Infrastructure.SwissEphemeris/native/linux-x64"
THIRD="$ROOT/third_party/swisseph"

UPSTREAM="https://github.com/aloistr/swisseph.git"
TAG="v2.10.3final"
EXPECTED_VERSION="2.10.03"

cd "$ROOT"

for tool in \
  git \
  cc \
  make \
  file \
  readelf \
  nm \
  sha256sum \
  ldd
do
    command -v "$tool" >/dev/null 2>&1 || {
        echo "ERROR: falta $tool"
        exit 450
    }
done

if [[ "$(uname -m)" != "x86_64" ]]; then
    echo "ERROR: este builder requiere x86_64."
    exit 451
fi

rm -rf "$SOURCE"
mkdir -p "$BUILD" "$NATIVE"

git clone \
  --depth 1 \
  --branch "$TAG" \
  "$UPSTREAM" \
  "$SOURCE"

cd "$SOURCE"

VERSION="$(
  sed -nE \
    's/^#define[[:space:]]+SE_VERSION[[:space:]]+"([^"]+)".*/\1/p' \
    sweph.h |
  head -n1
)"

if [[ "$VERSION" != "$EXPECTED_VERSION" ]]; then
    echo "ERROR: versión inesperada: $VERSION"
    exit 452
fi

SOURCE_COMMIT="$(git rev-parse HEAD)"

make clean || true

make \
  swedate.o \
  swehouse.o \
  swejpl.o \
  swemmoon.o \
  swemplan.o \
  sweph.o \
  swephlib.o \
  swecl.o \
  swehel.o

cc \
  -shared \
  -Wl,-z,defs \
  -o libswe.so \
  swedate.o \
  swehouse.o \
  swejpl.o \
  swemmoon.o \
  swemplan.o \
  sweph.o \
  swephlib.o \
  swecl.o \
  swehel.o \
  -lm

INFO="$(file libswe.so)"

echo "$INFO"

echo "$INFO" | grep -q 'ELF 64-bit'
echo "$INFO" | grep -q 'x86-64'
echo "$INFO" | grep -q 'shared object'

readelf -h libswe.so |
  grep -Eq 'Class:[[:space:]]+ELF64'

for symbol in \
  swe_version \
  swe_set_ephe_path \
  swe_calc_ut \
  swe_julday \
  swe_houses_ex \
  swe_close
do
    nm -D --defined-only libswe.so |
      awk '{print $3}' |
      grep -Fxq "$symbol" || {
        echo "ERROR: símbolo ausente: $symbol"
        exit 453
      }
done

UNRESOLVED="$(
  ldd -r libswe.so 2>&1 |
  grep -E 'undefined symbol|not found' ||
  true
)"

if [[ -n "$UNRESOLVED" ]]; then
    echo "$UNRESOLVED"
    exit 454
fi

install -m 0755 \
  libswe.so \
  "$NATIVE/libswe.so"

HASH="$(
  sha256sum "$NATIVE/libswe.so" |
  awk '{print $1}'
)"

echo "SwissVersion=$VERSION"
echo "SourceCommit=$SOURCE_COMMIT"
echo "SHA256=$HASH"
echo "NativeBuild=PASS"

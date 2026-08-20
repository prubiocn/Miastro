#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
TESTS="$ROOT/tests/Miastro.Tests/Miastro.Tests.csproj"
UI="$ROOT/src/Miastro.UI.Avalonia/Miastro.UI.Avalonia.csproj"
PUBLISH="$ROOT/artifacts/publish/fase3-linux-x64"

cd "$ROOT"

echo "=== MIASTRO — VERIFICACIÓN LOCAL FASE 3 ==="

dotnet restore Miastro.sln

dotnet build \
  Miastro.sln \
  --configuration Release \
  --no-restore

dotnet test \
  "$TESTS" \
  --configuration Release \
  --no-build \
  --logger "console;verbosity=minimal"

dotnet restore \
  "$UI" \
  --runtime linux-x64

rm -rf "$PUBLISH"

dotnet publish \
  "$UI" \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained true \
  --no-restore \
  --output "$PUBLISH"

test -x \
  "$PUBLISH/Miastro.UI.Avalonia"

test -f \
  "$PUBLISH/native/linux-x64/libswe.so"

test -f \
  "$PUBLISH/native/linux-x64/native-manifest.json"

for file in \
  manifest.json \
  sepl_18.se1 \
  semo_18.se1 \
  seas_18.se1
do
    test -f \
      "$PUBLISH/ephemeris/$file"
done

UNRESOLVED="$(
  ldd -r \
    "$PUBLISH/native/linux-x64/libswe.so" \
    2>&1 |
  grep -E \
    'undefined symbol|not found' ||
  true
)"

if [[ -n "$UNRESOLVED" ]]; then
    echo "$UNRESOLVED"
    exit 460
fi

"$ROOT/tools/dev/build-fase3-deb.sh"

git diff --check

echo
echo "Build=PASS"
echo "Tests=PASS"
echo "Publish=PASS"
echo "DebBuild=PASS"
echo "NativeABI=PASS"
echo "LocalPhase3=PASS"

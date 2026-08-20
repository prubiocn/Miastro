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

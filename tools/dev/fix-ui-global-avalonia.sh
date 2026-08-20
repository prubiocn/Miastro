#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
FILE="$ROOT/src/Miastro.UI.Avalonia/App.axaml.cs"

sed -i \
  's/public sealed partial class App : Avalonia\.Application/public sealed partial class App : global::Avalonia.Application/' \
  "$FILE"

cd "$ROOT"

dotnet build Miastro.sln \
  --configuration Release \
  --no-restore

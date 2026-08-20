#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
FILE="$ROOT/src/Miastro.Domain/Rulerships/RulershipCatalog.cs"

python3 - <<'PY'
from pathlib import Path

p = Path("/home/pablo/Aplicaciones/Miastro/src/Miastro.Domain/Rulerships/RulershipCatalog.cs")
text = p.read_text()

text = text.replace(
    "    public static IReadOnlyCollection<Rulership> All =>\n        Rulerships.Values;",
    "    public static IReadOnlyCollection<Rulership> All =>\n        Rulerships.Values.ToArray();"
)

p.write_text(text)
PY

cd "$ROOT"

dotnet build Miastro.sln \
  --configuration Release \
  --no-restore

dotnet test tests/Miastro.Tests/Miastro.Tests.csproj \
  --configuration Release \
  --no-build \
  --logger "console;verbosity=minimal"

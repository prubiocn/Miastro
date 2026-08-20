#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
TESTS="$ROOT/tests/Miastro.Tests"

python3 - <<'PY'
from pathlib import Path

tests = Path("/home/pablo/Aplicaciones/Miastro/tests/Miastro.Tests")

files = [
    tests / "Phase2AngularAndDerivedTests.cs",
    tests / "Phase2ZodiacHousePlacementTests.cs",
    tests / "Phase2RulershipTests.cs",
]

for path in files:
    text = path.read_text()
    text = text.replace("[DataTestMethod]", "[TestMethod]")
    path.write_text(text)
PY

cd "$ROOT"

dotnet build Miastro.sln \
  --configuration Release \
  --no-restore

dotnet test tests/Miastro.Tests/Miastro.Tests.csproj \
  --configuration Release \
  --no-build \
  --logger "console;verbosity=minimal"

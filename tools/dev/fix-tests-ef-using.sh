#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
FILE="$ROOT/tests/Miastro.Tests/BootstrapTests.cs"

python3 - <<'PY'
from pathlib import Path

p = Path("/home/pablo/Aplicaciones/Miastro/tests/Miastro.Tests/BootstrapTests.cs")
text = p.read_text()

if "using Microsoft.EntityFrameworkCore;" not in text:
    text = "using Microsoft.EntityFrameworkCore;\n" + text

p.write_text(text)
PY

cd "$ROOT"

dotnet build Miastro.sln \
  --configuration Release \
  --no-restore

dotnet test tests/Miastro.Tests/Miastro.Tests.csproj \
  --configuration Release \
  --no-build \
  --logger "console;verbosity=normal"

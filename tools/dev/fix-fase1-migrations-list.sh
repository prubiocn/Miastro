#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
cd "$ROOT"

test -d src/Miastro.Infrastructure.Persistence/Migrations
test "$(find src/Miastro.Infrastructure.Persistence/Migrations -name '*.cs' | wc -l)" -ge 2

dotnet tool run dotnet-ef migrations list \
  --project src/Miastro.Infrastructure.Persistence/Miastro.Infrastructure.Persistence.csproj \
  --startup-project src/Miastro.Infrastructure.Persistence/Miastro.Infrastructure.Persistence.csproj

dotnet build Miastro.sln \
  --configuration Release \
  --no-restore

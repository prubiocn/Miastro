#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
FILE="$ROOT/src/Miastro.Domain/DomainAssemblyMarker.cs"

cat > "$FILE" <<'EOF'
namespace Miastro.Domain;

/// <summary>
/// Marcador técnico del ensamblado de dominio.
/// No contiene lógica astrológica de producción.
/// </summary>
public sealed class DomainAssemblyMarker
{
    private DomainAssemblyMarker()
    {
    }
}
EOF

cd "$ROOT"

dotnet build Miastro.sln \
  --configuration Release \
  --no-restore

dotnet test tests/Miastro.Tests/Miastro.Tests.csproj \
  --configuration Release \
  --no-build \
  --logger "console;verbosity=normal"

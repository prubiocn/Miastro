#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
cd "$ROOT"

echo "============================================================"
echo "MIASTRO FASE 1 — BOOTSTRAP INICIAL"
echo "Raíz: $ROOT"
echo "SDK: $(dotnet --version)"
echo "============================================================"

mkdir -p \
  docs/architecture/ADR \
  src \
  tests \
  assets/fonts \
  assets/glyphs \
  assets/icons \
  assets/styles \
  data/ephemeris \
  data/geodata \
  data/licenses \
  packaging/debian \
  tools/dev \
  .github/workflows

cat > global.json <<'JSON'
{
  "sdk": {
    "version": "10.0.100",
    "rollForward": "latestPatch",
    "allowPrerelease": false
  }
}
JSON

cat > Directory.Build.props <<'XML'
<Project>
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <LangVersion>14.0</LangVersion>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <Deterministic>true</Deterministic>
  </PropertyGroup>
</Project>
XML

cat > Directory.Packages.props <<'XML'
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <ItemGroup>
    <PackageVersion Include="Avalonia" Version="12.1.1" />
    <PackageVersion Include="Avalonia.Desktop" Version="12.1.1" />
    <PackageVersion Include="Avalonia.Themes.Fluent" Version="12.1.1" />

    <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="10.0.11" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.11" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.11" />

    <PackageVersion Include="Microsoft.Extensions.Configuration" Version="10.0.11" />
    <PackageVersion Include="Microsoft.Extensions.Configuration.Json" Version="10.0.11" />
    <PackageVersion Include="Microsoft.Extensions.DependencyInjection" Version="10.0.11" />
    <PackageVersion Include="Microsoft.Extensions.Logging" Version="10.0.11" />
    <PackageVersion Include="Microsoft.Extensions.Logging.Console" Version="10.0.11" />
  </ItemGroup>
</Project>
XML

cat > .gitignore <<'EOF2'
bin/
obj/
artifacts/
TestResults/
.vs/
.idea/
*.user
*.suo
EOF2

rm -f Miastro.sln Miastro.slnx
dotnet new sln -n Miastro --format sln --force

PROJECTS=(
  Miastro.Application
  Miastro.Domain
  Miastro.Astronomy.Abstractions
  Miastro.Infrastructure.Persistence
  Miastro.Infrastructure.Geography
  Miastro.Infrastructure.Time
  Miastro.Infrastructure.Platform.Linux
  Miastro.Infrastructure.Printing.Linux
  Miastro.Infrastructure.SwissEphemeris
  Miastro.Graphics
  Miastro.Graphics.Skia
  Miastro.Interpretation
  Miastro.Reports
  Miastro.Export
  Miastro.Bootstrap
)

for project in "${PROJECTS[@]}"; do
  dotnet new classlib \
    --name "$project" \
    --output "src/$project" \
    --framework net10.0 --force

  rm -f "src/$project/Class1.cs"
  dotnet sln Miastro.sln add "src/$project/$project.csproj"
done

mkdir -p \
  src/Miastro.UI.Avalonia/Views \
  src/Miastro.UI.Avalonia/ViewModels \
  src/Miastro.UI.Avalonia/Commands \
  src/Miastro.UI.Avalonia/Navigation \
  src/Miastro.UI.Avalonia/Services \
  src/Miastro.UI.Avalonia/States \
  src/Miastro.UI.Avalonia/Controls \
  src/Miastro.UI.Avalonia/Styles \
  src/Miastro.UI.Avalonia/Resources \
  src/Miastro.UI.Avalonia/Behaviors \
  src/Miastro.UI.Avalonia/Converters \
  src/Miastro.UI.Avalonia/Accessibility

cat > src/Miastro.UI.Avalonia/Miastro.UI.Avalonia.csproj <<'XML'
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Avalonia" />
    <PackageReference Include="Avalonia.Desktop" />
    <PackageReference Include="Avalonia.Themes.Fluent" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="../Miastro.Application/Miastro.Application.csproj" />
    <ProjectReference Include="../Miastro.Bootstrap/Miastro.Bootstrap.csproj" />
  </ItemGroup>

</Project>
XML

dotnet sln Miastro.sln add \
  src/Miastro.UI.Avalonia/Miastro.UI.Avalonia.csproj

dotnet add src/Miastro.Application/Miastro.Application.csproj reference \
  src/Miastro.Domain/Miastro.Domain.csproj \
  src/Miastro.Astronomy.Abstractions/Miastro.Astronomy.Abstractions.csproj

dotnet add src/Miastro.Infrastructure.Persistence/Miastro.Infrastructure.Persistence.csproj reference \
  src/Miastro.Application/Miastro.Application.csproj

dotnet add src/Miastro.Infrastructure.Geography/Miastro.Infrastructure.Geography.csproj reference \
  src/Miastro.Application/Miastro.Application.csproj

dotnet add src/Miastro.Infrastructure.Time/Miastro.Infrastructure.Time.csproj reference \
  src/Miastro.Application/Miastro.Application.csproj

dotnet add src/Miastro.Infrastructure.Platform.Linux/Miastro.Infrastructure.Platform.Linux.csproj reference \
  src/Miastro.Application/Miastro.Application.csproj

dotnet add src/Miastro.Infrastructure.Printing.Linux/Miastro.Infrastructure.Printing.Linux.csproj reference \
  src/Miastro.Application/Miastro.Application.csproj

dotnet add src/Miastro.Infrastructure.SwissEphemeris/Miastro.Infrastructure.SwissEphemeris.csproj reference \
  src/Miastro.Astronomy.Abstractions/Miastro.Astronomy.Abstractions.csproj

dotnet add src/Miastro.Graphics/Miastro.Graphics.csproj reference \
  src/Miastro.Domain/Miastro.Domain.csproj

dotnet add src/Miastro.Graphics.Skia/Miastro.Graphics.Skia.csproj reference \
  src/Miastro.Graphics/Miastro.Graphics.csproj

dotnet add src/Miastro.Interpretation/Miastro.Interpretation.csproj reference \
  src/Miastro.Domain/Miastro.Domain.csproj

dotnet add src/Miastro.Reports/Miastro.Reports.csproj reference \
  src/Miastro.Domain/Miastro.Domain.csproj

dotnet add src/Miastro.Export/Miastro.Export.csproj reference \
  src/Miastro.Graphics/Miastro.Graphics.csproj \
  src/Miastro.Reports/Miastro.Reports.csproj

dotnet add src/Miastro.Bootstrap/Miastro.Bootstrap.csproj reference \
  src/Miastro.Application/Miastro.Application.csproj \
  src/Miastro.Infrastructure.Persistence/Miastro.Infrastructure.Persistence.csproj \
  src/Miastro.Infrastructure.Geography/Miastro.Infrastructure.Geography.csproj \
  src/Miastro.Infrastructure.Time/Miastro.Infrastructure.Time.csproj \
  src/Miastro.Infrastructure.Platform.Linux/Miastro.Infrastructure.Platform.Linux.csproj \
  src/Miastro.Infrastructure.Printing.Linux/Miastro.Infrastructure.Printing.Linux.csproj

dotnet add src/Miastro.Application/Miastro.Application.csproj package \
  Microsoft.Extensions.DependencyInjection

dotnet add src/Miastro.Application/Miastro.Application.csproj package \
  Microsoft.Extensions.Logging

dotnet add src/Miastro.Infrastructure.Persistence/Miastro.Infrastructure.Persistence.csproj package \
  Microsoft.EntityFrameworkCore

dotnet add src/Miastro.Infrastructure.Persistence/Miastro.Infrastructure.Persistence.csproj package \
  Microsoft.EntityFrameworkCore.Sqlite

dotnet add src/Miastro.Infrastructure.Persistence/Miastro.Infrastructure.Persistence.csproj package \
  Microsoft.EntityFrameworkCore.Design

dotnet add src/Miastro.Bootstrap/Miastro.Bootstrap.csproj package \
  Microsoft.Extensions.Configuration

dotnet add src/Miastro.Bootstrap/Miastro.Bootstrap.csproj package \
  Microsoft.Extensions.Configuration.Json

dotnet add src/Miastro.Bootstrap/Miastro.Bootstrap.csproj package \
  Microsoft.Extensions.DependencyInjection

dotnet add src/Miastro.Bootstrap/Miastro.Bootstrap.csproj package \
  Microsoft.Extensions.Logging

dotnet add src/Miastro.Bootstrap/Miastro.Bootstrap.csproj package \
  Microsoft.Extensions.Logging.Console

echo
echo ">>> SOLUCIÓN"
dotnet sln Miastro.sln list

echo
echo ">>> RESTORE"
dotnet restore Miastro.sln

echo
echo ">>> BUILD"
dotnet build Miastro.sln \
  --configuration Release \
  --no-restore

echo
echo ">>> FIN"
echo "Bootstrap inicial completado correctamente."

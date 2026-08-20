#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
cd "$ROOT"

mkdir -p \
  src/Miastro.UI.Avalonia/Navigation \
  src/Miastro.UI.Avalonia/Services \
  src/Miastro.UI.Avalonia/States \
  docs/architecture/ADR

# ------------------------------------------------------------
# UI — estado base
# ------------------------------------------------------------

cat > src/Miastro.UI.Avalonia/States/OperationState.cs <<'EOF'
namespace Miastro.UI.Avalonia.States;

public sealed class OperationState
{
    public bool IsLoading { get; private set; }

    public string? UserMessage { get; private set; }

    public void Begin()
    {
        IsLoading = true;
        UserMessage = null;
    }

    public void Complete()
    {
        IsLoading = false;
        UserMessage = null;
    }

    public void Fail(string userMessage)
    {
        IsLoading = false;
        UserMessage = userMessage;
    }
}
EOF

# ------------------------------------------------------------
# UI — política común de errores
# ------------------------------------------------------------

cat > src/Miastro.UI.Avalonia/Services/IUserErrorService.cs <<'EOF'
namespace Miastro.UI.Avalonia.Services;

public interface IUserErrorService
{
    string GetUserMessage(Exception exception);
}
EOF

cat > src/Miastro.UI.Avalonia/Services/UserErrorService.cs <<'EOF'
namespace Miastro.UI.Avalonia.Services;

public sealed class UserErrorService : IUserErrorService
{
    public string GetUserMessage(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return "No se pudo completar la operación. "
             + "Consulta el registro técnico si el problema continúa.";
    }
}
EOF

# ------------------------------------------------------------
# UI — navegación mínima
# ------------------------------------------------------------

cat > src/Miastro.UI.Avalonia/Navigation/INavigationService.cs <<'EOF'
namespace Miastro.UI.Avalonia.Navigation;

public interface INavigationService
{
    string CurrentRoute { get; }

    void NavigateTo(string route);
}
EOF

cat > src/Miastro.UI.Avalonia/Navigation/NavigationService.cs <<'EOF'
namespace Miastro.UI.Avalonia.Navigation;

public sealed class NavigationService : INavigationService
{
    public string CurrentRoute { get; private set; } = "home";

    public void NavigateTo(string route)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route);
        CurrentRoute = route;
    }
}
EOF

# ------------------------------------------------------------
# UI — registro DI
# ------------------------------------------------------------

cat > src/Miastro.UI.Avalonia/Services/UiServiceCollectionExtensions.cs <<'EOF'
using Microsoft.Extensions.DependencyInjection;
using Miastro.UI.Avalonia.Navigation;
using Miastro.UI.Avalonia.States;
using Miastro.UI.Avalonia.ViewModels;

namespace Miastro.UI.Avalonia.Services;

public static class UiServiceCollectionExtensions
{
    public static IServiceCollection AddMiastroUi(
        this IServiceCollection services)
    {
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IUserErrorService, UserErrorService>();
        services.AddTransient<OperationState>();
        services.AddSingleton<MainWindowViewModel>();

        return services;
    }
}
EOF

python3 - <<'PY'
from pathlib import Path

p = Path("/home/pablo/Aplicaciones/Miastro/src/Miastro.UI.Avalonia/Program.cs")
text = p.read_text()

if "using Miastro.UI.Avalonia.Services;" not in text:
    text = text.replace(
        "using Miastro.UI.Avalonia.ViewModels;",
        "using Miastro.UI.Avalonia.ViewModels;\nusing Miastro.UI.Avalonia.Services;"
    )

text = text.replace(
    "            services.AddSingleton<MainWindowViewModel>();",
    "            services.AddMiastroUi();"
)

p.write_text(text)
PY

# ------------------------------------------------------------
# ADR-001
# ------------------------------------------------------------

cat > docs/architecture/ADR/ADR-001-linux-ubuntu-dotnet10-avalonia.md <<'EOF'
# ADR-001 — Linux Ubuntu + .NET 10 + Avalonia

## Estado

Aceptado.

## Decisión

Miastro se implementa como aplicación de escritorio para Linux Ubuntu utilizando:

- C#
- .NET 10 LTS
- Avalonia UI
- MVVM

## Consecuencias

La interfaz queda desacoplada de la lógica de aplicación y dominio.

La distribución principal será nativa para Linux x64.

No se introduce dependencia de WPF, WinForms ni tecnologías exclusivas de Windows.
EOF

# ------------------------------------------------------------
# ADR-002
# ------------------------------------------------------------

cat > docs/architecture/ADR/ADR-002-modular-monolith-clean-architecture.md <<'EOF'
# ADR-002 — Monolito modular + Clean Architecture

## Estado

Aceptado.

## Decisión

Miastro utilizará un monolito modular con separación explícita entre:

- UI
- Application
- Domain
- abstracciones
- infraestructura
- gráficos
- interpretación
- informes
- exportación

La dirección de dependencias debe apuntar hacia contratos y modelos internos estables.

## Restricciones principales

Domain no depende de Avalonia, EF Core, SkiaSharp ni Swiss Ephemeris.

UI no accede directamente a DbContext ni Swiss Ephemeris.

Interpretation no depende de Swiss Ephemeris.

Graphics no depende de SQLite.

Persistence no depende de UI.
EOF

# ------------------------------------------------------------
# ADR-007
# ------------------------------------------------------------

cat > docs/architecture/ADR/ADR-007-sqlite-ef-core.md <<'EOF'
# ADR-007 — SQLite + Entity Framework Core

## Estado

Aceptado.

## Decisión

La persistencia local de Miastro utilizará SQLite mediante Entity Framework Core.

La base de datos de usuario reside en el directorio XDG Data de Miastro.

Las modificaciones de esquema se realizan mediante migraciones versionadas.

## Fase 1

Solo existe un esquema técnico mínimo destinado a validar:

- creación controlada
- migraciones
- lectura
- escritura
- permisos del fichero

No existe todavía un modelo de Persona ni entidades astrológicas de producción.
EOF

# ------------------------------------------------------------
# ADR-014
# ------------------------------------------------------------

cat > docs/architecture/ADR/ADR-014-xdg-directories.md <<'EOF'
# ADR-014 — Directorios XDG

## Estado

Aceptado.

## Decisión

Miastro utiliza las convenciones XDG para todos los datos generados durante ejecución.

Ubicaciones por defecto:

- datos persistentes: `~/.local/share/miastro/`
- configuración: `~/.config/miastro/`
- caché: `~/.cache/miastro/`
- estado y logs: `~/.local/state/miastro/`

Se respetan las variables XDG equivalentes cuando están definidas correctamente.

`XDG_RUNTIME_DIR` se utiliza para datos efímeros de ejecución cuando está disponible.

## Seguridad

Los directorios propios de Miastro se crean con permisos privados para el usuario.

Las rutas técnicas no forman parte de la interfaz ordinaria de usuario.
EOF

# ------------------------------------------------------------
# ADR-015
# ------------------------------------------------------------

cat > docs/architecture/ADR/ADR-015-self-contained-deb.md <<'EOF'
# ADR-015 — Publicación self-contained y paquete .deb

## Estado

Aceptado.

## Decisión

La distribución principal para Ubuntu será:

- `linux-x64`
- self-contained
- paquete `.deb`

Los binarios de aplicación se instalarán bajo `/usr`.

Los datos personales y de ejecución permanecerán fuera del paquete y se gestionarán mediante XDG.

## Desinstalación

La eliminación del paquete no debe borrar datos del usuario.
EOF

# ------------------------------------------------------------
# ADR-018
# ------------------------------------------------------------

cat > docs/architecture/ADR/ADR-018-reproducible-versioning.md <<'EOF'
# ADR-018 — Versionado reproducible

## Estado

Aceptado.

## Decisión

Los artefactos de Miastro deben poder asociarse de forma inequívoca con:

- versión de aplicación
- versión de migraciones
- versión del runtime
- versiones de dependencias
- versión futura de efemérides
- versión futura de TZDB
- versión futura de reglas de cálculo
- versión futura de plantillas de interpretación e informes

Las compilaciones se realizan con generación determinista habilitada.

No se incorporan todavía versiones funcionales de datos astrológicos externos en Fase 1.
EOF

# ------------------------------------------------------------
# Tests UI infrastructure
# ------------------------------------------------------------

cat > tests/Miastro.Tests/UiInfrastructureSourceTests.cs <<'EOF'
namespace Miastro.Tests;

[TestClass]
public sealed class UiInfrastructureSourceTests
{
    private static readonly string Root = FindRepositoryRoot();

    [TestMethod]
    public void Error_service_does_not_expose_stack_trace_to_user()
    {
        var file = Path.Combine(
            Root,
            "src",
            "Miastro.UI.Avalonia",
            "Services",
            "UserErrorService.cs");

        var text = File.ReadAllText(file);

        Assert.IsFalse(
            text.Contains("StackTrace", StringComparison.Ordinal));

        Assert.IsFalse(
            text.Contains("exception.ToString", StringComparison.Ordinal));
    }

    [TestMethod]
    public void Required_ADRs_exist()
    {
        string[] files =
        [
            "ADR-001-linux-ubuntu-dotnet10-avalonia.md",
            "ADR-002-modular-monolith-clean-architecture.md",
            "ADR-007-sqlite-ef-core.md",
            "ADR-014-xdg-directories.md",
            "ADR-015-self-contained-deb.md",
            "ADR-018-reproducible-versioning.md"
        ];

        foreach (var file in files)
        {
            Assert.IsTrue(
                File.Exists(
                    Path.Combine(
                        Root,
                        "docs",
                        "architecture",
                        "ADR",
                        file)),
                $"Falta ADR requerido: {file}");
        }
    }

    private static string FindRepositoryRoot()
    {
        var current =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            if (File.Exists(
                Path.Combine(current.FullName, "Miastro.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException(
            "No se encontró la raíz del repositorio.");
    }
}
EOF

dotnet restore Miastro.sln

dotnet build Miastro.sln \
  --configuration Release \
  --no-restore

dotnet test tests/Miastro.Tests/Miastro.Tests.csproj \
  --configuration Release \
  --no-build \
  --logger "console;verbosity=minimal"

echo
echo "=== IMPLEMENTACIÓN COMPLETADA ==="
echo "Navegación mínima: OK"
echo "Estado loading/error: OK"
echo "Política de error de usuario: OK"
echo "Servicios UI vía DI: OK"
echo "ADR obligatorios: OK"
echo "Build/tests: OK"

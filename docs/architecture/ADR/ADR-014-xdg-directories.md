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

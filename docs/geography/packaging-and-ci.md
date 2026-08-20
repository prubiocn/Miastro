# Fase 4 — empaquetado y CI del catálogo geográfico

## Release/local

El paquete de release usa el catálogo oficial generado desde el snapshot
GeoNames bloqueado por SHA-256.

Ruta instalada:

`/usr/share/miastro/geodata/`

Contenido:

- `geonames.sqlite`;
- `manifest.json`.

Licencias y atribución:

`/usr/share/doc/miastro/geonames/`

## CI

CI no depende del catálogo completo para cada ejecución.

Genera un fixture controlado y determinista de ocho localidades que permite
validar:

- publish;
- estructura SQLite;
- apertura read-only;
- empaquetado `.deb`;
- instalación;
- búsqueda smoke.

La reproducibilidad del catálogo oficial completo se prueba por separado con
el source lock y el DataBuilder de release.

## Portabilidad

`MIASTRO_GEODATA_DIR` permite seleccionar la entrada de geodata durante
publish sin rutas `/home/...` codificadas en el repositorio.

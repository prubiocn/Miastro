# GeoNames — fuentes de distribución de Fase 4

## Snapshot inicial

Fecha de adquisición: 2026-08-20.

Dataset base aprobado:

- `cities500.zip`;
- `alternateNamesV2.zip`;
- `countryInfo.txt`;
- `admin1CodesASCII.txt`;
- `admin2Codes.txt`;
- `timeZones.txt`.

Los tamaños y SHA-256 exactos se registran en:

`data/geography/geonames-source.lock.json`

## Licencia y atribución

GeoNames distribuye estos datos bajo Creative Commons Attribution 4.0
International.

Miastro conserva en el repositorio:

- `docs/licenses/GeoNames/ATTRIBUTION.md`;
- `docs/licenses/GeoNames/CC-BY-4.0.txt`;
- `docs/licenses/GeoNames/GeoNames-readme.txt`.

## Cobertura de cities500

`cities500` incluye ciudades con población superior a 500 y sedes de
divisiones administrativas hasta PPLA4.

Esta cobertura puede excluir localidades pequeñas válidas como lugares de
nacimiento. La limitación queda aceptada y documentada para V1; no se cambia
automáticamente a `allCountries`.

## Reproducibilidad

El DataBuilder de release debe rechazar cualquier fuente cuyo SHA-256 no
coincida con `geonames-source.lock.json`.

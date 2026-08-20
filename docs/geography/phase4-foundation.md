# Fase 4 — Base de geografía offline

## Estado del bloque 1

Se establece el contrato `ILocationSearchService`, tipos seguros para
latitud/longitud/IANA zone ID, errores tipados, normalización Unicode y un
catálogo SQLite de solo lectura.

## Esquema 1

Tablas:

- `metadata`;
- `locations`;
- `alternate_names`;
- `location_fts` (FTS5).

Índices:

- nombre normalizado;
- nombre ASCII normalizado;
- alternativos;
- país;
- admin1;
- admin2;
- zona horaria;
- GeoNameId como clave primaria.

## Fixture

`data/geography/fixtures/phase4-locations.tsv` es solo un corpus técnico de
pruebas. No sustituye a los dumps oficiales de GeoNames.

El DataBuilder genera:

- `geonames.sqlite`;
- `manifest.json`;
- SHA-256 de fixture;
- SHA-256 y tamaño de la base resultante.

La incorporación del dataset oficial completo y su inventario de licencia,
fecha y hashes permanece PENDING.

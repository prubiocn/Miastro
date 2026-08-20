# GeoNames — catálogo SQLite de distribución

## Builder

`tools/DataBuilder/build-geonames-release.py`

El builder verifica hashes, procesa los dumps de forma secuencial, conserva
nombres humanos originales, genera claves de búsqueda normalizadas, índices,
FTS5, `PRAGMA integrity_check` y un manifiesto con SHA-256.

## Esquema

Versión: 2.

Tablas:

- metadata;
- locations;
- alternate_names;
- location_fts.

El catálogo normal de aplicación se abre en modo SQLite `ReadOnly`.

## Ranking

1. nombre principal exacto;
2. alternativo exacto;
3. prefijo principal/ASCII;
4. prefijo alternativo;
5. FTS5;
6. orden administrativo estable.

La población no selecciona automáticamente un homónimo.

## TZDB

Cada zona IANA del catálogo se valida contra el proveedor TZDB embebido de
Noda Time.

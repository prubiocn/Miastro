# ADR-006 — GeoNames offline

## Estado

Aceptado.

## Decisión

Miastro utilizará GeoNames como catálogo geográfico offline, transformado de
forma reproducible a `geonames.sqlite`.

El catálogo de runtime se abre en modo SQLite `ReadOnly`.

## Dataset de release previsto

- cities500;
- alternateNamesV2;
- countryInfo;
- admin1CodesASCII;
- admin2Codes;
- zona IANA asociada a cada localidad.

## Ranking

1. nombre principal exacto;
2. nombre alternativo exacto;
3. prefijo de nombre principal/ASCII;
4. prefijo alternativo;
5. FTS en la evolución del DataBuilder;
6. jerarquía administrativa y población únicamente como desempates
   controlados.

Nunca se selecciona automáticamente un homónimo solo por población.

## Fase 4 — fixture

El primer bloque usa un fixture público mínimo para validar esquema,
normalización, homónimos, coordenadas y zonas antes de incorporar el dump
completo de GeoNames.

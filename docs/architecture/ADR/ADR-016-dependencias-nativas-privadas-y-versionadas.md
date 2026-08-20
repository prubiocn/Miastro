# ADR-016 — Dependencias nativas privadas y versionadas

## Estado

Aceptado.

## Decisión

Las dependencias nativas de Miastro deben:

- tener versión exacta;
- proceder de una fuente documentada;
- disponer de hash de integridad;
- cargarse desde una ruta controlada;
- no depender de `LD_LIBRARY_PATH`;
- no depender de una instalación global;
- validar plataforma y ABI.

Swiss Ephemeris usa esta política para `libswe.so`.

## Distribución

El publish `linux-x64` incorpora el artefacto nativo y su manifiesto.

El paquete Debian podrá instalarlo bajo la jerarquía privada de Miastro.

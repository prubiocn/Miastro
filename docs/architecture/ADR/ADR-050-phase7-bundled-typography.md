# ADR-050 — Tipografía empaquetada y determinista

## Estado

Aceptado para Fase 7.

## Decisión

Miastro empaqueta Source Sans 3 Regular dentro de
Miastro.Graphics.Skia.

La fuente se carga desde un EmbeddedResource y no mediante búsqueda de
fuentes instaladas en el sistema.

## Licencia

Source Sans 3 se distribuye bajo SIL Open Font License 1.1.

El archivo de licencia se conserva junto al recurso.

## Integridad

SHA-256 fuente:

    4644c81b86ec9caaa76b634889968ed3c4f4f52f054855933acc7c2b21e53b0f

SHA-256 licencia:

    56af9b9c6715597e458284a474dc118a50a4150e9d547c70f7b4a33c3e6a9328

## Restricciones

Miastro.Graphics no depende de:

- archivos TTF
- SkiaSharp
- Avalonia
- fuentes del sistema

Los símbolos astrológicos siguen utilizando el catálogo vectorial
interno.

# ADR-048 — Catálogo vectorial astrológico interno

## Estado

Aceptado para Fase 7.

## Contexto

La rueda debe ser reproducible en instalaciones, CI, render headless
y futuros destinos de exportación.

Depender de una fuente astrológica externa produciría diferencias de
métricas, disponibilidad y representación.

## Decisión

Los símbolos astrológicos se representan mediante geometría vectorial
propia almacenada en Miastro.Graphics.

GlyphNode referencia una GlyphKey.

El backend gráfico resuelve esa clave mediante
NatalVectorGlyphCatalog.

## Restricciones

El catálogo:

- no depende de SkiaSharp
- no depende de Avalonia
- no depende de fuentes astrológicas del sistema
- no contiene cálculos astrológicos
- es determinista

## Consecuencias

La misma definición puede ser reutilizada por distintos backends y
por la futura exportación.

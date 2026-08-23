# ADR-043 — Snapshot de layout determinista e inspeccionable

## Estado

Aceptado para Fase 7.

## Contexto

Los tests geométricos, el render headless y la futura exportación
necesitan una representación intermedia independiente del renderer.

## Decisión

Miastro.Graphics genera un NatalWheelLayoutSnapshot antes de crear
la escena final.

El snapshot contiene métricas, sectores zodiacales, marcas de grado,
cúspides de casas y ejes principales.

Misma entrada y misma configuración deben producir exactamente el
mismo snapshot.

No se permite:

- aleatoriedad
- dependencia del orden accidental de colecciones
- cálculo astronómico
- acceso a Swiss Ephemeris
- dependencia de Avalonia
- dependencia de SkiaSharp

## Consecuencias

La cadena gráfica queda separada en:

datos
-> layout
-> scene
-> render

La futura lógica anti-solapamiento extenderá este snapshot sin
modificar las posiciones astrológicas reales.

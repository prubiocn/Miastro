# ADR-062 — Viewport responsive, HiDPI y accesibilidad Avalonia

## Estado

Aceptado para Fase 7.

## Decisión

Avalonia controla únicamente:

- tamaño del viewport
- factor físico RenderScaling
- foco
- eventos de ratón y teclado
- presentación accesible

La geometría natal permanece en Miastro.Graphics.

## Escalado

La imagen utiliza Stretch Uniform.

Hit testing y renderer comparten la misma semántica de transformación
uniforme y centrada.

Las zonas de letterboxing no son interactivas.

## HiDPI

RenderScaling solo determina la resolución física del PNG.

No modifica longitudes astrológicas, layout ni Scene Graph.

## Accesibilidad

Los objetos visibles pueden recorrerse por teclado.

La selección gráfica tiene equivalente textual mediante el ViewModel y
AutomationProperties.

## Límites

Avalonia no contiene:

- transformación eclíptica
- algoritmo anti-solapamiento
- cálculo de casas
- astronomía
- acceso directo a Swiss Ephemeris

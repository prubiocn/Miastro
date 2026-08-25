# ADR-061 — Escalado HiDPI uniforme y contexto por render

## Estado

Aceptado para Fase 7.

## Problema

El renderer utilizaba escalas X e Y independientes.

Una superficie con relación de aspecto diferente podía deformar la
geometría.

Además, determinados catálogos y la tipografía se reconstruían por nodo.

## Decisión

El renderer utiliza un único factor de escala uniforme y centra la
escena sobre la superficie de destino.

Cada RenderPng crea un RenderContext con los catálogos de glifos,
estilos y tipografía.

Los nodos reutilizan ese contexto.

## HiDPI

La densidad física de salida no modifica el Scene Graph.

El mismo modelo lógico puede renderizarse a 1x, 1,5x, 2x o 3x.

## Rendimiento

Las optimizaciones no introducen cachés globales mutables ni cambian la
API pública.

El contexto tiene duración de una única operación RenderPng.

## Arquitectura

Graphics.Skia continúa sin astronomía, Swiss Ephemeris ni acceso a
persistencia.

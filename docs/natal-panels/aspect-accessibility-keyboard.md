# Fase 8 — accesibilidad y teclado de Aspectos

## Identidad accesible

Cada celda significativa consume `AccessibleName` generado en la capa headless.

El nombre incluye:

- primer objeto;
- tipo de aspecto;
- segundo objeto;
- orbe.

Ejemplo:

Sol — cuadratura — Saturno — orbe 2°14′

## Navegación

El control visual mantiene el comportamiento estándar de `ListBox` de Avalonia.

Las flechas recorren las celdas visibles y la selección se propaga mediante `SelectedItem` a `SelectedAspectCell`.

No existe un handler global que capture las flechas.

Escape continúa siendo la única tecla global de neutralización.

## No dependencia del color

Cada aspecto muestra simultáneamente:

- objeto A;
- símbolo;
- nombre textual del aspecto;
- objeto B;
- orbe.

La información esencial no depende de color.

## Tooltip

La misma descripción factual accesible se utiliza como tooltip y como texto de ayuda de automatización.

## Alcance

Este bloque refuerza accesibilidad y operación por teclado de las celdas significativas.

La composición visual triangular completa se implementará separadamente para no acoplar layout y semántica de accesibilidad.

# Fase 8 — Shell XAML de paneles natales

## Estructura

La rueda sigue ocupando la zona principal de la carta natal.

El panel derecho utiliza aproximadamente un tercio del espacio disponible mediante una relación 2:1.

## Pestañas

El shell contiene exactamente:

1. Datos.
2. Posiciones.
3. Aspectos.
4. Distribución.
5. Resumen.

La pestaña inicial continúa siendo Posiciones y su estado se mantiene en `NatalPanelHostViewModel`.

## Migración progresiva

El panel lateral de detalle de objeto de Fase 7 se sustituye por el shell de Fase 8.

Los paneles lineales heredados de Posiciones y Aspectos situados debajo de la rueda se mantienen temporalmente para proteger la regresión de Fase 7.

Se retirarán únicamente cuando la selección y accesibilidad nuevas hayan quedado validadas.

## Aspectos

En este bloque se presentan las celdas significativas de la matriz como shell funcional de lectura.

La matriz triangular visual completa, navegación bidimensional por teclado y selección dual se implementan en bloques posteriores.

## Distribución

Distribución utiliza exclusivamente contenido textual y estructurado.

No existen barras ni dashboard gráfico.

## Accesibilidad

Las filas significativas de aspectos consumen `AccessibleName` procedente de la capa headless.

Los cinco TabItems tienen nombres funcionales y navegables mediante el comportamiento estándar de Avalonia.

## Compatibilidad

Se preservan:

- rueda natal;
- controles de visibilidad;
- modos Consulta/Presentación;
- hit testing;
- tooltip;
- Scene Graph;
- renderer SkiaSharp.

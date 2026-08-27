# ADR-080 — Presentación estructurada de Distribución y Resumen

## Estado

Aceptado para Fase 8.

## Contexto

Los servicios headless ya calculan toda la información necesaria, pero la UI
solo mostraba listas planas de líneas.

## Decisión

Crear una proyección exclusivamente de presentación para cada sección de
Distribución.

La proyección conserva:

- etiqueta;
- recuento;
- nombres de objetos;
- estado de predominio.

No se recalcula ninguna categoría.

Las siete secciones se muestran como texto estructurado sin barras.

Resumen expone directamente las propiedades de `NatalSummaryReadModel` y la
lista ya ordenada de aspectos principales.

## Consecuencias

La pantalla gana legibilidad sin trasladar reglas de dominio a Avalonia.

La política de objetos sigue centralizada en `NatalDistributionProfile`.

La selección de aspectos, orbes y movimiento no se recalculan.

La presentación sigue siendo factual y breve.

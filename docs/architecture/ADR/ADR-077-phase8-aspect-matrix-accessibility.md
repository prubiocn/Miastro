# ADR-077 — Accesibilidad de la matriz de aspectos natal

## Estado

Aceptado para Fase 8.

## Contexto

La matriz de aspectos debe ser operable sin ratón y comprensible sin depender del color.

## Decisión

La semántica accesible procede de `NatalAspectMatrixCell.AccessibleName`.

La presentación visual muestra explícitamente los dos objetos, símbolo, nombre del aspecto y orbe.

La navegación por teclado reutiliza el comportamiento estándar del control `ListBox` y no introduce un sistema paralelo de foco.

La ventana solo intercepta Escape como acción global de limpieza.

## Consecuencias

La misma identidad factual alimenta lector de pantalla, tooltip y selección dual.

No se duplica la descripción de los aspectos en la UI.

El layout triangular visual puede evolucionar sin cambiar el contrato de accesibilidad.

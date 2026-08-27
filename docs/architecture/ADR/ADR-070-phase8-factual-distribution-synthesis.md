# ADR-070 — Síntesis factual de Distribución

## Estado

Aceptado para Fase 8.

## Contexto

La pestaña Distribución necesita cerrar con una lectura rápida de su estructura sin entrar en interpretación astrológica extensa.

## Decisión

Crear `NatalDistributionSynthesisBuilder`.

El builder consume exclusivamente resultados de:

- `NatalDistributionService`;
- `NatalHouseDistributionService`.

Produce un máximo de siete líneas factuales y deterministas.

No introduce conclusiones psicológicas, conductuales, predictivas ni normativas.

Un predominio se muestra únicamente cuando el servicio ya ha determinado un máximo único.

Los empates se preservan y no se resuelven arbitrariamente.

Las dos distribuciones deben usar el mismo `DistributionProfile`.

## Consecuencias

La UI obtiene una síntesis breve sin duplicar reglas.

Resumen puede reutilizar esta capa.

La futura interpretación textual queda separada explícitamente de los hechos y agregados estructurales.

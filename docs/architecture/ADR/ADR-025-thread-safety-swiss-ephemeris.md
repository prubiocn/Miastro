# ADR-025 — Thread safety de Swiss Ephemeris

## Estado

Aceptado.

## Contexto

Swiss Ephemeris mantiene estado global relacionado con configuración,
paths y recursos internos.

## Decisión

Miastro no presupone thread safety nativa.

Todas las llamadas Swiss se serializan mediante una puerta única dentro
del adaptador.

## Consecuencia

Se prioriza corrección y reproducibilidad.

La optimización concurrente queda fuera de Fase 3.

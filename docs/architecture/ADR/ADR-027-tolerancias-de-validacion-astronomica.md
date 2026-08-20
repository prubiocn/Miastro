# ADR-027 — Tolerancias de validación astronómica

## Estado

Aceptado.

## Decisión

La comparación con referencias externas no utiliza igualdad exacta de
`double`.

Tolerancias V1:

- longitud: 0.0001°
- velocidad longitudinal: 0.0001°/día
- cúspides: 0.0001°
- ASC: 0.0001°
- MC: 0.0001°

Los valores del adaptador no se redondean previamente.

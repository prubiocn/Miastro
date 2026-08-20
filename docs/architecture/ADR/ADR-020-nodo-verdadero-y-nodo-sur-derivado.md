# ADR-020 — Nodo Verdadero y Nodo Sur derivado

## Estado

Aceptado.

## Decisión

Miastro V1 utiliza Nodo Norte Verdadero.

Nodo Sur se obtiene exclusivamente como:

`Nodo Norte Verdadero + 180°`

normalizado a `[0°,360°)`.

## Consecuencia

El Nodo Sur existe como objeto astrológico, pero no requiere un cálculo astronómico independiente.

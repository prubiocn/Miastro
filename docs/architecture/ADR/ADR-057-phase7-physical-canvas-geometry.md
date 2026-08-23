# ADR-057 — El canvas físico gobierna la geometría

## Estado

Aceptado para Fase 7.

## Contexto

La implementación inicial calculaba:

effectiveSize = max(min(width, height), MinimumUsableSize)

Esto podía producir una rueda físicamente mayor que un canvas pequeño.

## Decisión

La geometría utiliza:

effectiveSize = min(width, height)

El tamaño mínimo recomendado deja de intervenir en el cálculo físico de
radios.

## Separación de responsabilidades

NatalWheelMetrics:

- tamaño
- centro
- escala
- radios

NatalWheelResponsivePolicy:

- Full
- Compact
- Minimal
- degradación de detalle

## Consecuencias

La geometría queda contenida incluso en superficies inferiores al
tamaño recomendado y mantiene comportamiento determinista.

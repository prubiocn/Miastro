# ADR-046 — Política de leader lines

## Estado

Aceptado para Fase 7.

## Decisión

Todo objeto conserva una marca exacta en su posición real.

Si el glifo se desplaza una distancia igual o superior al umbral
definido por NatalGlyphLayoutPolicy, se genera una leader line desde
RealAnchor hasta VisualCenter.

No se genera leader line cuando el desplazamiento es pequeño o
inexistente.

La línea es un recurso puramente gráfico y nunca altera la longitud
astrológica.

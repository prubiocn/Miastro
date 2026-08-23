# ADR-044 — Separación entre posición astrológica real y posición visual

## Estado

Aceptado para Fase 7.

## Decisión

Todo objeto visible conserva por separado:

- longitud real
- ángulo gráfico real
- marca de posición real
- posición visual del glifo
- bounding box visual
- nivel radial
- leader line cuando proceda

La longitud real es inmutable.

Los algoritmos gráficos nunca escriben de vuelta sobre datos de
dominio ni snapshots natales persistidos.

## Consecuencias

La legibilidad puede mejorarse sin falsear la posición
astrológica.

Etiquetas y valores numéricos deben derivar siempre del dato real.

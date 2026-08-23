# ADR-052 — Configuración visual independiente del cálculo natal

## Estado

Aceptado para Fase 7.

## Decisión

Las opciones de visibilidad y los modos Consultation/Presentation se
aplican después del cálculo de layout y placement.

La cadena es:

snapshot natal
-> layout
-> placement
-> configuración visual
-> scene graph
-> renderer

## Consecuencia principal

Cambiar una preferencia visual no puede provocar:

- llamada a Swiss Ephemeris
- recálculo natal
- cambio de longitud real
- cambio de cúspides
- cambio de placement persistido
- escritura de posiciones visuales en base de datos

## Responsive

NatalWheelResponsivePolicy decide únicamente qué detalle secundario
se conserva según el tamaño disponible.

La geometría astrológica principal sigue siendo la misma.

## Persistencia futura

Si se persisten preferencias visuales, solo podrán almacenarse
opciones semánticas.

Nunca se persistirán posiciones gráficas absolutas.

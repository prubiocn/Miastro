# Miastro — Cableado de retrogradación en etiquetas

## Fuente

La única fuente para el marcador visual de retrogradación es:

NatalPlacementSnapshot.Motion

persistido por la fase de cálculo natal.

## Mapeo

El adaptador gráfico aplica:

MotionState.Retrograde -> IsRetrograde = true

Los estados:

- Direct
- Stationary
- null

producen IsRetrograde = false.

## Prohibición de recálculo

Miastro.Graphics no:

- inspecciona velocidad para deducir movimiento
- recalcula posiciones
- consulta Swiss Ephemeris
- modifica Motion

## Presentación

NatalSceneObjectInput transporta el dato semántico.

NatalWheelSceneBuilder añade el marcador discreto R cuando
IsRetrograde es true.

Por tanto, la R visible es una proyección directa del snapshot
persistido.

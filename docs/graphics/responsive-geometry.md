# Miastro — Geometría responsive de la rueda natal

## Regla principal

El canvas físico gobierna siempre los límites geométricos.

NatalWheelMetrics utiliza el lado menor real de la superficie para
calcular radios y escala.

MinimumUsableSize no puede inflar la geometría.

## Geometría y detalle

Son responsabilidades distintas:

- NatalWheelMetrics define geometría física.
- NatalWheelResponsivePolicy define degradación visual.

Full, Compact y Minimal pueden reducir detalle, pero nunca aumentar los
radios más allá del viewport.

## Invariantes

Para cualquier superficie válida:

- OuterRadius no supera la mitad del lado menor.
- el círculo exterior queda completamente dentro del canvas.
- los radios concéntricos mantienen orden estricto.
- AspectRadius permanece positivo.
- el centro coincide con el centro físico.
- mismo tamaño produce exactamente las mismas métricas.

## Tamaños validados

Fase 7 cubre explícitamente:

- 300
- 360
- 480
- 720
- 800

además de superficies rectangulares.

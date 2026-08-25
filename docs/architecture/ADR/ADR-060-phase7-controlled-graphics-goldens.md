# ADR-060 — Goldens gráficos controlados y tolerantes

## Estado

Aceptado para Fase 7.

## Contexto

Una comparación exclusiva de bytes PNG puede ser excesivamente frágil
ante diferencias menores de rasterización.

Por otro lado, una tolerancia sin artefactos de referencia no protege
adecuadamente frente a regresiones visuales.

## Decisión

Miastro conserva PNG de referencia controlados y un manifiesto SHA-256.

La validación normal combina:

- integridad SHA-256 del baseline
- comparación raster tolerante
- invariantes geométricos y estructurales
- determinismo byte a byte en el mismo runtime

## Tolerancia

Un canal se considera significativamente distinto cuando su diferencia
es mayor de 12.

El render pasa cuando:

- como máximo el 1 % de píxeles presenta diferencia significativa
- la diferencia media por canal no supera 0,50

## Sistemas de casas

Los fixtures denominados Placidus y Koch contienen cúspides ya
resueltas.

Graphics no calcula casas ni consulta Swiss Ephemeris.

## Regeneración

La suite nunca actualiza goldens de manera implícita.

Se requiere MIASTRO_UPDATE_PHASE7_GOLDENS=1.

## Consecuencia

Los cambios de estilo o renderer requieren una actualización deliberada
y revisable de los baselines.

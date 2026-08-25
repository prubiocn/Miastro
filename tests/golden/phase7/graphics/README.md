# Miastro Phase 7 graphics goldens

Este directorio contiene baselines técnicas del renderer headless de la
rueda natal.

Casos obligatorios:

- simple
- stellium
- many-aspects
- placidus
- koch

Placidus y Koch son fixtures gráficos sintéticos. Graphics recibe las
cúspides ya persistidas y no calcula sistemas de casas.

## Actualización

Los baselines solo pueden regenerarse mediante la variable explícita:

MIASTRO_UPDATE_PHASE7_GOLDENS=1

Después deben ejecutarse nuevamente los tests sin esa variable.

## Integridad

manifest.tsv contiene SHA-256 del PNG y SHA-256 de la descripción
determinista de la escena usada al crear cada baseline.

El hash protege los artefactos de referencia frente a modificaciones
accidentales.

## Comparación visual

La regresión visual no depende únicamente de igualdad exacta de PNG.

Se decodifica el raster RGBA y se aplican estas tolerancias:

- diferencia relevante de canal: mayor de 12
- máximo de píxeles relevantes modificados: 1 %
- diferencia media máxima por canal: 0,50

Esto permite pequeñas variaciones de rasterización sin aceptar cambios
visuales estructurales.

## Invariantes adicionales

Los tests verifican también:

- 12 signos
- 12 cúspides
- coordenadas finitas
- ausencia de colisiones entre etiquetas de objetos
- ausencia de colisiones etiqueta/glifo
- determinismo byte a byte dentro del mismo runtime

La fuente tipográfica de la escena está embebida en Miastro.Graphics.Skia.

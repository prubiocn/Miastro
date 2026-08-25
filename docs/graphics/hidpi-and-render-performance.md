# Miastro — HiDPI y rendimiento del renderer natal

## Escalado

El Scene Graph trabaja en unidades lógicas.

SkiaNatalSceneRenderer recibe por separado las dimensiones físicas del
PNG.

El factor de escala es uniforme:

min(pixelWidth / scene.Width, pixelHeight / scene.Height)

Cuando la relación de aspecto de la superficie difiere de la escena,
la escena se centra y conserva su proporción.

No se permite deformación independiente en X e Y.

## HiDPI

El renderer soporta superficies físicas equivalentes a:

- 1x
- 1,5x
- 2x
- 3x

El Scene Graph no cambia por la densidad de píxel.

## Contexto de render

Cada llamada RenderPng crea un único contexto que contiene:

- NatalVectorGlyphCatalog
- NatalSceneStyleCatalog
- SkiaTypographyProvider

Estos recursos se reutilizan para todos los nodos de esa imagen.

Los recursos nativos de tipografía se liberan al finalizar el render.

## Rendimiento

La suite incluye un smoke test sobre una escena deliberadamente cargada.

El objetivo no es medir microsegundos ni imponer un benchmark de
hardware.

El límite de 10 segundos para cinco renders de 800x800 detecta
únicamente regresiones catastróficas y evita fragilidad en CI.

## Determinismo

Para una misma escena y una misma resolución física, el PNG sigue siendo
determinista byte a byte dentro del mismo runtime.

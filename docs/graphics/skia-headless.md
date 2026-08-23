# Miastro — Renderer Skia headless

## Responsabilidad

Miastro.Graphics.Skia es un backend de render.

Consume NatalScene y produce píxeles.

No calcula astronomía, casas, aspectos ni posiciones.

## Clipping

El renderer aplica clipping explícito al rectángulo completo de la
superficie antes de pintar la escena.

## Escalado

NatalScene usa unidades geométricas independientes del dispositivo.

El renderer escala la escena al tamaño real de la superficie de
salida.

## Headless

El renderer no depende de Avalonia.

Puede generar PNG directamente sobre una superficie Skia sin ventana
ni servidor gráfico.

## PNG técnico

SkiaTechnicalPngWriter permite guardar una imagen de diagnóstico para
tests y herramientas internas.

No constituye todavía la exportación final del usuario.

## Glifos y texto en 7C1

GlyphNode se representa temporalmente con una marca vectorial
geométrica determinista.

TextNode se representa temporalmente mediante su bounding box.

Esto evita introducir en 7C1 dependencias de fuentes del sistema.

El catálogo vectorial astrológico interno y la tipografía empaquetada
se implementarán en bloques posteriores.

## Determinismo

Con la misma escena, tamaño de superficie, versión de Skia y recursos
controlados, el renderer produce la misma salida.

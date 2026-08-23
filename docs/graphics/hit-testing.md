# Miastro — Hit testing de la rueda natal

## Regla principal

La interacción usa la geometría visual final.

Si un glifo ha sido desplazado por el algoritmo anti-solapamiento,
su zona interactiva se desplaza con él.

No se usa la longitud real para decidir dónde ha pulsado el usuario.

## Objetivos seleccionables

En esta fase son seleccionables:

- BodyLayer
- PointLayer

Solo se consideran GlyphNode cuyo Id comienza por object-glyph-.

Los glifos zodiacales no son objetivos de selección.

## Bounding boxes

El hit testing utiliza Bounds del GlyphNode.

Esos bounds son los mismos que utiliza el layout visual.

Puede aplicarse una tolerancia positiva para mejorar la ergonomía de
puntero sin modificar el layout.

## Determinismo

Si varias geometrías seleccionables coincidieran, la prioridad es:

1. capa gráfica superior
2. Id ordinal estable

No se usa orden accidental de colección.

## Separación real / visual

RealAnchor continúa representando la posición astrológica.

VisualCenter y Bounds representan la posición interactiva.

La selección nunca modifica ninguno de ambos.

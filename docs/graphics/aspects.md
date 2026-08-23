# Miastro — Aspectos en la rueda natal

## Fuente de verdad

La capa gráfica no determina aspectos.

Los aspectos llegan desde datos ya calculados y persistidos.

Miastro.Graphics no contiene:

- reglas de orbe
- reglas de aplicación/separación
- búsqueda de aspectos
- recálculo astronómico

## Geometría

Cada extremo de un aspecto se obtiene a partir del ángulo gráfico
real del objeto.

No se usa VisualCenter.

Por tanto, un glifo desplazado por anti-solapamiento no desplaza la
geometría astrológica del aspecto.

## Radio interior

Los aspectos se proyectan sobre AspectRadius.

Esto mantiene las líneas alejadas de:

- glifos de cuerpos
- etiquetas
- anillo zodiacal

y preserva una zona central más limpia.

## Visibilidad

ShowAspects controla exclusivamente la generación de nodos de la
capa AspectLayer.

Ocultar aspectos no:

- recalcula la carta
- modifica placements
- modifica longitudes
- cambia el layout de cuerpos

## Objetos ocultos

Si uno de los extremos no está presente entre los placements visibles,
el aspecto no se dibuja.

## Jerarquía visual

La clasificación visual llega explícitamente como:

- Major
- Secondary

Miastro.Graphics no deduce esa clasificación a partir de ángulos.

Los estilos se diferencian mediante grosor, patrón y opacidad, no solo
mediante color.

## Determinismo

Los aspectos se ordenan por Id ordinal estable antes de crear la
escena.

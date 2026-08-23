# ADR-051 — Los aspectos gráficos consumen datos persistidos

## Estado

Aceptado para Fase 7.

## Decisión

Miastro.Graphics recibe aspectos ya existentes como entrada.

El módulo gráfico no calcula si dos cuerpos forman un aspecto.

NatalAspectSceneBuilder únicamente transforma:

- identidad de aspecto
- objetos extremos
- clasificación visual

en geometría Scene Graph.

## Posición

Los extremos se calculan usando la posición real del objeto y
AspectRadius.

Nunca se utiliza la posición desplazada del glifo para alterar la
geometría astrológica.

## Visibilidad

La ocultación de AspectLayer es una preferencia visual y no provoca
recalculo.

## Límites arquitectónicos

El builder no depende de:

- Swiss Ephemeris
- persistencia
- Avalonia
- SkiaSharp
- reglas de aspectos del dominio

La futura adaptación desde el snapshot natal persistido se realizará
fuera del renderer.

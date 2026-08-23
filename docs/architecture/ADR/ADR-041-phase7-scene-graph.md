# ADR-041 — Scene Graph propio para la rueda natal

## Estado

Aceptado para Fase 7.

## Contexto

La rueda natal debe poder renderizarse en pantalla y reutilizarse en
fases posteriores para otros destinos sin duplicar geometría.

La lógica geométrica no puede residir en controles Avalonia ni en
comandos imperativos específicos de Skia.

## Decisión

Miastro.Graphics será propietario de un scene graph independiente
del renderer.

Las primitivas iniciales son:

- CircleNode
- ArcNode
- LineNode
- GlyphNode
- TextNode
- PathNode
- GroupNode

El orden de pintura se representa mediante SceneLayer.

Miastro.Graphics.Skia consumirá esta escena, pero no podrá:

- recalcular astronomía
- modificar longitudes astrológicas
- decidir reglas de aspectos
- introducir dependencias Avalonia en el scene graph

## Flujo arquitectónico

    datos persistidos
    -> layout
    -> scene graph
    -> renderer

La escena podrá ser inspeccionada mediante tests y utilizada en
render headless.

# Fase 8 — atenuación en selección dual

## Regla

La atenuación se activa únicamente cuando existe una selección dual asociada
a un aspecto.

La selección simple mantiene el comportamiento de resaltado de Fase 8F3 sin
atenuar el resto de la rueda.

## Elementos destacados

Permanecen destacados:

- objeto A;
- objeto B;
- todos los segmentos visibles de la línea del aspecto activo.

Utilizan `InteractionSelected`.

## Elementos atenuados

Se atenúan:

- los demás nodos `object-glyph-*`;
- las demás líneas de `AspectLayer`.

Utilizan `InteractionDimmed`.

## Elementos no afectados

No se atenúan:

- anillos;
- grados;
- casas;
- cúspides;
- geometría angular;
- etiquetas;
- fondo.

## Arquitectura

La escena base permanece intacta.

La atenuación y el resaltado se implementan mediante copias visuales en
`InteractionOverlay`.

No se modifica ningún nodo base ni se recalcula ningún aspecto.

## Idempotencia

Antes de aplicar un nuevo estado se eliminan únicamente los nodos previos de
selección cuyo identificador comienza por `selection-`.

Aplicar dos veces el mismo estado produce el mismo conjunto de overlays.

## Render

`InteractionDimmed` es un estilo semántico registrado en
`NatalSceneStyleCatalog`.

Skia lo consume mediante el mecanismo normal de `StyleKey`; no contiene
lógica específica de selección.

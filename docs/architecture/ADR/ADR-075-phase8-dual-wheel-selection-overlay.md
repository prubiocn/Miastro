# ADR-075 — Fase 8: overlay visual para selección dual natal

## Estado

Aceptado.

## Contexto

Fase 8 requiere sincronizar la selección factual de objetos y aspectos con
la rueda natal ya cerrada en Fase 7.

La solución no puede introducir cálculo astronómico en Graphics ni alterar
las posiciones reales o el layout antisolapamiento.

## Decisión

La selección visual se implementa mediante NatalSceneSelectionOverlay.

El componente recibe una escena ya construida y añade copias visuales de
los nodos seleccionados en SceneLayer.InteractionOverlay.

Los nodos base permanecen sin modificar.

Para objetos se reutiliza el GlyphNode correspondiente a
object-glyph-{ObjectId}.

Para un aspecto se reutilizan todos los LineNode cuyo identificador
corresponde al par seleccionado. Esto cubre tanto una línea única como los
segmentos producidos por el recorte alrededor del núcleo.

Los overlays usan NatalSceneStyleKeys.InteractionSelected.

Antes de aplicar un nuevo estado se eliminan solamente nodos propios con
prefijo selection-, haciendo la operación idempotente.

## Consecuencias

No hay cambios en:

- astronomía;
- persistencia;
- longitudes reales;
- posiciones visuales;
- geometría de casas;
- geometría de aspectos;
- goldens de la escena neutra.

La selección dual puede reconstruirse tras cambios de viewport o
configuración visual sin recalcular la carta.

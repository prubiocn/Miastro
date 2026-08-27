# ADR-081 — Atenuación semántica de selección dual

## Estado

Aceptado para Fase 8.

## Contexto

La selección de una celda de aspectos debe destacar los dos objetos y la
línea activa, y reducir visualmente la prominencia del resto.

La implementación previa solo añadía resaltado.

## Decisión

Añadir el estilo semántico `InteractionDimmed`.

La atenuación se construye exclusivamente en
`NatalSceneSelectionOverlay`.

Cuando existe un par de aspecto activo:

- los objetos no seleccionados reciben una copia visual atenuada;
- los aspectos ajenos al par activo reciben una copia visual atenuada;
- los dos objetos seleccionados reciben `InteractionSelected`;
- los segmentos del aspecto activo reciben `InteractionSelected`.

La escena base no se modifica.

La selección simple no activa atenuación.

## Consecuencias

El requisito de destacar el par y atenuar el resto queda expresado de forma
semántica y testeable.

Skia no necesita conocer el estado de selección.

Hit-testing, geometría y snapshot permanecen independientes del efecto visual.

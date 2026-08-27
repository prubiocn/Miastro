# Fase 8 — Selección dual de aspectos

## Estado dual

Seleccionar una celda significativa de Aspectos crea un `NatalSelectionState` dual con:

- objeto primario;
- objeto secundario;
- aspecto activo.

La pareja se normaliza mediante `NatalObjectOrder`.

## Sincronización

La selección dual sincroniza:

- `SelectedAspectCell`;
- fila primaria de Datos;
- fila secundaria de Posiciones;
- estado factual dual.

La pestaña activa pasa a Aspectos.

## MainWindow

El host emite `AspectSelectionRequested`.

`MainWindowViewModel` recibe el evento y mantiene el puente con el mecanismo de selección de rueda existente.

El resaltado gráfico simultáneo de ambos objetos y de la línea del aspecto se implementará en un bloque visual posterior.

## Limpieza

`ClearSelection` elimina:

- aspecto activo;
- ambos objetos;
- filas seleccionadas.

El resultado vuelve a `NatalSelectionState.Neutral`.

## Prevención de bucles

`SyncDualSelection` es una operación programática y no vuelve a emitir `AspectSelectionRequested`.

## Alcance

Este bloque define estado, binding y ciclo de selección dual.

No modifica geometría, Scene Graph ni renderer.

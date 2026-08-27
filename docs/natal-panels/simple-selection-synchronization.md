# Fase 8 — Sincronización simple de selección

## Identidad

Toda sincronización utiliza `AstrologicalObjectId`.

No se utilizan nombres, índices visuales ni posiciones de fila como identidad.

## Datos → rueda

Seleccionar una fila de Datos:

1. identifica el objeto por `ObjectId`;
2. sincroniza la fila equivalente de Posiciones;
3. abre Posiciones;
4. solicita a `MainWindowViewModel` la selección del mismo objeto en la rueda.

## Posiciones → rueda

Seleccionar una fila de Posiciones solicita la selección del mismo objeto en la rueda.

Datos queda sincronizado con el mismo `ObjectId`.

## Rueda → Posiciones

La selección realizada por hit testing o teclado en la rueda:

1. conserva el mecanismo de Fase 7;
2. sincroniza Datos y Posiciones;
3. abre Posiciones.

## Prevención de bucles

`SyncSelectedObject` utiliza una sección interna de sincronización que actualiza ambos paneles sin volver a emitir `ObjectSelectionRequested`.

Por tanto, rueda → panel no provoca panel → rueda de forma recursiva.

## Limpieza

Cuando la rueda vuelve al estado neutral, las filas seleccionadas de Datos y Posiciones se limpian sin emitir una nueva solicitud.

## Alcance

Este bloque implementa únicamente selección simple.

La selección dual de aspectos se implementará posteriormente.

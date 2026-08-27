# ADR-073 — Sincronización simple rueda-panel por ObjectId

## Estado

Aceptado para Fase 8.

## Contexto

Datos, Posiciones y rueda deben mantener una única selección coherente sin introducir acoplamiento entre controles ni bucles de eventos.

## Decisión

Usar `AstrologicalObjectId` como única identidad de sincronización.

`NatalPanelHostViewModel` mantiene las filas seleccionadas de Datos y Posiciones y emite una solicitud tipada al seleccionar desde un panel.

`MainWindowViewModel` adapta esa solicitud al mecanismo de selección de rueda existente.

La selección procedente de rueda vuelve al host mediante `SyncSelectedObject`.

Las actualizaciones internas no vuelven a emitir solicitudes.

Seleccionar desde Datos abre automáticamente Posiciones.

## Consecuencias

No hay sincronización basada en texto ni índices.

La lógica es comprobable sin interacción gráfica real.

La selección dual de aspectos podrá extender el mismo contrato sin sustituir la selección simple.

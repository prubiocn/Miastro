# ADR-063 — Ciclo de vida determinista de selección natal

## Estado

Aceptado para Fase 7.

## Decisión

La selección se identifica por el Id semántico del objeto.

Los rebuilds puramente visuales intentan restaurar ese Id después de
crear la nueva escena.

La restauración solo se acepta si el Scene Graph resultante contiene
todavía un object-glyph seleccionable con ese Id.

## Cambios de snapshot

Una nueva NatalChartSnapshotReadModel invalida cualquier selección
anterior.

## Objetos ocultos

Ocultar un planeta o punto seleccionado elimina inmediatamente la
selección.

No existe estado de selección invisible.

## Ratón y teclado

Un click enfoca la rueda antes de aplicar hit testing.

Esto permite continuar la interacción con teclado sin una acción de foco
adicional.

## Arquitectura

La UI consulta a Miastro.Graphics qué objetos de escena son
seleccionables.

Avalonia no calcula bounds, longitudes ni geometría natal.

# Fase 8 — panel natal responsive

## Objetivo

La rueda natal conserva la prioridad visual.

El panel derecho permanece legible y puede retirarse cuando el espacio
horizontal es limitado.

## Layout

El contenedor de rueda y panel utiliza dos columnas:

- rueda: `*`;
- panel: `Auto`.

La rueda consume todo el espacio restante.

El panel abierto mantiene una anchura nominal de 360 px, con límites de
300 px y 420 px.

## Colapso manual

El usuario dispone de un control explícito para mostrar u ocultar el panel.

Ocultarlo no cambia:

- la pestaña activa;
- la selección natal;
- la selección dual;
- el snapshot.

## Adaptación estrecha

Cuando el área rueda/panel baja de 720 px, el panel se oculta
automáticamente.

Si posteriormente vuelve a existir espacio suficiente, solo se reabre si
había sido ocultado automáticamente.

Un cierre manual no se revierte automáticamente.

## Tablas y matriz

Datos y Posiciones conservan desplazamiento horizontal cuando resulte
necesario.

La matriz de aspectos conserva desplazamiento horizontal y no comprime sus
celdas.

## Arquitectura

El comportamiento responsive reside en la vista Avalonia.

No recalcula astronomía, casas, aspectos ni distribución.

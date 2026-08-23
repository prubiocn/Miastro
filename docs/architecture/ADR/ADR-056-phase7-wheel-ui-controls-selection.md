# ADR-056 — Controles visuales y selección de la rueda

## Estado

Aceptado para Fase 7.

## Decisión

Las preferencias de visibilidad y modo son estado visual de la UI y se
traducen a contratos semánticos de Miastro.Graphics.

Avalonia no filtra geometría ni implementa algoritmos de layout.

## Modos

Los modos Consulta y Presentación utilizan NatalWheelViewMode.

## Visibilidad

Los controles utilizan NatalWheelVisibilityOptions.

## Selección

Avalonia entrega coordenadas de puntero al motor de hit testing de
Miastro.Graphics.

El resultado se relaciona con el NatalChartSnapshotReadModel ya cargado
para presentar los datos descriptivos del objeto.

## Prohibiciones

Los controles visuales no pueden provocar:

- llamadas a Swiss Ephemeris
- cálculo de casas
- detección de aspectos
- cambio de longitudes reales
- persistencia de posiciones gráficas absolutas

## Motivo

La UI debe coordinar presentación e interacción sin convertirse en un
segundo motor gráfico.

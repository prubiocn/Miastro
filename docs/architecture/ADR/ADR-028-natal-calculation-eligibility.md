# ADR-028 — Elegibilidad para cálculo natal completo

Estado: Propuesto durante Fase 6.

## Decisión

La Carta Natal completa V1 solo se calcula cuando la Persona dispone de:

- precisión Exacta; o
- precisión Aproximada;

y además:

- hora local concreta;
- resolución histórica válida;
- Instant UTC persistido;
- TZDB version persistida.

## Casos bloqueados

Rango:

`BirthTimeRangeRequiresResolution`

Momento del día:

`BirthTimeDayPeriodInsufficient`

Desconocida:

`BirthTimeUnknown`

Hora pendiente:

`HistoricalTimePending`

Hora ambigua sin elección:

`HistoricalTimeAmbiguousUnresolved`

Hora inexistente:

`HistoricalTimeSkipped`

## Hora aproximada

La carta puede calcularse, pero el snapshot debe conservar explícitamente
que procede de una hora aproximada.

## Carta parcial

Fase 6 no implementa todavía carta parcial sin casas.

No se inventan ASC, MC, casas ni una hora ficticia.

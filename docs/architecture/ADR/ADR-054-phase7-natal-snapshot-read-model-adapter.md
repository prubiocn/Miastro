# ADR-054 — Adaptador desde NatalChartSnapshotReadModel

## Estado

Aceptado para Fase 7.

## Decisión

Miastro.Graphics consume directamente el read model natal definido en
Miastro.Application.

Se añade una dependencia:

Miastro.Graphics -> Miastro.Application

Esta dependencia se limita a contratos/read models.

## Flujo

INatalChartStore
-> NatalChartSnapshotReadModel
-> NatalChartSnapshotGraphicsAdapter
-> layout
-> placement
-> Scene Graph
-> renderer

## Prohibiciones

NatalChartSnapshotGraphicsAdapter no puede:

- consultar INatalChartStore
- acceder a DbContext
- acceder a SQLite
- llamar Swiss Ephemeris
- detectar aspectos
- recalcular casas
- modificar el snapshot

## Motivo

Se evita crear un segundo DTO natal equivalente dentro de Graphics y
se mantiene una única fuente de verdad entre persistencia y
presentación.

## Futuro UI

La UI solicitará el snapshot actual mediante Application y entregará
el read model al adaptador gráfico.

La UI no realizará geometría central.

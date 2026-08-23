# ADR-030 — Snapshot y versionado de Carta Natal

Estado: Propuesto durante Fase 6.

## Decisión

Una Persona puede tener múltiples snapshots natales históricos.

Estados:

- Current
- Superseded
- Invalidated

Solo debe existir una carta vigente por combinación funcional activa.

Cada snapshot conserva:

- Persona;
- hash de entradas;
- precisión aproximada;
- fecha/hora local;
- Instant UTC;
- localidad;
- coordenadas;
- IANA;
- TZDB;
- sistema de casas;
- CalculationProfile;
- versión Miastro;
- motor y versión;
- adaptador;
- identidad de efemérides;
- fecha de cálculo;
- placements;
- cúspides;
- aspectos.

## Persistencia

Se utilizan tablas normalizadas:

- NatalCharts
- NatalPlacements
- NatalHouseCusps
- NatalAspects

No se almacena la carta completa como JSON opaco.

## Idempotencia

`PersonId + InputHash` es único.

Si existe el mismo snapshot, no se crea un duplicado.

Una entrada distinta crea un nuevo snapshot y la carta vigente anterior pasa
a `Superseded`.

Una modificación natal puede dejar la vigente en `Invalidated`.

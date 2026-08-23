# Miastro — Adaptador del snapshot natal persistido

## Fuente

La rueda de Fase 7 consume NatalChartSnapshotReadModel de
Miastro.Application.Natal.

Ese read model ya contiene:

- placements persistidos
- 12 cúspides
- aspectos persistidos
- ASC
- MC

## Regla arquitectónica

Miastro.Graphics puede depender del read model de Application.

No depende de:

- EF Core
- SQLite
- EfNatalChartStore
- Swiss Ephemeris
- Avalonia

La lectura física del snapshot continúa siendo responsabilidad de
INatalChartStore y su implementación de infraestructura.

## Posiciones

LongitudeDegrees de NatalPlacementSnapshot se copia como entrada real
del layout.

No se calcula una nueva longitud.

ASC y MC se obtienen de sus placements persistidos.

Las doce cúspides se obtienen directamente de HouseCusps.

## Objetos visibles por defecto

La rueda base contiene:

- Sol
- Luna
- Mercurio
- Venus
- Marte
- Júpiter
- Saturno
- Urano
- Neptuno
- Plutón
- Quirón
- ASC
- MC

Los demás puntos persistidos son opcionales.

## Puntos opcionales

IncludeOptionalPoints habilita:

- Nodo Norte verdadero
- Nodo Sur
- Lilith media
- Parte de Fortuna
- Ceres
- Pallas
- Juno
- Vesta

## Aspectos

Los aspectos proceden exclusivamente de NatalAspectSnapshot.

El adaptador no ejecuta AspectEngine ni NatalAspectCalculator.

La clasificación Major/Secondary es una traducción visual del
AspectKind ya persistido.

## Determinismo

El adaptador aplica orden canónico por identificadores de dominio.

Mismo snapshot, tamaño y opciones producen el mismo modelo gráfico.

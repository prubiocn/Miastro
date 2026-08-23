# ADR-034 — Identidad reproducible de BirthData en Carta Natal

Estado: Propuesto durante Fase 6.

## Decisión

Cada snapshot natal conserva dos identidades diferentes:

- `BirthDataHash`: identidad de los datos históricos de nacimiento;
- `InputHash`: identidad completa del cálculo natal.

La versión inicial de la identidad histórica es:

`BirthDataVersion = 1`

## BirthDataHash V1

El hash incluye de forma canónica:

- fecha local;
- hora local;
- Instant UTC;
- precisión horaria;
- GeoNameId;
- localidad;
- latitud;
- longitud;
- IANA Time Zone ID;
- versión TZDB;
- offset histórico;
- selección explícita de ambigüedad.

Se utiliza SHA-256 hexadecimal en minúsculas.

## Separación respecto a InputHash

`BirthDataHash` no depende de:

- sistema de casas;
- perfil de cálculo;
- motor astronómico;
- versión del motor;
- corpus de efemérides.

Por tanto, Placidus y Koch calculados a partir del mismo nacimiento comparten
el mismo `BirthDataHash`, pero poseen `InputHash` diferentes.

## Persistencia

El snapshot conserva además:

- BirthDataVersion;
- BirthTimePrecision;
- GeoNameId;
- HistoricalOffsetSeconds;
- AmbiguousSelection.

Esto permite auditar y reproducir qué resolución histórica alimentó una carta
sin depender del estado actual de la ficha Persona.

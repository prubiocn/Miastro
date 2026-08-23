# ADR-029 — Hash determinista e idempotencia natal

Estado: Propuesto durante Fase 6.

## Decisión

Cada snapshot natal tendrá un hash SHA-256 determinista de las entradas que
afectan al cálculo.

La representación canónica V1 incluye:

- fecha local;
- hora local;
- Instant UTC;
- latitud;
- longitud;
- zona IANA;
- versión TZDB;
- sistema de casas;
- CalculationProfile;
- motor astronómico;
- versión del motor;
- identidad/versionado de efemérides.

## Objetivo

El hash permite:

- detectar entradas idénticas;
- evitar snapshots duplicados;
- detectar obsolescencia;
- reproducir el cálculo;
- implementar recalculado trazable.

El hash no contiene nombre, contacto ni nota privada.

## Revisión Fase 6 / Bloque 3B

El hash canónico pasa a `miastro-natal-input-v2`.

Además de los parámetros astronómicos incorpora semántica natal que debe
invalidar/reidentificar una snapshot aunque algunas coordenadas astronómicas
coincidan:

- precisión horaria;
- GeoNameId;
- localidad;
- offset histórico;
- selección explícita de ambigüedad.

Esto evita reutilizar una snapshot Aproximada como Exacta o conservar
metadatos natales obsoletos bajo el mismo hash.

Un recálculo con el mismo hash vigente continúa siendo idempotente y no crea
otra snapshot.

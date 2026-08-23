# Fase 6 — baseline de rendimiento natal

## Objetivo

Fase 6 registra una línea base reproducible del cálculo natal real sin
convertir tiempos dependientes del hardware en criterios arbitrarios de CI.

El baseline usa:

- SQLite real;
- Persona persistida;
- Swiss Ephemeris real;
- 17 objetos Swiss;
- casas Placidus;
- 21 placements canónicos;
- aspectos Miastro V1;
- persistencia del snapshot natal;
- recarga del snapshot;
- fast path idempotente de una carta ya existente.

## Métricas

La prueba `Phase6NatalPerformanceBaselineTests` emite:

- `PersonLoadMs`;
- `Swiss17ObjectsMs`;
- `HouseCalculationMs`;
- `DerivedAndAspectsMs`;
- `FullNatalCalculationAndPersistenceMs`;
- `PersistedSnapshotReloadMs`;
- `ExistingSnapshotFastPathMs`.

## Warm-up

Antes de medir se inicializan:

- diagnóstico/metadatos del motor;
- una llamada Swiss;
- una llamada de casas.

De esta forma el coste puntual de carga ABI/JIT no se confunde con la
operación estable.

## Política de CI

Los tiempos se registran, pero Fase 6 no define un máximo absoluto en
milisegundos.

Motivos:

- hardware distinto;
- máquinas virtuales;
- carga compartida de runners;
- caché de disco;
- scheduler;
- JIT.

El gate comprueba corrección funcional y que el escenario real pueda medirse.

Una futura política de regresión de rendimiento deberá usar entorno
controlado, múltiples muestras y criterio estadístico explícito.

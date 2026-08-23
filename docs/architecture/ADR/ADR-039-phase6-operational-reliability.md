# ADR-039 — Fiabilidad operativa natal en Fase 6

Estado: Propuesto durante Fase 6.

## Backup

Miastro conserva el mecanismo SQLite nativo ya existente.

`SqliteDatabaseBackupService` utiliza `BackupDatabase`, por lo que el backup
copia la base completa y no mantiene una lista manual de tablas.

Fase 6 añade cobertura específica para verificar la presencia y los datos de:

- NatalCharts;
- NatalPlacements;
- NatalHouseCusps;
- NatalAspects;
- historial de migraciones.

El backup se reabre como base SQLite normal y se valida su contenido.

## Migración Fase 5 → Fase 6

La actualización se realiza mediante las migraciones EF Core existentes.

La prueba parte explícitamente de:

`20260820180432_Phase5PersonFunctionalSchema`

inserta una Persona en ese esquema y posteriormente ejecuta la migración hasta
el modelo actual.

La Persona debe sobrevivir intacta y deben aparecer las tablas natales sin
resetear la base.

## Swiss Ephemeris y concurrencia

Swiss Ephemeris mantiene estado global.

Miastro conserva la política establecida en Fase 3:

- acceso serializado;
- un único `SwissEphemerisGate.SyncRoot`;
- posiciones y casas usan el mismo gate;
- no se ejecutan llamadas nativas Swiss concurrentemente.

La prueba de Fase 6 lanza llamadas concurrentes de posiciones y casas y
comprueba resultados deterministas.

No se introduce paralelismo dentro de Swiss Ephemeris.

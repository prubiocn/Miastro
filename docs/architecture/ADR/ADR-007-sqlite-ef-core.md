# ADR-007 — SQLite + Entity Framework Core

## Estado

Aceptado.

## Decisión

La persistencia local de Miastro utilizará SQLite mediante Entity Framework Core.

La base de datos de usuario reside en el directorio XDG Data de Miastro.

Las modificaciones de esquema se realizan mediante migraciones versionadas.

## Fase 1

Solo existe un esquema técnico mínimo destinado a validar:

- creación controlada
- migraciones
- lectura
- escritura
- permisos del fichero

No existe todavía un modelo de Persona ni entidades astrológicas de producción.

# Fase 4 — política de ambigüedad temporal

## Hora normal

Se conserva:

- fecha/hora local original;
- IANA TimeZoneId;
- offset;
- Instant UTC;
- versión TZDB.

## Hora ambigua

Nunca se elige automáticamente.

El resultado conserva dos candidatos:

- offset;
- Instant;
- ZonedDateTime.

La futura capa funcional deberá solicitar elección explícita y registrar una
decisión auditable.

## Hora inexistente

Nunca se desplaza silenciosamente.

El resultado conserva:

- estado `Skipped`;
- offset anterior;
- offset posterior;
- instante de transición.

No se crea un Instant artificial.

## Persistencia futura

`HistoricalTimeSelectionSnapshot` prepara el modelo persistible de Fase 5 sin
crear todavía Persona ni DatosNacimiento.

# ADR-009 — Persistencia de resolución histórica natal

Estado: Aceptado en Fase 5.

Persistir, cuando corresponda:

- LocalDateTime original;
- IANA TimeZoneId;
- versión TZDB;
- estado de resolución;
- offset elegido;
- Instant UTC;
- información de ambigüedad;
- decisión explícita;
- override manual y su auditoría.

Nunca seleccionar silenciosamente una hora ambigua ni desplazar una hora
inexistente.

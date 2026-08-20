# Fase 4 — Base de tiempo histórico

Implementación oficial:

- Noda Time 3.3.3;
- IANA TZDB embebida/proporcionada por Noda Time;
- resolución mediante `MapLocal`;
- registro de `DateTimeZoneProviders.Tzdb.VersionId`.

Estados:

- `Resolved`: un candidato;
- `Ambiguous`: dos candidatos, offsets e instantes preservados;
- `Skipped`: cero candidatos, transición y offsets preservados.

No existe resolución silenciosa de horas ambiguas o inexistentes.

Casos iniciales:

- Europe/Madrid normal;
- Europe/Madrid transición de otoño;
- Europe/Madrid transición de primavera;
- Asia/Kathmandu (+05:45);
- Australia/Adelaide (+09:30 en invierno austral).

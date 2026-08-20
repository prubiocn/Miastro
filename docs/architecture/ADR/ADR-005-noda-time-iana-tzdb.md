# ADR-005 — Noda Time + IANA TZDB

## Estado

Aceptado.

## Decisión

Miastro usa Noda Time como implementación oficial de tiempo histórico y
`DateTimeZoneProviders.Tzdb` como proveedor canónico IANA TZDB.

La resolución se realiza mediante `DateTimeZone.MapLocal(LocalDateTime)`.

## Política

- mapeo único: `Resolved`;
- mapeo doble: `Ambiguous`, conservando ambos candidatos;
- mapeo inexistente: `Skipped`, sin desplazar silenciosamente la hora;
- toda resolución registra `VersionId` de TZDB;
- no se usa `TimeZoneInfo` como fuente canónica;
- no se depende de `tzdata` arbitraria del sistema.

La UI futura deberá exigir elección explícita en una ambigüedad.

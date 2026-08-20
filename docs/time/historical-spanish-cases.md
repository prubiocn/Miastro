# Fase 4 — casos históricos españoles e internacionales

Este corpus existe para impedir que Miastro aplique offsets modernos fijos a
fechas históricas.

Archivo:

`data/time/goldens/historical-time-spanish.tsv`

Cobertura:

- Madrid antes y después del cambio de marzo de 1940;
- Barcelona mediante la zona histórica `Europe/Madrid`;
- Canarias antes y después del cambio de marzo de 1940;
- Nueva York en el periodo de transición al tiempo estándar;
- Katmandú antes y después de su cambio a UTC+05:45.

Los valores esperados están fijados como referencias de IANA TZDB y no se
obtienen invocando el servicio sometido a prueba.

La versión TZDB efectiva se registra en cada resolución por
`DateTimeZoneProviders.Tzdb.VersionId`.

## Corrección de referencia histórica

Para `Europe/Madrid` antes de 1901, IANA TZDB define LMT con offset
`-00:14:44`. Por tanto, una hora local `1900-01-01 12:00:00` corresponde a
`1900-01-01T12:14:44Z`.

El golden inicial contenía el signo invertido. Se corrigió el dato de
referencia; no se modificó el resolver.

## Corrección de referencia histórica — Canarias

El golden inicial suponía erróneamente que el cambio peninsular de marzo de
1940 se aplicaba también a `Atlantic/Canary`.

IANA TZDB modela Canarias con offset `-01:00` desde 1922 hasta el
30/09/1946. Por tanto:

- 16/03/1940 12:00 local → 13:00 UTC;
- 17/03/1940 12:00 local → 13:00 UTC.

Para cubrir un cambio realmente relevante de Canarias se añaden además dos
casos alrededor del 30/09/1946:

- 00:30 local con offset `-01:00`;
- 02:30 local con offset `+00:00`.

Se corrige únicamente el corpus golden; no se modifica el resolver.

## Corrección de referencia histórica — Nueva York 1883

`America/New_York` pasa de LMT `-04:56:02` a tiempo estándar `-05:00`
el 18/11/1883. Debido al retroceso de 3 minutos y 58 segundos, la hora local
`12:00:00` aparece dos veces.

Por tanto, el caso `1883-11-18 12:00:00` es `Ambiguous` con dos candidatos:

- offset `-04:56:02` → `1883-11-18T16:56:02Z`;
- offset `-05:00` → `1883-11-18T17:00:00Z`.

El test histórico se generaliza para validar tanto casos `Resolved` como
`Ambiguous` sin modificar el resolver.

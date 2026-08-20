# Fase 4 — casos golden de tiempo histórico

## Fuente

Los valores esperados de este corpus se fijan como referencias externas del
modelo de zonas IANA/TZDB, no se generan llamando al servicio
`NodaTimeHistoricalTimeResolver` sometido a prueba.

Proveedor temporal de runtime detectado en este build:

- Noda Time package: 3.3.3
- `DateTimeZoneProviders.Tzdb.VersionId`: `TZDB: 2026c (mapping: 48.2)`

Corpus:

`data/time/goldens/historical-time-goldens.tsv`

Incluye:

- Europa;
- América;
- Asia;
- hemisferio sur;
- offsets de 30 minutos;
- offsets de 45 minutos;
- horario estándar;
- DST;
- hora ambigua;
- hora inexistente.

Los casos históricos españoles anteriores a la normalización moderna se
mantienen como bloque separado para evitar introducir valores no verificados.

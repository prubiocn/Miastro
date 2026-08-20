# Tiempo astronómico y Julian Day

La frontera astronómica recibe `AstronomicalInstant`, que contiene un
`DateTimeOffset` normalizado a UTC.

La conversión a Julian Day UT está centralizada en:

`SwissJulianDayConverter`

Para el contrato de Fase 3 se utiliza `swe_julday()` y posteriormente
`swe_calc_ut()`.

No se dispersan conversiones temporales en calculadores individuales.

La política histórica completa de calendario civil, zonas horarias y
TZDB queda fuera de Fase 3.

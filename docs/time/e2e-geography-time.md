# Fase 4 — flujo E2E headless

Flujo validado:

`texto -> ILocationSearchService -> GeoNameId -> coordenadas + IANA zone ->
LocalDateTime -> IHistoricalTimeResolver -> Instant UTC`

El flujo es headless y no depende de Avalonia.

No calcula todavía una Carta Natal.

La conexión con `AstronomicalInstant` de Fase 3 se validará como frontera de
compatibilidad, manteniendo Geografía/Tiempo separados del adaptador Swiss.

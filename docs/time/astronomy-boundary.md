# Fase 4 — frontera con Astronomy.Abstractions

Fase 4 no construye una Carta Natal.

La integración validada es únicamente temporal:

`Noda Instant -> UTC DateTimeOffset -> Astronomy.Abstractions boundary`

La prueba confirma que el instante histórico resuelto puede representarse
exactamente en UTC para la frontera astronómica ya existente.

No se añade dependencia de `Miastro.Infrastructure.Time` sobre Swiss
Ephemeris y no se mezcla resolución geográfica/temporal con cálculo
astronómico.

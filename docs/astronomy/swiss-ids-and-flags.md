# Swiss Ephemeris — IDs y flags Miastro V1

## Flags

Miastro V1 solicita exclusivamente:

- `SEFLG_SWIEPH`
- `SEFLG_SPEED`

No activa:

- `SEFLG_TRUEPOS`
- `SEFLG_TOPOCTR`
- `SEFLG_SIDEREAL`
- `SEFLG_HELCTR`
- `SEFLG_EQUATORIAL`
- `SEFLG_RADIANS`

Por tanto, el resultado V1 se mantiene:

- tropical;
- geocéntrico;
- eclíptico;
- aparente;
- en grados;
- con velocidades.

Miastro rechaza cualquier fallback silencioso a una efeméride distinta
cuando se ha solicitado `SEFLG_SWIEPH`.

## IDs utilizados

- Sun → 0
- Moon → 1
- Mercury → 2
- Venus → 3
- Mars → 4
- Jupiter → 5
- Saturn → 6
- Uranus → 7
- Neptune → 8
- Pluto → 9
- True North Node → 11
- Mean Lilith / Mean Apogee → 12
- Chiron → 15
- Ceres → 17
- Pallas → 18
- Juno → 19
- Vesta → 20

Nodo Sur no se solicita a Swiss Ephemeris.
Continúa derivándose en el dominio.

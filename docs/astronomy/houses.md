# Casas con Swiss Ephemeris

## Sistemas V1

- Placidus → `P`
- Koch → `K`

La API utilizada es:

`swe_houses_ex`

Entrada:

- Julian Day UT;
- latitud geográfica;
- longitud geográfica;
- sistema de casas.

Salida aceptada por Miastro:

- 12 cúspides;
- Ascendente;
- Medio Cielo;
- metadatos del motor.

## ASC y MC

Swiss Ephemeris devuelve:

- `ascmc[0]` → Ascendente;
- `ascmc[1]` → Medio Cielo.

Miastro los conserva como ángulos, no como planetas.

## DSC e IC

Existe una única política semántica:

- DSC = ASC + 180°
- IC = MC + 180°

Ambos se derivan en `HouseCalculationResult`.

No se mantiene una segunda fuente independiente para estos dos puntos.

## Altas latitudes

Swiss Ephemeris puede devolver error para Placidus o Koch en regiones
polares.

Miastro:

- no acepta ceros ficticios;
- no acepta casas parciales;
- no transforma silenciosamente el resultado a otro sistema;
- devuelve `HouseCalculationUnavailable`.

Aunque Swiss Ephemeris pueda producir internamente un fallback geométrico
en determinadas condiciones, Miastro lo descarta cuando el código de
retorno indica fallo.

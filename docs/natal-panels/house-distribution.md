# Fase 8 — Distribución por casas

## Fuente

La distribución geométrica por casas utiliza exclusivamente `HouseNumber` persistido en cada placement.

No se recalculan casas, cúspides ni longitudes.

Un planeta próximo a una cúspide pertenece a la casa ya persistida en el snapshot.

## Perfil

Se reutiliza `NatalDistributionProfile.MiastroV1`.

Por tanto, la distribución principal sigue contando exclusivamente los diez planetas.

## Hemisferio Este / Oeste

La división se realiza por casas:

- Este: Casas 10, 11, 12, 1, 2 y 3.
- Oeste: Casas 4, 5, 6, 7, 8 y 9.

## Hemisferio Superior / Inferior

- Superior: Casas 7 a 12.
- Inferior: Casas 1 a 6.

## Cuadrantes

- I: Casas 1, 2 y 3.
- II: Casas 4, 5 y 6.
- III: Casas 7, 8 y 9.
- IV: Casas 10, 11 y 12.

## Naturaleza de las casas

- Angulares: 1, 4, 7 y 10.
- Sucedentes: 2, 5, 8 y 11.
- Cadentes: 3, 6, 9 y 12.

## Predominio y equilibrio

Se aplica la misma política determinista de la distribución zodiacal:

- solo existe predominio con máximo único;
- en empate no se escoge una categoría arbitrariamente;
- equilibrio básico significa diferencia máxima de uno entre categorías.

## Presentación

La información se presenta de forma textual y estructurada.

No se utilizan gráficos de barras.

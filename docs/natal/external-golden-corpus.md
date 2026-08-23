# Fase 6 — corpus externo de Carta Natal

## Fuente

La referencia de ampliación de Fase 6 se captura con el ejecutable oficial
`swetest` de Swiss Ephemeris 2.10.03.

El ejecutable no forma parte del adaptador Miastro ni invoca código de
`Miastro.Infrastructure.SwissEphemeris`.

El caso Madrid 2024 se contrasta además con la captura HTTP de Astrodienst
conservada desde Fase 3.

## Política

Los valores expected:

- no se generan mediante Miastro;
- no se redondean antes de comparar;
- conservan las respuestas externas crudas;
- tienen SHA-256 y tamaño registrados.

Tolerancias heredadas del ADR-027:

- longitud: 0.0001°;
- latitud: 0.0001°;
- velocidad longitudinal: 0.0001°/día;
- cúspides: 0.0001°;
- ASC: 0.0001°;
- MC: 0.0001°.

## Casos

1. Madrid, 2024-01-01 12:00 UTC, Placidus.
2. Sydney, 2024-03-20 03:00 UTC, Placidus.
3. Nueva York, 2000-01-01 12:00 UTC, Koch.
4. Madrid, 1900-01-01 12:00 UTC, Placidus.
5. Sydney, 1850-07-01 06:00 UTC, Koch.

Cobertura buscada:

- tres cartas modernas;
- dos históricas;
- hemisferio norte;
- hemisferio sur;
- Placidus;
- Koch;
- casas desiguales;
- Mercurio retrógrado;
- longitud próxima a 0°;
- Nodo Norte verdadero;
- Nodo Sur derivado en la validación natal;
- Lilith Media;
- Quirón;
- Ceres;
- Palas;
- Juno;
- Vesta.

## Normalización del corpus

`tests/golden/phase6/golden-values.json` se obtiene exclusivamente parseando
las respuestas crudas de `swetest`.

El proceso de normalización:

- no llama a Miastro;
- no llama al adaptador Swiss de Miastro;
- no recalcula posiciones;
- no recalcula cúspides;
- no altera los valores capturados.

La única magnitud derivada añadida es el Nodo Sur, definido por contrato de
dominio como Nodo Norte verdadero +180° normalizado. El valor externo primario
continúa siendo el Nodo Norte verdadero capturado por `swetest`.

## Ejecución Miastro contra los cinco goldens

Miastro se compara directamente contra las cinco cartas normalizadas desde las
capturas externas.

Para cada carta se validan:

- 17 posiciones Swiss;
- longitud eclíptica;
- latitud eclíptica;
- velocidad longitudinal;
- 12 cúspides;
- Ascendente;
- Medio Cielo;
- sistema de casas;
- Nodo Sur derivado desde el Nodo Norte verdadero.

Las comparaciones utilizan las tolerancias V1 de ADR-027.

El golden permanece inmutable durante la ejecución de los tests.

## Goldens derivados independientes

Las reglas que Swiss Ephemeris no entrega como resultado natal completo se
validan mediante un segundo corpus:

`tests/golden/phase6/derived-golden-values.json`

Sus expected se obtienen con una implementación de referencia independiente
que sólo consume las longitudes y cúspides externas congeladas.

Se cubren:

- Day/Night;
- Parte de Fortuna;
- ocupación de casas de los 21 objetos;
- aspectos Miastro V1.

El corpus derivado conserva el SHA-256 del golden primario.

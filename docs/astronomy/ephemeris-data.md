# Datos de efemérides — Fase 3

Conjunto mínimo controlado:

- `sepl_18.se1`
- `semo_18.se1`
- `seas_18.se1`

Rango temporal declarado:

**1800-01-01 a 2399-12-31 UTC**

Los ficheros `_18.se1` de Swiss Ephemeris cubren el intervalo
1800–2399.

El manifiesto reside en:

`data/ephemeris/manifest.json`

Registra:

- nombre;
- tamaño;
- SHA-256;
- versión;
- rango;
- obligatoriedad;
- propósito;
- rango temporal global soportado.

Antes de calcular una posición Miastro valida:

1. manifiesto;
2. existencia;
3. tamaño;
4. SHA-256;
5. rango temporal.

Un instante fuera del intervalo soportado produce
`UnsupportedTimeRange`.

No se acepta degradación silenciosa a Moshier.

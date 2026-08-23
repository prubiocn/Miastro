# ADR-036 — Goldens derivados natales independientes

Estado: Propuesto durante Fase 6.

## Decisión

Los valores esperados de reglas natales derivadas no se generan ejecutando
código de Miastro.

Se parte exclusivamente del corpus primario externo de Swiss Ephemeris:

`tests/golden/phase6/golden-values.json`

y se aplica una implementación de referencia independiente.

## Reglas cubiertas

- asignación de objetos a casas;
- determinación Day/Night;
- Parte de Fortuna;
- detección de aspectos V1.

## Asignación de casas

Se usan los arcos reales entre las 12 cúspides externas.

La cúspide exacta pertenece a la casa que comienza allí.

La tolerancia de estabilidad numérica es `1e-9°`.

## Day/Night

- casas 7–12: Day;
- casas 1–6: Night.

## Parte de Fortuna

Day:

`ASC + Moon - Sun`

Night:

`ASC + Sun - Moon`

El resultado se normaliza a `[0, 360)`.

## Aspectos

La implementación independiente reproduce literalmente el contrato público
Miastro V1 documentado:

- conjunción 0°, orbe 8°;
- semisextil 30°, orbe 2°;
- sextil 60°, orbe 4°;
- cuadratura 90°, orbe 6°;
- trígono 120°, orbe 6°;
- quincuncio 150°, orbe 3°;
- oposición 180°, orbe 8°;
- quintil 72°, orbe 2°;
- biquintil 144°, orbe 2°.

Sol o Luna añaden una sola vez 1° al orbe permitido.

Participan exclusivamente los 10 planetas, Quirón, Ceres, Palas, Juno,
Vesta, ASC y MC.

No participan los nodos, Lilith Media ni Parte de Fortuna.

## Trazabilidad

El golden derivado conserva SHA-256 del corpus primario del que fue obtenido.

Los tests verifican que ese SHA-256 continúa siendo válido y que Miastro no se
utilizó para generar los expected.

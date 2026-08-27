# Fase 8 — Hechos natales y regencias

## Frontera

Los paneles natales consumen hechos derivados exclusivamente del snapshot natal persistido y de catálogos del dominio.

No se recalcula astronomía y no se invoca Swiss Ephemeris.

## Orden

Los objetos se ordenan siempre mediante `NatalObjectOrder`.

Nunca se depende del orden devuelto por SQLite ni del orden original de una colección persistida.

## Regencia de signo

La regencia se obtiene exclusivamente de `RulershipCatalog`.

Se preservan las dobles regencias ya aprobadas:

- Escorpio: Marte / Plutón.
- Acuario: Saturno / Urano.
- Piscis: Júpiter / Neptuno.

## Regencia de casa

La regencia de casa no se asigna por número fijo.

Para un objeto situado en una casa:

1. se toma `HouseNumber` persistido;
2. se localiza la cúspide real de esa casa;
3. se obtiene el signo zodiacal de la longitud de la cúspide;
4. se consultan sus regentes en `RulershipCatalog`.

La regla funciona igualmente en el cruce Casa 12 → Casa 1 y 360° → 0° porque cada cúspide se evalúa por su propia longitud normalizada.

## Inmutabilidad

`NatalFactsReader` es una capa de lectura:

- no modifica placements;
- no modifica cúspides;
- no modifica aspectos;
- no infiere movimiento;
- no persiste datos derivados.

El movimiento se expone exactamente como fue persistido en el snapshot.

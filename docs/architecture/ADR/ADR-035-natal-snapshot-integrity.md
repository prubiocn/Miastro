# ADR-035 — Integridad estructural del snapshot natal

Estado: Propuesto durante Fase 6.

## Decisión

Una snapshot natal completa sólo puede persistirse si satisface el contrato
estructural V1.

Debe contener exactamente los 21 objetos definidos por `NatalObjectOrder`, sin
duplicados y en orden canónico.

Debe contener exactamente 12 cúspides, ordenadas de casa 1 a casa 12.

## Objetos derivados

Deben estar presentes:

- Nodo Sur;
- Ascendente;
- Medio Cielo;
- Parte de Fortuna.

El Nodo Sur debe corresponder al Nodo Norte verdadero +180°, con tolerancia
numérica de 1e-9°.

## Valores

Las longitudes deben estar normalizadas en `[0, 360)`.

Los grados en signo deben estar en `[0, 30)`.

Las casas, cuando existan, deben estar entre 1 y 12.

Todos los valores numéricos persistidos deben ser finitos.

## Identidad

`BirthDataHash` debe coincidir con el fingerprint histórico que acompaña a la
snapshot.

## Defensa en profundidad

La validación se ejecuta tanto en Application como dentro de
`EfNatalChartStore`, evitando que un consumidor alternativo del store pueda
persistir una carta incompleta o incoherente.

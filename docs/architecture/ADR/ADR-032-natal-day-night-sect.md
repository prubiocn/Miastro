# ADR-032 — Determinación diurna/nocturna natal

Estado: Propuesto durante Fase 6.

## Decisión

La condición Day/Night no se obtiene de la hora civil.

Se determina por la posición zodiacal real del Sol respecto al horizonte
representado por las cúspides de casas:

- Sol en casas 7–12: Day;
- Sol en casas 1–6: Night.

La asignación utiliza las cúspides reales y la misma política determinista de
cúspide exacta.

Consecuencias en el horizonte:

- Sol exactamente sobre DSC/cúspide 7: Day;
- Sol exactamente sobre ASC/cúspide 1: Night.

Esta decisión alimenta exclusivamente reglas que necesitan sect, incluida la
Parte de la Fortuna.

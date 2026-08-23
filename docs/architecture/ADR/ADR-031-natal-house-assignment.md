# ADR-031 — Asignación natal de objetos a casas

Estado: Propuesto durante Fase 6.

## Decisión

Las casas se asignan utilizando las cúspides reales calculadas por el motor de
casas.

No se asignan por signo.

Cada casa ocupa el arco zodiacal que comienza en su propia cúspide y termina
antes de la cúspide siguiente, respetando el cruce 360°/0°.

## Cúspide exacta

Si un objeto coincide con una cúspide dentro de una tolerancia numérica de
`1e-9` grados, pertenece a la casa que comienza en esa cúspide.

La tolerancia es exclusivamente de estabilidad de coma flotante y no es un
orbe astrológico.

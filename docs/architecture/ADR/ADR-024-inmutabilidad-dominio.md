# ADR-024 — Inmutabilidad del dominio

## Estado

Aceptado.

## Decisión

Se priorizan:

- records;
- readonly record structs;
- propiedades de solo lectura;
- colecciones expuestas como solo lectura;
- value objects.

## Consecuencia

El estado del dominio es predecible, testeable y resistente a mutaciones accidentales.

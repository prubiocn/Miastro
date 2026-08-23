# ADR-045 — Algoritmo determinista anti-solapamiento

## Estado

Aceptado para Fase 7.

## Decisión

La primera estrategia anti-solapamiento de Miastro utiliza niveles
radiales deterministas.

Orden de candidatos:

    0, +1, -1, +2, -2...

La entrada se normaliza y ordena por longitud real y después por Id.

Se acepta el primer candidato cuyo bounding box protegido no
intersecta ningún placement ya aceptado.

## Prioridades

1. conservar el orden zodiacal
2. minimizar el desplazamiento
3. impedir solapamientos
4. conservar trazabilidad a la posición real

## Determinismo

La misma colección lógica produce el mismo resultado aunque llegue
en distinto orden.

No existe aleatoriedad.

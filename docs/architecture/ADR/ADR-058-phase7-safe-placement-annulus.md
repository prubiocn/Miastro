# ADR-058 — Anillo seguro y desplazamiento angular determinista

## Estado

Aceptado para Fase 7.

## Problema

La primera estrategia anti-solapamiento utilizaba únicamente niveles
radiales.

Un nivel podía superar los límites visuales reservados a los objetos y
entrar en otras regiones de la rueda.

## Decisión

Todo placement debe permanecer dentro de un anillo seguro derivado de
las métricas de la rueda y del footprint protegido del glifo.

Los niveles radiales fuera del anillo se descartan.

Cuando los niveles radiales seguros no bastan, el motor introduce
desplazamiento angular determinista.

## Orden

Los objetos se procesan por longitud real y después por identificador.

Los ángulos visuales desenvueltos no pueden invertir ese orden.

## Fuente de verdad

RealLongitudeDegrees y RealScreenAngleDegrees no se modifican.

VisualScreenAngleDegrees puede diferir exclusivamente por necesidades de
legibilidad.

## Determinismo

No existe aleatoriedad.

Misma rueda, objetos y política producen exactamente el mismo snapshot.

## Arquitectura

La estrategia pertenece a Miastro.Graphics.

No depende de:

- Avalonia
- SkiaSharp
- Swiss Ephemeris
- persistencia

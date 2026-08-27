# ADR-065 — Hechos natales y regencia de casa en Fase 8

## Estado

Aceptado para Fase 8.

## Contexto

Datos, Posiciones, Distribución, matriz y Resumen necesitan una base factual común y reutilizable.

Duplicar reglas de signos, orden o regencias en ViewModels produciría divergencias entre paneles y dificultaría su reutilización futura en informes.

## Decisión

Crear una capa headless de lectura factual en `Miastro.Application.Natal.Reading`.

Esta capa:

- consume snapshots persistidos;
- utiliza `NatalObjectOrder`;
- utiliza `RulershipCatalog`;
- utiliza `ZodiacSignInfo`;
- conserva el movimiento persistido;
- no referencia Avalonia;
- no referencia Skia;
- no referencia Swiss;
- no modifica el snapshot.

La regencia de casa se determina por el signo de la cúspide real de la casa donde está el objeto, nunca por una asociación fija entre número de casa y signo.

## Consecuencias

Los paneles pueden compartir reglas deterministas sin duplicarlas.

La futura capa de interpretación queda separada de los hechos astrológicos.

Los mismos modelos pueden reutilizarse posteriormente en informes sin acoplamiento a controles Avalonia.

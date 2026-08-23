# ADR-047 — SkiaSharp como backend headless de Scene Graph

## Estado

Aceptado para Fase 7.

## Decisión

Miastro.Graphics.Skia contiene el backend SkiaSharp.

Miastro.Graphics permanece completamente independiente de Skia.

El renderer recibe exclusivamente NatalScene.

La cadena queda:

datos persistidos
-> layout
-> scene graph
-> Skia renderer
-> superficie de salida

## Restricciones

El renderer no puede:

- consultar Swiss Ephemeris
- recalcular carta natal
- acceder a persistencia
- modificar posiciones astrológicas
- depender de Avalonia para render headless

## Salidas

En Fase 7 se permite PNG técnico de diagnóstico.

No se considera todavía exportación final de usuario.

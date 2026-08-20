# ADR-019 — Modelo angular canónico

## Estado

Aceptado.

## Decisión

El dominio utiliza `Angle`, `EclipticLongitude` y `AngularSeparation`.

Las longitudes zodiacales se normalizan siempre a `[0°,360°)` y las separaciones mínimas a `[0°,180°]`.

## Consecuencia

Las reglas astrológicas no dependen de `double` sin semántica dispersos por el sistema.

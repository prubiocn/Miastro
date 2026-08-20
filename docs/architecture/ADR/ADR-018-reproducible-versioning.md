# ADR-018 — Versionado reproducible

## Estado

Aceptado.

## Decisión

Los artefactos de Miastro deben poder asociarse de forma inequívoca con:

- versión de aplicación
- versión de migraciones
- versión del runtime
- versiones de dependencias
- versión futura de efemérides
- versión futura de TZDB
- versión futura de reglas de cálculo
- versión futura de plantillas de interpretación e informes

Las compilaciones se realizan con generación determinista habilitada.

No se incorporan todavía versiones funcionales de datos astrológicos externos en Fase 1.

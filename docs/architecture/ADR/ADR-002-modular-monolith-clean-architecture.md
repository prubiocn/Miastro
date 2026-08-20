# ADR-002 — Monolito modular + Clean Architecture

## Estado

Aceptado.

## Decisión

Miastro utilizará un monolito modular con separación explícita entre:

- UI
- Application
- Domain
- abstracciones
- infraestructura
- gráficos
- interpretación
- informes
- exportación

La dirección de dependencias debe apuntar hacia contratos y modelos internos estables.

## Restricciones principales

Domain no depende de Avalonia, EF Core, SkiaSharp ni Swiss Ephemeris.

UI no accede directamente a DbContext ni Swiss Ephemeris.

Interpretation no depende de Swiss Ephemeris.

Graphics no depende de SQLite.

Persistence no depende de UI.

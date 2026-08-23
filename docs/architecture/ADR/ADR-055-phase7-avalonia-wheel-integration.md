# ADR-055 — Integración Avalonia mediante Scene Graph renderizado

## Estado

Aceptado para Fase 7.

## Decisión

Avalonia consume la rueda como resultado del pipeline gráfico ya
existente.

La UI depende de:

- Miastro.Graphics
- Miastro.Graphics.Skia

pero no implementa geometría central.

## Flujo

Application read model
-> Graphics adapter
-> layout/placement
-> Scene Graph
-> Graphics.Skia
-> Avalonia Image

## Interacción

Avalonia entrega coordenadas de puntero.

La transformación viewport -> Scene Graph y el hit testing pertenecen
a Miastro.Graphics.

## Razón

Esta solución conserva un único Scene Graph reutilizable por:

- UI
- render headless
- futuras exportaciones
- goldens

y evita mantener dos motores gráficos diferentes.

## Restricciones

La UI no puede:

- llamar Swiss Ephemeris para pintar
- recalcular aspectos
- calcular layout central
- persistir coordenadas gráficas absolutas

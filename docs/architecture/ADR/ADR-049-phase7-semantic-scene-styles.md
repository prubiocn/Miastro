# ADR-049 — Estilos semánticos independientes del renderer

## Estado

Aceptado para Fase 7.

## Decisión

SceneNode dispone de StyleKey.

Miastro.Graphics contiene NatalSceneStyleCatalog y los contratos de:

- color
- grosor
- patrón
- opacidad
- relleno

Miastro.Graphics.Skia traduce esos estilos a primitivas Skia.

## Motivación

Evita que la identidad visual quede codificada de forma ad hoc dentro
del backend gráfico.

Permite reutilizar la misma semántica en futuras salidas.

## Accesibilidad

Los elementos relevantes no se diferencian únicamente por color.

En particular, aspectos principales/secundarios y ejes
principales/secundarios utilizan también grosor, patrón u opacidad.

## Restricciones

Miastro.Graphics sigue sin depender de SkiaSharp ni Avalonia.

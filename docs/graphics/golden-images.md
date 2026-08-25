# Miastro — Golden images de Fase 7

## Finalidad

Las golden images detectan regresiones visuales del renderer Skia sin
arrancar Avalonia.

No sustituyen los tests geométricos.

## Corpus

Fase 7 mantiene como mínimo:

- carta sencilla
- stellium
- carta con muchas líneas de aspectos
- geometría con cúspides Placidus persistidas
- geometría con cúspides Koch persistidas

Los casos de casas no ejecutan algoritmos Placidus o Koch en Graphics.
Las cúspides son entrada ya resuelta.

## Estrategia de comparación

Se combinan tres mecanismos:

1. SHA-256 de los PNG de referencia para integridad del corpus.
2. Comparación raster con tolerancia documentada.
3. Invariantes estructurales sobre el Scene Graph.

No se usa exclusivamente comparación pixel-perfect.

## Control del entorno

El renderer utiliza:

- SkiaSharp fijado por gestión central de paquetes
- glifos vectoriales internos
- Source Sans 3 embebida
- Scene Graph determinista
- fondo y estilos semánticos propios

No depende de fuentes astrológicas del sistema.

## Regeneración

Los goldens solo se actualizan mediante opt-in explícito.

Una actualización visual debe revisarse como cambio deliberado, nunca
como consecuencia automática de ejecutar la suite normal.

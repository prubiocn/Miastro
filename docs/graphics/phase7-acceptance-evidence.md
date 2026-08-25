# Miastro — Evidencia técnica de aceptación de Fase 7

## Ventana central

La rueda natal forma parte de MainWindow.

Datos, Posiciones, Aspectos, rueda y panel de selección pertenecen a la
misma pantalla central.

No se crea una ventana específica para la rueda natal.

## Privacidad de logs gráficos

Miastro.Graphics y Miastro.Graphics.Skia no registran datos natales,
identificadores de persona, coordenadas, posiciones ni aspectos.

La UI tampoco incorpora esos datos a llamadas de logging de Fase 7.

## Límites de arquitectura

Miastro.Graphics:

- no depende de Avalonia
- no depende de SkiaSharp
- no depende de Swiss Ephemeris
- no depende de Astronomy

Miastro.Graphics.Skia:

- consume el Scene Graph
- no calcula astronomía
- no depende de Swiss Ephemeris

Miastro.UI.Avalonia:

- presenta la escena
- controla viewport, DPI, foco y eventos
- no contiene la geometría central de la rueda

## Evidencia visual

Los goldens técnicos requieren además inspección humana.

Casos:

- simple
- stellium
- many-aspects
- placidus
- koch

La inspección visual no se sustituye por hashes ni por invariantes
geométricas.

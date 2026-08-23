# Miastro — Tipografía determinista

## Fuente

Miastro empaqueta Source Sans 3 Regular como recurso interno.

Origen:

Adobe Fonts / source-sans, rama release.

Licencia:

SIL Open Font License 1.1.

## Integridad

SHA-256 de la fuente:

    4644c81b86ec9caaa76b634889968ed3c4f4f52f054855933acc7c2b21e53b0f

SHA-256 del archivo de licencia:

    56af9b9c6715597e458284a474dc118a50a4150e9d547c70f7b4a33c3e6a9328

## Motivación

La rueda no debe depender de la colección de fuentes disponible en el
sistema operativo.

Eso permite:

- CI reproducible
- render headless
- instalación consistente
- métricas tipográficas estables
- futuros renders de exportación coherentes

## Arquitectura

La fuente pertenece al backend Miastro.Graphics.Skia.

Miastro.Graphics no conoce archivos TTF ni APIs de Skia.

TextNode conserva únicamente:

- texto
- posición
- tamaño
- bounds
- StyleKey

## Render

SkiaTypographyProvider carga Source Sans 3 directamente desde un
EmbeddedResource.

No realiza búsqueda de fuentes del sistema.

## Glifos astrológicos

Los símbolos astrológicos continúan siendo vectores propios de
Miastro.

La fuente ordinaria se utiliza solo para texto humano:

- números de casas
- ASC / MC / DSC / IC
- posiciones
- etiquetas
- paneles y leyendas futuras

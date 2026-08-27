# ADR-079 — Presentación factual de Datos y Posiciones

## Estado

Aceptado para Fase 8.

## Contexto

Datos y Posiciones necesitan presentar todos los hechos natales relevantes
sin trasladar lógica astronómica ni de regencias a Avalonia.

También se necesitan glifos compactos sin acoplar controles de UI al renderer
Skia.

## Decisión

Mantener los hechos en los read models de Application.

Añadir `ObjectGlyphText` al catálogo de presentación factual.

Los read models calculan únicamente propiedades de presentación derivadas de
su propia identidad, sin modificar ni recalcular datos natales.

Datos utiliza cinco columnas compactas.

Posiciones utiliza una cabecera compacta y un detalle expandible.

El movimiento procede exclusivamente de `MotionText`.

La regencia de casa procede exclusivamente de la cúspide real ya resuelta por
la capa factual.

ASC y MC utilizan representación textual propia y etiqueta explícita de
ángulo.

## Consecuencias

Avalonia no depende de Skia para dibujar glifos dentro de listas.

No se duplica la lógica de regencias.

No existe inferencia de movimiento en UI.

Los puntos adicionales permanecen disponibles aun cuando la configuración
visual de la rueda pueda ocultarlos.

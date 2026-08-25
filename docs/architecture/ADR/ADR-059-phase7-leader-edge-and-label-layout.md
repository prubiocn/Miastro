# ADR-059 — Leader line al borde y layout determinista de etiquetas

## Estado

Aceptado para Fase 7.

## Leader lines

Las líneas guía terminan en GlyphBounds y no en VisualCenter.

Bounds de colisión y GlyphBounds se mantienen separados.

## Etiquetas

Las etiquetas de cuerpos y puntos pertenecen a LabelLayer.

El texto usa exclusivamente posición astrológica real.

La posición visual se utiliza solo para escoger un lugar legible en la
escena.

## Colisiones

Los candidatos de etiqueta tienen orden fijo.

No existe aleatoriedad.

Una etiqueta nunca puede ocupar los bounds protegidos de un objeto ni
los bounds de otra etiqueta aceptada.

## Retrogradación

NatalSceneObjectInput incorpora IsRetrograde.

Graphics no deduce movimiento ni recalcula velocidad.

El adaptador desde el snapshot es responsable de suministrar este dato.

## Arquitectura

La lógica permanece en Miastro.Graphics y no depende de Avalonia,
SkiaSharp, Swiss Ephemeris ni persistencia.

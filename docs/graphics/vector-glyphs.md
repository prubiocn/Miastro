# Miastro — Catálogo vectorial astrológico

## Objetivo

Los símbolos astrológicos de la rueda no dependen de fuentes
astrológicas instaladas en el sistema operativo.

Miastro dispone de un catálogo vectorial interno.

## Sistema geométrico

Cada glifo se define en un lienzo normalizado alrededor del origen.

Las coordenadas válidas quedan comprendidas entre -0.5 y 0.5.

El renderer escala esa geometría al tamaño solicitado por GlyphNode.

## Cobertura inicial

El catálogo incluye:

- doce signos zodiacales
- Sol
- Luna
- Mercurio
- Venus
- Marte
- Júpiter
- Saturno
- Urano
- Neptuno
- Plutón
- Nodo Norte verdadero
- Nodo Sur
- Lilith media
- Parte de Fortuna
- Quirón
- Ceres
- Pallas
- Juno
- Vesta
- ASC
- MC
- conjunción
- oposición
- trígono
- cuadratura
- sextil
- quincuncio

## Arquitectura

Las definiciones vectoriales pertenecen a Miastro.Graphics.

No contienen SkiaSharp.

Miastro.Graphics.Skia transforma las primitivas normalizadas en
operaciones concretas de render.

## Fuentes

Los glifos astrológicos no requieren una fuente del sistema.

La futura tipografía interna para textos ordinarios se tratará como
recurso separado.

## Determinismo

La resolución de una GlyphKey siempre produce la misma geometría.

El catálogo usa claves ordinales estables.

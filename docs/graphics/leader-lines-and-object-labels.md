# Miastro — Leader lines y etiquetas de objetos

## Leader lines

La línea guía parte del ancla correspondiente a la posición astrológica
real y termina en el borde geométrico del glifo visual.

No termina en el centro del símbolo.

Los bounds protegidos utilizados por el algoritmo anti-solapamiento y
los bounds físicos del glifo son conceptos distintos.

## GlyphBounds

NatalVisualPlacement conserva:

- Bounds: footprint protegido para colisiones
- GlyphBounds: rectángulo físico del símbolo
- GlyphSize: tamaño visual real

La Scene utiliza GlyphBounds para el glifo.

## Etiquetas

Las etiquetas de objetos se construyen a partir de:

- texto identificador
- longitud astrológica real
- grado y minuto dentro del signo
- abreviatura zodiacal
- marcador R cuando el input indica retrogradación

La posición visual desplazada nunca altera el contenido astrológico de
la etiqueta.

## Colisiones

El layout de etiquetas es determinista.

Cada etiqueta prueba candidatos en un orden fijo y se rechaza cuando:

- sale del canvas
- intersecta el footprint protegido de un glifo
- intersecta otra etiqueta
- intersecta texto estructural ya reservado

## Limitación del bloque 7F3B

El contrato soporta IsRetrograde, pero el adaptador de snapshot todavía
debe cablear MotionState.Retrograde al crear NatalSceneObjectInput.

Ese cableado corresponde al bloque 7F3C.

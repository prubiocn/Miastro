# Miastro — Corrección de orientación y glifos de Fase 7

## Orientación

Se mantiene el Ascendente a la izquierda.

La longitud zodiacal creciente desde el Ascendente avanza hacia el
hemisferio superior de la rueda.

Por tanto:

- ASC: izquierda
- DSC: derecha
- ASC + 90 grados: arriba
- ASC + 270 grados: abajo

La corrección se implementa en la transformación canónica central y no
mediante una rotación particular de Avalonia o Skia.

## MC

El MC se representa según su longitud real usando la misma transformación
canónica.

La corrección elimina la inversión vertical que colocaba el hemisferio
superior en la parte inferior del canvas.

## Signos zodiacales

Los doce signos emplean definiciones vectoriales internas reconocibles:

Aries, Tauro, Géminis, Cáncer, Leo, Virgo, Libra, Escorpio, Sagitario,
Capricornio, Acuario y Piscis.

No se utilizan fuentes astrológicas del sistema.

## Plutón

Plutón utiliza un monograma vectorial PL correspondiente al símbolo
astronómico/astrológico ♇.

No depende de una fuente externa.

## Goldens

Los baselines gráficos anteriores quedan obsoletos por cambio visual
intencionado.

No deben regenerarse hasta que la nueva salida haya sido revisada
visualmente.

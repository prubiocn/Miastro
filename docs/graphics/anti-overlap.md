# Miastro — Layout de cuerpos y anti-solapamiento

## Separación real y visual

Cada objeto conserva dos conceptos distintos:

- longitud astrológica real
- posición gráfica

La longitud real nunca se modifica.

RealAnchor representa la posición exacta derivada de la longitud
persistida.

VisualCenter representa el centro gráfico utilizado para dibujar el
glifo.

## Primer algoritmo determinista

La entrada se ordena mediante:

1. longitud zodiacal normalizada
2. Id estable como desempate

Para cada objeto se prueban niveles radiales en este orden:

    0, +1, -1, +2, -2, +3, -3...

Se selecciona el primer nivel cuyo bounding box protegido no
interseca ningún glifo previamente colocado.

No se usa aleatoriedad.

No se usan perturbaciones dependientes del orden recibido.

## Orden zodiacal

En 7B1 no existe desplazamiento angular.

Por tanto:

- el ángulo visual coincide con el ángulo real
- el orden zodiacal no puede invertirse
- solo cambia el radio gráfico

## Distancia mínima

NatalGlyphLayoutPolicy centraliza:

- tamaño del glifo
- separación mínima
- paso radial
- umbral de leader line
- nivel radial máximo

Estos valores derivan de NatalWheelMetrics y de su escala.

No se dispersan números mágicos por el algoritmo.

## Leader lines

La marca real permanece en RealAnchor.

Cuando el desplazamiento entre RealAnchor y VisualCenter supera el
umbral definido por la política se crea una leader line.

La línea no altera el dato astrológico.

## Evolución prevista

Si en casos extremos los niveles radiales no fueran suficientes,
un bloque posterior podrá introducir offset angular mínimo y
determinista manteniendo el orden zodiacal.

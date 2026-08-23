# Miastro — Coordenadas canónicas de la rueda natal

## Convención de pantalla

Miastro utiliza un sistema independiente de Avalonia y Skia:

- +X: derecha
- +Y: abajo
- 0 grados gráficos: derecha
- 90 grados gráficos: abajo
- 180 grados gráficos: izquierda
- 270 grados gráficos: arriba

## Orientación

La rueda se rota para situar el Ascendente a la izquierda.

ASC = 180 grados gráficos.

La conversión canónica es:

    relative = normalize(longitud - ASC)
    screenAngle = normalize(180 - relative)

Con esta transformación:

- longitud = ASC -> 180 grados
- ASC + 90 -> 90 grados
- ASC + 180 -> 0 grados
- ASC + 270 -> 270 grados

La longitud zodiacal creciente avanza visualmente en sentido
antihorario.

## Punto cartesiano

Para un ángulo gráfico theta:

    x = cx + r * cos(theta)
    y = cy + r * sin(theta)

La inversión vertical propia de las coordenadas de pantalla ya está
contenida en esta convención.

## Regla de dominio

La transformación gráfica nunca modifica la longitud astrológica real.

La posición real y la posición visual son conceptos diferentes.

La longitud real es inmutable.

Los algoritmos futuros de anti-solapamiento solo podrán modificar:

- posición gráfica
- offset angular visual
- nivel radial
- leader line

Nunca podrán modificar el dato astrológico.

# ADR-042 — Orientación canónica de la rueda natal

## Estado

Aceptado para Fase 7.

## Decisión

El Ascendente se representa siempre a 180 grados gráficos,
equivalente a la posición izquierda de la rueda,
aproximadamente las 9 en punto.

La fórmula es:

    relative = normalize(longitude - ascendant)
    screen = normalize(180 - relative)

El sistema gráfico usa:

- 0 grados: derecha
- 90 grados: abajo
- 180 grados: izquierda
- 270 grados: arriba

La longitud zodiacal creciente se muestra en sentido antihorario.

## Invariantes

- la longitud real nunca se altera
- la orientación depende únicamente del ASC persistido
- no se usa aleatoriedad
- misma entrada produce exactamente la misma transformación
- el renderer no contiene esta lógica

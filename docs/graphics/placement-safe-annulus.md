# Miastro — Anillo seguro de placements

## Objetivo

Los glifos visualmente redistribuidos no pueden abandonar el área
reservada a cuerpos y puntos.

La posición astrológica real permanece inmutable.

## Anillo seguro

El centro visual debe permanecer entre:

HouseInnerRadius + media diagonal protegida

y

ZodiacInnerRadius - media diagonal protegida

La media diagonal protegida incluye:

- tamaño del glifo
- separación mínima

Con esta regla, toda el área rectangular protegida del glifo cabe dentro
del anillo radial.

## Estrategia

El motor intenta primero:

1. posición angular real
2. nivel radial base
3. niveles radiales alternativos seguros

Si no existe espacio suficiente, introduce desplazamiento angular
determinista.

## Desplazamiento angular

El paso angular se calcula con el tamaño protegido y el radio seguro más
pequeño.

No se usa aleatoriedad.

El desplazamiento avanza en el mismo sentido que el orden zodiacal de
los objetos procesados para impedir cruces visuales.

## Prioridades

El algoritmo prioriza:

- longitud real intacta
- orden zodiacal
- cero solapamientos
- permanencia dentro del anillo
- mínimo desplazamiento angular necesario
- determinismo

## Leader lines

Cuando la distancia entre ancla real y centro visual supera el umbral
configurado, se mantiene la leader line existente.

## Casos de prueba

Se validan:

- stellium extremo
- canvas pequeño
- cluster en ASC
- cluster en MC
- desplazamiento angular
- orden zodiacal
- independencia del orden de entrada
- longitudes reales intactas

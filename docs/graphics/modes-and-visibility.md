# Miastro — Modos, visibilidad y degradación responsive

## Modos

La rueda dispone de dos modos visuales:

- Consultation
- Presentation

Consultation conserva el máximo contexto legible.

Presentation reduce información secundaria y utiliza por defecto una
escena sin etiquetas de texto.

Ningún modo recalcula la carta.

## Visibilidad funcional

La configuración permite controlar:

- planetas
- puntos
- aspectos
- cúspides
- etiquetas

La visibilidad opera sobre datos gráficos ya existentes.

No modifica:

- longitudes reales
- snapshot natal
- placements
- casas
- aspectos persistidos

## Planetas y puntos

BodyLayer y PointLayer pueden ocultarse de forma independiente.

Los placements originales permanecen intactos.

## Aspectos

Los aspectos solo se muestran cuando ambos extremos visibles están
presentes.

Esto evita líneas desconectadas de glifos ocultos.

## Cúspides

Ocultar cúspides elimina las líneas house-cusp.

ASC y MC permanecen como geometría principal independiente.

## Etiquetas

ShowLabels controla TextNode.

Los glifos y ejes principales siguen disponibles.

## Responsive

La política usa tres niveles:

Full:
- 720 unidades o más

Compact:
- desde 480 hasta menos de 720
- se eliminan marcas de grado menores de 5 grados

Minimal:
- menos de 480
- se conservan marcas de 10 grados
- se eliminan números de casa secundarios
- se reducen etiquetas de ejes secundarios
- se conservan ASC, MC y glifos principales

## Determinismo

La configuración visual no modifica el algoritmo de placement.

Misma carta, tamaño y configuración producen la misma escena.

# Miastro — Snapshot geométrico de rueda natal

## Objetivo

El layout de Fase 7 se construye antes del render.

El flujo es:

datos persistidos
-> entrada geométrica
-> layout determinista
-> scene graph
-> renderer

El renderer no calcula casas, ángulos ni astronomía.

## Métricas

NatalWheelMetrics deriva todas las dimensiones de un tamaño de
referencia de 800 unidades.

No existen coordenadas absolutas ligadas a una resolución concreta.

El centro se obtiene siempre del área disponible y los radios se
expresan como proporciones de la dimensión útil.

## Anillos

La jerarquía exterior a interior queda preparada como:

1. zodiaco
2. grados
3. casas
4. cuerpos y puntos
5. aspectos

## Zodiaco

Existen exactamente 12 sectores.

Cada sector conserva:

- longitud inicial real
- longitud central real
- ángulo gráfico inicial
- barrido gráfico

Cada sector ocupa exactamente 30 grados zodiacales.

## Grados

El snapshot contiene 360 marcas:

- 1 grado: marca menor
- 5 grados: marca intermedia
- 10 grados: marca principal

## Casas

Las doce cúspides provienen directamente de las longitudes reales
persistidas.

No se asumen casas iguales.

Cada casa conserva:

- número
- longitud real de cúspide
- ángulo gráfico
- punto exterior
- punto interior
- centro zodiacal real de la casa
- posición candidata del número de casa

## Ejes

El snapshot contiene:

- ASC
- DSC derivado como ASC + 180 grados
- MC
- IC derivado como MC + 180 grados

ASC y MC conservan su identidad como ejes principales.

DSC e IC son derivados visuales.

## Determinismo

Misma entrada y mismo tamaño producen exactamente el mismo snapshot.

ToDiagnosticText permite inspeccionar y comparar de forma estable:

- tamaño
- ASC
- MC
- signos
- casas
- ejes

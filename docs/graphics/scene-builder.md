# Miastro — Construcción de escena natal

## Responsabilidad

NatalWheelSceneBuilder transforma exclusivamente geometría ya
calculada en nodos del Scene Graph.

No consulta:

- Swiss Ephemeris
- base de datos
- servicios de tiempo
- reglas de aspectos
- controles Avalonia

## Entrada

El builder recibe:

- NatalWheelLayoutSnapshot
- NatalObjectPlacementSnapshot
- definiciones semánticas de objetos visibles

## Salida

Produce NatalScene con orden de pintura estable.

## Elementos actuales

La primera escena geométrica contiene:

- disco de fondo
- límites del anillo zodiacal
- 12 sectores zodiacales
- 12 posiciones de glifos zodiacales
- 360 marcas de grado
- 12 cúspides reales
- 12 números de casa
- ASC
- DSC
- MC
- IC
- etiquetas de ejes
- marca de posición real para cada objeto
- glifo en posición visual
- leader line cuando el placement la requiere

## Regla real frente a visual

La marca real usa siempre RealAnchor.

El glifo usa siempre VisualCenter.

El Scene Builder no modifica ninguno de los dos.

## Catálogo de glifos

En este bloque GlyphNode conserva una GlyphKey estable.

El catálogo vectorial interno que resolverá esas claves se implementa
en el bloque de renderer y recursos gráficos.

No se depende de una fuente astrológica del sistema.

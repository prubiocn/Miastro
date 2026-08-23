# Miastro — Controles y selección de la rueda natal

## Controles de visibilidad

La pantalla natal expone controles para:

- planetas
- puntos
- aspectos
- cúspides
- etiquetas

Cada opción se traduce a NatalWheelVisibilityOptions.

El cambio es exclusivamente visual y reutiliza el mismo snapshot natal
persistido.

## Modos

La interfaz expone:

- Consulta
- Presentación

Consulta conserva el detalle de trabajo.

Presentación aplica la política visual definida en Miastro.Graphics y
reduce información secundaria.

## Reconstrucción

Cuando cambia una opción visual, el ViewModel solicita una nueva
presentación a NatalWheelPresentationService utilizando el mismo
NatalChartSnapshotReadModel vigente.

No se recalculan:

- longitudes
- casas
- aspectos
- posiciones astrológicas

## Selección

La interacción se resuelve mediante NatalSceneHitTester.

El hit testing utiliza la geometría visual final del glifo, incluida la
posición desplazada por anti-solapamiento.

Una vez seleccionado un objeto, el panel descriptivo obtiene sus datos
del snapshot natal persistido.

## Panel

El panel muestra:

- nombre
- posición zodiacal
- casa
- movimiento

## Separación de responsabilidades

Avalonia coordina estado visual e interacción.

Miastro.Graphics conserva:

- layout
- geometría
- anti-solapamiento
- Scene Graph
- hit testing

La UI no implementa geometría central.

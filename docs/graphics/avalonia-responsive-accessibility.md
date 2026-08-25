# Miastro — Rueda natal responsive y accesible en Avalonia

## Viewport lógico

La rueda ya no tiene un tamaño fijo de 560x560.

El host de Avalonia calcula un viewport cuadrado a partir del espacio
horizontal disponible, con un máximo visual de 720 DIPs.

El Scene Graph continúa siendo responsable de toda la geometría natal.

## HiDPI

Avalonia obtiene RenderScaling desde el TopLevel.

NatalWheelPresentationService recibe:

- ancho lógico
- alto lógico
- RenderScaling

El Scene Graph usa dimensiones lógicas.

El PNG se crea a:

logicalWidth * RenderScaling
logicalHeight * RenderScaling

Así se conserva nitidez en pantallas HiDPI sin duplicar geometría en UI.

## Stretch e interacción

La imagen utiliza Stretch=Uniform.

NatalSceneHitTester aplica exactamente la misma transformación uniforme:

- escala por el mínimo de X/Y
- centrado
- exclusión de letterboxing
- transformación inversa a coordenadas de escena

## Teclado

La rueda puede recibir foco.

Controles:

- Derecha / Abajo: siguiente objeto visible
- Izquierda / Arriba: objeto visible anterior
- Home: primer objeto visible
- End: último objeto visible
- Escape: limpiar selección

La enumeración de objetos seleccionables procede de Graphics.

## Accesibilidad

AutomationProperties expone:

- nombre de la rueda
- instrucciones de teclado
- objeto seleccionado
- posición
- casa
- movimiento

El panel lateral continúa ofreciendo una representación textual
equivalente a la selección gráfica.

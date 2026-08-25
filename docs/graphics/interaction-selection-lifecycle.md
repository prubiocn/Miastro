# Miastro — Ciclo de vida de selección de la rueda natal

## Selección y cambios visuales

La selección actual se conserva cuando la rueda se reconstruye por un
cambio exclusivamente visual:

- tamaño del viewport
- RenderScaling
- modo Consulta / Presentación
- visibilidad de aspectos
- visibilidad de cúspides
- visibilidad de etiquetas

También se conserva al cambiar la visibilidad de planetas o puntos
siempre que el objeto seleccionado siga formando parte de la escena.

## Objeto oculto

Si una configuración elimina de la escena el objeto seleccionado, la
selección se limpia.

No se mantiene un objeto invisible como selección activa.

## Snapshot nuevo

Una carta natal nueva o recién cargada reinicia la selección.

La selección visual nunca se hereda implícitamente entre snapshots.

## Foco

Un click sobre la rueda mueve el foco de teclado al Image interactivo.

Después del click se puede continuar la navegación mediante las teclas
de dirección, Home, End y Escape.

## Fuente de verdad

La validez de una selección se comprueba contra los object-glyph
seleccionables presentes en el Scene Graph mediante
NatalSceneHitTester.GetSelectableObjectIds.

Avalonia no reproduce lógica geométrica.

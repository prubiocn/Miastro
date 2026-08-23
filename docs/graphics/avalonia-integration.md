# Miastro — Integración de la rueda natal con Avalonia

## Flujo

La pantalla de persona utiliza el snapshot natal vigente ya cargado
por MainWindowViewModel.

El flujo gráfico es:

NatalChartSnapshotReadModel
-> NatalChartSnapshotGraphicsAdapter
-> NatalWheelSceneComposer
-> NatalScene
-> SkiaNatalSceneRenderer
-> PNG en memoria
-> Avalonia Bitmap
-> Image

## Responsabilidades

Avalonia no calcula:

- longitudes
- casas
- orientación
- placements
- anti-solapamiento
- aspectos
- geometría zodiacal

La UI únicamente solicita una presentación y muestra el resultado.

## Interacción

PointerPressed obtiene coordenadas locales del Image.

NatalSceneHitTester transforma las coordenadas del viewport a
coordenadas de Scene Graph.

La selección usa Bounds del glifo visual desplazado.

## Snapshot

La rueda se reconstruye cuando ApplyNatalSnapshot recibe un snapshot
vigente, tanto al cargar una persona como después de calcular la carta.

ResetNatalDisplay elimina también la presentación gráfica.

## Recursos

La imagen se genera completamente en memoria.

No se crean archivos temporales de usuario.

## Privacidad

La representación gráfica no registra nombre, fecha, localidad ni
otros datos personales.

Los identificadores de objeto de la rueda son identificadores
astrológicos, no identificadores personales.

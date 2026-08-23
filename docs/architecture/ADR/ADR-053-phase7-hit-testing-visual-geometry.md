# ADR-053 — Hit testing sobre geometría visual final

## Estado

Aceptado para Fase 7.

## Decisión

La selección de objetos de la rueda se resuelve contra el Scene Graph
final.

Un objeto desplazado por anti-solapamiento debe ser seleccionable en
su posición gráfica real de pantalla.

## Motivación

Usar la longitud astrológica o RealAnchor como objetivo interactivo
rompería la correspondencia entre lo que el usuario ve y lo que puede
seleccionar.

## Límites

El motor:

- no depende de Avalonia
- no depende de SkiaSharp
- no accede a persistencia
- no recalcula astronomía
- no modifica placements

La UI futura convierte las coordenadas de puntero a coordenadas de
escena y delega en NatalSceneHitTester.

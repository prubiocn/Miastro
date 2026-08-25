# ADR-064 — Paneles natales y selección bidireccional

## Estado

Aceptado para Fase 7.

## Decisión

La pantalla natal mantiene tres paneles explícitos:

- Datos
- Posiciones
- Aspectos

NatalPlacementRowViewModel conserva AstrologicalObjectId como identidad
semántica.

La selección del panel y la selección de la rueda convergen en
ApplyNatalWheelSelection.

## Rueda a panel

El Id obtenido por hit testing se resuelve contra la colección
NatalPlacements ya cargada.

No se crea una fila paralela para representar la selección.

## Panel a rueda

SelectedNatalPlacement es TwoWay y dirige la selección mediante el Id
semántico.

## Aspectos

NatalAspectRowViewModel adapta exclusivamente datos del snapshot
persistido.

No existe recálculo de aspectos en Avalonia.

## Arquitectura

Avalonia sigue sin implementar geometría central, astronomía ni reglas
de aspectos.

# Miastro — Paneles Datos, Posiciones y Aspectos

## Datos

El panel Datos muestra el estado de la carta natal vigente y sus
metadatos de presentación.

No realiza cálculo astronómico.

## Posiciones

El panel Posiciones consume NatalPlacementSnapshot.

Cada fila conserva AstrologicalObjectId además del texto visible.

La lista es seleccionable con teclado mediante el comportamiento nativo
de ListBox.

SelectedItem está enlazado en modo TwoWay con SelectedNatalPlacement.

## Sincronización

Rueda -> panel:

el hit testing devuelve el Id del objeto y el ViewModel recupera la
misma instancia existente en NatalPlacements.

Panel -> rueda:

el setter SelectedNatalPlacement utiliza el mismo
ApplyNatalWheelSelection empleado por ratón y teclado.

No existen dos estados independientes de selección.

## Aspectos

El panel Aspectos consume exclusivamente NatalAspectSnapshot persistido.

Muestra:

- primer objeto
- tipo de aspecto
- segundo objeto
- orbe usado

La UI no recalcula separaciones, orbes ni reglas de aspectos.

## Casas extremas

La suite de Fase 7 contiene casos explícitos de:

- casa extremadamente estrecha
- casa muy ancha
- cruce 0/360

Se validan las 12 cúspides, los 12 números de casa y coordenadas finitas
dentro del canvas.

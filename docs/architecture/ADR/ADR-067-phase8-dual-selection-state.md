# ADR-067 — Estado de selección dual natal

## Estado

Aceptado para Fase 8.

## Contexto

La matriz de aspectos necesita seleccionar simultáneamente dos objetos y un aspecto, mientras la rueda y los paneles existentes trabajan principalmente con selección simple.

Introducir esta lógica directamente en controles Avalonia dificultaría las pruebas y produciría estados incoherentes entre rueda y paneles.

## Decisión

Definir un estado de selección natal headless con tres posibilidades:

1. neutral;
2. objeto simple;
3. selección dual de aspecto.

La selección dual contiene dos `AstrologicalObjectId` distintos y un `AspectKind`.

Los objetos se normalizan según `NatalObjectOrder`.

Seleccionar un objeto simple elimina cualquier selección dual previa.

Limpiar selección devuelve siempre el estado neutral.

Una celda de matriz sin aspecto no puede crear selección dual.

## Consecuencias

La sincronización rueda-panel podrá depender de un único contrato factual.

La UI no tendrá que decidir cómo normalizar parejas.

La futura lógica de resaltado podrá distinguir claramente entre selección simple y aspecto activo sin introducir recálculo.

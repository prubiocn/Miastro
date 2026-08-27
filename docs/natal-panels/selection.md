# Fase 8 — Selección natal

## Estados

La selección natal dispone de tres estados lógicos:

- neutral;
- selección simple de un objeto;
- selección dual de un aspecto.

## Selección simple

Una selección simple contiene únicamente `PrimaryObjectId`.

No existe aspecto activo ni segundo objeto.

Se utilizará para sincronización desde:

- rueda;
- Datos;
- Posiciones.

## Selección dual

Una selección de aspecto contiene:

- objeto primario;
- objeto secundario;
- `AspectKind` activo.

Los dos objetos se normalizan según `NatalObjectOrder`.

Esto hace determinista el estado independientemente del sentido A-B o B-A con el que llegue la selección.

## Estado neutral

El estado neutral no contiene:

- objeto primario;
- objeto secundario;
- aspecto activo.

La UI podrá volver a neutral mediante Escape, clic en fondo o acción equivalente.

## Matriz

Una celda sin aspecto no puede generar selección dual.

Una celda significativa conserva la identidad factual del aspecto persistido y puede convertirse directamente en estado dual.

## Frontera

Este modelo no conoce:

- Avalonia;
- controles;
- SkiaSharp;
- Scene Graph;
- persistencia;
- Swiss Ephemeris.

La adaptación visual de resaltado se implementará en una capa posterior.

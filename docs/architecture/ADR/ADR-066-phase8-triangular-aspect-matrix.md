# ADR-066 — Matriz triangular natal de aspectos

## Estado

Aceptado para Fase 8.

## Contexto

La pantalla natal necesita una matriz compacta, determinista, accesible y sincronizable con la rueda sin recalcular aspectos.

## Decisión

La matriz se construye en la capa headless de lectura a partir de los aspectos persistidos.

Los participantes se filtran mediante `MiastroV1AspectProfile` y se ordenan mediante `NatalObjectOrder`.

Solo se materializa el triángulo inferior estricto, con `RowIndex > ColumnIndex`.

Cada pareja se normaliza por orden canónico, de forma que A-B y B-A representan una única clave.

Una celda puede representar explícitamente ausencia de aspecto.

La matriz conserva todos los valores factuales persistidos necesarios para tooltip, accesibilidad y selección posterior.

## Consecuencias

No existe cálculo astronómico ni detección de aspectos en UI.

La matriz puede probarse completamente sin Avalonia.

La futura selección dual podrá utilizar `RowObjectId`, `ColumnObjectId` y `AspectKind` sin reinterpretar los datos.

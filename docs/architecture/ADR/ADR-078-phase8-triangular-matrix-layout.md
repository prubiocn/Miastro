# ADR-078 — Layout triangular de la matriz natal

## Estado

Aceptado para Fase 8.

## Contexto

La matriz de aspectos debe representar cada pareja una sola vez y seguir siendo utilizable en un panel lateral estrecho.

## Decisión

Representar el triángulo inferior estricto a partir de `RowIndex` y `ColumnIndex`.

La capa UI crea una proyección de layout con columnas y filas, pero mantiene las mismas instancias de `NatalAspectMatrixCell`.

Cada fila contiene exclusivamente las celdas cuyo `ColumnIndex` es menor que su `RowIndex`.

Las celdas con aspecto son botones estándar de Avalonia.

Las celdas sin aspecto se mantienen visibles pero desactivadas.

El contenedor admite scroll horizontal.

La lista accesible secuencial se conserva como alternativa compacta.

## Consecuencias

No aparecen duplicados A-B/B-A.

No hay una segunda implementación del cálculo de aspectos.

La selección triangular y la lista accesible comparten exactamente el mismo contrato `SelectedAspectCell`.

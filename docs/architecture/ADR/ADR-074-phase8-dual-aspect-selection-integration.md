# ADR-074 — Integración de selección dual de aspectos

## Estado

Aceptado para Fase 8.

## Contexto

La matriz representa relaciones entre dos objetos y necesita un estado más rico que la selección simple heredada de Fase 7.

## Decisión

`NatalPanelHostViewModel` mantiene `NatalSelectionState` como contrato factual de selección.

Una celda con aspecto puede crear una selección dual.

Una celda sin aspecto no produce selección dual.

La selección dual sincroniza los dos participantes y conserva `AspectKind`.

Los eventos de usuario y las sincronizaciones programáticas se separan para evitar bucles.

La limpieza devuelve un estado neutral completo.

## Consecuencias

La UI puede representar después dos objetos y una línea activa sin redefinir identidad ni orden.

La rueda de Fase 7 permanece compatible mientras se introduce progresivamente el resaltado dual.

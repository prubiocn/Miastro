# ADR-071 — Resumen natal factual de Fase 8

## Estado

Aceptado para Fase 8.

## Contexto

La pantalla natal necesita una vista breve que concentre los datos principales de la carta sin convertirse todavía en interpretación o informe.

## Decisión

Crear `NatalSummaryBuilder` en la capa headless de lectura.

El Resumen incluye:

- Sol;
- Luna;
- ASC;
- MC;
- predominio elemental;
- predominio modal;
- concentración de casas;
- retrogradaciones;
- máximo cinco aspectos principales.

Los aspectos se ordenan primero por menor desviación al exacto.

En empate se usa una prioridad canónica estable del tipo y después el orden natal de los participantes.

La concentración de casas exige un máximo único.

Las retrogradaciones proceden únicamente del estado persistido.

La salida no supera trece líneas estructurales.

## Consecuencias

La UI puede presentar un Resumen compacto sin duplicar reglas.

Los modelos son reutilizables por informes futuros.

La interpretación textual futura queda fuera de esta capa y fuera de Fase 8.

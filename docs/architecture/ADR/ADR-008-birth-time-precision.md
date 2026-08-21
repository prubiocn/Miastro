# ADR-008 — Precisión de hora natal

Estado: Aceptado en Fase 5.

Opciones de dominio:

- Exacta;
- Aproximada;
- Rango;
- Momento del día;
- Desconocida.

Reglas:

- Exacta y Aproximada pueden resolver tiempo histórico cuando existe hora.
- Rango conserva inicio y fin y no produce un único Instant.
- Momento del día conserva categoría y no inventa hora.
- Desconocida no produce Instant.

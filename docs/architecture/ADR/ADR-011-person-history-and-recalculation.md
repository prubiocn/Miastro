# ADR-011 — Historial mínimo e invalidación temporal

Estado: Aceptado en Fase 5.

Historial significativo:

- creación;
- edición relevante;
- borrado no se conserva tras hard delete salvo evidencia técnica externa no
  personal;
- futuras referencias a trabajos, cuando existan en fases posteriores.

No registrar clics ni navegación trivial.

Cambios en fecha, hora, precisión, localidad, coordenadas o zona IANA deben
invalidar la resolución natal previa y recalcularla cuando proceda.

# ADR-007 — Persona: dominio y persistencia

Estado: Aceptado en Fase 5.

Decisión pendiente de cerrar tras inspeccionar el modelo EF existente.

Principios obligatorios:

- Persona es el agregado funcional principal de la Fase 5.
- DatosNacimiento y ResidenciaActual son modelos separados.
- UI no expone entidades EF directamente.
- La entidad no incorpora cálculo astrológico.
- La persistencia debe conservar compatibilidad con futuras relaciones sin
  adelantar CartaNatal ni otros trabajos astrológicos.

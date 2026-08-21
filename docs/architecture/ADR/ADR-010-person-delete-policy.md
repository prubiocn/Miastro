# ADR-010 — Política de borrado de Persona

Estado: Aceptado en Fase 5.

Opción principal prevista: hard delete con confirmación explícita para el uso
personal/local de Miastro.

La cascada debe eliminar de forma íntegra:

- DatosNacimiento;
- ResidenciaActual;
- historial mínimo asociado.

No deben quedar huérfanos.

La decisión final se confirma al implementar el modelo EF.

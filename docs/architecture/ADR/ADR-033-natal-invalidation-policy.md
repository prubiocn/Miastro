# ADR-033 — Invalidación automática de Carta Natal

Estado: Propuesto durante Fase 6.

## Decisión

Una Carta Natal vigente queda `Invalidated` cuando cambia cualquier dato de
nacimiento que pueda afectar al cálculo.

Incluye:

- fecha;
- precisión;
- hora;
- rango o momento del día;
- localidad;
- GeoNameId;
- coordenadas;
- zona IANA;
- versión TZDB;
- estado de resolución;
- offset histórico;
- Instant UTC;
- elección de hora ambigua;
- override manual de coordenadas.

No invalidan la carta:

- nombre;
- apellidos;
- teléfono;
- email;
- nota privada;
- favorito;
- residencia actual.

## Atomicidad

La modificación de Persona, la invalidación del snapshot vigente y el evento
de historial se guardan mediante el mismo `MiastroDbContext` y el mismo
`SaveChangesAsync`.

## Historial

Eventos funcionales:

- NatalChartCalculated;
- NatalChartRecalculated;
- NatalChartInvalidated.

Los resúmenes no contienen nombre completo, contacto, nota privada ni datos
natales detallados.

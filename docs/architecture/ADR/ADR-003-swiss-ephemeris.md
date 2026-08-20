# ADR-003 — Swiss Ephemeris

## Estado

Aceptado.

## Decisión

Miastro utiliza Swiss Ephemeris como motor astronómico de bajo nivel.

Toda interacción con Swiss Ephemeris queda encapsulada en:

`Miastro.Infrastructure.SwissEphemeris`

El resto del sistema consume únicamente:

`Miastro.Astronomy.Abstractions`

No se exponen tipos, IDs, flags, buffers, handles ni errores C fuera del
adaptador.

## Perfil V1

- tropical;
- geocéntrico;
- eclíptico;
- aparente;
- velocidad;
- Nodo Verdadero;
- Lilith Media.

## Consecuencias

La integración nativa queda desacoplada del dominio, UI, interpretación y
casos de uso superiores.

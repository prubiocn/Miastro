# Modelo mínimo de carta

`AstrologicalChart` es un contenedor puro de dominio.

Puede contener:

- identificador;
- tipo de carta;
- placements;
- cúspides opcionales;
- sistema de casas opcional;
- metadatos de cálculo;
- CalculationProfile;
- AspectProfile.

Tipos modelados:

- Natal
- SolarReturn
- LunarReturn
- Transit
- SecondaryProgression
- SynastryReference

No se implementa cálculo astronómico en Fase 2.

## Invariantes

- El identificador no puede ser vacío.
- No puede existir el mismo objeto dos veces en una carta.
- Si existen cúspides, deben existir exactamente las casas 1–12.
- Si existen cúspides, debe declararse el sistema de casas.

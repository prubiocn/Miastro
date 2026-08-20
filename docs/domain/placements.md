# Posiciones astrológicas

`AstrologicalPlacement` representa una posición ya calculada por una capa externa.

Contiene:

- objeto;
- longitud eclíptica;
- signo derivado;
- grado dentro del signo;
- casa opcional;
- velocidad opcional;
- estado de movimiento opcional.

No contiene tipos ni dependencias de Swiss Ephemeris.

La retrogradación se deriva del signo de la velocidad sin redondeo:

- velocidad > 0 → Direct
- velocidad < 0 → Retrograde
- velocidad = 0 → Stationary

# ADR-037 — Política natal de movimiento estacionario

Estado: Propuesto durante Fase 6.

## Decisión

La clasificación de movimiento V1 utiliza exclusivamente el signo de la
velocidad longitudinal calculada por Swiss Ephemeris:

- velocidad > 0: Direct;
- velocidad < 0: Retrograde;
- velocidad exactamente 0: Stationary.

No se introduce un umbral artificial de proximidad a cero.

## Motivo

Un umbral numérico elegido por Miastro sería una convención adicional que no
procede del motor astronómico y podría clasificar como estacionario un objeto
que Swiss devuelve con movimiento real distinto de cero.

La política de cero exacto:

- conserva el dato físico entregado por el motor;
- evita tolerancias astrológicas arbitrarias;
- es determinista y reproducible;
- mantiene la compatibilidad con el contrato de movimiento existente.

Si una futura versión adopta una definición astrológica de "estacionario",
deberá versionarse como una política distinta y documentar explícitamente su
criterio.

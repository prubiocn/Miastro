# ADR-040 — Baseline de rendimiento natal sin umbrales arbitrarios

Estado: Propuesto durante Fase 6.

## Decisión

Miastro medirá el pipeline natal real en pruebas automatizadas, pero Fase 6 no
hará fallar CI por un límite absoluto de tiempo.

Se registran por separado carga de Persona, 17 llamadas Swiss, casas,
derivados/aspectos, cálculo completo con persistencia, recarga de snapshot y
fast path idempotente.

## Motivo

Los tiempos absolutos de una aplicación de escritorio dependen fuertemente del
hardware y de la carga del runner.

Un límite elegido sin entorno de referencia produciría falsos positivos y no
sería una garantía técnica defendible.

## Evolución

Si se establece un entorno de benchmark estable, los valores de esta baseline
podrán convertirse en series históricas y utilizar percentiles o límites de
regresión relativos versionados.

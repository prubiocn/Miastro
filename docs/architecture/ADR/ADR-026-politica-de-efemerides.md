# ADR-026 — Política de datos de efemérides

## Estado

Aceptado.

## Decisión

Los datos obligatorios de efemérides:

- se manifiestan explícitamente;
- registran tamaño y SHA-256;
- se validan antes del cálculo;
- no admiten degradación silenciosa.

Un fichero ausente o corrupto produce un error tipado.

Miastro no considera válido un resultado obtenido mediante fallback no
solicitado cuando el perfil exige Swiss Ephemeris.

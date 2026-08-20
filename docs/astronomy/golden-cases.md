# Golden cases — Fase 3

## Fuente externa

Fuente independiente del adaptador Miastro:

**Astrodienst Swiss Ephemeris Test Page**

Servicio:

`https://www.astro.com/cgi/swetest.cgi`

Motor de referencia:

`Swiss Ephemeris 2.10.03`

Las respuestas HTTP originales se conservan en:

`tests/golden/phase3/external/`

Los valores parseados se conservan en:

`tests/golden/phase3/golden-values.json`

Miastro no genera los valores esperados.

## Parámetros CGI

El formulario oficial se invoca mediante los campos:

- `b` — fecha
- `n` — número de pasos
- `s` — paso
- `p` — objetos
- `e` — motor de efemérides
- `f` — formato
- `arg` — opciones adicionales

## Posiciones

Configuración:

- Swiss Ephemeris;
- tropical;
- geocéntrica;
- eclíptica;
- aparente;
- UT;
- velocidad activada;
- sin topocentrismo;
- sin modo sidéreo;
- sin `TRUEPOS`.

Casos:

- 2024-01-01 12:00 UT;
- 1900-01-01 12:00 UT;
- Sol a Plutón;
- Nodo Verdadero;
- Lilith Media;
- Quirón;
- Ceres;
- Palas;
- Juno;
- Vesta;
- Mercurio retrógrado.

## Casas

Casos:

- Madrid — hemisferio norte;
- Sydney — hemisferio sur;
- Placidus;
- Koch;
- 12 cúspides;
- ASC;
- MC.

Los datos de ubicación se expresan en grados decimales.

## Trazabilidad

Cada respuesta HTTP tiene:

- tamaño;
- SHA-256;
- fecha de captura;
- versión declarada del motor.

Una captura sin valores astronómicos es considerada inválida y no puede
convertirse en golden case.

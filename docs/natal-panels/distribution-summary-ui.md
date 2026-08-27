# Fase 8 — Distribución y Resumen en UI

## Distribución

La pestaña representa de forma textual y estructurada:

- elementos;
- modalidades;
- polaridad;
- hemisferio Este / Oeste;
- hemisferio Superior / Inferior;
- cuadrantes I-IV;
- casas angulares, sucedentes y cadentes.

Cada fila muestra etiqueta, recuento y objetos incluidos.

No se utilizan barras, gráficos ni proporciones visuales basadas en ancho.

## Perfil

La UI muestra el identificador del perfil y la cantidad de objetos contados.

La política de inclusión continúa perteneciendo a `NatalDistributionProfile`.

La UI no decide qué objetos cuentan.

## Predominio y equilibrio

Cada sección muestra uno de estos estados factuales:

- predominio de una categoría;
- equilibrado;
- sin predominio único.

Estos estados proceden del read model existente.

## Síntesis

La síntesis factual ya calculada se conserva al final de la pestaña.

No se transforma en interpretación extensa.

## Resumen

Resumen muestra de forma breve:

- Sol: signo y casa;
- Luna: signo y casa;
- ASC: signo;
- MC: signo;
- elemento predominante;
- modalidad predominante;
- concentración de casas;
- retrógrados relevantes;
- hasta cinco aspectos principales.

Los aspectos principales proceden directamente de `NatalSummaryBuilder`.

## Alcance

La UI no recalcula distribución, casas, movimiento ni aspectos.

No modifica el snapshot y no utiliza Swiss Ephemeris.

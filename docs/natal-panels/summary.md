# Fase 8 — Resumen natal factual

## Objetivo

Resumen proporciona una lectura rápida y compacta antes de una consulta detallada.

No es un informe interpretativo.

## Contenido

Incluye:

- Sol: signo y casa;
- Luna: signo y casa;
- ASC: signo;
- MC: signo;
- elemento predominante o estado sin predominio;
- modalidad predominante o estado sin predominio;
- concentración de casas;
- retrogradaciones de los diez planetas;
- hasta cinco aspectos principales.

## Aspectos principales

Los aspectos principales se seleccionan de forma determinista.

Orden:

1. menor desviación respecto al ángulo exacto;
2. prioridad estable del tipo de aspecto;
3. orden canónico de los participantes.

Se muestran como máximo cinco.

La UI no recalcula aspectos.

## Concentración de casas

Se utiliza exclusivamente `HouseNumber` persistido de los objetos incluidos en `NatalDistributionProfile`.

Solo se declara una casa predominante cuando existe un máximo único.

Un empate no se resuelve arbitrariamente.

## Retrogradaciones

El Resumen utiliza exclusivamente el `MotionState` persistido.

No infiere movimiento a partir de velocidad.

## Longitud

El modelo está limitado estructuralmente a:

- ocho líneas base;
- máximo cinco aspectos.

Por tanto, el Resumen tiene como máximo trece líneas.

## Frontera interpretativa

No se generan afirmaciones sobre:

- personalidad;
- destino;
- misión;
- conducta recomendada;
- predicción.

El contenido es exclusivamente factual y estructural.

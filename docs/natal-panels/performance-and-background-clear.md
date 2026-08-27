# Fase 8 — rendimiento y limpieza de selección

## Limpieza por fondo

La rueda usa un único pipeline de hit-testing.

`SelectNatalWheelAt` obtiene un hit mediante `NatalSceneHitTester` y envía:

`hit?.ObjectId`

a `ApplyNatalWheelSelection`.

Cuando el clic cae sobre el fondo de la rueda, el hit es nulo y el endpoint
recibe `null`.

Por tanto, clic de fondo y Escape convergen en el mismo estado neutral.

No existe una segunda implementación de limpieza.

## Rendimiento

La Fase 8 mide de forma automatizada:

- construcción de Datos;
- construcción de Posiciones;
- construcción de Aspectos;
- construcción de Distribución;
- construcción de Resumen;
- construcción del host completo;
- selección simple;
- selección dual de aspecto;
- limpieza de selección.

Las mediciones utilizan `Stopwatch` sobre un snapshot determinista de prueba.

Cada prueba realiza calentamiento previo y varias iteraciones.

Los tiempos se imprimen como:

- tiempo total;
- microsegundos medios por operación.

## Umbral

El umbral automatizado de 5 segundos es deliberadamente defensivo.

Su objetivo es detectar bloqueos o regresiones catastróficas en CI, no
convertir pruebas unitarias en un benchmark dependiente del hardware.

Las cifras observadas sirven como evidencia de rendimiento de la fase.

## Arquitectura

Las pruebas no recalculan datos mediante Swiss Ephemeris.

Los ViewModels consumen exclusivamente snapshot, lectores y servicios
headless existentes.

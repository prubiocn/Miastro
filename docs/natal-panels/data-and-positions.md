# Fase 8 — Datos y Posiciones

## Modelos de lectura

Datos y Posiciones se construyen desde `NatalFactsReader`.

Los modelos son headless y no dependen de Avalonia, SkiaSharp, Swiss Ephemeris ni persistencia.

## Datos

La vista Datos consume:

- ObjectId;
- nombre;
- grado y minuto dentro del signo;
- signo;
- regente o regentes;
- indicador de ángulo.

El glifo se resolverá posteriormente en presentación a partir de `ObjectId`, evitando acoplar `Miastro.Application` al catálogo gráfico.

## Posiciones

La vista Posiciones consume:

- ObjectId;
- nombre;
- posición exacta en signo;
- casa persistida;
- movimiento persistido;
- regente o regentes del signo;
- signo de la cúspide de la casa;
- regente o regentes de esa cúspide;
- indicador de ángulo.

ASC y MC permanecen como hechos seleccionables pero se identifican como ángulos para que la UI no los trate visualmente como planetas.

## Puntos adicionales

Nodo, Lilith, Parte de Fortuna y asteroides no se eliminan del modelo de lectura aunque su visibilidad gráfica de rueda esté desactivada.

La visibilidad de rueda y la disponibilidad factual son conceptos independientes.

## Formato

Las posiciones se presentan en grados y minutos.

La precisión original del snapshot se conserva internamente.

El redondeo visual se realiza exclusivamente durante la presentación y es independiente de la cultura del sistema.

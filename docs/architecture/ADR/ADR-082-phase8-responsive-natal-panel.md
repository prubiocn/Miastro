# ADR-082 — Layout responsive del panel natal

## Estado

Aceptado para Fase 8.

## Contexto

La composición `2*,*` con un panel de anchura mínima fija podía reducir la
rueda o forzar contenido ilegible en áreas estrechas.

## Decisión

Cambiar el contenedor natal a `*,Auto`.

El panel mantiene una anchura legible y dispone de colapso explícito.

La vista observa cambios de tamaño del contenedor.

Por debajo de 720 px, el panel se colapsa automáticamente.

La vista registra si ese colapso fue automático. Solo en ese caso puede
reabrirlo al recuperar anchura.

Las decisiones de pestaña y selección continúan en los ViewModels
correspondientes.

## Consecuencias

La rueda recibe prioridad sobre el espacio sobrante.

Las tablas no se comprimen para intentar encajar artificialmente.

La matriz sigue usando desplazamiento horizontal.

El comportamiento responsive no introduce lógica de dominio.

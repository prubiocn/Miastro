# Fase 8 — teclado y estado neutral natal

## Estado neutral único

Toda limpieza de selección pasa por `MainWindowViewModel.ClearNatalSelection`.

Ese método reutiliza el pipeline existente `ApplyNatalWheelSelection(null)`.

Como consecuencia se limpian conjuntamente:

- selección de la rueda;
- fila seleccionada de Datos;
- fila seleccionada de Posiciones;
- celda seleccionada de Aspectos;
- selección dual;
- aspecto activo;
- overlay gráfico de selección.

No existe una segunda lógica independiente de limpieza.

## Escape

Escape funciona tanto cuando el foco está:

- en la rueda;
- en Datos;
- en Posiciones;
- en Aspectos;
- en Distribución;
- en Resumen;
- en el TabControl natal.

La ventana escucha únicamente Escape como acción global de estado neutral.

## Navegación estándar

No se intercepta Tab.

Los `TabItem` y `ListBox` conservan la navegación estándar de Avalonia.

La rueda conserva:

- flechas;
- Home;
- End;
- Escape.

## Compatibilidad

`ClearNatalWheelSelection` se mantiene como método compatible con Fase 7, pero delega en `ClearNatalSelection`.

## Alcance

Este bloque no modifica:

- astronomía;
- Scene Graph base;
- layout;
- persistencia;
- snapshot;
- cálculo de aspectos.

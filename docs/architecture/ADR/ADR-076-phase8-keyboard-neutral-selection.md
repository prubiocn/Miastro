# ADR-076 — Estado neutral y Escape global en la pantalla natal

## Estado

Aceptado para Fase 8.

## Contexto

Fase 7 permitía Escape cuando el foco estaba sobre la rueda.

Fase 8 añade cinco paneles navegables y una selección dual, por lo que Escape debe producir el mismo estado neutral independientemente del control con foco.

## Decisión

Definir `MainWindowViewModel.ClearNatalSelection` como único endpoint de limpieza.

El endpoint reutiliza `ApplyNatalWheelSelection(null)`, que ya sincroniza los paneles y vuelve a renderizar la selección neutral.

`MainWindow` recibe `KeyDown` y únicamente intercepta Escape.

No se interceptan Tab ni las teclas de navegación propias de controles estándar.

El handler específico de la rueda conserva flechas, Home y End, y delega Escape en el mismo endpoint.

`ClearNatalWheelSelection` se conserva como compatibilidad de Fase 7.

## Consecuencias

Rueda y paneles no pueden quedar con estados de selección divergentes tras Escape.

No aparece un segundo reducer ni una segunda ruta de limpieza.

La navegación estándar de Avalonia permanece intacta.

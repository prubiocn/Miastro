# Fase 8 — ViewModels de paneles natales

## Estructura

La UI natal utiliza ViewModels separados:

- `NatalDataPanelViewModel`;
- `NatalPositionsPanelViewModel`;
- `NatalAspectsPanelViewModel`;
- `NatalDistributionPanelViewModel`;
- `NatalSummaryPanelViewModel`.

`NatalPanelHostViewModel` únicamente coordina las cinco vistas y la pestaña activa.

## Pestaña inicial

La pestaña inicial es `Positions`.

No se depende de la posición visual del control para definir este comportamiento.

## Frontera

Los ViewModels consumen modelos de lectura de `Miastro.Application.Natal.Reading`.

No contienen:

- cálculo astronómico;
- detección de aspectos;
- reglas de regencia;
- reglas de distribución;
- persistencia.

## Datos

`NatalDataPanelViewModel` expone filas factuales.

La resolución visual del glifo se realizará en la capa de presentación a partir de `ObjectId`, sin mover el catálogo gráfico a Application.

## Posiciones

`NatalPositionsPanelViewModel` conserva la distinción `IsAngle` para ASC y MC.

## Aspectos

`NatalAspectsPanelViewModel` expone la matriz triangular real ya construida por Application.

No reconstruye parejas ni recalcula aspectos.

## Distribución

`NatalDistributionPanelViewModel` expone:

- distribución zodiacal;
- distribución por casas;
- síntesis textual factual.

No utiliza gráficos de barras.

## Resumen

`NatalSummaryPanelViewModel` expone directamente las líneas compactas del Resumen factual.

## Selección

La sincronización simple/dual se incorporará posteriormente mediante el contrato `NatalSelectionState`.

No se duplica esa lógica dentro de cada panel.

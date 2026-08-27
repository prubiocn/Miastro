# Fase 8 — Integración de paneles en MainWindowViewModel

## Punto único de construcción

La construcción de los cinco paneles se realiza al aplicar un snapshot mediante `ApplyNatalSnapshot`.

Ese método ya es el punto común utilizado tanto al cargar una carta natal vigente como después de un cálculo correcto.

Por tanto, no se duplican flujos de construcción de paneles.

## Host

`MainWindowViewModel` expone únicamente:

- `NatalPanels`;
- `HasNatalPanels`.

La estructura interna de cada pestaña permanece encapsulada por `NatalPanelHostViewModel`.

## Ciclo de vida

Al aplicar un snapshot:

1. se establece el snapshot vigente;
2. se construye `NatalPanelHostViewModel`;
3. se mantienen los mecanismos existentes de posiciones y rueda.

Al resetear la carta:

1. se elimina el snapshot vigente;
2. se elimina el host de paneles;
3. se limpian los modelos de presentación existentes;
4. se limpia la rueda.

## Pestaña inicial

Cada host nuevo comienza en `Positions`.

Esto se define en `NatalPanelHostViewModel`, no en XAML.

## Frontera

`MainWindowViewModel.NatalPanels.cs` no:

- calcula astronomía;
- detecta aspectos;
- resuelve casas;
- infiere movimiento;
- calcula regencias;
- calcula distribución.

Solo adapta el ciclo de vida del snapshot al host de presentación.

## Compatibilidad Fase 7

Los mecanismos ya existentes de:

- rueda;
- `NatalPlacements`;
- `NatalAspects`;
- hit testing;
- tooltip;
- selección simple;

no se eliminan en este bloque.

La migración del XAML se hará progresivamente.

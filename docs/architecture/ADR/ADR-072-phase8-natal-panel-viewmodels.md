# ADR-072 — ViewModels separados para paneles natales

## Estado

Aceptado para Fase 8.

## Contexto

La pantalla natal necesita cinco paneles funcionales sin convertir `MainWindowViewModel` en un ViewModel monolítico.

## Decisión

Crear cinco ViewModels pequeños, uno por panel:

- Datos;
- Posiciones;
- Aspectos;
- Distribución;
- Resumen.

Crear además un `NatalPanelHostViewModel` responsable únicamente de:

- contener los cinco paneles;
- mantener la pestaña activa;
- establecer Posiciones como pestaña inicial.

La lógica factual permanece en `Miastro.Application.Natal.Reading`.

La UI no recalcula astronomía, aspectos, casas, regencias ni distribución.

## Consecuencias

Cada panel puede evolucionar y probarse de forma independiente.

La integración posterior con XAML se limita a binding y comportamiento de selección.

Los modelos headless continúan reutilizables por informes futuros.

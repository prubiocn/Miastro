# Rendimiento básico — Swiss Ephemeris

## Alcance

Medición orientativa de Fase 3.

No constituye un benchmark contractual ni un objetivo de optimización.

Entorno:

- plataforma: Linux
- arquitectura: `X64`
- Swiss Ephemeris: `2.10.03`
- configuración: Release
- ejecución: headless

## Resultados orientativos

| Operación | Tiempo |
|---|---:|
| Inicialización y diagnóstico | 49,005 ms |
| Primer cálculo con validación de efemérides | 6,117 ms |
| Cálculo secuencial de 10 planetas | 25,710 ms |
| Casas Placidus | 4,293 ms |

## Interpretación

Estas cifras sirven únicamente como línea base técnica.

La Fase 3 prioriza:

- corrección;
- integridad;
- reproducibilidad;
- encapsulación;
- seguridad de carga nativa.

No se aplica optimización prematura.

El acceso a Swiss Ephemeris continúa serializado por política de
thread safety.

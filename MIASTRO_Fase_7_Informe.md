# MIASTRO — Informe de Fase 7

## Estado

**EN IMPLEMENTACIÓN — NO CERRADA**

Baseline oficial:

- Fase 6: CERRADA;
- aceptación Fase 6: 83 PASS / 0 FAIL / 0 PENDING;
- tests heredados al inicio de Fase 7: 345;
- Fase 8: NO iniciada.

## Objetivo

Implementar la primera rueda natal geométrica funcional de Miastro
a partir exclusivamente de snapshots natales persistidos de Fase 6.

La prioridad de esta fase es:

> legibilidad, determinismo y fidelidad astrológica sin solapamientos.

## Principios obligatorios

- no recalcular astronomía en Graphics ni en UI;
- separar posición astrológica real de posición gráfica;
- scene graph propio e independiente de Avalonia;
- layout separado del render;
- SkiaSharp como backend;
- misma carta + misma configuración = mismo layout;
- ASC orientado a la izquierda;
- ningún glifo visible puede solaparse con otro;
- la rueda no es el único acceso a la información;
- no implementar interpretación ni informes.

## Capas previstas

1. Background
2. ZodiacRing
3. DegreeRing
4. HouseLayer
5. AngleLayer
6. BodyLayer
7. PointLayer
8. AspectLayer
9. LabelLayer
10. InteractionOverlay

## Bloques de implementación

### 7A — Núcleo geométrico
- coordenadas canónicas;
- orientación por ASC;
- primitivas geométricas;
- scene graph;
- capas;
- snapshot de layout inspeccionable.

### 7B — Layout de cuerpos
- posición real vs visual;
- bounding boxes;
- niveles radiales;
- anti-solapamiento;
- orden zodiacal;
- leader lines.

### 7C — Renderer Skia
- render headless;
- clipping;
- escalado;
- HiDPI;
- PNG técnico;
- catálogo vectorial de glifos.

### 7D — Rueda completa
- zodiaco;
- grados;
- casas;
- ASC/MC/IC/DSC;
- cuerpos;
- puntos;
- aspectos;
- etiquetas;
- retrogradación.

### 7E — Interacción y UI
- Modo Consulta;
- Modo Presentación;
- panel Datos;
- panel Posiciones;
- panel Aspectos;
- hit testing;
- selección bidireccional;
- visibilidad.

### 7F — Validación visual
- corpus sintético;
- stelliums;
- casas extremas;
- determinismo;
- escalado;
- golden images;
- rendimiento.

### 7G — Operación y cierre
- publish;
- Debian Fase 7;
- instalación real;
- smoke gráfico;
- CI remoto;
- informe final.

## Exclusiones deliberadas

No implementar en esta fase:

- interpretación textual;
- informes astrológicos;
- Revolución Solar;
- Revolución Lunar;
- tránsitos;
- progresiones;
- sinastría;
- exportación artística avanzada;
- impresión final;
- estilos artísticos múltiples.

## Criterios de aceptación

- PASS: 0
- FAIL: 0
- PENDING: 88

## Estado de fase siguiente

Fase 8 no iniciada.

## Bloque 7A1 — Geometría canónica y Scene Graph

Implementado:

- sistema canónico de coordenadas independiente del renderer
- normalización angular determinista
- transformación longitud eclíptica a ángulo gráfico
- ASC fijado a 180 grados gráficos
- avance zodiacal visual antihorario
- primitivas ChartPoint y ChartRect
- Scene Graph base independiente de Avalonia y Skia
- orden explícito de capas
- nodos Circle, Arc, Line, Glyph, Text, Path y Group
- escena natal inspeccionable y con orden estable
- ADR-041 sobre Scene Graph
- ADR-042 sobre orientación
- documentación de coordenadas
- tests geométricos y de arquitectura iniciales

Este bloque no introduce renderer Skia ni UI de rueda.

La posición astrológica real permanece separada de cualquier
posición gráfica futura.

## Bloque 7A2 — Snapshot geométrico de layout

Implementado:

- NatalWheelMetrics escalable
- tamaño mínimo geométrico
- NatalWheelLayoutSnapshot inspeccionable
- builder determinista
- 12 sectores zodiacales exactos
- 360 marcas de grado
- jerarquía de marcas 1/5/10 grados
- 12 cúspides desde longitudes reales
- centros geométricos de casas
- soporte de wrap 0/360
- ASC
- DSC derivado
- MC
- IC derivado
- posiciones candidatas para números de casa
- representación diagnóstica estable
- ADR-043 sobre snapshot determinista
- tests de escala y determinismo

No se recalcula astronomía.

No se asumen casas iguales.

No se modifica ninguna longitud astrológica real.

## Bloque 7B1 — Posición real y visual

Implementado:

- separación formal entre longitud real y posición visual
- RealAnchor exacto e inmutable
- VisualCenter independiente
- bounding boxes protegidos
- política geométrica centralizada
- niveles radiales deterministas
- anti-solapamiento inicial
- conservación de orden zodiacal
- independencia del orden recibido
- leader lines por umbral geométrico
- corpus de 2, 3, 5 y 9 objetos próximos
- caso de colisión alrededor de 0/360 grados
- ADR-044 posición real vs visual
- ADR-045 anti-solapamiento determinista
- ADR-046 leader lines

En 7B1 no se aplica offset angular.

El ángulo visual permanece igual al ángulo real y únicamente cambia
el nivel radial cuando es necesario.

## Bloque 7B2 — Construcción de Scene Graph natal

Implementado:

- NatalWheelSceneBuilder independiente del renderer
- disco base
- anillo zodiacal exterior e interior
- 12 sectores zodiacales
- 12 placeholders semánticos de glifos zodiacales
- 360 marcas de grado
- 12 cúspides
- 12 números de casa
- ASC
- DSC
- MC
- IC
- etiquetas de ejes
- marcas exactas de posición real
- glifos en posición visual
- leader lines
- diferenciación BodyLayer / PointLayer
- orden de pintura estable
- tests de composición y determinismo

El Scene Builder no contiene astronomía, persistencia, Avalonia ni
código específico de Skia.

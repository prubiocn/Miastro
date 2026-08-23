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

## Bloque 7C1 — Renderer Skia headless

Implementado:

- SkiaSharp en Miastro.Graphics.Skia
- assets nativos Linux
- renderer independiente de Avalonia
- superficie headless
- clipping explícito
- escalado de escena
- CircleNode
- ArcNode
- LineNode
- PathNode
- GroupNode
- GlyphNode con placeholder vectorial técnico
- TextNode con placeholder geométrico técnico
- PNG técnico en memoria
- escritura de PNG técnico a disco
- test de dimensiones
- test de determinismo binario
- test de separación Graphics / Skia
- ADR-047

El catálogo vectorial astrológico definitivo y las fuentes internas
quedan para los siguientes bloques de Fase 7.

## Bloque 7C2 — Catálogo vectorial astrológico

Implementado:

- catálogo vectorial interno en Miastro.Graphics
- sistema normalizado independiente del renderer
- 12 signos zodiacales
- 10 planetas
- Nodo Norte verdadero
- Nodo Sur
- Lilith media
- Parte de Fortuna
- Quirón
- Ceres
- Pallas
- Juno
- Vesta
- ASC
- MC
- glifos de aspectos mayores
- renderer Skia consume el catálogo
- eliminado el placeholder técnico de GlyphNode
- fallback vectorial para claves desconocidas
- ningún requisito de fuente astrológica del sistema
- ADR-048

La tipografía ordinaria de etiquetas sigue siendo una responsabilidad
separada del catálogo astrológico.

## Bloque 7C3 — Estilos semánticos

Implementado:

- SceneColor independiente del renderer
- SceneStyle
- StyleKey en SceneNode
- catálogo semántico centralizado
- paleta clara marfil/carbón/azul-gris/arena
- jerarquía visual 1/5/10 grados
- ASC y MC como ejes principales
- DSC e IC como ejes secundarios
- diferenciación de aspectos no basada solo en color
- estilos de cuerpos y puntos
- estilos de marca real
- estilos de leader line
- traducción de patrón sólido/discontinuo al backend Skia
- ADR-049
- auditoría de fuentes empaquetadas

No se incorporan todavía fuentes externas del sistema.

La tipografía empaquetada se tratará separadamente.

## Bloque 7C4 — Tipografía empaquetada

Implementado:

- Source Sans 3 Regular empaquetada
- licencia SIL OFL 1.1 conservada
- hash SHA-256 de fuente y licencia
- fuente cargada desde EmbeddedResource
- ausencia de lookup de fuentes del sistema
- TextNode renderiza texto real
- soporte de caracteres españoles
- soporte del símbolo de grado
- render tipográfico headless
- determinismo tipográfico
- ADR-050

Fuente SHA-256:

4644c81b86ec9caaa76b634889968ed3c4f4f52f054855933acc7c2b21e53b0f

Licencia SHA-256:

56af9b9c6715597e458284a474dc118a50a4150e9d547c70f7b4a33c3e6a9328

## Bloque 7D1 — Aspectos persistidos en Scene Graph

Implementado:

- contratos gráficos de aspectos
- entrada sin reglas de cálculo
- clasificación visual Major/Secondary explícita
- geometría sobre posición real
- independencia de VisualCenter
- AspectRadius interior
- ocultación funcional de AspectLayer
- ocultación sin relayout
- aspectos de objetos no visibles omitidos
- orden determinista
- estilos major/secundarios
- ADR-051

Miastro.Graphics no calcula aspectos ni orbes.

## Bloque 7D2 — Visibilidad y modos visuales

Implementado:

- modo Consultation
- modo Presentation
- ShowPlanets
- ShowPoints
- ShowAspects
- ShowCusps
- ShowLabels
- filtrado visual sin recalcular
- filtrado de aspectos por extremos visibles
- política responsive Full/Compact/Minimal
- degradación de marcas de grado
- reducción de etiquetas secundarias
- ASC y MC preservados en tamaño mínimo
- placements inmutables ante cambios visuales
- configuración determinista
- ADR-052

No se persisten posiciones gráficas absolutas.

## Bloque 7E1A — Hit testing geométrico

Implementado:

- hit testing independiente de Avalonia
- BodyLayer seleccionable
- PointLayer seleccionable
- selección sobre Bounds visuales reales
- soporte de glifos desplazados
- tolerancia opcional de puntero
- glifos zodiacales excluidos
- prioridad determinista
- ADR-053

El adaptador al read model natal persistido se implementará contra los
contratos reales existentes después del preflight de Fase 6.

## Bloque 7E1B — Adaptador del snapshot natal persistido

Implementado:

- consumo directo de NatalChartSnapshotReadModel
- Graphics -> Application limitado a read models
- 12 cúspides persistidas
- ASC persistido
- MC persistido
- longitudes persistidas
- 10 planetas visibles por defecto
- Quirón visible por defecto
- ASC y MC visibles por defecto
- puntos opcionales
- aspectos persistidos
- clasificación visual de AspectKind
- ninguna detección de aspectos
- ninguna lectura de EF/SQLite
- ningún recálculo astronómico
- salida determinista para Scene Graph
- ADR-054

## Bloque 7E2B — Primera integración Avalonia

Implementado:

- UI referencia Graphics y Graphics.Skia
- snapshot vigente reutilizado
- Scene Graph generado desde read model persistido
- render Skia en memoria
- visualización mediante Avalonia Bitmap/Image
- rueda visible al cargar carta vigente
- rueda actualizada después del cálculo natal
- limpieza al cambiar/resetear persona
- pointer hit testing sobre geometría visual
- transformación viewport -> Scene Graph fuera de Avalonia
- selección inicial de cuerpos y puntos
- accesibilidad nominal de la rueda
- ningún archivo gráfico temporal
- ADR-055

La UI no contiene cálculo central de geometría.

### 7E2B FIX1 — actualización de guard heredado de Fase 6

El test histórico de Fase 6 que prohibía cualquier rueda natal en la
UI fue actualizado al activarse legítimamente la funcionalidad en
Fase 7.

Se conserva la restricción arquitectónica relevante:

- la UI no puede depender directamente de Swiss Ephemeris

Se elimina únicamente la prohibición temporal de rueda natal que
pertenecía al alcance de Fase 6.

## Bloque 7E3 — Selección y controles visuales

Implementado y validado:

- selector Consulta/Presentación
- control Planetas
- control Puntos
- control Aspectos
- control Cúspides
- control Etiquetas
- reconstrucción visual desde el mismo snapshot
- panel de objeto seleccionado
- posición zodiacal
- casa
- movimiento
- selección mediante hit testing de Graphics
- rueda situada en la sección natal correcta
- rueda fuera de listas de localización
- geometría central ausente de Avalonia
- ningún recálculo astronómico por preferencias
- ADR-056

Validación 7E3:

- tests específicos: 6/6
- tests Fase 7: 112/112
- regresión global: 457/457

## Bloque 7F1 — Endurecimiento geométrico responsive

Implementado:

- canvas físico como frontera geométrica
- eliminación del floor artificial de MinimumUsableSize
- validación 300/360/480/720/800
- validación de canvases rectangulares
- círculo exterior contenido
- radios concéntricos ordenados
- escala monotónica
- centro físico correcto
- determinismo de métricas
- separación geometría/degradación visual
- ADR-057

## Bloque 7F2 — Límites seguros del anti-solapamiento

Implementado:

- anillo radial seguro para cuerpos y puntos
- footprint protegido incluido en los límites
- descarte de niveles radiales fuera del anillo
- desplazamiento angular determinista cuando es necesario
- AngularOffsetDegrees firmado
- preservación del orden zodiacal
- longitudes reales inmutables
- leader lines conservadas
- stelliums extremos
- clusters ASC/MC
- canvas pequeño
- independencia del orden de entrada
- ADR-058

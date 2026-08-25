# Miastro — Evidencia técnica de aceptación de Fase 7

## Ventana central

La rueda natal forma parte de MainWindow.

Datos, Posiciones, Aspectos, rueda y panel de selección pertenecen a la
misma pantalla central.

No se crea una ventana específica para la rueda natal.

## Privacidad de logs gráficos

Miastro.Graphics y Miastro.Graphics.Skia no registran datos natales,
identificadores de persona, coordenadas, posiciones ni aspectos.

La UI tampoco incorpora esos datos a llamadas de logging de Fase 7.

## Límites de arquitectura

Miastro.Graphics:

- no depende de Avalonia
- no depende de SkiaSharp
- no depende de Swiss Ephemeris
- no depende de Astronomy

Miastro.Graphics.Skia:

- consume el Scene Graph
- no calcula astronomía
- no depende de Swiss Ephemeris

Miastro.UI.Avalonia:

- presenta la escena
- controla viewport, DPI, foco y eventos
- no contiene la geometría central de la rueda

## Evidencia visual

Los goldens técnicos requieren además inspección humana.

Casos:

- simple
- stellium
- many-aspects
- placidus
- koch

La inspección visual no se sustituye por hashes ni por invariantes
geométricas.

<!-- PHASE7-FINAL-ACCEPTANCE-START -->

# Aceptación final — Fase 7

Estado formal: **CERRADA**

- PASS: **88**
- FAIL: **0**
- PENDING: **0**

## Evidencia de cierre

- Commit de implementación validado: `d1a0fb4aa87bda514f7d60e9710f8381539fa581`.
- GitHub Actions run: `32868872498`.
- Job remoto: `97870799842`.
- CI remota: `success`.
- Tests locales Fase 7: `215/215 PASS`.
- Regresión global local: `559/559 PASS`.
- Build Release: `0 warnings / 0 errors`.
- Publish linux-x64 self-contained: PASS.
- Debian `0.7.0~phase7-1`: PASS.
- Instalación real: PASS.
- Smoke gráfico instalado bajo Xvfb: PASS.
- SkiaSharp managed/native alineados en `4.151.1`.
- Goldens oficiales: PASS.
- SourceSans3 validada como recurso embebido.

## Enmiendas funcionales aprobadas durante Fase 7

Los criterios 33 y 35 se evalúan contra la especificación final aprobada durante la fase:

- Criterio 33: la posición astronómica real se conserva en el modelo pero no se dibuja una marca visible de posición real.
- Criterio 35: no se imprimen grados/posiciones planetarias sobre la rueda; esa información se expone mediante tooltip y paneles de lectura.
- El movimiento directo/retrógrado permanece disponible en el modelo de lectura y tooltip/panel.

## Matriz 1–88

| Nº | Criterio | Estado |
|---:|---|:---:|
| 1 | Rueda natal funcional existente | **PASS** |
| 2 | Fuente de datos: snapshot natal persistido | **PASS** |
| 3 | El renderer no recalcula astronomía | **PASS** |
| 4 | Scene Graph propio | **PASS** |
| 5 | Layout separado de render | **PASS** |
| 6 | Render SkiaSharp operativo | **PASS** |
| 7 | Orientación canónica respecto al ASC | **PASS** |
| 8 | ASC situado a la izquierda | **PASS** |
| 9 | Doce signos zodiacales | **PASS** |
| 10 | Anillo de grados | **PASS** |
| 11 | Doce casas reales | **PASS** |
| 12 | ASC visible | **PASS** |
| 13 | MC visible | **PASS** |
| 14 | IC/DSC derivados y coherentes | **PASS** |
| 15 | Sol | **PASS** |
| 16 | Luna | **PASS** |
| 17 | Mercurio | **PASS** |
| 18 | Venus | **PASS** |
| 19 | Marte | **PASS** |
| 20 | Júpiter | **PASS** |
| 21 | Saturno | **PASS** |
| 22 | Urano | **PASS** |
| 23 | Neptuno | **PASS** |
| 24 | Plutón | **PASS** |
| 25 | Quirón por defecto | **PASS** |
| 26 | Puntos opcionales | **PASS** |
| 27 | Posición real preservada | **PASS** |
| 28 | Posición gráfica separada de la real | **PASS** |
| 29 | Sin solapamientos visibles de glifos | **PASS** |
| 30 | Gestión determinista de stelliums | **PASS** |
| 31 | Orden zodiacal preservado | **PASS** |
| 32 | Leader lines cuando son necesarias | **PASS** |
| 33 | Posición real preservada sin marca visual, según especificación final aprobada | **PASS** |
| 34 | Movimiento retrógrado/directo disponible en modelo de lectura | **PASS** |
| 35 | Datos de posición disponibles por tooltip/panel, sin etiquetas impresas sobre la rueda según especificación final | **PASS** |
| 36 | Aspectos persistidos representados | **PASS** |
| 37 | Aspectos secundarios distinguibles | **PASS** |
| 38 | Aspectos ocultables | **PASS** |
| 39 | Cúspides ocultables | **PASS** |
| 40 | Puntos ocultables | **PASS** |
| 41 | Etiquetas ocultables | **PASS** |
| 42 | Cambios visuales no recalculan astronomía | **PASS** |
| 43 | Modo Consulta | **PASS** |
| 44 | Modo Presentación | **PASS** |
| 45 | Diseño claro y ligero | **PASS** |
| 46 | La rueda es el elemento protagonista | **PASS** |
| 47 | Panel Datos | **PASS** |
| 48 | Panel Posiciones | **PASS** |
| 49 | Panel Aspectos | **PASS** |
| 50 | Selección desde la rueda | **PASS** |
| 51 | Selección desde panel | **PASS** |
| 52 | Sincronización bidireccional | **PASS** |
| 53 | Hit testing sobre geometría visual real | **PASS** |
| 54 | Tooltip implementado | **PASS** |
| 55 | Escalado responsivo | **PASS** |
| 56 | HiDPI | **PASS** |
| 57 | Tamaño mínimo usable | **PASS** |
| 58 | Render headless | **PASS** |
| 59 | Validación PNG | **PASS** |
| 60 | Goldens controlados | **PASS** |
| 61 | Pruebas anti-solapamiento | **PASS** |
| 62 | Pruebas geométricas | **PASS** |
| 63 | Determinismo | **PASS** |
| 64 | Pruebas de escala | **PASS** |
| 65 | Casos extremos de casas | **PASS** |
| 66 | Placidus | **PASS** |
| 67 | Koch | **PASS** |
| 68 | Panel accesible equivalente | **PASS** |
| 69 | Navegación por teclado en panel | **PASS** |
| 70 | Sin texto interpretativo | **PASS** |
| 71 | Sin informes en Fase 7 | **PASS** |
| 72 | Sin revolución solar | **PASS** |
| 73 | Sin tránsitos | **PASS** |
| 74 | Preferencias visuales | **PASS** |
| 75 | No se persisten posiciones gráficas absolutas | **PASS** |
| 76 | Renderer reutilizable/exportable | **PASS** |
| 77 | UI central de persona | **PASS** |
| 78 | Invalidación evita mostrar cálculo obsoleto | **PASS** |
| 79 | Logs sin datos privados | **PASS** |
| 80 | Pruebas de arquitectura | **PASS** |
| 81 | Regresión heredada | **PASS** |
| 82 | Build Release | **PASS** |
| 83 | Publish linux-x64 self-contained | **PASS** |
| 84 | Paquete Debian | **PASS** |
| 85 | Instalación real | **PASS** |
| 86 | Smoke gráfico instalado | **PASS** |
| 87 | CI remota completa | **PASS** |
| 88 | Fase 8 no iniciada | **PASS** |

Resultado: **88 PASS / 0 FAIL / 0 PENDING**.

La Fase 8 no se ha iniciado.

<!-- PHASE7-FINAL-ACCEPTANCE-END -->

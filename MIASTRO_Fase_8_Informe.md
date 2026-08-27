# MIASTRO — Informe técnico Fase 8

## Estado

**Fase 8: CERRADA.**

Resultado final:

- PASS: **85**
- FAIL: **0**
- PENDING: **0**

La Fase 9 no forma parte de este cierre y no ha sido iniciada.

---

## Objetivo completado

La Fase 8 completa la experiencia de consulta de carta natal alrededor de la rueda existente mediante cinco paneles funcionales:

- Datos
- Posiciones
- Aspectos
- Distribución
- Resumen

La rueda natal continúa siendo el elemento visual protagonista.

La solución permanece factual y técnica. No introduce interpretación psicológica extensa, informes finales, Revolución Solar, tránsitos, progresiones ni sinastría.

---

## Arquitectura implementada

La lectura natal se separa de la presentación mediante una capa headless bajo:

`src/Miastro.Application/Natal/Reading/`

Esta capa consume exclusivamente datos ya persistidos en el snapshot natal y catálogos de dominio.

Incluye, entre otros:

- lectura factual de posiciones;
- regencias de signo;
- regencia de casa desde el signo real de la cúspide;
- perfiles explícitos de distribución;
- distribución por elementos, modalidades y polaridad;
- distribución por hemisferios y cuadrantes;
- clasificación angular, sucedente y cadente;
- síntesis factual;
- resumen natal factual;
- matriz triangular de aspectos;
- estado de selección simple y dual.

La UI no recalcula posiciones ni aspectos astronómicos.

---

## Panel Datos

El panel Datos presenta de forma compacta:

- glifo;
- nombre;
- grado;
- signo;
- regente.

Se preservan las regencias dobles consolidadas de Escorpio, Acuario y Piscis.

El orden de objetos es determinista.

---

## Panel Posiciones

El panel Posiciones es la pestaña inicial.

Cada fila contiene información compacta y expandible:

- glifo;
- objeto;
- posición exacta;
- signo;
- casa;
- movimiento persistido;
- regente del signo;
- signo de la cúspide de casa;
- regente o regentes de la casa.

ASC y MC se presentan como ángulos y no como planetas.

Los puntos adicionales persistidos pueden presentarse aunque no formen parte visible de la rueda.

---

## Panel Aspectos

Se implementó una matriz triangular real de aspectos.

Propiedades:

- participantes según el perfil Miastro V1;
- ausencia de pares A-B / B-A duplicados;
- datos exclusivamente persistidos;
- sin recálculo de aspectos en UI;
- nombre y símbolo del aspecto;
- separación y orbe;
- nombre accesible;
- navegación por teclado;
- selección dual;
- sincronización con la rueda;
- realce del aspecto seleccionado;
- atenuación del resto en selección dual.

La matriz conserva una lista compacta accesible auxiliar.

---

## Panel Distribución

La distribución se presenta de forma textual y estructurada, sin barras.

Incluye:

- elementos;
- modalidades;
- polaridad;
- hemisferios Este/Oeste;
- hemisferios superior/inferior;
- cuadrantes;
- casas angulares, sucedentes y cadentes.

El perfil V1 utiliza una política explícita de objetos.

La síntesis permanece factual y no introduce interpretación psicológica.

---

## Panel Resumen

El resumen incluye de forma breve:

- Sol por signo y casa;
- Luna por signo y casa;
- ASC;
- MC;
- elemento predominante;
- modalidad predominante;
- principales aspectos;
- concentración por casas;
- retrogradaciones relevantes persistidas.

No se generan afirmaciones de personalidad, destino, misión o interpretación extensa.

---

## Sincronización de selección

Se implementó sincronización bidireccional entre:

- rueda;
- Datos;
- Posiciones;
- Aspectos.

La selección de un aspecto produce selección dual de objetos.

Escape y el clic sobre fondo neutralizan el estado de selección.

La escena gráfica base no se modifica; los estados visuales se aplican mediante overlays de interacción.

---

## Accesibilidad y responsive

La matriz de aspectos es navegable mediante controles accesibles.

El aspecto no se comunica exclusivamente por color.

El panel lateral dispone de comportamiento adaptativo y colapsable.

La matriz y las posiciones mantienen scroll cuando el ancho disponible es reducido.

Se corrigió durante la validación final un conflicto de namescope de Avalonia causado por dos controles denominados `NatalPositionsList`.

El bloque Fase 8 conserva `NatalPositionsList` y la lista heredada utiliza `NatalPositionsLegacyList`.

Existe una prueba específica de regresión que impide nuevos atributos Name duplicados en MainWindow.

---

## Rendimiento

Se midieron explícitamente:

- construcción de Datos;
- construcción de Posiciones;
- construcción de Aspectos;
- construcción de Distribución;
- construcción de Resumen;
- construcción del host completo;
- selección simple;
- selección dual;
- limpieza de selección.

Las mediciones se encuentran documentadas en:

`docs/natal-panels/performance-and-background-clear.md`

---

## Validación de código

Validación local final de implementación:

- build Release: PASS;
- Fase 8: **210/210 PASS**;
- regresión Fase 7: **214/214 PASS**;
- regresión global: **769/769 PASS**;
- warnings de build: 0;
- errores de build: 0.

---

## Publish y distribución Linux

Publish final validado:

- runtime: `linux-x64`;
- self-contained;
- ejecutable Avalonia;
- Swiss Ephemeris privada;
- efemérides;
- catálogo GeoNames offline.

Paquete Debian nominal:

`miastro_0.8.0~phase8-1_amd64.deb`

Versión instalada:

`0.8.0~phase8-1`

Se verificó:

- instalación real;
- Swiss Ephemeris instalada;
- ABI nativa;
- integridad de efemérides;
- catálogo GeoNames;
- apertura read-only;
- búsqueda geográfica;
- preservación de datos XDG;
- smoke de la aplicación instalada.

El smoke instalado permaneció vivo durante la ventana de validación sin errores fatales.

---

## Persistencia de datos de usuario

La reinstalación del paquete no modificó los datos XDG existentes.

Áreas verificadas:

- `~/.local/share/miastro/`
- `~/.config/miastro/`
- `~/.cache/miastro/`
- `~/.local/state/miastro/`

No se introducen datos runtime dentro del repositorio.

---

## Evidencia Git

Commit de implementación Fase 8:

`07ee8ade1febfd695ca142c6aea9455138402141`

Rama:

`main`

El commit fue empujado al remoto y el SHA remoto coincidió exactamente.

---

## Evidencia CI remota

Workflow:

`Miastro CI`

Run:

`33096923663`

Job:

`98604146136`

Commit validado:

`07ee8ade1febfd695ca142c6aea9455138402141`

Resultado:

**SUCCESS**

La ejecución remota completó correctamente:

- checkout;
- .NET 10;
- dependencias nativas Avalonia;
- restore;
- fixture GeoNames;
- build;
- tests;
- recursos Swiss;
- publish linux-x64 self-contained;
- verificación del publish;
- construcción Debian;
- instalación Debian;
- verificación GeoNames instalada;
- verificación Swiss instalada;
- preservación XDG;
- smoke de aplicación instalada.

La advertencia de GitHub Actions sobre la transición de acciones basadas en Node.js 20 a Node.js 24 es informativa y no bloqueó el workflow.

---

## ADRs

La Fase 8 añade ADR-065 a ADR-082, cubriendo:

- hechos natales y regencias;
- matriz triangular;
- selección dual;
- DistributionProfile;
- geometría de casas;
- síntesis factual;
- resumen factual;
- ViewModels de panel;
- sincronización simple;
- selección dual de aspectos;
- overlay gráfico dual;
- estado neutral por teclado;
- accesibilidad;
- layout triangular;
- presentación Datos/Posiciones;
- UI Distribución/Resumen;
- atenuación de selección dual;
- panel responsive.

---

## Matriz final de aceptación

| # | Criterio | Estado |
|---:|---|---|
| 1 | Datos functional | **PASS** |
| 2 | Posiciones | **PASS** |
| 3 | Aspectos | **PASS** |
| 4 | Distribución | **PASS** |
| 5 | Resumen | **PASS** |
| 6 | Posiciones default tab | **PASS** |
| 7 | Datos glyph | **PASS** |
| 8 | name | **PASS** |
| 9 | degree | **PASS** |
| 10 | sign | **PASS** |
| 11 | ruler | **PASS** |
| 12 | double rulers | **PASS** |
| 13 | exact pos | **PASS** |
| 14 | house | **PASS** |
| 15 | motion | **PASS** |
| 16 | sign ruler | **PASS** |
| 17 | house ruler | **PASS** |
| 18 | ASC | **PASS** |
| 19 | MC | **PASS** |
| 20 | additional points | **PASS** |
| 21 | triangular matrix | **PASS** |
| 22 | no duplicate | **PASS** |
| 23 | V1 participants | **PASS** |
| 24 | correct aspect | **PASS** |
| 25 | correct absence | **PASS** |
| 26 | aspect tooltip | **PASS** |
| 27 | aspect selection | **PASS** |
| 28 | dual selection | **PASS** |
| 29 | matrix accessible | **PASS** |
| 30 | keyboard matrix | **PASS** |
| 31 | not color only | **PASS** |
| 32 | elements | **PASS** |
| 33 | modalities | **PASS** |
| 34 | polarity | **PASS** |
| 35 | hemispheres | **PASS** |
| 36 | quadrants | **PASS** |
| 37 | angular/succedent/cadent if included | **PASS** |
| 38 | distribution object policy defined | **PASS** |
| 39 | no bars | **PASS** |
| 40 | factual synthesis | **PASS** |
| 41 | Summary Sun | **PASS** |
| 42 | Moon | **PASS** |
| 43 | ASC | **PASS** |
| 44 | MC | **PASS** |
| 45 | predominance | **PASS** |
| 46 | main aspects | **PASS** |
| 47 | brief | **PASS** |
| 48 | no personality | **PASS** |
| 49 | Data selection→wheel | **PASS** |
| 50 | Positions selection→wheel | **PASS** |
| 51 | Aspects selection→wheel | **PASS** |
| 52 | wheel→panels | **PASS** |
| 53 | Escape clears | **PASS** |
| 54 | neutral UI | **PASS** |
| 55 | responsive panel | **PASS** |
| 56 | collapsible if appropriate | **PASS** |
| 57 | matrix narrow usable | **PASS** |
| 58 | tables legible | **PASS** |
| 59 | wheel protagonist | **PASS** |
| 60 | headless logic | **PASS** |
| 61 | no Swiss recalc | **PASS** |
| 62 | no snapshot mutation | **PASS** |
| 63 | deterministic order | **PASS** |
| 64 | Data tests | **PASS** |
| 65 | Positions tests | **PASS** |
| 66 | matrix tests | **PASS** |
| 67 | distribution tests | **PASS** |
| 68 | Summary tests | **PASS** |
| 69 | sync tests | **PASS** |
| 70 | accessibility tests | **PASS** |
| 71 | layout tests | **PASS** |
| 72 | architecture tests | **PASS** |
| 73 | inherited tests | **PASS** |
| 74 | Phase7 regression | **PASS** |
| 75 | build | **PASS** |
| 76 | publish | **PASS** |
| 77 | deb | **PASS** |
| 78 | real install | **PASS** |
| 79 | installed smoke | **PASS** |
| 80 | remote CI | **PASS** |
| 81 | no extensive interpretation | **PASS** |
| 82 | no reports | **PASS** |
| 83 | no Solar Return | **PASS** |
| 84 | no transits | **PASS** |
| 85 | Phase9 not started | **PASS** |

---

## Resultado final

**PASS: 85**

**FAIL: 0**

**PENDING: 0**

**FASE 8 CERRADA.**

La Fase 9 queda expresamente fuera de alcance de este cierre y no ha sido iniciada.

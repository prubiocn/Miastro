# MIASTRO — Informe técnico final Fase 3

## Estado

**FASE 3 CERRADA**

Integración real de Swiss Ephemeris en Linux.

| Estado | Total |
|---|---:|
| PASS | 57 |
| FAIL | 0 |
| PENDING | 0 |

Resultado de aceptación:

**57/57 PASS**

No se ha iniciado la Fase 4.

---

## Evidencia de cierre

Commit de implementación validado:

`99e8de0cd595c78a15cf5f61d2824f20ec9cc2e0`

GitHub Actions:

- workflow: Miastro CI
- run: `32387889379`
- resultado: **SUCCESS**
- URL: `https://github.com/prubiocn/Miastro/actions/runs/32387889379`

Validación local final:

- build: PASS
- warnings: 0
- errors: 0
- tests: **166/166 PASS**
- skipped: 0

---

## Swiss Ephemeris

Versión integrada:

`2.10.03`

Upstream:

`v2.10.3final`

Plataforma:

`linux-x64`

Biblioteca privada:

`libswe.so`

SHA-256:

`47e6fed985ccb5f067b7a0f6f746ec3567a7b54ce5f86140b2138616a8e6a653`

Propiedades verificadas:

- ELF64;
- x86-64;
- biblioteca compartida;
- ABI C;
- Cdecl;
- sin símbolos nativos sin resolver;
- dependencia de libm resuelta;
- sin dependencia de instalación global;
- sin `LD_LIBRARY_PATH`;
- carga mediante ruta controlada;
- versión nativa comprobada en runtime.

---

## Datos de efemérides

Conjunto mínimo distribuido:

- `sepl_18.se1`
- `semo_18.se1`
- `seas_18.se1`

Rango soportado declarado:

**1800–2399**

Integridad validada mediante:

- manifiesto;
- existencia;
- tamaño;
- SHA-256;
- obligatoriedad;
- rango temporal.

No se permite degradación silenciosa a Moshier cuando el perfil exige
Swiss Ephemeris.

---

## Perfil astronómico Miastro V1

Configuración:

- tropical;
- geocéntrica;
- longitud eclíptica;
- posición aparente;
- velocidad;
- sin topocentrismo;
- Nodo Verdadero;
- Lilith Media.

Flags centralizados:

- `SEFLG_SWIEPH`
- `SEFLG_SPEED`

No se utiliza:

- TRUEPOS;
- TOPOCTR;
- SIDEREAL;
- HELCTR;
- EQUATORIAL;
- RADIANS.

---

## Cuerpos integrados

Posición y velocidad reales para:

- Sol;
- Luna;
- Mercurio;
- Venus;
- Marte;
- Júpiter;
- Saturno;
- Urano;
- Neptuno;
- Plutón;
- Nodo Norte Verdadero;
- Lilith Media;
- Quirón;
- Ceres;
- Palas;
- Juno;
- Vesta.

El Nodo Sur continúa derivándose exclusivamente en dominio como
Nodo Norte + 180°.

---

## Casas

Sistemas soportados:

- Placidus;
- Koch.

Resultados:

- 12 cúspides;
- ASC nativo;
- MC nativo;
- DSC derivado de ASC + 180°;
- IC derivado de MC + 180°.

Las situaciones no calculables a latitudes extremas producen un resultado
explícito de indisponibilidad y nunca un resultado parcial considerado
válido.

---

## Tiempo astronómico

La conversión temporal y el Julian Day están centralizados.

Se utiliza:

- UTC;
- calendario gregoriano;
- `swe_julday`;
- precisión temporal sin redondeo artificial en el adaptador.

---

## Golden cases externos

Fuente externa:

**Astrodienst Swiss Ephemeris Test Page**

Versión de referencia:

`2.10.03`

Los valores esperados:

- no son generados por Miastro;
- se conservan como artefactos de referencia;
- conservan respuesta externa original;
- registran trazabilidad y hashes.

Casos cubiertos:

- fecha moderna;
- fecha histórica;
- hemisferio norte;
- hemisferio sur;
- longitud próxima a 0°;
- Sol–Plutón;
- Nodo Verdadero;
- Lilith Media;
- Quirón;
- Ceres;
- Palas;
- Juno;
- Vesta;
- Mercurio retrógrado;
- Placidus;
- Koch;
- ASC;
- MC.

Tolerancias V1:

- longitud: 0.0001°;
- velocidad longitudinal: 0.0001°/día;
- cúspides: 0.0001°;
- ASC: 0.0001°;
- MC: 0.0001°.

---

## Thread safety

Swiss Ephemeris mantiene estado global.

Miastro aplica una política conservadora:

- una única frontera nativa;
- serialización de llamadas;
- configuración protegida;
- no se presupone thread safety del motor C.

---

## Rendimiento

Existe una línea base técnica documentada en:

`docs/astronomy/performance-baseline.md`

La medición es orientativa y no contractual.

No se ha realizado optimización prematura.

---

## Distribución Linux

Publish validado:

- runtime: linux-x64;
- self-contained;
- ejecutable Miastro;
- `libswe.so`;
- manifiesto nativo;
- efemérides;
- integridad completa.

Paquete Debian validado:

`miastro_0.3.0~phase3-1_amd64.deb`

Instala:

- aplicación;
- biblioteca Swiss privada;
- manifiesto;
- efemérides;
- licencias.

Rutas principales:

- `/usr/lib/miastro/native/libswe.so`
- `/usr/share/miastro/ephemeris/`
- `/usr/share/doc/miastro/swiss-ephemeris/`

La instalación real y el arranque bajo entorno gráfico virtual han
superado el smoke test.

---

## Errores controlados

Cobertura explícita para:

- biblioteca ausente;
- biblioteca no cargable;
- versión inesperada;
- hash nativo incorrecto;
- ABI incompatible;
- efemérides ausentes;
- efemérides corruptas;
- hash de datos incorrecto;
- path incorrecto;
- rango temporal no soportado;
- objeto no soportado;
- error de cálculo;
- resultado inválido;
- casas no disponibles;
- arquitectura incompatible.

---

## Arquitectura

La frontera Swiss permanece exclusivamente en:

`Miastro.Infrastructure.SwissEphemeris`

No se exponen detalles nativos a:

- Domain;
- Application;
- UI;
- Interpretation;
- Persistence;
- Graphics.

Las abstracciones públicas residen en:

`Miastro.Astronomy.Abstractions`

Tipos principales:

- `AstronomicalInstant`
- `EclipticPosition`
- `HouseCalculationResult`
- `AstronomyEngineDiagnostic`
- `AstronomyEngineException`

---

## ADRs

Decisiones relevantes:

- ADR-003 — Swiss Ephemeris
- ADR-016 — Dependencias nativas privadas y versionadas
- ADR-025 — Thread safety de Swiss Ephemeris
- ADR-026 — Política de efemérides
- ADR-027 — Tolerancias de validación astronómica

---

## Matriz final de aceptación

| # | Criterio | Estado |
|---:|---|---|
| 1 | Biblioteca Swiss Ephemeris real | PASS |
| 2 | Biblioteca privada y controlada | PASS |
| 3 | Sin instalación global ni LD_LIBRARY_PATH | PASS |
| 4 | Arquitectura ELF64 x86-64 validada | PASS |
| 5 | SHA-256 nativo validado | PASS |
| 6 | Versión exacta 2.10.03 validada | PASS |
| 7 | Fuente y build upstream documentados | PASS |
| 8 | Licencias Swiss Ephemeris incluidas | PASS |
| 9 | Build nativo compartido compatible con Linux | PASS |
| 10 | NativeLibrary.Load con ruta controlada | PASS |
| 11 | ABI/PInvoke encapsulado exclusivamente en adaptador | PASS |
| 12 | Configuración de path de efemérides | PASS |
| 13 | Manifiesto técnico de recursos | PASS |
| 14 | Diagnóstico seguro del motor y datos | PASS |
| 15 | CalculationProfile.MiastroV1 validado | PASS |
| 16 | Flags Swiss centralizados | PASS |
| 17 | Posiciones reales Sol–Plutón | PASS |
| 18 | Velocidades reales Sol–Plutón | PASS |
| 19 | Nodo Norte Verdadero real | PASS |
| 20 | Lilith Media real | PASS |
| 21 | Quirón real | PASS |
| 22 | Ceres real | PASS |
| 23 | Palas real | PASS |
| 24 | Juno real | PASS |
| 25 | Vesta real | PASS |
| 26 | Nodo Sur derivado únicamente en dominio | PASS |
| 27 | Casas Placidus reales | PASS |
| 28 | Casas Koch reales | PASS |
| 29 | ASC nativo | PASS |
| 30 | MC nativo | PASS |
| 31 | DSC e IC derivados | PASS |
| 32 | Latitudes no calculables controladas | PASS |
| 33 | Tiempo astronómico centralizado | PASS |
| 34 | Julian Day centralizado mediante Swiss | PASS |
| 35 | EclipticPosition implementado | PASS |
| 36 | HouseCalculationResult implementado | PASS |
| 37 | Sin redondeo prematuro en adaptador | PASS |
| 38 | Golden cases de fuente externa | PASS |
| 39 | Fuente/configuración/tolerancias documentadas | PASS |
| 40 | Casos modernos e históricos | PASS |
| 41 | Casos hemisferio norte y sur | PASS |
| 42 | Caso próximo a longitud 0° | PASS |
| 43 | Retrogradación validada externamente | PASS |
| 44 | Nodo, Quirón y asteroides en golden cases | PASS |
| 45 | Placidus/Koch/ASC/MC en golden cases | PASS |
| 46 | Errores de biblioteca y hash cubiertos | PASS |
| 47 | Errores de efemérides y path cubiertos | PASS |
| 48 | Rango temporal y objeto inválido cubiertos | PASS |
| 49 | ABI, arquitectura y ejecución headless cubiertas | PASS |
| 50 | Rendimiento básico medido | PASS |
| 51 | Estado global/thread safety documentado y serializado | PASS |
| 52 | CI incluye integración nativa linux-x64 | PASS |
| 53 | Publish incluye libswe.so y datos | PASS |
| 54 | .deb incluye binario, datos y licencias | PASS |
| 55 | Instalación y arranque Ubuntu validados | PASS |
| 56 | ADRs, inventario y documentación consolidados sin regresiones ni Fase 4 | PASS |
| 57 | GitHub Actions remoto del commit de implementación finaliza en SUCCESS | PASS |

---

## Resultado final

**PASS: 57**

**FAIL: 0**

**PENDING: 0**

**FASE 3 CERRADA.**

La Fase 4 no forma parte de este cierre y no ha sido iniciada.

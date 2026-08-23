# MIASTRO — Fase 6 — Carta Natal fiable

## Estado

**EN IMPLEMENTACIÓN — NO CERRADA**

Baseline oficial de Fase 5:

- commit de cierre: `af8f06e2f8657d43daeb47da1b3de4e3b451f373`
- aceptación Fase 5: 69 PASS / 0 FAIL / 0 PENDING
- tests heredados: 254/254
- CI final Fase 5: SUCCESS

La Fase 6 queda iniciada sobre este baseline.

La Fase 7 no está iniciada.

## Objetivo

Implementar por primera vez el cálculo funcional y persistible de Carta Natal
desde una Persona persistida, reutilizando el snapshot histórico de nacimiento,
Astronomy Abstractions, Swiss Ephemeris, dominio astrológico y motor de aspectos.

## Política funcional inicial

La Carta Natal completa V1 requiere hora Exacta o Aproximada con hora concreta
y resolución histórica persistida válida.

Rango, Momento del día y Desconocida no producen una carta completa ficticia.

La hora ambigua sin elección explícita y la hora inexistente bloquean el cálculo.

## Restricciones

No implementar todavía:

- rueda gráfica;
- revoluciones;
- tránsitos;
- progresiones;
- sinastría;
- interpretación textual;
- informes;
- exportación gráfica final;
- impresión astrológica final.

## Criterios de aceptación

- PASS: 82
- FAIL: 0
- PENDING: 1

Pendiente exclusivamente:

- criterio 82 — CI remoto del candidato en `SUCCESS`.

## Bloque 0 — Baseline y auditoría

En curso:

- validación del cierre Fase 5;
- inventario exacto de Astronomy Abstractions;
- inventario exacto del adaptador Swiss;
- inventario del dominio astrológico Fase 2;
- inventario de persistencia Fase 5;
- inventario de configuración, CI y empaquetado;
- identificación de contratos reutilizables;
- preparación del diseño persistente natal.

## Exclusiones deliberadas

La Fase 7 no está iniciada.

No existe rueda natal funcional en esta fase.

## Bloque 1A — Elegibilidad y hash natal

Implementado:

- política explícita de elegibilidad;
- Exacta calculable;
- Aproximada calculable y marcada;
- Rango bloqueado para carta completa;
- Momento del día bloqueado;
- Desconocida bloqueada;
- hora ambigua sin elección bloqueada;
- hora inexistente bloqueada;
- estados Current/Superseded/Invalidated;
- fingerprint natal canónico;
- SHA-256 determinista;
- sistema de casas incluido en el hash;
- orden canónico estable de objetos;
- ADR-012 y ADR-013 propuestos.

No se ha iniciado la rueda natal.

## Bloque 1B — Persistencia natal normalizada

Implementado:

- contrato INatalChartStore;
- snapshot de lectura/escritura independiente de EF;
- tablas NatalCharts;
- NatalPlacements;
- NatalHouseCusps;
- NatalAspects;
- relación Persona → múltiples snapshots;
- cascada sobre hijos;
- estado Current/Superseded/Invalidated;
- unicidad PersonId + InputHash;
- idempotencia persistente;
- reemplazo trazable de carta vigente;
- invalidación explícita;
- migración EF incremental desde Fase 5;
- tests SQLite reales;
- ADR-014 propuesto.

No se ha iniciado la rueda natal.

## Bloque 2A — Geometría de casas y sect

Implementado:

- asignación de placements mediante cúspides reales;
- soporte de casas desiguales;
- soporte del cruce 360°/0°;
- política determinista de cúspide exacta;
- tolerancia numérica 1e-9°;
- Day/Night por posición real del Sol respecto al horizonte de casas;
- cálculo natal de aspectos mediante MiastroV1AspectProfile;
- orden estable de pares;
- exclusión de Nodo/Lilith/Fortuna conforme al perfil V1;
- ADR-031 y ADR-032 propuestos.

No se ha iniciado la rueda natal.

## Bloque 2B — CalculateNatalChart

Implementado:

- caso de uso CalculateNatalChartUseCase;
- entrada principal PersonId;
- Placidus como default;
- Koch soportado;
- reutilización del Instant UTC persistido;
- verificación de elegibilidad;
- idempotencia previa al cálculo;
- cálculo mediante puertos Astronomy Abstractions;
- 17 objetos solicitados al motor;
- Nodo Sur derivado sin llamada independiente;
- ASC/MC obtenidos del cálculo de casas;
- DSC/IC continúan derivados por HouseCalculationResult;
- Parte de Fortuna en dominio;
- Day/Night en dominio;
- casas de placements mediante cúspides reales;
- aspectos MiastroV1;
- AstrologicalChart de tipo Natal;
- snapshot persistible;
- resultado funcional tipado;
- Application sin dependencia del adaptador Swiss.

Pendiente del Bloque 2C:

- wiring productivo del adaptador Swiss;
- proveedor real de identidad de efemérides;
- E2E con Swiss real.

No se ha iniciado la rueda natal.

## Bloque 2B — CalculateNatalChart

Implementado:

- caso de uso CalculateNatalChartUseCase;
- entrada principal PersonId;
- Placidus como default;
- Koch soportado;
- reutilización del Instant UTC persistido;
- verificación de elegibilidad;
- idempotencia previa al cálculo;
- cálculo mediante puertos Astronomy Abstractions;
- 17 objetos solicitados al motor;
- Nodo Sur derivado sin llamada independiente;
- ASC/MC obtenidos del cálculo de casas;
- DSC/IC continúan derivados por HouseCalculationResult;
- Parte de Fortuna en dominio;
- Day/Night en dominio;
- casas de placements mediante cúspides reales;
- aspectos MiastroV1;
- AstrologicalChart de tipo Natal;
- snapshot persistible;
- resultado funcional tipado;
- Application sin dependencia del adaptador Swiss.

Pendiente del Bloque 2C:

- wiring productivo del adaptador Swiss;
- proveedor real de identidad de efemérides;
- E2E con Swiss real.

No se ha iniciado la rueda natal.

## Fix Bloque 2B — Composition root Swiss

Corregido:

- Bootstrap registra IEclipticPositionCalculator;
- Bootstrap registra IHouseCalculator;
- Bootstrap registra IAstronomyEngineDiagnostics;
- Bootstrap registra INatalCalculationMetadataProvider;
- SwissEphemerisOptions se resuelve para desarrollo, publish e instalación;
- identidad de efemérides basada en SHA-256 del manifest;
- DI completa validable con ValidateOnBuild;
- se mantiene Application sin referencia a Swiss.

La serialización de Swiss continúa siendo responsabilidad del adaptador
existente mediante SwissEphemerisGate.

No se ha iniciado la rueda natal.

## Bloque 2C — E2E natal real

Implementado:

- E2E natal completo con Swiss real;
- Persona sintética persistida;
- Instant UTC persistido reutilizado;
- posiciones y velocidades reales;
- 17 cuerpos solicitados al motor;
- Nodo Sur derivado;
- ASC/MC reales;
- 12 cúspides reales;
- Parte de Fortuna;
- aspectos natales;
- snapshot SQLite;
- cierre y reapertura;
- carta vigente recuperada;
- comparación de Id y hash;
- E2E específico con hora Aproximada;
- Koch validado con motor real.

No se ha iniciado la rueda natal.

## Bloque 3A — Invalidación automática e historial

Implementado:

- detección explícita de cambios natales;
- invalidación automática de snapshot Current;
- estado Invalidated;
- InvalidatedAtUtc;
- identidad/contacto/residencia no invalidan;
- historial NatalChartCalculated;
- historial NatalChartRecalculated preparado;
- historial NatalChartInvalidated;
- actualización + invalidación + historial en la misma unidad de trabajo EF;
- sin datos privados en resúmenes de historial;
- ADR-033 propuesto.

El recálculo explícito se implementará en Bloque 3B con versionado de revisión,
para no romper la idempotencia por InputHash.

No se ha iniciado la rueda natal.

## Bloque 3B — Identidad de entrada y recálculo

Implementado:

- fingerprint natal V2;
- precisión Exact/Aproximada incluida en identidad;
- GeoNameId y localidad incluidos;
- offset histórico incluido;
- selección ambigua incluida;
- hash SHA-256 determinista;
- RecalculateNatalChartUseCase;
- mismo hash vigente no genera duplicado;
- DI de recálculo;
- ADR de Fase 6 renumerados desde ADR-028 para evitar colisiones históricas.

No se ha iniciado la rueda natal.

## Bloque 3C — Circuito E2E de invalidación y recálculo

Validado con SQLite y Swiss reales:

- carta inicial Current;
- modificación de hora natal;
- invalidación automática;
- ausencia temporal de carta Current;
- recálculo;
- nuevo InputHash;
- nueva snapshot Current;
- snapshot anterior conservada como Invalidated;
- una única Current;
- historial Calculated;
- historial Invalidated;
- historial Recalculated;
- privacidad de los resúmenes de historial;
- placements y cúspides recuperables tras recálculo.

No se ha iniciado la rueda natal.

## Bloque 4A — Snapshot reproducible de BirthData

Implementado:

- BirthDataVersion = 1;
- BirthDataHash SHA-256 determinista;
- BirthDataHash independiente del sistema de casas;
- persistencia de BirthTimePrecision;
- persistencia de GeoNameId;
- persistencia de HistoricalOffsetSeconds;
- persistencia de AmbiguousSelection;
- migración EF incremental;
- reapertura E2E conservando BirthDataHash;
- ADR-034.

No se ha iniciado la rueda natal.

## Bloque 4B — Integridad de snapshot natal

Implementado:

- validación estricta de 21 objetos;
- orden canónico;
- objetos únicos;
- ASC/MC obligatorios;
- Parte de Fortuna obligatoria;
- Nodo Sur derivado validado;
- 12 cúspides;
- longitudes normalizadas;
- valores finitos;
- casas 1–12;
- aspectos sin pares duplicados;
- coherencia BirthDataHash;
- validación en Application y Persistence;
- ADR-035.

No se ha iniciado la rueda natal.

## Bloque 5A1 — Captura externa para goldens natales

Preparado corpus externo de cinco cartas:

- tres modernas;
- dos históricas;
- norte y sur;
- Placidus y Koch;
- retrogradación;
- longitud próxima a 0°;
- casas desiguales.

Fuente: Swiss Ephemeris `swetest` 2.10.03.

El caso Madrid 2024 se contrasta con la captura Astrodienst conservada en
Fase 3.

Los expected values no se generan mediante Miastro.

Pendiente: parser del corpus y ejecución de los goldens completos de Fase 6.

## Bloque 5A2 — Normalización golden externa

El corpus crudo de cinco cartas se normaliza a
`tests/golden/phase6/golden-values.json`.

La normalización no utiliza Miastro para generar valores expected.

Cobertura:

- 3 cartas modernas;
- 2 históricas;
- 17 cuerpos Swiss por carta;
- 12 cúspides por carta;
- ASC/MC;
- Placidus/Koch;
- norte/sur;
- retrogradación;
- longitud próxima a 0°;
- Nodo Sur derivado por contrato.

La integridad de las respuestas crudas se protege mediante SHA-256.

## Bloque 5A3 — Ejecución contra goldens externos

Se ejecuta el adaptador Swiss de Miastro contra las cinco cartas externas de
Fase 6.

Validación:

- 5/5 cartas;
- 17 cuerpos Swiss por carta;
- longitud;
- latitud;
- velocidad longitudinal;
- 12 cúspides;
- ASC;
- MC;
- Placidus y Koch;
- Nodo Norte verdadero;
- Nodo Sur derivado;
- retrogradación;
- longitud próxima a 0°;
- norte y sur;
- 3 modernas;
- 2 históricas.

Los valores expected permanecen derivados exclusivamente del corpus externo.

## Bloque 5B — Goldens derivados independientes

Añadida validación independiente de:

- secta Day/Night;
- Parte de Fortuna;
- ocupación de casas para los 21 objetos;
- aspectos V1.

Los valores expected se obtienen exclusivamente a partir del corpus primario
externo mediante una implementación de referencia independiente de Miastro.

Se registra el SHA-256 del corpus primario.

ADR-036 documenta reglas y procedencia.

## Bloque 5C1 — Integridad semántica

Endurecimientos:

- el InputHash entregado al store debe coincidir con el fingerprint;
- los pares de aspectos son no orientados: A/B y B/A son duplicados;
- sólo participantes Miastro V1 pueden persistirse como aspectos;
- se validan magnitudes y orbe de aspectos;
- Stationary se define únicamente como velocidad longitudinal exactamente 0.

ADR-037 documenta la política de movimiento estacionario.

## Bloque 5C2 — Identidad e invalidación

Se alinea `NatalInputFingerprint` con la política real de invalidación de
BirthData.

Quedan representados criptográficamente los campos de precisión horaria,
procedencia geográfica, resolución temporal histórica, candidatos ambiguos y
override manual de coordenadas.

`InputHash` pasa a canonicalización `miastro-natal-input-v3`.

Se mantiene la separación semántica:

- `BirthDataHash`: identidad de nacimiento;
- `InputHash`: identidad de nacimiento + configuración de cálculo.

ADR-038 documenta el contrato.

## Bloque 6A1 — Fiabilidad operativa

Se añade evidencia automatizada de:

- backup completo de snapshots natales;
- reapertura y lectura del backup;
- migración real desde el esquema final de Fase 5;
- conservación de Persona durante la migración;
- creación de las tablas natales sin reset;
- serialización compartida de Swiss Ephemeris para posiciones y casas;
- determinismo bajo llamadas concurrentes.

No se modifica la implementación de backup ni la política Swiss existente.

ADR-039 documenta estas garantías.

## Bloque 6A2 — Baseline de rendimiento

Se añade una medición reproducible y no bloqueante del pipeline natal real.

Métricas separadas:

- carga de Persona;
- 17 cuerpos Swiss;
- casas;
- derivados y aspectos;
- cálculo natal completo con persistencia;
- recarga del snapshot;
- fast path idempotente.

No se establece un umbral temporal arbitrario en CI.

ADR-040 y `docs/natal/performance-baseline.md` documentan la política.

## Bloque 6B1 — UI natal mínima

La ficha de Persona incorpora:

- detección de snapshot natal vigente;
- selector Placidus/Koch;
- acción `Calcular carta natal`;
- estado funcional;
- fecha y sistema de casas;
- tabla básica de las 21 posiciones.

El cálculo se bloquea mientras existan cambios de ficha sin guardar.

No se exponen hashes, ABI, rutas, flags ni versiones técnicas en la UI normal.

No se implementa rueda natal.

## Bloque 6C1 — Preparación de empaquetado y CI

Preparado:

- paquete Debian `0.6.0~phase6-1`;
- publish dedicado `fase6-linux-x64`;
- staging Debian dedicado de Fase 6;
- workflow CI actualizado a publish/package de Fase 6;
- regresión XDG heredada de Fase 5 conservada;
- coherencia entre nombres y cabeceras ADR-028..ADR-040;
- tests de contrato de cierre técnico.

Todavía pendientes antes del cierre:

- upgrade real instalado Fase 5 → Fase 6;
- matriz final de 83 criterios;
- commit candidato;
- CI remoto candidato;
- cierre documental final;
- CI remoto de cierre.

La Fase 7 no está iniciada.


## Bloque 6C2 — Validación de instalación y upgrade real

Validación operativa completada con paquetes Debian reales:

- paquete Fase 5 `0.5.0~phase5-1` instalado;
- arranque real de Fase 5 con XDG aislado;
- base Fase 5 creada y migrada;
- Persona y BirthData de fixture persistidos;
- actualización real a `0.6.0~phase6-1`;
- migraciones Fase 6 aplicadas sin reset;
- Persona y BirthData preservados;
- tablas natales creadas;
- integridad SQLite y claves foráneas correctas;
- desinstalación y reinstalación de Fase 6;
- datos XDG preservados tras reinstalación.

Criterio 81: PASS.

## Bloque 6C3 — Candidato de cierre local

Validación de la aplicación instalada `0.6.0~phase6-1`:

- cálculo natal real: PASS;
- persistencia del snapshot: PASS;
- 21 posiciones: PASS;
- 12 cúspides: PASS;
- 55 aspectos persistidos en el caso validado;
- cierre del contenedor y reapertura: PASS;
- recuperación del snapshot vigente: PASS;
- identidad del snapshot preservada: PASS;
- segundo cálculo con la misma entrada:
  `ExistingCurrentSnapshot`;
- `PRAGMA integrity_check`: PASS;
- `PRAGMA foreign_key_check`: PASS;
- código de producto modificado durante esta validación: NO.

Validación local global:

- build Release: PASS;
- tests: 345/345 PASS;
- publish Fase 6: PASS;
- `.deb` Fase 6: PASS;
- upgrade instalado Fase 5 → Fase 6: PASS;
- supervivencia de datos tras reinstalación: PASS;
- Fase 7 iniciada: NO;
- rueda natal implementada: NO.

Estado del candidato:

- PASS: 82
- FAIL: 0
- PENDING: 1
- pendiente único: criterio 82 — CI remoto.

La Fase 6 continúa abierta hasta obtener `SUCCESS` remoto.
La Fase 7 no está iniciada.

# MIASTRO — Fase 4 — Informe final

## Estado

**FASE 4 CERRADA**

| Métrica | Resultado |
|---|---:|
| PASS | 57 |
| FAIL | 0 |
| PENDING | 0 |
| Tests locales | 208/208 PASS |
| CI candidato | SUCCESS |
| Commit candidato | `4243f9ccefe58ce596d63e52e71506a53e3f9133` |
| Run candidato | `32395772961` |
| Fase 5 iniciada | NO |

## Objetivo alcanzado

Se ha implementado la infraestructura real para:

- catálogo geográfico GeoNames completamente offline;
- búsqueda de localidades y homónimos;
- coordenadas geográficas validadas;
- zona IANA;
- resolución histórica mediante Noda Time + TZDB;
- DST;
- horas ambiguas;
- horas inexistentes;
- conversión reproducible de fecha/hora local a Instant UTC;
- flujo headless localidad → coordenadas/IANA → Instant.

No se ha construido el caso de uso completo de Carta Natal.

## Dataset geográfico

Snapshot GeoNames: **2026-08-20**.

Fuentes bloqueadas por SHA-256:

- `cities500.zip`;
- `alternateNamesV2.zip`;
- `countryInfo.txt`;
- `admin1CodesASCII.txt`;
- `admin2Codes.txt`;
- `timeZones.txt`;
- `readme.txt`.

Catálogo oficial generado:

- localidades: **235417**;
- nombres alternativos: **1880224+**;
- schema: **2**;
- SQLite: solo lectura en runtime;
- instalación: `/usr/share/miastro/geodata/`.

La limitación inherente a `cities500` está documentada.

## Tiempo histórico

Tecnología:

- Noda Time 3.3.3;
- IANA TZDB embebido;
- TZDB observado durante la implementación:
  `TZDB: 2026c (mapping: 48.2)`.

Política:

- hora normal → un candidato;
- hora ambigua → dos candidatos, sin elección silenciosa;
- hora inexistente → estado skipped, sin desplazamiento silencioso;
- se conserva versión TZDB, offset e Instant;
- la elección explícita de una ambigüedad es auditable.

Cobertura temporal:

- Madrid;
- Barcelona;
- Canarias;
- Nueva York;
- Katmandú;
- Kolkata;
- Adelaida;
- Europa, América, Asia y hemisferio sur;
- offsets de 30 y 45 minutos;
- DST;
- transiciones históricas.

## Empaquetado y CI

Validado:

- publish `linux-x64` self-contained;
- Swiss Ephemeris sigue empaquetado;
- catálogo GeoNames en publish;
- paquete `.deb`;
- GeoNames en `/usr/share/miastro/geodata/`;
- licencia y atribución GeoNames;
- fixture controlado para CI;
- instalación del paquete;
- validación read-only del catálogo instalado;
- smoke de aplicación instalada.

CI candidato:

- run: **32395772961**;
- commit: **4243f9ccefe58ce596d63e52e71506a53e3f9133**;
- conclusión: **SUCCESS**.

La advertencia de GitHub Actions sobre acciones basadas originalmente en
Node.js 20 es informativa y no bloquea el cierre; el runner ejecutó el job
completo correctamente.

## Arquitectura

Se mantiene:

- Domain sin Noda Time;
- UI Avalonia sin Noda Time ni SQLite geográfico directo;
- Geography/Time sin dependencia de Swiss Ephemeris;
- resolución temporal separada del cálculo astronómico;
- frontera con `Astronomy.Abstractions` validada;
- sin acoplamiento de geografía/tiempo a Avalonia;
- sin Fase 5.

## Criterios de aceptación

| # | Criterio | Estado |
|---:|---|---|
| 1 | GeoNames integrado offline | PASS |
| 2 | Dataset y licencia documentados | PASS |
| 3 | DataBuilder reproducible | PASS |
| 4 | `geonames.sqlite` se genera correctamente | PASS |
| 5 | Catálogo funciona en solo lectura | PASS |
| 6 | Búsqueda exacta funciona | PASS |
| 7 | Prefijo funciona | PASS |
| 8 | Búsqueda sin tildes funciona | PASS |
| 9 | Nombres alternativos funcionan | PASS |
| 10 | Homónimos se distinguen | PASS |
| 11 | Pamplona devuelve resultados diferenciables | PASS |
| 12 | País correcto | PASS |
| 13 | Región correcta | PASS |
| 14 | Coordenadas correctas | PASS |
| 15 | Zona IANA correcta | PASS |
| 16 | Latitud validada | PASS |
| 17 | Longitud validada | PASS |
| 18 | No se selecciona homónimo automáticamente por población | PASS |
| 19 | Ranking determinista | PASS |
| 20 | FTS/índices funcionan | PASS |
| 21 | Noda Time integrado | PASS |
| 22 | IANA TZDB utilizada | PASS |
| 23 | No se depende de offset fijo | PASS |
| 24 | Hora normal resuelve a Instant | PASS |
| 25 | Offset histórico correcto | PASS |
| 26 | Versión TZDB registrada | PASS |
| 27 | Hora ambigua devuelve dos candidatos | PASS |
| 28 | Hora ambigua no se resuelve silenciosamente | PASS |
| 29 | Hora inexistente se detecta | PASS |
| 30 | Hora inexistente no se corrige silenciosamente | PASS |
| 31 | Offsets de transición conservados | PASS |
| 32 | Casos de España pasan | PASS |
| 33 | Casos internacionales pasan | PASS |
| 34 | Offsets fraccionarios pasan | PASS |
| 35 | Golden cases temporales documentados | PASS |
| 36 | Resolución reproducible | PASS |
| 37 | Errores geográficos tipados | PASS |
| 38 | Errores temporales tipados | PASS |
| 39 | Catálogo ausente controlado | PASS |
| 40 | Catálogo corrupto controlado | PASS |
| 41 | Zone ID inválido controlado | PASS |
| 42 | E2E localidad → Instant pasa | PASS |
| 43 | Compatibilidad con abstracción astronómica validada | PASS |
| 44 | Sin llamadas web en uso normal | PASS |
| 45 | Memoria razonable; no se carga todo el catálogo en memoria | PASS |
| 46 | Rendimiento de búsqueda medido | PASS |
| 47 | Tests headless pasan | PASS |
| 48 | Tests arquitectónicos pasan | PASS |
| 49 | Tests heredados pasan | PASS |
| 50 | Publish funciona | PASS |
| 51 | `.deb` incluye catálogo | PASS |
| 52 | `.deb` incluye licencias | PASS |
| 53 | Instalación real encuentra catálogo | PASS |
| 54 | Aplicación sigue arrancando | PASS |
| 55 | CI Ubuntu completo termina en SUCCESS | PASS |
| 56 | Documentación de geografía/tiempo y ADRs completa | PASS |
| 57 | Fase 5 no iniciada y alcance de Fase 4 respetado | PASS |

## Resultado final

**PASS: 57**

**FAIL: 0**

**PENDING: 0**

**Fase 4: CERRADA.**

La Fase 5 no se inicia como parte de este cierre.

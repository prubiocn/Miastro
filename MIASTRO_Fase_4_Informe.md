# MIASTRO — Fase 4 — Informe

## Estado

**CANDIDATO DE CIERRE — CI REMOTO PENDIENTE**

La Fase 4 no se considera cerrada hasta disponer de una ejecución CI remota
satisfactoria sobre el commit candidato y una ejecución final satisfactoria
sobre el commit de cierre.

## Alcance implementado

Geografía offline:

- proveedor GeoNames;
- snapshot oficial bloqueado por fecha y SHA-256;
- `cities500`;
- `alternateNamesV2`;
- `countryInfo`;
- `admin1CodesASCII`;
- `admin2Codes`;
- `timeZones`;
- licencia CC BY 4.0 y atribución;
- DataBuilder reproducible y automatizable;
- SQLite de solo lectura;
- esquema versionado;
- índices y FTS5;
- búsqueda exacta, prefijo, acentos y nombres alternativos;
- homónimos preservados;
- coordenadas validadas;
- IANA TimeZoneId;
- catálogo de distribución en `/usr/share/miastro/geodata/`.

Tiempo histórico:

- Noda Time;
- IANA TZDB embebido;
- resolución normal;
- hora ambigua con dos candidatos y sin selección silenciosa;
- hora inexistente sin desplazamiento silencioso;
- versión TZDB registrada;
- offset e Instant conservados;
- corpus golden;
- Madrid;
- Barcelona;
- Canarias;
- Nueva York;
- Katmandú;
- Kolkata;
- Adelaida;
- DST y offsets fraccionarios.

Integración:

- flujo headless `localidad -> coordenadas + IANA -> hora local -> Instant`;
- frontera compatible con `Astronomy.Abstractions`;
- sin dependencia de Geografía/Tiempo hacia Swiss Ephemeris;
- sin implementación de Carta Natal;
- Fase 5 no iniciada.

Distribución y CI:

- publish linux-x64;
- catálogo oficial en publish de release;
- `.deb` con geodata en `/usr/share/miastro/geodata/`;
- licencia GeoNames incluida;
- fixture geográfico controlado para CI;
- validación read-only e integridad SQLite;
- validación de instalación configurada en CI.

## Evidencia local del candidato

- Build Release: PASS.
- Tests: 208/208 PASS.
- Publish oficial: PASS.
- Debian package: PASS.
- Catálogo empaquetado: 235417 localidades.
- Integridad SQLite: PASS.
- Apertura read-only: PASS.
- Phase4Closed: NO.
- Phase5Started: NO.

## Criterios de aceptación

Los 57 criterios permanecen formalmente **PENDING** hasta validar CI remoto.

- PASS: 0
- FAIL: 0
- PENDING: 57

## Cierre

Pendiente de:

1. commit candidato;
2. CI remoto SUCCESS;
3. auditoría final de 57 criterios;
4. commit final de cierre;
5. CI final SUCCESS.

No iniciar Fase 5.

## Candidato CI — corrección 1

El primer commit candidato (`8420a83052c695be9eca56bdca302e591ef3b13d`)
compiló correctamente en CI pero falló en el paso de tests.

Causa: varios tests de integración geográfica apuntaban directamente a
`data/geography/release/geonames.sqlite`, un artefacto grande que
deliberadamente no se versiona.

Corrección aplicada:

- ruta de catálogo de tests centralizada;
- `MIASTRO_GEODATA_DIR` tiene prioridad;
- CI usa el fixture controlado generado en el propio job;
- local puede seguir usando el catálogo oficial de release;
- no se versiona `geonames.sqlite`.

La Fase 4 permanece abierta hasta obtener CI SUCCESS.

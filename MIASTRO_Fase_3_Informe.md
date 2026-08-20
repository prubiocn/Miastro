# MIASTRO — Informe técnico Fase 3

## Estado

Fase 3 — Integración de Swiss Ephemeris en Linux.

Estado previo a validación CI remota:

- PASS: 56
- FAIL: 0
- PENDING: 1

El único PENDING es la ejecución remota final de GitHub Actions sobre el
commit de Fase 3.

La fase no se declara oficialmente cerrada mientras dicho workflow no
termine en SUCCESS.

## Swiss Ephemeris

- versión: 2.10.03
- tag upstream: v2.10.3final
- plataforma: linux-x64
- biblioteca: libswe.so
- carga privada y controlada
- sin LD_LIBRARY_PATH
- sin instalación global requerida
- ABI: Cdecl
- arquitectura: ELF64 x86-64
- hash libswe.so:
  `47e6fed985ccb5f067b7a0f6f746ec3567a7b54ce5f86140b2138616a8e6a653`

## Datos

Efemérides mínimas:

- sepl_18.se1
- semo_18.se1
- seas_18.se1

Rango:

- 1800–2399

Integridad:

- tamaño
- SHA-256
- obligatoriedad
- manifiesto
- rango temporal

## Cuerpos reales

- Sol
- Luna
- Mercurio
- Venus
- Marte
- Júpiter
- Saturno
- Urano
- Neptuno
- Plutón
- Nodo Norte Verdadero
- Lilith Media
- Quirón
- Ceres
- Palas
- Juno
- Vesta

Nodo Sur continúa derivándose en dominio.

## Casas

- Placidus
- Koch
- 12 cúspides
- ASC
- MC
- DSC derivado
- IC derivado
- error explícito en casos no calculables

## Flags V1

- SEFLG_SWIEPH
- SEFLG_SPEED

Sin:

- TRUEPOS
- TOPOCTR
- SIDEREAL
- HELCTR
- EQUATORIAL
- RADIANS

## Thread safety

Acceso nativo serializado mediante una única puerta interna.

## Golden cases

Fuente externa:

Astrodienst Swiss Ephemeris Test Page.

Incluyen:

- fecha moderna
- fecha histórica
- hemisferio norte
- hemisferio sur
- Nodo Verdadero
- Lilith Media
- Quirón
- asteroides
- retrogradación
- Placidus
- Koch
- ASC
- MC

Los expected values no son generados por Miastro.

## Tolerancias

- longitud: 0.0001°
- velocidad: 0.0001°/día
- cúspides: 0.0001°
- ASC: 0.0001°
- MC: 0.0001°

## Rendimiento

Existe baseline orientativo documentado en:

`docs/astronomy/performance-baseline.md`

No se ha realizado optimización prematura.

## Publish

Validado:

- linux-x64
- self-contained
- libswe.so
- manifiesto nativo
- efemérides
- integridad
- ABI

## Debian

El paquete de prueba instala:

- aplicación
- libswe.so
- efemérides
- manifiestos
- licencias

Rutas:

- `/usr/lib/miastro/native/`
- `/usr/share/miastro/ephemeris/`

La aplicación instalada supera smoke test.

## Errores controlados

- library missing
- library not loadable
- ABI incompatible
- versión inesperada
- ephemeris missing
- ephemeris corrupta
- hash incorrecto
- rango temporal no soportado
- objeto no soportado
- error de cálculo
- resultado inválido
- casas no disponibles

## Tests

Los tests heredados de Fases 1 y 2 permanecen incluidos.

Validación local final: 166 tests ejecutados, 166 superados, 0 fallidos y 0 omitidos.

## ADRs

- ADR-003 — Swiss Ephemeris
- ADR-016 — Dependencias nativas privadas y versionadas
- ADR-025 — Thread safety de Swiss Ephemeris
- ADR-026 — Política de efemérides
- ADR-027 — Tolerancias de validación astronómica

## Exclusiones deliberadas

No se implementa:

- UI astrológica funcional
- rueda astrológica
- GeoNames funcional
- interpretación
- informes astrológicos finales
- Revolución Solar
- Revolución Lunar
- módulo funcional de tránsitos
- progresiones
- sinastría
- Fase 4

## Aceptación provisional

| Estado | Total |
|---|---:|
| PASS | 56 |
| FAIL | 0 |
| PENDING | 1 |

El único PENDING es GitHub Actions remoto.

**FASE 3 NO CERRADA TODAVÍA.**

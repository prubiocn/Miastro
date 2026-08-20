# MIASTRO — Informe técnico Fase 2

## 1. Estado

Fase: **Fase 2 — Núcleo de dominio astrológico**

Estado final:

- PASS: 44
- FAIL: 0
- PENDING: 0

Validación remota completada correctamente.

La Fase 2 queda oficialmente aprobada y cerrada.

## 2. Implementaciones

Se ha construido un núcleo puro de dominio astrológico independiente de infraestructura.

Incluye:

- `Angle`
- `EclipticLongitude`
- `AngularSeparation`
- signos zodiacales;
- elementos;
- modalidades;
- polaridades;
- ejes zodiacales;
- casas 1–12;
- ejes de casas;
- Placidus y Koch como tipos de dominio;
- objetos astrológicos V1;
- categorías de objetos;
- Nodo Norte Verdadero;
- Nodo Sur derivado;
- Lilith Media;
- Parte de la Fortuna;
- regencias tradicionales y modernas;
- nueve aspectos canónicos;
- orbes V1;
- regla única de +1° por participación de Sol o Luna;
- política de participantes;
- `MiastroV1AspectProfile`;
- motor puro y determinista de aspectos;
- `ZodiacPosition`;
- `AstrologicalPlacement`;
- movimiento Direct/Retrograde/Stationary;
- `AstrologicalChart`;
- tipos de carta;
- cúspides opcionales;
- metadatos de cálculo;
- `CalculationProfile.MiastroV1`.

## 3. Reglas implementadas

### Nodo

Nodo Norte V1 = Nodo Verdadero.

Nodo Sur:

`Nodo Norte + 180°`

normalizado a `[0°,360°)`.

### Parte de la Fortuna

Diurna:

`ASC + Luna - Sol`

Nocturna:

`ASC + Sol - Luna`

### Aspectos

- Conjunción — 0°
- Semisextil — 30°
- Sextil — 60°
- Cuadratura — 90°
- Trígono — 120°
- Quincuncio — 150°
- Oposición — 180°
- Quintil — 72°
- Biquintil — 144°

La selección es determinista:

1. menor desviación;
2. prioridad estable como desempate.

## 4. Invariantes

El dominio impide, entre otros:

- ángulos no finitos;
- casas fuera de 1–12;
- ejes inválidos;
- orbes negativos;
- ángulos de aspecto fuera de rango;
- perfiles sin aspectos;
- perfiles sin participantes;
- aspectos duplicados dentro de un perfil;
- identificadores de carta vacíos;
- objetos duplicados dentro de una carta;
- conjuntos parciales o duplicados de cúspides.

## 5. Tests

Se mantienen los tests heredados de Fase 1 y se añaden tests de Fase 2 para:

- normalización angular;
- separación angular;
- signos;
- elementos;
- modalidades;
- polaridades;
- casas;
- ejes;
- Nodo Sur;
- Parte de la Fortuna;
- regencias;
- posiciones;
- retrogradación;
- los nueve aspectos;
- orbes;
- luminares;
- participantes;
- cruce de 0°;
- determinismo;
- carta y metadatos;
- invariantes;
- tests generativos reproducibles;
- arquitectura.

Seed generativa:

`20260820`

## 6. ADRs

Nuevos ADR:

- ADR-019 — Modelo angular canónico
- ADR-020 — Nodo Verdadero y Nodo Sur derivado
- ADR-021 — Perfil de aspectos V1
- ADR-022 — Regencias tradicionales y modernas
- ADR-023 — CalculationProfile V1
- ADR-024 — Inmutabilidad del dominio

## 7. Build y publicación

Validación local:

- restore: PASS
- build Release: PASS
- tests: PASS
- publish linux-x64 self-contained: PASS
- `libhostfxr.so` presente: PASS

## 8. Arquitectura

`Miastro.Domain` no referencia:

- Avalonia;
- Entity Framework Core;
- SkiaSharp;
- Swiss Ephemeris;
- infraestructura Miastro.

No se ha añadido integración funcional con Swiss Ephemeris.

## 9. Incidencias resueltas

Durante la implementación se corrigieron:

- sintaxis de constructores en dos `readonly record struct`;
- exposición de colección de regencias;
- atributos obsoletos de MSTest 4.

No quedan incidencias técnicas locales abiertas.

## 10. Deuda técnica deliberada

Queda fuera de Fase 2:

- cálculo astronómico real;
- Swiss Ephemeris funcional;
- geografía;
- TZDB funcional;
- carta natal calculada;
- retornos solares/lunares funcionales;
- tránsitos;
- progresiones;
- sinastría funcional;
- rueda;
- Skia funcional para astrología;
- UI astrológica;
- interpretación textual;
- informes astrológicos finales;
- impresión astrológica.

## 11. Resultado final de aceptación

| Estado | Total |
|---|---:|
| PASS | 44 |
| FAIL | 0 |
| PENDING | 0 |

GitHub Actions remoto ha finalizado en `SUCCESS`.

### Verificación remota

- Workflow: `Miastro CI`
- Rama: `main`
- Commit técnico verificado: `f2838f46baa65e9fc0ceac6c1af2b9da33a20e3d`
- Run ID: `32360029059`
- Inicio: `2026-08-20T10:39:57Z`
- Finalización: `2026-08-20T10:46:00Z`
- Job `build-test-publish`: PASS
- Checkout: PASS
- Setup .NET 10: PASS
- Restore: PASS
- Build: PASS
- Tests: PASS
- Publish linux-x64 self-contained: PASS
- Estado global: SUCCESS
- Ejecución: https://github.com/prubiocn/Miastro/actions/runs/32360029059

## 12. Cierre oficial

**PASS: 44**

**FAIL: 0**

**PENDING: 0**

**FASE 2 — APROBADA Y CERRADA**

No se inicia automáticamente la Fase 3.

La siguiente fase prevista es:

`Fase 3 — Integración de Swiss Ephemeris en Linux`

y deberá comenzar únicamente mediante una orden independiente.

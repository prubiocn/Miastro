# Miastro — Fase 6 — Carta Natal fiable

## Estado

Fase 6 iniciada.

La Fase 5 permanece cerrada.

La Fase 7 no está iniciada.

## Objetivo

Implementar el caso de uso funcional completo de cálculo de Carta Natal desde
una Persona persistida, con prioridad en:

- fiabilidad;
- reproducibilidad;
- persistencia de snapshots;
- validación externa;
- trazabilidad;
- idempotencia;
- invalidación segura.

## Flujo aprobado

Persona
→ DatosNacimiento
→ Instant UTC persistido
→ Astronomy Abstractions
→ Swiss Ephemeris
→ posiciones y velocidades
→ casas
→ ASC / MC
→ puntos derivados
→ aspectos de dominio
→ snapshot natal persistente

## Elegibilidad V1

La Carta Natal completa requiere:

- hora Exacta; o
- hora Aproximada con hora concreta.

No se crea carta natal completa para:

- Rango;
- Momento del día;
- Desconocida;
- hora ambigua sin elección;
- hora inexistente.

## Casas V1

- Placidus
- Koch

Default inicial:

- Placidus

## Restricciones

No implementar en Fase 6:

- rueda natal funcional;
- Revolución Solar;
- Revolución Lunar;
- Tránsitos;
- Progresiones;
- Sinastría;
- interpretación;
- informes astrológicos;
- exportación gráfica final;
- impresión astrológica final.

La siguiente fase será Fase 7 — Rueda natal geométrica, pero no se inicia
automáticamente.

## Política de hash

El hash de entradas natal es SHA-256 sobre una representación canónica
versionada (`miastro-natal-input-v1`).

No incluye datos privados de identidad.

## Orden canónico de objetos

1. Sol
2. Luna
3. Mercurio
4. Venus
5. Marte
6. Júpiter
7. Saturno
8. Urano
9. Neptuno
10. Plutón
11. Nodo Norte Verdadero
12. Nodo Sur
13. Lilith Media
14. Parte de la Fortuna
15. Quirón
16. Ceres
17. Palas
18. Juno
19. Vesta
20. ASC
21. MC

## Persistencia natal

El snapshot natal se almacena de forma normalizada:

- NatalCharts;
- NatalPlacements;
- NatalHouseCusps;
- NatalAspects.

Una Persona puede conservar múltiples cálculos históricos.

Estados:

- Current;
- Superseded;
- Invalidated.

`PersonId + InputHash` impide duplicados equivalentes.

## Asignación de casas

La casa de cada objeto se determina por las cúspides reales.

No se utilizan signos como sustituto de casas.

Regla de cúspide exacta:

- tolerancia numérica: 1e-9°;
- el objeto pertenece a la casa que comienza en esa cúspide.

## Día y noche

La sect natal se determina por el Sol respecto al horizonte:

- casas 7–12: Day;
- casas 1–6: Night.

No se utiliza la hora civil.

## Aspectos natales

Los aspectos se calculan exclusivamente con `MiastroV1AspectProfile`.

El orden de pares sigue `NatalObjectOrder`.

Nodo Norte, Nodo Sur, Lilith Media y Parte de Fortuna quedan excluidos según
el perfil V1.

## CalculateNatalChart

El caso de uso principal recibe:

- PersonId;
- HouseSystem, con Placidus como valor por defecto.

Flujo:

1. cargar Persona;
2. evaluar elegibilidad;
3. reutilizar Instant UTC persistido;
4. validar coordenadas;
5. construir fingerprint reproducible;
6. comprobar idempotencia;
7. calcular casas mediante `IHouseCalculator`;
8. calcular cuerpos mediante `IEclipticPositionCalculator`;
9. derivar Nodo Sur;
10. incorporar ASC y MC;
11. determinar Day/Night;
12. calcular Parte de Fortuna;
13. asignar casas;
14. calcular aspectos V1;
15. construir `AstrologicalChart` Natal;
16. persistir snapshot.

Application no conoce el adaptador Swiss concreto.

## CalculateNatalChart

El caso de uso principal recibe:

- PersonId;
- HouseSystem, con Placidus como valor por defecto.

Flujo:

1. cargar Persona;
2. evaluar elegibilidad;
3. reutilizar Instant UTC persistido;
4. validar coordenadas;
5. construir fingerprint reproducible;
6. comprobar idempotencia;
7. calcular casas mediante `IHouseCalculator`;
8. calcular cuerpos mediante `IEclipticPositionCalculator`;
9. derivar Nodo Sur;
10. incorporar ASC y MC;
11. determinar Day/Night;
12. calcular Parte de Fortuna;
13. asignar casas;
14. calcular aspectos V1;
15. construir `AstrologicalChart` Natal;
16. persistir snapshot.

Application no conoce el adaptador Swiss concreto.

## Composition root astronómico

Bootstrap registra productivamente:

- `IEclipticPositionCalculator`
  → `SwissEphemerisPositionCalculator`;
- `IHouseCalculator`
  → `SwissEphemerisHouseCalculator`;
- `IAstronomyEngineDiagnostics`
  → `SwissEphemerisDiagnostics`;
- `INatalCalculationMetadataProvider`
  → proveedor de composición de Fase 6.

Resolución de recursos:

Desarrollo:

- `src/Miastro.Infrastructure.SwissEphemeris/native/linux-x64/libswe.so`;
- `data/ephemeris/`.

Publish:

- `native/linux-x64/libswe.so`;
- `ephemeris/`.

Instalación Debian:

- `/usr/lib/miastro/native/libswe.so`;
- `/usr/share/miastro/ephemeris/`.

La identidad de efemérides utilizada por el snapshot es el SHA-256 del
`manifest.json`, por lo que un cambio de corpus modifica el fingerprint natal.

## E2E natal real

La Fase 6 valida ya el flujo real:

Persona persistida
→ BirthData persistido
→ Instant UTC persistido
→ Bootstrap
→ Swiss Ephemeris
→ posiciones y velocidades
→ casas
→ Nodo Sur derivado
→ ASC / MC
→ sect
→ Parte de Fortuna
→ aspectos
→ snapshot SQLite
→ cierre del contexto
→ reapertura
→ recuperación de carta vigente.

Existe además un E2E específico de hora Aproximada con sistema Koch.

Los tests E2E no utilizan datos personales reales.

## Invalidación

Las cartas vigentes se invalidan automáticamente si cambian entradas natales.

Cambios de identidad, contacto, nota privada, favorito o residencia no
invalidan la carta.

La invalidación y el historial se persisten en la misma unidad de trabajo que
la actualización de Persona.

## Recálculo e identidad

`RecalculateNatalChartUseCase` conserva la política de idempotencia:

- mismo input hash vigente: devuelve la snapshot existente;
- entradas distintas: `CalculateNatalChart` produce una nueva snapshot;
- una carta invalidada por modificación natal deja de ser vigente.

El fingerprint V2 incluye también precisión, identidad de localidad y
resolución histórica.

## Circuito de invalidación y recálculo

El flujo completo queda validado:

1. existe una Carta Natal Current;
2. cambia un dato natal relevante;
3. la carta vigente pasa a Invalidated;
4. deja de existir una Current;
5. RecalculateNatalChart vuelve a ejecutar el cálculo;
6. el nuevo input produce un hash diferente;
7. se crea una nueva snapshot Current;
8. la snapshot anterior permanece conservada como Invalidated;
9. el historial registra cálculo, invalidación y recálculo.

Los eventos de historial no incluyen nombre, hora natal, localidad, contacto ni
nota privada.

## Identidad reproducible de BirthData

Cada carta guarda:

- BirthDataVersion;
- BirthDataHash;
- precisión horaria;
- GeoNameId;
- offset histórico;
- selección de ambigüedad.

`BirthDataHash` identifica exclusivamente el snapshot histórico de nacimiento.

`InputHash` identifica el cálculo completo y puede variar, por ejemplo, entre
Placidus y Koch aun compartiendo el mismo `BirthDataHash`.

## Integridad de snapshot

Antes de persistirse, una carta natal completa valida:

- 21 placements canónicos;
- orden y unicidad;
- ASC y MC;
- Parte de Fortuna;
- Nodo Sur derivado correctamente;
- 12 cúspides;
- longitudes normalizadas;
- valores finitos;
- casas válidas;
- BirthDataHash coherente;
- ausencia de pares de aspectos duplicados.

La validación se aplica en Application y Persistence.

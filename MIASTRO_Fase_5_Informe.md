# MIASTRO — Fase 5 — Persistencia funcional y ficha de Persona

## Estado

**CERRADA**

La Fase 4 está oficialmente cerrada.

La Fase 5 queda iniciada sobre ese baseline.

No se considera cerrada hasta completar los 69 criterios de aceptación con:

- PASS: 69
- FAIL: 0
- PENDING: 0
- CI remoto final: SUCCESS

## Objetivo

Implementar persistencia funcional real de personas y una primera ficha de
Persona usable, conectando identidad, datos de nacimiento, localidad,
coordenadas, IANA TimeZoneId, resolución histórica, residencia, contacto,
nota privada, favoritos, última consulta, historial mínimo, edición y borrado.

## Restricciones

No implementar en esta fase:

- Carta Natal funcional;
- rueda astrológica;
- aspectos aplicados a una persona;
- revoluciones;
- tránsitos;
- progresiones;
- sinastría;
- interpretación;
- informes astrológicos;
- impresión astrológica final.

La Fase 6 no está iniciada.

## Criterios de aceptación

Estado final:

- PASS: 69
- FAIL: 0
- PENDING: 0

Los 69 criterios de aceptación quedan satisfechos.

El commit candidato superó el CI remoto completo antes de autorizar este
cierre. El commit oficial de cierre se valida nuevamente mediante CI remoto.

## Baseline técnico

Pendiente de completar tras la auditoría inicial de:

- DbContext actual;
- migraciones existentes;
- repositorios/casos de uso existentes;
- infraestructura de backup;
- shell Avalonia actual;
- estrategia de persistencia XDG;
- estructura de tests;
- empaquetado y CI heredados.

## Bloque 1B — Evidencia de dominio y persistencia

Añadida cobertura específica de Fase 5 para:

- Persona válida y nombre obligatorio;
- favorito y última consulta;
- precisión Exacta y Aproximada;
- Rango y validación de orden;
- Momento del día sin hora inventada;
- Desconocida sin Instant inventado;
- ambigüedad con elección explícita;
- migración desde esquema técnico de Fase 4;
- CRUD SQLite real;
- cascadas;
- índices funcionales.

Tests totales tras este bloque: 220.

## Bloque 2 — Application y repositorio funcional

Implementado:

- contratos de persistencia de Persona;
- DTOs/modelos de lectura;
- create/read/update/delete;
- búsqueda parcial por nombre/apellidos;
- filtros All/Recent/Favorites;
- órdenes por nombre, apellidos, última consulta y favorita;
- confirmación obligatoria de borrado;
- favorito;
- política explícita de última consulta;
- validación de precisión natal antes de persistir;
- repositorio EF sin exposición de entidades EF a UI.

## Bloque 3 — Geografía y tiempo natal

Implementado:

- búsqueda natal y de residencia con GeoNames;
- sin autoselección de homónimos;
- resolución histórica natal mediante la infraestructura de Fase 4;
- estados normal, ambiguo e inexistente;
- cobertura integrada inicial de tiempo natal.

## Bloque 4 — Orquestación natal y E2E headless

Implementado:

- selección explícita de localidad por GeoNameId;
- snapshot temporal persistible;
- resolución normal a Instant UTC;
- ambigüedad con candidato elegido y auditoría;
- hora desconocida sin Instant;
- invalidación explícita al cambiar datos natales;
- E2E SQLite de creación, persistencia y reapertura.

## Bloque 5 — Backup y privacidad

Implementado:

- backup SQLite consistente;
- inclusión de tablas funcionales de Fase 5;
- inclusión del historial de migraciones;
- protección contra sobrescritura accidental;
- tests de privacidad sobre logging;
- comprobación de ausencia de dependencias de red en Application/People.

## Bloque 6B — UI funcional básica de Persona

Implementado:

- shell con cabecera, panel lateral y ficha central;
- listado real persistido;
- búsqueda;
- filtros Todas/Recientes/Favoritas;
- orden por nombre, apellidos, última consulta y favoritas;
- Nueva persona;
- abrir y editar ficha;
- contacto y nota privada;
- favorito;
- última consulta;
- Guardar/Cancelar;
- borrado con confirmación explícita de dos pasos;
- estados Guardado/No guardado/error;
- ausencia de campos técnicos en la UI.

## Bloque 6C — UI nacimiento, localidad y residencia

Implementado:

- sección Nacimiento;
- precisión Exacta/Aproximada/Rango/Momento del día/Desconocida;
- búsqueda GeoNames integrada;
- selección explícita de localidad;
- resolución histórica visible;
- ambigüedad con dos elecciones explícitas;
- hora inexistente sin corrección automática;
- residencia actual con buscador reutilizado;
- ausencia de campos técnicos en pantalla.

## Bloque 7 — Endurecimiento funcional

Implementado:

- UpdateResidenceUseCase explícito;
- protección de cambios sin guardar;
- confirmación antes de descartar;
- historial relevante visible en ficha;
- mantenimiento del aislamiento Application/UI.

## Bloque 8 — Validación visible y accesibilidad

Implementado:

- validación previa al guardado;
- errores visibles por campo;
- validación de email;
- validación de fecha/hora/rango;
- obligatoriedad de localidad cuando corresponde;
- etiquetas accesibles en controles críticos;
- errores funcionales sin exposición de detalles técnicos.

## Bloque 9B — E2E GeoNames/XDG y DI definitiva

Implementado:

- eliminación de la factoría reflectiva temporal de Geography;
- registro explícito GeoNamesCatalogOptions/SqliteLocationSearchService;
- política de resolución dev/publish/instalado para geonames.sqlite;
- E2E con catálogo GeoNames real;
- resolución histórica real;
- creación de Persona en SQLite XDG;
- cierre y recreación completa del contenedor;
- recuperación íntegra de Persona tras reapertura.

## Bloque 10B — Remate candidato de cierre

Completado:

- ADR 007–011 aceptados;
- empaquetado nominal Fase 5;
- versión Debian candidata 0.5.0~phase5-1;
- CI actualizado a artefactos Fase 5;
- verificación CI de preservación XDG tras `dpkg -r`;
- reinstalación del mismo paquete sin pérdida de `miastro.db`;
- informe preparado como candidato 68 PASS / 1 PENDING.

Evidencia del candidato:

- Commit candidato: `df449db7ad8c95e76e398eccaf84a6b57f98335d`
- Workflow candidato: `32474831494`
- CI candidato: SUCCESS

Con esta evidencia se autoriza el cierre documental de Fase 5. La Fase 6 no
queda iniciada.


## Cierre oficial de Fase 5

Resultado de aceptación:

- PASS: 69
- FAIL: 0
- PENDING: 0
- Tests locales del candidato: 254/254
- Commit candidato: `df449db7ad8c95e76e398eccaf84a6b57f98335d`
- CI candidato: `32474831494` — SUCCESS
- Fase 5: CERRADA
- Fase 6: NO INICIADA

No se implementan funcionalidades correspondientes a Fase 6.

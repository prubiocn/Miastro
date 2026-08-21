
## Casos de uso de Application

Implementados:

- CreatePerson;
- UpdatePerson;
- GetPerson;
- SearchPeople;
- DeletePerson;
- SetFavorite;
- RecordPersonConsultation.

`Recent` significa personas que ya tienen `LastConsultationAtUtc`, ordenadas
por esa fecha de forma descendente cuando se usa el orden correspondiente.

La eliminación exige confirmación explícita en el caso de uso.

UI recibe modelos de lectura y nunca entidades EF.

### Orden por última consulta y SQLite

EF Core SQLite no traduce `ORDER BY` sobre `DateTimeOffset`.

Miastro mantiene `LastConsultationAtUtc` como `DateTimeOffset` para conservar
semántica temporal correcta. Para el orden `LastConsultation`, el repositorio
proyecta únicamente el modelo ligero de lista y realiza el orden estable en
memoria, con límite público máximo de 500 resultados.

## Integración geográfica y temporal

La ficha reutiliza `ILocationSearchService` para nacimiento y residencia.
No existe autoselección de homónimos.

Las horas Exacta/Aproximada reutilizan `IHistoricalTimeResolver`.
Ambiguous devuelve dos candidatos y Skipped no inventa un Instant.

## Orquestación natal persistible

La resolución de Fase 4 se transforma explícitamente en el snapshot
persistible de `BirthData`.

Reglas:

- `Resolved` guarda offset histórico, TZDB e Instant UTC.
- `Ambiguous` requiere selección explícita 1/2 y timestamp de auditoría.
- `Skipped` no genera Instant y no puede guardarse como nacimiento resuelto.
- `Unknown` no intenta ejecutar el resolver.
- cambios en fecha, hora, precisión o ubicación invalidan el snapshot anterior.

La selección de una localidad se realiza mediante `GeoNameId` explícito después
de presentar al usuario los resultados de búsqueda.

## Backup

La base funcional de Persona queda incluida en la copia SQLite consistente
mediante la API nativa de backup de SQLite.

El backup incluye:

- People;
- BirthData;
- CurrentResidences;
- PersonHistory;
- historial de migraciones EF.

El servicio no sobrescribe silenciosamente un backup existente.

## Privacidad

Teléfono, email y nota privada son datos locales.

Reglas:

- no se registran en logs;
- no se incluyen en errores técnicos;
- los casos de uso de Persona no utilizan clientes HTTP;
- nacimiento y localidad no se envían a servicios externos.

## Backup

La base funcional de Persona queda incluida en la copia SQLite consistente
mediante la API nativa de backup de SQLite.

El backup incluye:

- People;
- BirthData;
- CurrentResidences;
- PersonHistory;
- historial de migraciones EF.

El servicio no sobrescribe silenciosamente un backup existente.

## Privacidad

Teléfono, email y nota privada son datos locales.

Reglas:

- no se registran en logs;
- no se incluyen en errores técnicos;
- los casos de uso de Persona no utilizan clientes HTTP;
- nacimiento y localidad no se envían a servicios externos.

## UI de Personas

La ventana principal evoluciona al patrón de Fase 5:

- cabecera;
- panel lateral de personas;
- área central de ficha;
- búsqueda;
- filtros y orden;
- nueva persona;
- edición;
- favorito;
- última consulta;
- guardado/cancelación;
- eliminación con confirmación explícita.

La UI consume casos de uso de Application mediante scopes cortos y no captura
DbContext ni repositorios scoped dentro del ViewModel singleton.

## UI de nacimiento y residencia

La ficha permite seleccionar explícitamente una localidad GeoNames tanto para
nacimiento como para residencia.

La UI muestra nombres humanos de localidad, región y país. No muestra IDs,
versiones TZDB, offsets internos ni rutas técnicas.

Para hora Exacta/Aproximada:

- se exige fecha, hora y localidad;
- la resolución histórica es explícita;
- una hora ambigua presenta dos opciones al usuario;
- la elección queda auditada;
- una hora inexistente se muestra como error funcional y no se inventa hora.

Rango, Momento del día y Desconocida no generan un Instant artificial.

## Endurecimiento de ficha

La edición protege cambios pendientes: Cancelar requiere una segunda
confirmación cuando existen modificaciones sin guardar.

La ficha presenta además el historial mínimo de eventos relevantes de Persona.

Application expone un caso de uso específico `UpdateResidenceUseCase`,
manteniendo la actualización de residencia separada de la UI y preservando
el resto de los datos de Persona.

## Validación y accesibilidad

La ficha valida antes de persistir y muestra errores visibles asociados a los
campos principales:

- nombre;
- apellidos;
- email;
- fecha de nacimiento;
- hora o rango horario;
- localidad de nacimiento;
- residencia.

Los controles críticos incluyen nombres accesibles y siguen la navegación
estándar por teclado de Avalonia. Los mensajes de error no muestran detalles
internos de persistencia, GeoNames o TZDB.

## Reapertura y recuperación XDG

La persistencia funcional de Persona se valida a través del mismo
`IApplicationPaths` utilizado por la aplicación.

El flujo E2E cubre:

1. búsqueda real en el catálogo GeoNames;
2. selección explícita de localidad;
3. resolución histórica de la hora;
4. creación de Persona;
5. escritura en `miastro.db` dentro de XDG Data;
6. cierre completo del contenedor DI;
7. creación de un nuevo contenedor;
8. migración/reapertura de la misma base;
9. recuperación íntegra de Persona, nacimiento, residencia e historial.

Geography queda registrado mediante tipos explícitos y se elimina la factoría
reflectiva temporal utilizada durante la integración inicial de Fase 5.

# MIASTRO_Arquitectura_Tecnica_Definitiva

**Proyecto:** Miastro  
**Documento:** Arquitectura técnica definitiva previa a implementación  
**Fecha:** 20/08/2026  
**Estado:** Candidata a aprobación para iniciar Fase 1  
**Plataforma objetivo:** Linux Ubuntu  
**Documento rector:** `MIASTRO_Reglas_y_Especificaciones_Consolidadas.md`

> **Regla de prevalencia:** `MIASTRO_Reglas_y_Especificaciones_Consolidadas.md` continúa siendo el documento rector. Esta arquitectura traduce sus requisitos funcionales, doctrinales, gráficos y de navegación a una solución técnica para Linux Ubuntu.
>
> **Trazabilidad:** la propuesta anterior `MIASTRO_Arquitectura_Tecnica_Propuesta.md` queda conceptualmente sustituida por este documento. Las decisiones basadas en WPF, Windows, Windows Print Spooler o APIs específicas de Windows quedan anuladas.

---

## 0. Resumen ejecutivo

Miastro se construirá como una **aplicación personal de escritorio para Linux Ubuntu**, offline-first y organizada como **Modular Monolith + Clean Architecture / Ports & Adapters**.

La pila principal queda fijada como:

- **Lenguaje:** C#.
- **Runtime:** .NET 10 LTS.
- **Interfaz de escritorio:** Avalonia UI.
- **Patrón de presentación:** MVVM.
- **Persistencia:** SQLite + Entity Framework Core.
- **Motor astronómico:** Swiss Ephemeris mediante adaptador nativo propio.
- **Tiempo y zonas horarias:** Noda Time + IANA TZDB.
- **Catálogo geográfico offline:** GeoNames preprocesado a SQLite.
- **Motor gráfico:** modelo de escena propio + SkiaSharp.
- **Exportación de rueda:** PNG y PDF vectorial desde el mismo modelo de escena.
- **Informes PDF:** QuestPDF como motor principal de composición documental, con fuentes internas controladas y gráficos vectoriales procedentes del motor de rueda.
- **Impresión:** generación previa de PDF + XDG Desktop Portal Print, integrado con el sistema de impresión Linux/CUPS.
- **Empaquetado Ubuntu:** paquete `.deb` autocontenido por arquitectura, con runtime .NET y dependencias privadas necesarias incluidas.
- **Datos de usuario:** rutas XDG.
- **Fuentes:** empaquetadas y cargadas explícitamente; la aplicación no dependerá de fuentes instaladas en el sistema.
- **Glifos astrológicos:** recursos vectoriales propios del producto o conjunto vectorial empaquetado y licenciado, no dependiente de Unicode/fonts del sistema.

Se mantienen como principios estructurales:

1. La interfaz no conoce Swiss Ephemeris.
2. El dominio no depende de Avalonia, EF Core, Skia ni librerías nativas.
3. La interpretación no depende del motor astronómico.
4. Los informes no dependen de controles de pantalla.
5. La posición astrológica real nunca se altera para resolver colisiones gráficas.
6. La posición gráfica es un resultado derivado, determinista y reproducible.
7. Los glifos nunca se solapan.
8. ASC y MC permanecen siempre visibles.
9. La sinastría se calcula de forma canónica y simétrica.
10. La misma escena gráfica alimenta pantalla, PNG, PDF e impresión.
11. Efemérides, TZDB, catálogo geográfico, reglas interpretativas y plantillas se versionan.

No se ha detectado ninguna incompatibilidad seria que impida usar **C# + .NET 10 + Avalonia UI** en Ubuntu. La combinación es técnicamente adecuada para Miastro.

---

# 1. Arquitectura general

## 1.1 Tipo de arquitectura

**Monolito modular de escritorio con Clean Architecture y Ports & Adapters.**

Miastro no necesita microservicios ni un backend remoto en V1.

La aplicación debe:

- iniciar y funcionar sin conexión;
- mantener la latencia de consulta baja;
- conservar los datos bajo control del usuario;
- encapsular dependencias nativas;
- ser sencilla de instalar y mantener;
- permitir ampliar técnicas astrológicas sin reconstruir la base.

El monolito modular permite una única aplicación de escritorio, pero con límites internos fuertes.

---

## 1.2 Capas y módulos

### `Miastro.UI.Avalonia`

Responsable de:

- ventana principal;
- navegación;
- panel lateral de personas;
- pantalla central de persona;
- Natal;
- Revolución Solar;
- Revolución Lunar;
- Tránsitos;
- Progresiones;
- Sinastría;
- pestañas de análisis;
- controles de carta;
- formularios;
- accesibilidad;
- teclado;
- feedback visual;
- previsualización.

No debe:

- invocar Swiss Ephemeris;
- abrir SQLite directamente;
- calcular aspectos;
- decidir reglas de interpretación;
- calcular offsets históricos;
- modificar posiciones reales de glifos.

### `Miastro.Application`

Casos de uso y orquestación.

Ejemplos:

- crear persona;
- editar persona;
- resolver localidad;
- calcular natal;
- calcular revolución;
- calcular tránsitos;
- calcular progresión;
- crear sinastría;
- generar informe;
- exportar;
- imprimir;
- realizar copia de seguridad.

Responsable de:

- coordinación;
- transacciones;
- validación de caso de uso;
- selección de estrategias;
- publicación de eventos internos;
- modelos de lectura.

### `Miastro.Domain`

Núcleo independiente.

Subdominios:

- `People`
- `Astrology`
- `Geography`
- `Interpretation`
- `Reports`
- `Shared`

Contiene:

- longitudes;
- signos;
- casas;
- cuerpos;
- puntos;
- aspectos;
- regencias;
- cartas;
- relaciones;
- perfiles;
- invariantes.

No contiene referencias a:

- Avalonia;
- EF Core;
- SQLite;
- Skia;
- Swiss Ephemeris;
- D-Bus;
- CUPS;
- filesystem Linux concreto.

### `Miastro.Astronomy.Abstractions`

Puertos del cálculo astronómico.

Define contratos para:

- posiciones;
- velocidades;
- casas;
- ángulos;
- retornos;
- capacidades del motor;
- versión;
- disponibilidad de ficheros.

### `Miastro.Infrastructure.SwissEphemeris`

Adaptador nativo.

Responsable de:

- cargar `libswe.so`;
- P/Invoke;
- mapear flags;
- validar ABI;
- configurar efemérides;
- traducir códigos de error;
- exponer resultados normalizados.

### `Miastro.Infrastructure.Time`

Implementa:

- Noda Time;
- IANA TZDB;
- resolución de `LocalDateTime`;
- ambigüedad histórica;
- offsets;
- conversión a `Instant`.

### `Miastro.Infrastructure.Geography`

Implementa:

- GeoNames offline;
- búsqueda;
- ranking;
- regiones;
- países;
- coordenadas;
- zona IANA.

### `Miastro.Infrastructure.Persistence`

Implementa:

- SQLite;
- Entity Framework Core;
- migraciones;
- repositorios;
- transacciones;
- backups;
- integridad.

### `Miastro.Graphics`

Motor gráfico independiente de UI.

Submódulos:

- `Geometry`
- `Layout`
- `Collision`
- `Scene`
- `Typography`
- `Glyphs`
- `Styles`

Produce una escena inmutable y testeable.

### `Miastro.Graphics.Skia`

Backend de render.

Convierte el modelo de escena a:

- superficie raster para pantalla;
- PNG;
- SVG cuando se necesite interoperabilidad vectorial;
- PDF vectorial para una rueda aislada.

No debe contener cálculo astrológico.

### `Miastro.Interpretation`

Implementa:

- extracción de hechos;
- grafo de evidencias;
- reglas;
- priorización;
- síntesis;
- catálogo de textos.

### `Miastro.Reports`

Implementa:

- plantillas;
- secciones;
- modelo documental;
- composición;
- integración de rueda vectorial;
- previsualización;
- PDF.

### `Miastro.Infrastructure.Printing.Linux`

Implementa:

- XDG Desktop Portal Print mediante D-Bus;
- flujo de impresión PDF-first;
- integración con el entorno de escritorio;
- gestión de errores de impresión.

### `Miastro.Infrastructure.Platform.Linux`

Implementa:

- rutas XDG;
- apertura de ficheros/directorios mediante mecanismos Linux;
- información de plataforma;
- permisos;
- detección de servicios necesarios.

---

## 1.3 Flujo de cálculo natal

```text
Avalonia View
    ↓
ViewModel
    ↓
Application Use Case
    ↓
Geography + Historical Time
    ↓
Astronomy Port
    ↓
Swiss Ephemeris Adapter
    ↓
Domain Chart
    ↓
Aspect Engine
    ↓
Persistence
    ↓
Chart Scene Builder
    ↓
Skia Renderer
    ↓
Avalonia View
```

---

## 1.4 Flujo de interpretación

```text
Domain Chart(s)
    ↓
Fact Extractor
    ↓
Interpretation Fact Graph
    ↓
Rule Engine
    ↓
Evidence Set
    ↓
Semantic Blocks
    ↓
Text Realizer
    ↓
Interpretation Result
```

---

## 1.5 Flujo de informe

```text
Report Request
    ↓
Report Data Assembler
    ↓
Calculated Snapshots
    +
Interpretation Result
    +
Wheel Scene
    ↓
Report Document Model
    ↓
QuestPDF Renderer
    ↓
PDF
    ├─ Preview
    ├─ Export
    └─ XDG Print Portal → CUPS
```

---

## 1.6 Dependencias permitidas

- UI → Application.
- Application → Domain.
- Application → abstracciones/puertos.
- Infrastructure → puertos de Application/Domain.
- Graphics → modelos de lectura del dominio.
- Reports → modelos de informe, interpretación y escenas exportables.
- Export → escena gráfica/modelo documental.
- Linux adapters → puertos de plataforma.

---

## 1.7 Dependencias prohibidas

- Domain → Avalonia.
- Domain → EF Core.
- Domain → SkiaSharp.
- Domain → Swiss Ephemeris.
- Domain → D-Bus/CUPS.
- UI → Swiss Ephemeris.
- UI → `DbContext`.
- UI → `libswe.so`.
- Interpretation → Swiss Ephemeris.
- Reports → controles Avalonia.
- Graphics → SQLite.
- Persistence → UI.
- Astronomy → UI.
- Printing → lógica interpretativa.

---

# 2. Tecnología base

## 2.1 Pila aprobable

| Área | Tecnología |
|---|---|
| Sistema objetivo | Linux Ubuntu |
| Lenguaje | C# |
| Runtime | .NET 10 LTS |
| UI | Avalonia UI |
| Arquitectura UI | MVVM |
| Base de datos | SQLite |
| ORM | Entity Framework Core |
| Efemérides | Swiss Ephemeris |
| Interop nativo | P/Invoke + `NativeLibrary` |
| Fechas históricas | Noda Time |
| TZ | IANA TZDB |
| Geografía | GeoNames offline |
| Gráficos | SkiaSharp |
| Escena | Modelo propio de Miastro |
| PNG | SkiaSharp |
| PDF rueda | Skia PDF |
| PDF informes | QuestPDF |
| Impresión | PDF-first + XDG Desktop Portal Print |
| Sistema de impresión | CUPS, a través de integración de escritorio |
| Empaquetado | `.deb` autocontenido |
| Convenciones de datos | XDG Base Directory |
| Logging | `Microsoft.Extensions.Logging` + sink local |
| DI | `Microsoft.Extensions.DependencyInjection` |
| Configuración | `Microsoft.Extensions.Configuration` |
| Tests | xUnit + assertions + snapshots visuales |

---

## 2.2 .NET 10 LTS

.NET 10 es una versión LTS y está soportada hasta noviembre de 2028.

Miastro se publicará como aplicación **self-contained**, de manera que el usuario no tenga que instalar previamente el runtime .NET correcto.

V1 debe definir al menos:

- `linux-x64`.

La arquitectura no impide añadir `linux-arm64` posteriormente, pero cada arquitectura tendrá su paquete y binarios nativos propios.

---

# 3. Avalonia UI en Ubuntu

## 3.1 Decisión

**Avalonia UI es el framework de interfaz principal de Miastro.**

No se usará Avalonia XPF ni ninguna capa de compatibilidad con WPF.

Se usará Avalonia nativo.

---

## 3.2 Integración con .NET 10

Avalonia soporta escritorio Linux sobre .NET moderno y su documentación actual contempla .NET 10.

La aplicación deberá fijar:

- versión concreta de Avalonia;
- versión concreta de .NET SDK;
- lock de dependencias;
- actualización controlada.

No se adoptarán automáticamente actualizaciones mayores de Avalonia.

---

## 3.3 Backend Linux

En Linux, Avalonia usa X11 como backend estable predeterminado.

Wayland nativo existe, pero en la versión documentada actualmente se considera experimental.

### Política V1

- backend soportado principal: X11/XWayland;
- GNOME/Ubuntu Wayland funcionará vía XWayland cuando corresponda;
- Wayland nativo no será dependencia funcional de V1;
- se podrá habilitar y validar en una fase posterior.

Esto reduce riesgo sin impedir el uso normal en Ubuntu moderno.

---

## 3.4 MVVM

La estructura de UI seguirá MVVM.

### View

Responsable de:

- composición visual;
- binding;
- accesibilidad;
- estados visuales;
- layout.

### ViewModel

Responsable de:

- comandos;
- estado de pantalla;
- selección;
- navegación;
- loading/error states;
- proyección de modelos de Application.

No contiene SQL ni cálculos astronómicos.

### Modelos de dominio

No se enlazan directamente cuando eso exponga comportamiento interno.

La UI recibe modelos específicos de lectura.

---

## 3.5 Pantalla principal

La interfaz respeta:

- una pantalla central por persona;
- cabecera global;
- panel izquierdo;
- área central;
- rueda protagonista;
- panel derecho;
- mínimo uso de ventanas flotantes.

Los diálogos nativos solo se usarán cuando sean funcionalmente necesarios:

- apertura/guardado;
- impresión;
- confirmaciones de sistema.

---

## 3.6 DPI y escalado

Miastro debe ser **DPI-aware por diseño**.

Reglas:

- layouts expresados en unidades lógicas de Avalonia;
- no asumir 96 DPI físicos;
- la rueda recibe tamaño lógico + factor de render;
- raster de pantalla se genera a resolución física adecuada;
- exportación usa dimensiones/DPI explícitos, independientes del monitor.

Debe probarse:

- 100 %;
- 125 %;
- 150 %;
- 200 %;
- cambios entre monitores cuando proceda.

La geometría del dominio gráfico permanece en coordenadas normalizadas o lógicas, no en píxeles de monitor.

---

## 3.7 Teclado

Todos los flujos principales deben poder realizarse con teclado.

Requisitos:

- orden de tabulación coherente;
- foco visible;
- `Esc` para cerrar contexto secundario cuando proceda;
- navegación por lista de personas;
- navegación por pestañas;
- activación de acciones;
- accesos rápidos aprobados;
- equivalentes de teclado para controles de rueda relevantes.

La interacción del ratón nunca debe ser la única vía para una función esencial.

---

## 3.8 Accesibilidad

Avalonia en Linux puede exponer información de accesibilidad a través de AT-SPI2 cuando existe un bus D-Bus de sesión y un servicio de accesibilidad activo.

Miastro debe:

- definir nombres accesibles;
- roles;
- estados;
- descripciones;
- foco;
- lectura de tablas;
- equivalentes textuales de la rueda.

La rueda gráfica no puede ser el único vehículo de información.

Su contenido esencial estará disponible también en:

- Posiciones;
- Aspectos;
- Datos;
- descripciones accesibles.

Pruebas manuales:

- Orca;
- Accerciser.

---

## 3.9 Controles complejos

La rueda no se implementará como cientos de controles Avalonia independientes.

Se utilizará:

- un control de host;
- una escena gráfica;
- un renderer;
- un índice de hit-testing.

Ventajas:

- rendimiento;
- determinismo;
- igualdad con exportación;
- control de colisiones;
- menor coste de layout UI.

Las interacciones se resuelven mediante la escena:

`pointer coordinate -> hit-test -> SceneElementId -> ViewModel selection`

---

## 3.10 Integración con Skia

Avalonia ya utiliza Skia en Linux, pero Miastro **no dependerá de APIs internas de render de Avalonia**.

El motor gráfico será independiente.

Para la rueda:

1. Miastro calcula `WheelScene`.
2. `Miastro.Graphics.Skia` renderiza la escena.
3. La UI presenta el resultado.
4. El hit-testing usa geometría de escena.

Esto evita que un cambio interno de Avalonia rompa el motor gráfico.

---

# 4. Motor astronómico

## 4.1 Motor aprobado

**Swiss Ephemeris.**

Se mantiene completamente aislado.

Contrato conceptual principal:

`IAstronomicalEphemeris`

Capacidades:

- posiciones;
- velocidades;
- casas;
- ASC;
- MC;
- cuerpos menores;
- retorno de longitud;
- versión;
- diagnóstico.

---

## 4.2 Configuración base V1

- tropical;
- geocéntrico;
- longitud eclíptica;
- posiciones aparentes;
- velocidad;
- sin topocentrismo;
- Placidus;
- Koch;
- Lilith Media;
- Nodo Verdadero.

Cada resultado conserva un `CalculationProfileVersion`.

---

## 4.3 Nodo Verdadero

Decisión cerrada:

- Nodo Norte = **Nodo Verdadero**.
- Nodo Sur = oposición exacta:
  - `normalize(NodoNorte + 180°)`.

El Nodo Sur se presenta como entidad astrológica propia, aunque su posición sea derivada.

No se solicita de forma independiente al motor si no es necesario.

---

## 4.4 Integración Swiss Ephemeris en Linux

### Biblioteca

Se compilará o incorporará una versión validada de:

`libswe.so`

para cada arquitectura soportada.

### Estrategia

- construir con `-fPIC`;
- producir shared library;
- versionarla junto a Miastro;
- no instalarla globalmente;
- no depender de una `libswe.so` del sistema.

### Carga

Se usará P/Invoke con resolución controlada.

La carga nativa debe:

1. determinar arquitectura;
2. construir ruta interna autorizada;
3. verificar que el fichero existe;
4. validar hash;
5. cargar con `NativeLibrary.Load`;
6. registrar el handle/resolver;
7. ejecutar una llamada de diagnóstico;
8. comprobar versión esperada.

No depender de modificar globalmente `LD_LIBRARY_PATH`.

---

## 4.5 ABI

Se creará una frontera de interop mínima.

Reglas:

- tipos C explícitos;
- `CallingConvention.Cdecl` cuando proceda;
- buffers dimensionados;
- strings codificados de forma explícita;
- no exponer structs nativos al dominio;
- tests de smoke ABI en CI Ubuntu.

---

## 4.6 Archivos de efemérides

Recursos instalados de solo lectura:

```text
/usr/share/miastro/ephemeris/
```

o equivalente dentro del árbol de recursos del paquete.

La aplicación pasa esa ruta explícitamente a Swiss Ephemeris.

No se solicitará al usuario elegir carpetas.

---

## 4.7 Integridad

Se distribuirá un manifiesto con:

- fichero;
- tamaño;
- hash;
- versión;
- rango temporal previsto.

Al iniciar el subsistema astronómico:

- validar presencia;
- validar integridad;
- validar biblioteca;
- registrar versión.

---

## 4.8 Fallo de dependencia

Si falta o está corrupta `libswe.so` o un fichero obligatorio:

- Miastro puede abrir;
- las operaciones de cálculo quedan inhabilitadas;
- se muestra un error claro;
- se registra diagnóstico técnico;
- nunca se producen posiciones parciales presentadas como válidas.

La aplicación debe distinguir:

- biblioteca no cargable;
- ABI incompatible;
- fichero de efemérides ausente;
- fichero corrupto;
- rango no soportado;
- error de cálculo.

---

## 4.9 Empaquetado Swiss Ephemeris

La librería nativa y sus datos forman parte del paquete `.deb`.

No se declarará una dependencia del sistema a un paquete Swiss Ephemeris externo.

Esto garantiza:

- versión reproducible;
- instalación limpia;
- menor variabilidad;
- mismo resultado entre equipos.

La licencia de Swiss Ephemeris queda documentada y debe revisarse antes de cualquier distribución a terceros.

Para la herramienta personal no bloquea el desarrollo inicial.

---

# 5. Formato interno del cálculo

## 5.1 `EclipticPosition`

Campos conceptuales:

- objeto;
- longitud [0,360);
- latitud;
- distancia;
- velocidad longitudinal;
- velocidad latitudinal;
- velocidad radial;
- instante;
- frame;
- flags efectivos;
- versión motor.

---

## 5.2 `AstrologicalPlacement`

- `ObjectId`
- `Longitude`
- `Sign`
- `DegreeInSign`
- `House`
- `Speed`
- `IsRetrograde`
- `Source`

---

## 5.3 `HouseSet`

- sistema;
- 12 cúspides;
- ASC;
- MC;
- DSC derivado;
- IC derivado;
- localización;
- instante;
- datos auxiliares necesarios para auditoría.

---

## 5.4 Velocidades y retrogradación

La retrogradación se determina desde velocidad real:

`LongitudeSpeed < 0`

No mediante posiciones redondeadas.

---

## 5.5 Parte de la Fortuna

Cálculo en dominio.

Carta diurna:

`ASC + Luna - Sol`

Carta nocturna:

`ASC + Sol - Luna`

Normalización:

`[0,360)`.

La decisión día/noche debe ser una regla explícita y testeada a partir de la relación solar con el horizonte.

---

## 5.6 Quirón y asteroides

- Quirón;
- Ceres;
- Palas;
- Juno;
- Vesta.

El adaptador es responsable de IDs externos.

El dominio usa IDs propios.

---

## 5.7 Revoluciones

Búsqueda de retorno por longitud real.

Pipeline:

1. objetivo natal;
2. ventana inicial;
3. diferencia angular firmada;
4. detección de intervalo;
5. búsqueda/refinamiento;
6. validación del error final.

Nunca por igualdad de grado redondeado.

---

## 5.8 Tránsitos

Separar:

- snapshot;
- eventos temporales.

Evento:

- entrada en orbe;
- exactitud;
- salida de orbe.

---

## 5.9 Progresiones secundarias

Regla aprobada:

**1 día después del nacimiento = 1 año de vida.**

Pendientes no bloqueantes:

- método exacto de ASC/MC progresados;
- convención matemática exacta para fracciones temporales.

La arquitectura define estrategias sustituibles.

No se debe fijar ninguna de las dos por accidente en código general.

---

## 5.10 Sinastría

Dos cartas canónicas.

Se calculan:

- aspectos cruzados;
- activación A→B;
- activación B→A;
- ejes;
- patrones recíprocos.

Clave canónica:

`ordered(PersonId1, PersonId2)`

La elección de quién aparece dentro/fuera de la rueda es una propiedad de presentación.

---

# 6. Modelo de datos

## 6.1 Principio

Persistir:

- datos de entrada;
- identidad;
- decisiones del usuario;
- trabajos guardados;
- historial;
- snapshots relevantes;
- versiones técnicas que afecten reproducibilidad.

Calcular bajo demanda:

- geometría;
- selección;
- layout visual;
- derivados baratos;
- vistas temporales no guardadas.

---

## 6.2 Persona

Responsabilidad:

agregado raíz del usuario.

Campos:

- Id;
- nombre;
- apellidos;
- teléfono;
- email;
- nota;
- favorita;
- timestamps;
- última consulta.

Relaciones:

- DatosNacimiento;
- ResidenciaActual;
- trabajos;
- informes;
- historial.

Persistencia: sí.

---

## 6.3 DatosNacimiento

Campos:

- fecha local;
- hora local;
- precisión;
- rango cuando proceda;
- localidad;
- coordenadas;
- zona IANA;
- offset usado;
- instante UTC;
- versión TZDB;
- elección ante ambigüedad si existió.

Persistencia: sí.

---

## 6.4 Localidad

Campos:

- GeoNameId;
- nombre;
- nombre ASCII;
- nombres alternativos;
- país;
- admin1;
- admin2;
- latitud;
- longitud;
- IANA zone;
- población;
- feature code.

Persistencia:

catálogo de solo lectura.

---

## 6.5 ResidenciaActual

- localidad;
- país;
- región;
- coordenadas;
- zona IANA;
- fecha de actualización.

Persistencia: sí.

---

## 6.6 CartaNatal

- Id;
- PersonId;
- instante;
- ubicación;
- sistema de casas;
- perfil;
- motor/versiones;
- posiciones;
- casas;
- ángulos;
- hash de entradas;
- fecha de cálculo.

Persistencia: sí.

---

## 6.7 RevolucionSolar

- persona;
- año;
- objetivo solar natal;
- localidad;
- instante exacto;
- carta;
- perfil;
- error residual;
- referencia natal.

---

## 6.8 RevolucionLunar

- persona;
- periodo;
- objetivo lunar;
- localidad;
- instante;
- carta;
- referencia natal.

---

## 6.9 Transito

`TransitWork`:

- persona;
- instante/intervalo;
- configuración;
- natal de referencia;
- eventos;
- notas;
- fecha.

Snapshots no guardados se calculan bajo demanda.

---

## 6.10 Progresion

- persona;
- fecha objetivo;
- método Secondary;
- convención temporal;
- estrategia de ángulos;
- instante equivalente;
- carta;
- referencia natal.

---

## 6.11 Sinastria

- id;
- PersonLowId;
- PersonHighId;
- cartas de referencia;
- aspectos cruzados;
- activaciones dirección 1;
- activaciones dirección 2;
- configuración;
- fecha.

El contenido semántico no depende del orden de carga.

---

## 6.12 Informe

- id;
- tipo;
- personas;
- trabajos;
- versión plantilla;
- versión reglas;
- versión textos;
- modelo estructurado;
- fecha;
- estado;
- hash;
- exportaciones opcionales.

Persistir el modelo, no solo el PDF.

---

## 6.13 Historial

- id;
- persona;
- tipo;
- referencia;
- fecha;
- descripción;
- metadatos.

No registrar cada interacción visual.

---

## 6.14 Configuracion

- sistema de casas predeterminado;
- preferencias exportación;
- impresión;
- versión datasets;
- idioma;
- valores configurables aprobados.

---

## 6.15 PreferenciasVisuales

- modo Consulta/Presentación;
- puntos opcionales;
- etiquetas;
- estilo;
- tamaño dentro de límites;
- panel;
- zoom.

Invariantes:

- ASC visible;
- MC visible;
- Quirón visible por defecto;
- aspectos visibles por defecto.

---

# 7. Catálogo geográfico offline y TZDB

## 7.1 GeoNames

Dataset de base:

- `cities500`;
- `alternateNamesV2`;
- `countryInfo`;
- `admin1CodesASCII`;
- `admin2Codes`;
- zonas IANA.

Se preprocesa en construcción/release.

Resultado:

`geonames.sqlite` de solo lectura.

---

## 7.2 Búsqueda

Índices:

- nombre normalizado;
- ASCII;
- alternativos;
- país;
- admin;
- FTS5.

Ranking:

1. exacto;
2. alternativo exacto;
3. prefijo;
4. FTS;
5. jerarquía/población como desempate.

Nunca seleccionar automáticamente una homónima solo por población.

---

## 7.3 Presentación de ambiguos

Formato:

`Localidad — Región — País`

Si no basta:

- provincia/admin2;
- coordenadas.

---

## 7.4 Zona histórica

Flujo:

```text
LocalDate + LocalTime
+ IanaTimeZoneId
→ Noda Time MapLocal
→ ZonedDateTime
→ Instant
```

---

## 7.5 Política aprobada de horas ambiguas/inexistentes

Obligatoria:

- detectar automáticamente;
- explicar al usuario;
- mostrar alternativas históricas válidas;
- exigir elección cuando proceda;
- guardar elección;
- guardar offset;
- guardar versión TZDB;
- no resolver silenciosamente.

Para una hora inexistente, la UI debe indicar el salto horario y las opciones permitidas por la política funcional aprobada para ese caso concreto.

---

## 7.6 Empaquetado

Recursos instalados:

```text
/usr/share/miastro/geodata/
```

La base de GeoNames se abre en modo lectura.

TZDB:

- se usa la versión empaquetada/embebida por Noda Time;
- se registra su `VersionId`;
- no se depende de la tzdata del sistema para reproducir cálculos.

---

## 7.7 Actualización

V1:

- GeoNames y TZDB se actualizan con una nueva versión del paquete Miastro;
- no hay actualización automática de red dentro de la aplicación.

Futuro:

se puede añadir un mecanismo firmado de datasets sin alterar el dominio.

---

# 8. Motor gráfico de la rueda

## 8.1 Arquitectura

### `ChartGeometry`

Matemática:

- ángulos;
- radios;
- sectores;
- puntos;
- arcos.

### `WheelLayout`

Decide:

- posición visual;
- niveles;
- etiquetas;
- colisiones;
- conectores.

### `WheelScene`

Escena inmutable:

- círculos;
- arcos;
- líneas;
- paths;
- texto;
- glifos;
- grupos;
- clips;
- metadatos de hit-test.

### `Renderer`

Backends:

- Skia pantalla;
- PNG;
- SVG de intercambio vectorial;
- PDF.

---

## 8.2 Posición real y visual

Cada elemento tendrá, como mínimo:

- `ObjectId`;
- `TrueLongitude`;
- `TrueAnchorPoint`;
- `DisplayAngle`;
- `DisplayRadius`;
- `DisplayCenter`;
- `BoundingBox`;
- `RadialLevel`;
- `Displacement`;
- `NeedsLeaderLine`;
- `Priority`;
- `StableOrderKey`.

`TrueLongitude` nunca cambia.

---

## 8.3 Orden de dibujo

1. fondo;
2. signos;
3. grados;
4. casas;
5. cúspides;
6. ASC/MC;
7. aspectos;
8. marcas de posición real;
9. conectores;
10. glifos;
11. etiquetas;
12. highlight/interacción.

---

## 8.4 Algoritmo determinista de redistribución

### Paso 1 — posición ideal

- ángulo real;
- radio base;
- caja del glifo.

### Paso 2 — detección circular de clusters

Ordenar por longitud.

Detectar colisiones considerando también 359°↔0°.

### Paso 3 — orden canónico

Dentro de cluster:

1. longitud real;
2. prioridad visual;
3. ObjectId.

No usar orden de inserción.

### Paso 4 — separación angular

Calcular mínimo angular a partir de:

- ancho del glifo;
- radio;
- margen.

Resolver mediante relajación determinista con restricciones:

- orden conservado;
- mínimo garantizado;
- desplazamiento minimizado;
- desplazamiento máximo acotado.

### Paso 5 — niveles radiales

Si no cabe:

- distribuir en niveles discretos;
- orden estable;
- secuencia fija.

Ejemplo:

`0, +1, -1, +2, -2`

### Paso 6 — ajuste tangencial

Microajuste acotado solo si la geometría tipográfica todavía toca.

### Paso 7 — conector

Si el desplazamiento supera umbral:

- línea fina al grado real;
- marca real visible.

### Paso 8 — verificación

Detector global de intersecciones.

Invariante:

**cero solapamientos entre glifos.**

Fallback secuencial:

1. más niveles;
2. ampliar banda;
3. reducir glifo hasta mínimo;
4. nunca aceptar colisión.

---

## 8.5 Stelliums

Casos obligatorios:

- 2 cuerpos misma longitud;
- 5 en 2°;
- 10+ en 5°;
- cruce 0°;
- junto a ASC;
- junto a MC;
- junto a cúspide;
- puntos secundarios activados.

ASC y MC conservan prioridad visual.

---

## 8.6 Aspectos gráficos

Los aspectos conectan posiciones reales.

Nunca:

`aspect line -> displaced glyph center`

Siempre:

`aspect line -> true astrological anchor`

---

## 8.7 Doble rueda

Dos layouts independientes:

- interior;
- exterior.

Aspectos cruzados entre anclas reales.

La geometría visual puede cambiar por orden de presentación.

La semántica no.

---

# 9. SkiaSharp en Ubuntu

## 9.1 Decisión

**Se mantiene SkiaSharp.**

No existe una incompatibilidad seria que justifique sustituirlo.

SkiaSharp es multiplataforma y dispone de assets nativos Linux. Avalonia utiliza Skia en su pipeline Linux, por lo que el stack es coherente.

---

## 9.2 Dependencias nativas

Miastro no confiará en una instalación arbitraria de SkiaSharp en el sistema.

Se fijarán:

- versión NuGet;
- paquete de native assets Linux correspondiente;
- Runtime Identifier;
- hashes del output de publicación.

Cuando el paquete seleccionado requiera librerías del sistema, estas quedarán declaradas como dependencias del `.deb`.

Avalonia documenta en Linux dependencias como:

- `libx11-6`;
- `libice6`;
- `libsm6`;
- `libfontconfig1`.

La matriz final de dependencias se validará contra las versiones Ubuntu soportadas.

---

## 9.3 Riesgo de `libSkiaSharp.so`

La compatibilidad depende de:

- arquitectura;
- glibc;
- versión del paquete;
- variantes native assets.

Mitigación:

- no mezclar versiones;
- lockfile;
- build reproducible;
- smoke test en Ubuntu limpio;
- test de carga nativa;
- test de render antes de release.

---

## 9.4 Pantalla

La escena se renderiza según:

- tamaño lógico;
- escala DPI;
- pixel ratio.

La UI no utiliza capturas de controles para exportar.

---

## 9.5 PNG

SkiaSharp será el backend oficial.

Soporta:

- tamaño explícito;
- alta resolución;
- fondo;
- antialiasing;
- metadatos necesarios.

Formato V1:

**PNG**.

JPEG no será formato principal de la rueda.

---

## 9.6 PDF vectorial

Skia dispone de backend PDF mediante documento/canvas.

Para exportar una rueda aislada:

`WheelScene -> Skia PDF canvas -> PDF`

Los paths, líneas y texto permanecen vectoriales siempre que la operación gráfica lo permita.

---

## 9.7 SVG intermedio

Para integrar una rueda en un informe QuestPDF se recomienda:

`WheelScene -> Skia SVG canvas -> SVG -> QuestPDF`

Ventajas:

- vectorial;
- escalable;
- reutiliza el mismo renderer;
- desacopla QuestPDF del motor de layout;
- evita rasterizar la rueda en el informe.

---

# 10. Fuentes y glifos

## 10.1 Principio

Miastro no dependerá de:

- fuentes instaladas por Ubuntu;
- versión de DejaVu/Noto del equipo;
- configuración Fontconfig del usuario;
- fallback tipográfico del sistema.

---

## 10.2 Tipografía de interfaz

Se empaquetará una familia aprobada con licencia redistribuible.

Recomendación:

**Inter**.

Debe incluir pesos realmente usados.

No empaquetar pesos innecesarios.

---

## 10.3 Carga en Avalonia

Las fuentes se declaran como recursos de aplicación.

Los estilos referencian la familia interna.

No se selecciona `"sans-serif"` como fuente principal si eso permite variar entre instalaciones.

---

## 10.4 Carga en Skia

Skia cargará la fuente desde recurso/fichero empaquetado mediante `SKTypeface` explícito.

No usará `SKTypeface.Default` para contenido cuyo aspecto deba ser reproducible.

---

## 10.5 QuestPDF

Se deshabilitará el uso de fuentes de entorno para el documento final.

Se registrarán explícitamente las fuentes empaquetadas.

Se activarán pruebas que fallen si un glifo requerido no existe.

---

## 10.6 Glifos astrológicos

Para máxima estabilidad, la rueda no debe depender exclusivamente de caracteres Unicode.

Recomendación:

**catálogo vectorial interno de glifos astrológicos**, cada uno con:

- `GlyphId`;
- path;
- viewbox;
- métricas;
- licencia;
- versión.

Incluye:

- signos;
- planetas;
- Nodo Norte/Sur;
- Quirón;
- Ceres;
- Palas;
- Juno;
- Vesta;
- Lilith Media;
- Parte de Fortuna;
- ASC/MC si se representan con marcas textuales.

Ventaja:

el glifo es idéntico en:

- pantalla;
- PNG;
- PDF;
- impresión.

---

## 10.7 Licencias de fuentes

Cada fuente/recurso debe incluir:

- origen;
- licencia;
- versión;
- fichero de licencia;
- permiso de redistribución.

No se incluirán fuentes cuya licencia impida empaquetado o embedding en PDF.

---

# 11. Sistema de aspectos

## 11.1 `AspectDefinition`

- id;
- nombre;
- ángulo;
- orbe base;
- incremento luminares;
- símbolo;
- categoría;
- prioridad;
- participantes;
- estilo.

---

## 11.2 Perfil V1

| Aspecto | Ángulo | Orbe | Sol/Luna |
|---|---:|---:|---:|
| Conjunción | 0° | 8° | 9° |
| Semisextil | 30° | 2° | 3° |
| Sextil | 60° | 4° | 5° |
| Cuadratura | 90° | 6° | 7° |
| Trígono | 120° | 6° | 7° |
| Quincuncio | 150° | 3° | 4° |
| Oposición | 180° | 8° | 9° |
| Quintil | 72° | 2° | 3° |
| Biquintil | 144° | 2° | 3° |

Regla:

`orbe = base + (participaSolOLuna ? 1° : 0°)`

Sol + Luna sigue sumando solo 1°.

---

## 11.3 Participantes V1

Sí:

- 10 planetas;
- Quirón;
- Ceres;
- Palas;
- Juno;
- Vesta;
- ASC;
- MC.

No:

- Nodo Norte;
- Nodo Sur;
- Lilith Media;
- Parte Fortuna.

---

## 11.4 Cálculo

Separación:

`minimumCircularSeparation ∈ [0°,180°]`

Selección:

- delta mínimo;
- dentro de orbe;
- desempate por prioridad estable.

---

## 11.5 Matriz triangular

Es proyección de datos existentes.

La UI no recalcula.

Cada celda:

- aspecto;
- orbe;
- exactitud;
- IDs;
- estilo.

---

# 12. Sistema interpretativo

## 12.1 Capas

### Hechos

Objetivos dentro del modelo astrológico.

### Evidencias

Patrones semánticos derivados.

### Texto

Realización lingüística.

---

## 12.2 Grafo interpretativo

`InterpretationFactGraph`

Nodos:

- cuerpo;
- signo;
- casa;
- eje;
- polaridad;
- aspecto;
- regente;
- tema;
- persona.

Relaciones:

- ocupa;
- rige;
- aspecta;
- activa;
- complementa;
- tensiona;
- compensa;
- repite.

Es un modelo en memoria, no requiere una base de datos de grafos.

---

## 12.3 Reglas

Cada regla:

- ID;
- versión;
- contexto;
- condiciones;
- evidencias;
- prioridad;
- resultado semántico.

Nunca contiene el texto definitivo como lógica.

---

## 12.4 Ejes

Se modelan explícitamente:

### Zodiacales

- Aries–Libra;
- Tauro–Escorpio;
- Géminis–Sagitario;
- Cáncer–Capricornio;
- Leo–Acuario;
- Virgo–Piscis.

### Casas

- 1–7;
- 2–8;
- 3–9;
- 4–10;
- 5–11;
- 6–12.

---

## 12.5 Regencias

Modelo multivalor.

No perder:

- Marte/Plutón;
- Saturno/Urano;
- Júpiter/Neptuno.

Capa esotérica futura separada.

---

## 12.6 Informes interpretativos

Soportará:

- resumen natal;
- interpretación natal;
- kármica;
- astromédica;
- orientación profesional;
- pareja;
- revolución solar;
- tránsitos.

---

## 12.7 Astromédica

Guardrail obligatorio:

- simbólica;
- no diagnóstica;
- no afirma enfermedad;
- no predice patologías;
- Júpiter puede tratarse como significador simbólico de zona hepática según doctrina del proyecto.

---

## 12.8 Pareja

Entrada:

`SymmetricSynastryModel`

Debe producir:

- dinámica compartida;
- A→B;
- B→A;
- reciprocidades;
- desequilibrios;
- síntesis.

Permutar las personas no cambia la conclusión global.

---

# 13. Sistema de informes

## 13.1 Pipeline

```text
ReportRequest
→ ReportDataAssembler
→ InterpretationEngine
→ SectionBuilder
→ ReportDocumentModel
→ QuestPDF
→ PDF
```

---

## 13.2 Plantilla

Define:

- tipo;
- secciones;
- orden;
- condiciones;
- portada;
- rueda;
- estilos;
- cabecera;
- pie;
- paginación.

No calcula astrología.

---

## 13.3 `ReportDocumentModel`

Contiene:

- metadatos;
- secciones;
- párrafos;
- tablas;
- bloques;
- gráficos;
- rueda;
- advertencias;
- estilos semánticos.

---

## 13.4 QuestPDF

Se recomienda como motor documental principal porque:

- funciona en Linux;
- permite composición paginada;
- gestiona fuentes registradas;
- admite SVG;
- puede integrar gráficos producidos con Skia;
- evita construir un paginador propio.

Su licencia vigente debe registrarse en el inventario de terceros.

Para uso individual/personal existe actualmente una modalidad Community aplicable según sus condiciones; esta circunstancia se revisará en cada cambio de contexto legal o distribución.

---

## 13.5 Preview

La previsualización usa el mismo modelo documental que PDF.

No se mantiene una versión HTML paralela.

Opciones de visualización internas pueden rasterizar páginas para preview, pero el documento fuente sigue siendo el mismo.

---

# 14. Exportación PDF

## 14.1 Rueda aislada

Backend principal:

**Skia PDF**.

Ventajas:

- vectorial;
- mismo motor;
- trazos idénticos;
- control total.

---

## 14.2 Informe

Backend:

**QuestPDF**.

La rueda se incorpora como vector SVG generado desde la escena.

---

## 14.3 Fuentes PDF

Política:

- fuentes propias registradas;
- embedding permitido por licencia;
- nada de fonts del sistema;
- comprobación de disponibilidad de glifos.

Skia PDF incorpora fuentes al documento según su backend PDF; Miastro debe verificar el resultado mediante tests automáticos y herramientas de inspección PDF.

---

## 14.4 Consistencia

Una referencia gráfica única:

`WheelScene`

La misma entrada debe generar una composición equivalente en:

- pantalla;
- PNG;
- PDF aislado;
- informe;
- impresión.

Se aceptan diferencias de rasterización subpíxel, no diferencias de layout.

---

# 15. Impresión en Linux Ubuntu

## 15.1 Estrategia principal

**PDF-first + XDG Desktop Portal Print.**

No se imprime directamente desde controles Avalonia.

---

## 15.2 Flujo

1. crear `ReportDocumentModel` o `PrintableWheelDocument`;
2. generar PDF final;
3. abrir `PreparePrint` del XDG Desktop Portal;
4. usuario elige impresora, papel y opciones en diálogo del escritorio;
5. utilizar parámetros devueltos;
6. enviar PDF mediante `Print`;
7. el backend del escritorio lo entrega al sistema de impresión/CUPS.

---

## 15.3 Razón

Esta estrategia:

- evita lógica específica de drivers;
- evita gestionar PPD manualmente;
- preserva el PDF exacto;
- funciona bien con entornos sandboxed y no sandboxed que ofrezcan portal;
- permite usar el diálogo del sistema;
- se integra con el stack de impresión Ubuntu.

---

## 15.4 CUPS

CUPS es el sistema de impresión subyacente esperado en Ubuntu.

Miastro no hablará con impresoras mediante protocolos propietarios.

La capa de aplicación conoce:

`IPrintService`

El adaptador Linux conoce:

- D-Bus;
- XDG Print Portal.

CUPS queda por debajo de esa frontera.

---

## 15.5 Ausencia del portal o servicio

Si el portal no está disponible:

- el PDF se genera correctamente;
- la aplicación informa de que la integración de impresión no está disponible;
- ofrece guardar el PDF para impresión externa.

No se ejecutará silenciosamente un comando shell construido con strings de usuario.

---

# 16. Configuración

## 16.1 Configurable en V1

- Placidus/Koch;
- Consulta/Presentación;
- puntos opcionales;
- etiquetas;
- perfil exportación;
- estilo aprobado;
- impresora/opciones devueltas por el diálogo;
- filtros;
- favoritos.

---

## 16.2 Fijo en V1

- Linux Ubuntu;
- Avalonia;
- tropical;
- geocéntrico;
- aparente;
- longitud eclíptica;
- sin topocentrismo;
- Nodo Verdadero;
- Nodo Sur a 180°;
- Lilith Media;
- Parte Fortuna;
- aspectos V1;
- orbes;
- +1° Sol/Luna;
- participantes V1;
- ASC siempre visible;
- MC siempre visible;
- Quirón por defecto;
- aspectos por defecto;
- sinastría simétrica;
- interpretación por ejes;
- modo oscuro fuera de V1.

---

# 17. Sistema de archivos Linux y XDG

## 17.1 Principio

Separar:

- recursos instalados;
- datos de usuario;
- configuración;
- estado;
- caché;
- exportaciones.

---

## 17.2 Recursos instalados

Gestionados por `.deb`, solo lectura:

```text
/usr/lib/miastro/
    Miastro
    *.dll
    native/

 /usr/share/miastro/
    ephemeris/
    geodata/
    fonts/
    glyphs/
    templates/
    licenses/

 /usr/share/applications/
    com.miastro.Miastro.desktop

 /usr/share/icons/hicolor/
    ...
```

---

## 17.3 Datos del usuario

Resolver variables XDG primero.

### Datos persistentes

`$XDG_DATA_HOME/miastro/`

fallback:

`~/.local/share/miastro/`

Contendrá:

```text
miastro.db
backups/
exports-metadata/
user-content/
```

---

## 17.4 Configuración

`$XDG_CONFIG_HOME/miastro/`

fallback:

`~/.config/miastro/`

Ejemplo:

```text
settings.json
```

No guardar datos personales aquí.

---

## 17.5 Caché

`$XDG_CACHE_HOME/miastro/`

fallback:

`~/.cache/miastro/`

Contendrá solo datos regenerables:

- previews;
- thumbnails;
- render cache;
- índices temporales.

Puede borrarse sin pérdida funcional.

---

## 17.6 Estado

`$XDG_STATE_HOME/miastro/`

fallback:

`~/.local/state/miastro/`

Para:

- logs;
- estado de sesión no esencial;
- diagnóstico;
- historial técnico local.

---

## 17.7 Runtime

`$XDG_RUNTIME_DIR`

Solo para:

- locks;
- sockets;
- IPC temporal.

Nunca:

- base de datos;
- documentos;
- logs persistentes.

---

## 17.8 Permisos

Al crear directorios privados:

- permisos restrictivos;
- preferencia `0700` para directorios de usuario de Miastro;
- ficheros sensibles no deben ser world-readable.

La aplicación debe respetar `umask`.

---

## 17.9 UI

No mostrar estas rutas en el flujo normal.

Solo pueden aparecer en:

- diagnóstico avanzado;
- gestión de backup/export;
- soporte técnico.

---

# 18. SQLite en Linux

## 18.1 Ubicación

Base principal:

```text
$XDG_DATA_HOME/miastro/miastro.db
```

fallback:

```text
~/.local/share/miastro/miastro.db
```

---

## 18.2 Recursos frente a datos modificables

### Solo lectura

`/usr/share/miastro/...`

- GeoNames;
- efemérides;
- fuentes;
- glifos;
- plantillas base.

### Modificables

`$XDG_DATA_HOME/miastro/...`

- base;
- informes guardados;
- backups;
- contenido usuario.

### Configuración

`$XDG_CONFIG_HOME/miastro/...`

### Caché

`$XDG_CACHE_HOME/miastro/...`

---

## 18.3 Concurrencia

V1 es aplicación personal de un único proceso.

Usar:

- WAL cuando sea apropiado;
- transacciones;
- busy timeout;
- lock de instancia si se decide impedir dos procesos escribiendo el mismo perfil.

---

## 18.4 Backup

Política recomendada:

### Backup automático

- al cerrar tras cambios significativos o con periodicidad controlada;
- usar API de backup SQLite, no copiar un fichero activo de forma insegura;
- rotación limitada.

### Ubicación

```text
$XDG_DATA_HOME/miastro/backups/
```

### Backup manual

Permite exportar una copia a un lugar elegido mediante diálogo.

---

## 18.5 Datos personales

La base contiene:

- contacto;
- notas;
- datos natales.

Medidas:

- permisos de fichero restrictivos;
- no enviar telemetría;
- no subir backups automáticamente;
- no registrar teléfono/email/notas en logs;
- exportación explícita.

Cifrado completo de base no se incorpora por defecto en V1 salvo decisión posterior.

---

# 19. Empaquetado e instalación Ubuntu

## 19.1 Estrategia principal

**Paquete Debian `.deb` autocontenido.**

Esta es la opción principal de Miastro.

---

## 19.2 Motivos

Para una aplicación destinada específicamente a Ubuntu:

- instalación familiar;
- integración con el escritorio;
- gestión clara de archivos;
- posibilidad de declarar dependencias;
- acceso normal a CUPS/portals;
- no añade sandbox que complique Swiss Ephemeris o impresión;
- permite bundle self-contained de .NET;
- permite futuro repositorio APT para updates.

---

## 19.3 Contenido

El `.deb` incluirá:

- ejecutable;
- runtime .NET self-contained;
- assemblies;
- `libswe.so`;
- native assets Skia requeridos;
- efemérides;
- geodata;
- fuentes;
- glifos;
- plantillas base;
- licencias;
- iconos;
- desktop file.

---

## 19.4 Dependencias del sistema

Solo las inevitables del entorno Linux gráfico, declaradas por paquete.

No exigir:

- SDK .NET;
- runtime .NET;
- instalación manual de Swiss Ephemeris;
- instalación manual de fuentes;
- instalación manual de GeoNames.

---

## 19.5 Launcher

`com.miastro.Miastro.desktop`

Debe definir:

- nombre;
- icono;
- categoría;
- executable;
- MIME types si se añaden en futuro;
- terminal=false.

---

## 19.6 Icono

Instalar tamaños apropiados bajo:

`/usr/share/icons/hicolor/...`

No depender de un path dentro del home.

---

## 19.7 Actualizaciones

Fase inicial:

- nueva versión `.deb`.

Futuro:

- repositorio APT firmado.

La aplicación no necesita un updater privilegiado propio.

---

## 19.8 Integridad

Build de release debe generar:

- checksums;
- SBOM;
- inventario de terceros;
- versiones;
- manifiesto de recursos.

---

# 20. Estructura de carpetas del repositorio

```text
Miastro/
├─ docs/
│  ├─ architecture/
│  │  ├─ ADR/
│  │  └─ diagrams/
│  ├─ domain/
│  ├─ validation/
│  └─ licenses/
│
├─ src/
│  ├─ Miastro.UI.Avalonia/
│  │  ├─ Views/
│  │  ├─ ViewModels/
│  │  ├─ Controls/
│  │  ├─ Styles/
│  │  ├─ Resources/
│  │  ├─ Navigation/
│  │  ├─ Behaviors/
│  │  ├─ Converters/
│  │  ├─ Accessibility/
│  │  └─ Services/
│  │
│  ├─ Miastro.Application/
│  │  ├─ People/
│  │  ├─ Charts/
│  │  ├─ Revolutions/
│  │  ├─ Transits/
│  │  ├─ Progressions/
│  │  ├─ Synastry/
│  │  ├─ Interpretation/
│  │  ├─ Reports/
│  │  ├─ Geography/
│  │  ├─ Export/
│  │  └─ Configuration/
│  │
│  ├─ Miastro.Domain/
│  │  ├─ Astrology/
│  │  ├─ People/
│  │  ├─ Geography/
│  │  ├─ Interpretation/
│  │  ├─ Reports/
│  │  └─ Shared/
│  │
│  ├─ Miastro.Astronomy.Abstractions/
│  │
│  ├─ Miastro.Infrastructure.SwissEphemeris/
│  │  ├─ Native/
│  │  ├─ Interop/
│  │  ├─ Mapping/
│  │  └─ Diagnostics/
│  │
│  ├─ Miastro.Infrastructure.Persistence/
│  │
│  ├─ Miastro.Infrastructure.Geography/
│  │
│  ├─ Miastro.Infrastructure.Time/
│  │
│  ├─ Miastro.Infrastructure.Platform.Linux/
│  │  ├─ Xdg/
│  │  ├─ Desktop/
│  │  └─ Permissions/
│  │
│  ├─ Miastro.Infrastructure.Printing.Linux/
│  │  ├─ Portal/
│  │  └─ Diagnostics/
│  │
│  ├─ Miastro.Graphics/
│  │  ├─ Geometry/
│  │  ├─ Layout/
│  │  ├─ Collision/
│  │  ├─ Scene/
│  │  ├─ Typography/
│  │  ├─ Glyphs/
│  │  └─ Styles/
│  │
│  ├─ Miastro.Graphics.Skia/
│  │
│  ├─ Miastro.Interpretation/
│  │  ├─ Facts/
│  │  ├─ Rules/
│  │  ├─ Evidence/
│  │  └─ TextCatalog/
│  │
│  ├─ Miastro.Reports/
│  │  ├─ Model/
│  │  ├─ Templates/
│  │  ├─ Sections/
│  │  ├─ Preview/
│  │  └─ Rendering/
│  │
│  └─ Miastro.Export/
│
├─ assets/
│  ├─ fonts/
│  ├─ glyphs/
│  ├─ icons/
│  └─ styles/
│
├─ data/
│  ├─ geonames/
│  ├─ tzdb/
│  ├─ ephemeris/
│  └─ licenses/
│
├─ packaging/
│  └─ debian/
│
├─ tests/
│  ├─ Miastro.Domain.Tests/
│  ├─ Miastro.Astronomy.Tests/
│  ├─ Miastro.Astronomy.NativeLinuxTests/
│  ├─ Miastro.Geography.Tests/
│  ├─ Miastro.Time.Tests/
│  ├─ Miastro.Persistence.IntegrationTests/
│  ├─ Miastro.Graphics.Tests/
│  ├─ Miastro.Graphics.VisualRegressionTests/
│  ├─ Miastro.UI.Avalonia.Tests/
│  ├─ Miastro.Platform.Linux.Tests/
│  ├─ Miastro.Printing.Linux.Tests/
│  ├─ Miastro.Export.Tests/
│  └─ Miastro.Installation.Tests/
│
└─ tools/
   ├─ DataBuilder/
   ├─ EphemerisValidator/
   └─ VisualBaseline/
```

---

# 21. Estrategia de pruebas

## 21.1 Cálculo astronómico

Golden cases:

- planetas;
- Luna;
- Nodo Verdadero;
- Quirón;
- asteroides;
- Lilith Media;
- velocidad;
- retrogradación;
- casas;
- ASC;
- MC.

Comparación contra referencias profesionales aprobadas.

---

## 21.2 Aspectos

- exacto;
- límite;
- fuera;
- 0° circular;
- Sol;
- Luna;
- Sol+Luna;
- quintil;
- biquintil;
- exclusiones;
- matriz.

---

## 21.3 Casas

- Placidus;
- Koch;
- latitudes;
- IC/DSC;
- errores.

---

## 21.4 TZDB

- DST;
- cambio primavera;
- cambio otoño;
- horas ambiguas;
- horas inexistentes;
- offsets fraccionarios;
- históricos.

Además:

- comprobar que nunca se resuelve una ambigüedad silenciosamente.

---

## 21.5 Parte Fortuna

- diurna;
- nocturna;
- normalización;
- cruce 0°.

---

## 21.6 Revoluciones

- retornos;
- años consecutivos;
- exactitud;
- localidad diferente.

---

## 21.7 Progresiones

- 1 año;
- múltiples edades;
- bisiestos;
- Luna progresada;
- reproducibilidad.

No fijar tests definitivos de ASC/MC hasta aprobar método.

---

## 21.8 Sinastría

Property:

`Canonical(A,B) == Canonical(B,A)`

con swap de relaciones direccionales.

La síntesis global debe ser idéntica.

---

## 21.9 Algoritmo de glifos

Unitarios:

- cero overlaps;
- true longitude intacta;
- determinismo;
- 359/0;
- stellium;
- ASC;
- MC;
- doble rueda.

Property-based:

miles de distribuciones pseudoaleatorias con seed fija de test.

---

## 21.10 Regresión visual

Corpus:

1. carta dispersa;
2. conjunción;
3. stellium;
4. cluster 0°;
5. ASC;
6. MC;
7. casas desiguales;
8. doble rueda;
9. sinastría densa;
10. A4.

El entorno baseline será Linux Ubuntu en contenedor/VM controlada.

---

## 21.11 Headless

Cuando sea posible:

- Domain;
- Astronomy;
- Geography;
- Persistence;
- Scene;
- Skia raster/PDF;
- Reports.

deben ejecutarse sin display.

Los tests de UI que necesiten compositor se ejecutan:

- con Xvfb/X11 virtual;
- o entorno de CI preparado.

No usar headless para “aprobar” comportamiento que depende realmente del window manager sin un test real adicional.

---

## 21.12 Avalonia/Skia

Pruebas:

- inicio de control;
- render;
- resize;
- hit-test;
- selección;
- DPI;
- escala;
- invalidación.

---

## 21.13 DPI

Baselines:

- 1.0;
- 1.25;
- 1.5;
- 2.0.

Comprobar:

- cajas;
- clipping;
- texto;
- glifos;
- interacción.

---

## 21.14 Fuentes

En entorno limpio sin fuentes de usuario:

- Inter interno se carga;
- glifos vectoriales se cargan;
- PDF contiene fuentes;
- QuestPDF no cae a font del sistema;
- ningún símbolo requerido es missing glyph.

---

## 21.15 PDF

Comprobar:

- PDF válido;
- tamaño;
- páginas;
- media box;
- fonts embebidas;
- vectorialidad de la rueda;
- Unicode de texto;
- impresión visual.

---

## 21.16 Impresión

Tests unitarios:

- adaptación del portal;
- errores D-Bus;
- cancelación;
- token;
- ficheros temporales.

Integración:

- entorno Ubuntu con portal;
- impresora CUPS virtual;
- PDF enviado correctamente.

---

## 21.17 XDG

Con variables custom:

- DATA_HOME;
- CONFIG_HOME;
- CACHE_HOME;
- STATE_HOME;
- RUNTIME_DIR.

Comprobar:

- rutas absolutas;
- fallback;
- aislamiento;
- creación;
- no escritura en `/usr/share`;
- no datos personales en cache.

---

## 21.18 Permisos

Casos:

- directorio no escribible;
- DB read-only;
- disco lleno;
- HOME no disponible;
- archivo bloqueado;
- backup fallido.

Errores comprensibles y sin pérdida silenciosa.

---

## 21.19 Carga nativa Swiss Ephemeris

CI Ubuntu:

- carga correcta;
- arquitectura incorrecta;
- `libswe.so` ausente;
- hash inválido;
- ABI incompatible;
- efemérides ausentes;
- datos corruptos.

---

## 21.20 Instalación limpia

VM Ubuntu limpia:

1. instalar `.deb`;
2. lanzar desde menú;
3. crear perfil XDG;
4. abrir GeoNames;
5. cargar TZDB;
6. cargar Swiss Ephemeris;
7. render Skia;
8. generar PDF;
9. imprimir mediante portal;
10. desinstalar sin borrar datos de usuario por defecto.

---

# 22. Seguridad, privacidad y robustez

## 22.1 Local-first

No se requiere servicio remoto para funcionalidad principal.

---

## 22.2 Datos personales

No deben salir del equipo salvo exportación explícita.

---

## 22.3 Logs

Prohibido escribir por defecto:

- notas;
- email;
- teléfono;
- contenido íntegro de informes;
- datos privados innecesarios.

---

## 22.4 Librerías nativas

Solo cargar desde rutas controladas.

No buscar `libswe.so` arbitrariamente en directorios escribibles por otros usuarios.

---

## 22.5 Datos externos

GeoNames/efemérides:

- hashes;
- versión;
- origen;
- licencia.

---

## 22.6 Backups

No incluir automáticamente exports innecesarios.

Backup de base con permisos restrictivos.

---

# 23. ADR actualizados

Antes de producción de Fase 1 se crearán:

### ADR-001 — Linux Ubuntu + .NET 10 + Avalonia UI

Fija plataforma, runtime y UI.

### ADR-002 — Modular Monolith + Clean Architecture / Ports & Adapters

Fija límites.

### ADR-003 — Swiss Ephemeris

Fija motor y capa anticorrupción.

### ADR-004 — Nodo Verdadero

Fija Nodo Norte verdadero y Sur a 180°.

### ADR-005 — Noda Time + IANA TZDB

Fija tiempo histórico.

### ADR-006 — GeoNames offline

Fija catálogo local.

### ADR-007 — SQLite + EF Core

Fija persistencia.

### ADR-008 — Scene Graph propio + SkiaSharp

Fija motor gráfico.

### ADR-009 — Redistribución determinista de glifos

Fija invariantes de layout.

### ADR-010 — Sinastría canónica simétrica

Fija independencia semántica del orden.

### ADR-011 — Hechos → evidencias → texto

Fija interpretación.

### ADR-012 — QuestPDF para informes

Fija composición documental y política de fuentes/licencia.

### ADR-013 — PDF-first + XDG Desktop Portal Print

Fija impresión Linux/CUPS.

### ADR-014 — Política XDG

Fija DATA/CONFIG/CACHE/STATE/RUNTIME.

### ADR-015 — Paquete `.deb` autocontenido

Fija instalación Ubuntu.

### ADR-016 — Dependencias nativas privadas y versionadas

Fija `libswe.so`, Skia native assets y su carga.

### ADR-017 — Fuentes y glifos internos

Fija independencia de fonts del sistema.

### ADR-018 — Versionado reproducible

Fija motor, efemérides, TZDB, datasets, reglas y plantillas.

---

# 24. Plan de construcción por fases

## Fase 1 — Base técnica

Objetivo:

crear la base sin funcionalidad final.

Incluye:

- solución .NET 10;
- proyectos/modularización;
- Avalonia shell mínimo;
- DI;
- logging;
- configuración;
- XDG path service;
- SQLite inicial;
- CI Ubuntu;
- test runner;
- build `linux-x64`;
- estructura de assets;
- empaquetado `.deb` mínimo de prueba;
- carga de configuración.

No incluye carta final.

Criterio de salida:

- compila;
- inicia en Ubuntu;
- tests;
- rutas XDG;
- instalación limpia básica;
- límites de arquitectura comprobables.

---

## Fase 2 — Núcleo de dominio

- longitudes;
- grados;
- signos;
- cuerpos;
- casas;
- Nodo verdadero;
- aspectos;
- regencias;
- perfiles.

Sin Swiss en dominio.

---

## Fase 3 — Swiss Ephemeris Linux

- compilar/validar `libswe.so`;
- P/Invoke;
- carga segura;
- posiciones;
- velocidades;
- casas;
- ASC/MC;
- cuerpos;
- diagnostics.

---

## Fase 4 — Geografía y tiempo

- GeoNames;
- FTS;
- homónimos;
- Noda Time;
- TZDB;
- política de ambigüedad.

---

## Fase 5 — Persistencia y Persona

- modelos;
- EF Core;
- migraciones;
- CRUD;
- backups iniciales;
- historial mínimo.

---

## Fase 6 — Natal fiable

- instante;
- posiciones;
- Nodo Verdadero;
- Nodo Sur;
- Lilith Media;
- Parte Fortuna;
- casas;
- ASC/MC;
- aspectos.

Aprobar golden charts.

---

## Fase 7 — Rueda natal geométrica

- escena;
- signos;
- grados;
- casas;
- planetas;
- ángulos;
- aspectos.

---

## Fase 8 — Glifos deterministas

- cluster;
- separación;
- niveles;
- conectores;
- test de no overlap;
- regression visual.

---

## Fase 9 — UI Natal completa

- pantalla central;
- personas;
- rueda;
- panel;
- pestañas;
- selección sincronizada;
- accesibilidad;
- teclado.

---

## Fase 10 — Validación de base

No avanzar a técnicas hasta cerrar:

- cálculo;
- TZ;
- Persistencia;
- rueda;
- DPI;
- fuentes;
- Ubuntu.

---

## Fase 11 — Revoluciones

Solar y Lunar.

---

## Fase 12 — Tránsitos

Eventos temporales.

---

## Fase 13 — Progresiones

Planetas progresados.

ASC/MC quedan desactivados hasta aprobar método.

---

## Fase 14 — Sinastría

Modelo simétrico + doble rueda.

---

## Fase 15 — Interpretación

Hechos, evidencias y textos.

---

## Fase 16 — Informes

QuestPDF + plantillas.

---

## Fase 17 — Exportación e impresión avanzada

- PNG;
- PDF;
- portal impresión;
- Modo Presentación;
- QA final.

---

# 25. Fuentes técnicas verificadas para esta revisión

Estas fuentes se usan para validar la arquitectura Linux; no sustituyen al documento rector funcional.

- Microsoft .NET Support Policy: .NET 10 es LTS y permanece soportado hasta noviembre de 2028.
  - https://dotnet.microsoft.com/platform/support/policy
- Avalonia — Supported Platforms / Desktop Linux.
  - https://docs.avaloniaui.net/docs/supported-platforms
  - https://docs.avaloniaui.net/docs/platform-specific-guides/linux
- SkiaSharp — repositorio y native assets Linux.
  - https://github.com/mono/SkiaSharp
- Skia — backend PDF.
  - https://skia.org/docs/user/sample/pdf/
  - https://skia.org/docs/dev/design/pdftheory/
- XDG Desktop Portal — Print.
  - https://flatpak.github.io/xdg-desktop-portal/docs/doc-org.freedesktop.portal.Print.html
- CUPS — printing.
  - https://www.cups.org/doc/options.html
- XDG Base Directory Specification.
  - https://specifications.freedesktop.org/basedir/latest/
- QuestPDF — Linux, fuentes, SVG e integración Skia.
  - https://www.questpdf.com/
- Swiss Ephemeris.
  - https://www.astro.com/swisseph/
- GeoNames dump.
  - https://download.geonames.org/export/dump/
- Noda Time / TZDB.
  - https://nodatime.org/TimeZones

---

# Arquitectura aprobable

La arquitectura final recomendada de Miastro es:

- **Linux Ubuntu** como plataforma objetivo.
- Aplicación personal de escritorio.
- **C# + .NET 10 LTS + Avalonia UI**.
- **Modular Monolith + Clean Architecture / Ports & Adapters**.
- MVVM en presentación.
- SQLite + EF Core.
- Swiss Ephemeris mediante `libswe.so` privada y adaptador P/Invoke.
- Noda Time + IANA TZDB.
- GeoNames offline en SQLite de solo lectura.
- Scene graph propio.
- SkiaSharp como backend gráfico.
- PNG mediante SkiaSharp.
- rueda PDF mediante Skia PDF.
- informes PDF mediante QuestPDF.
- impresión **PDF-first + XDG Desktop Portal Print**, integrada con CUPS.
- fuentes empaquetadas y cargadas explícitamente.
- glifos astrológicos vectoriales internos.
- datos/configuración/caché/estado mediante XDG.
- `.deb` autocontenido como mecanismo de instalación principal.
- cálculo, dominio, gráfico, interpretación, informes, persistencia y plataforma estrictamente desacoplados.
- regresión astronómica y visual obligatoria.
- versionado completo para reproducibilidad.

# Decisiones cerradas

- Plataforma: Linux Ubuntu.
- Framework: Avalonia UI.
- Runtime: .NET 10 LTS.
- Arquitectura: Modular Monolith + Clean Architecture.
- Base de datos: SQLite + EF Core.
- Motor: Swiss Ephemeris.
- Nodo Norte: Nodo Verdadero.
- Nodo Sur: oposición exacta a 180°.
- Tiempo: Noda Time + IANA TZDB.
- Horas históricas ambiguas: nunca resolución silenciosa.
- Geografía: GeoNames offline.
- Gráficos: escena propia + SkiaSharp.
- Glifos: posición real separada de posición visual.
- Layout: determinista y sin solapamientos.
- Sinastría: canónica y simétrica.
- Fuentes y glifos: internos, no dependientes del sistema.
- Impresión: PDF-first + XDG Desktop Portal Print/CUPS.
- Empaquetado: `.deb` autocontenido.
- Sistema de archivos: XDG.
- Interpretación: hechos → evidencias → texto.
- Versionado de efemérides, TZDB, reglas, datasets y plantillas.

# Decisiones no bloqueantes pendientes

- Método exacto de ASC/MC en progresiones secundarias.
- Convención matemática exacta de fracciones temporales en progresiones secundarias.
- Estilos artísticos concretos posteriores a la rueda estándar.
- Vulcano y capa esotérica.
- Lilith Verdadera configurable.
- Arco solar.
- Direcciones.
- Carta compuesta.
- Retornos planetarios adicionales.
- Carta dracónica.
- Armónicos.
- Soporte nativo Wayland cuando el backend de Avalonia alcance la madurez requerida.
- Paquete `linux-arm64` si posteriormente se necesita.

# Riesgos específicos de Linux

1. **Backend gráfico X11/XWayland/Wayland.**  
   V1 debe validarse sobre Ubuntu real con X11/XWayland y no depender del backend Wayland experimental.

2. **Dependencias nativas Skia/Avalonia.**  
   Cambios de glibc, native assets o paquetes gráficos pueden romper el arranque si no se fijan y prueban versiones.

3. **Carga de `libswe.so`.**  
   Deben controlarse ABI, arquitectura, hashes y ruta privada para evitar fallos o cargar una biblioteca incorrecta.

4. **Portal de impresión no disponible.**  
   Algunos entornos Linux mínimos pueden no tener el backend XDG Desktop Portal funcional. La exportación PDF seguirá disponible y debe existir un error de impresión claro.

5. **Variación de entornos de escritorio.**  
   GNOME, XWayland, escalado fraccional y temas pueden revelar problemas de DPI o layout que no aparecen en CI headless.

6. **Fuentes.**  
   Si algún renderer cae accidentalmente a fuentes del sistema, pantalla y PDF pueden divergir. Debe impedirse mediante carga explícita y tests.

7. **Permisos XDG.**  
   Homes no estándar, directorios read-only, umask, disco lleno o variables XDG personalizadas deben tratarse sin pérdida de datos.

8. **Empaquetado de native assets.**  
   El `.deb` debe probarse siempre en una instalación Ubuntu limpia para detectar dependencias implícitas presentes solo en la máquina de desarrollo.

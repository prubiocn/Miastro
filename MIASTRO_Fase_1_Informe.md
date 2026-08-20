# MIASTRO — Informe Fase 1: Base técnica

Fecha de cierre técnico local: 2026-08-20T11:07:37+02:00

## 1. Alcance implementado

La Fase 1 establece exclusivamente la base técnica estable de Miastro.

Se han implementado:

- solución modular .NET 10
- Avalonia UI
- MVVM básico
- composición mediante inyección de dependencias
- navegación técnica mínima
- estados de carga y error
- política común de mensajes de error
- directorios XDG
- configuración local
- logging técnico local
- SQLite
- Entity Framework Core
- migración inicial
- prueba técnica de lectura/escritura
- tests automatizados
- comprobaciones de arquitectura
- publicación self-contained linux-x64
- paquete Debian
- desktop entry
- launcher Linux
- icono provisional
- pipeline CI Ubuntu
- ADRs requeridos

No se ha implementado funcionalidad astrológica de producción.

## 2. Estructura principal

```
Miastro/
├── Miastro.sln
├── src/
├── tests/
├── assets/
├── data/
├── docs/
├── packaging/
├── tools/
├── artifacts/
└── .github/workflows/
```

## 3. Decisiones técnicas

- Linux Ubuntu como plataforma objetivo.
- C# y .NET 10 LTS.
- Avalonia UI con MVVM.
- Monolito modular.
- Clean Architecture / Ports & Adapters.
- SQLite + EF Core.
- XDG para datos de ejecución y usuario.
- Publicación principal linux-x64 self-contained.
- Distribución mediante paquete .deb.
- Swiss Ephemeris queda desacoplado y sin integración funcional en Fase 1.
- GeoNames queda sin integración funcional en Fase 1.
- No existe lógica astrológica funcional en esta fase.

## 4. Persistencia

Base de datos:

```
/home/pablo/.local/share/miastro/miastro.db
```

Configuración:

```
/home/pablo/.config/miastro/settings.json
```

Logs:

```
/home/pablo/.local/state/miastro/logs/miastro.log
```

Cache:

```
/home/pablo/.cache/miastro/
```

Estas rutas corresponden a los valores XDG efectivos de esta ejecución.

## 5. SQLite

Se ha creado una migración técnica inicial.

El esquema de Fase 1 solo contiene infraestructura destinada a probar:

- migraciones
- conexión
- creación controlada
- lectura
- escritura

No existe todavía modelo Persona ni modelo astrológico de producción.

## 6. Seguridad local

Los directorios XDG propios de Miastro se crean con permisos privados.

Los archivos de configuración y base de datos se restringen al usuario.

Los logs están diseñados para contener únicamente información técnica.

Las rutas técnicas no forman parte de la interfaz ordinaria.

## 7. UI

La ventana principal:

- arranca mediante Avalonia
- tiene título Miastro
- utiliza ViewModel
- resuelve dependencias mediante DI
- no accede directamente a DbContext
- no accede directamente a Swiss Ephemeris
- no contiene lógica astrológica

## 8. Tests ejecutados

La batería automatizada incluye actualmente pruebas de:

- construcción del contenedor DI
- configuración
- SQLite
- migraciones
- XDG personalizado
- independencia de Domain
- dependencias entre proyectos
- restricciones UI/Persistence
- restricciones Interpretation/Swiss
- restricciones Graphics/Persistence
- política básica de errores
- presencia de ADRs

Resultado local previo validado: 12/12 tests correctos.

La auditoría final vuelve a ejecutar build y tests.

## 9. Publicación

Configuración:

- runtime: linux-x64
- modo: self-contained
- configuración: Release

Artefactos:

```
artifacts/publish/linux-x64/
```

## 10. Paquete Debian

Paquete construido:

```
artifacts/deb/miastro_0.1.0~phase1-1_amd64.deb
```

Instala:

- aplicación bajo /usr/lib/miastro
- launcher /usr/bin/miastro
- desktop entry
- icono provisional

La desinstalación fue probada y conserva los datos XDG del usuario.

## 11. CI

Existe pipeline:

```
.github/workflows/ci.yml
```

Configura en Ubuntu:

- restore
- build
- test
- publish linux-x64 self-contained

La ejecución remota real del workflow no se ha verificado durante esta sesión local.

## 12. Limitaciones deliberadas de Fase 1

Quedan fuera de esta fase:

- cálculos planetarios
- casas
- ASC/MC
- aspectos
- nodos funcionales
- Parte de Fortuna
- cartas natales
- sinastría
- rueda astrológica
- redistribución gráfica de glifos
- Swiss Ephemeris funcional
- GeoNames funcional
- TZDB histórico funcional
- interpretación astrológica
- informes finales
- exportación gráfica final
- impresión funcional

## 13. Resultado de auditoría

- [PASS] La solución Miastro.sln restaura y compila en Release.
- [PASS] La batería automatizada de pruebas se ejecuta correctamente.
- [PASS] Existe el módulo Miastro.UI.Avalonia.
- [PASS] Existe el módulo Miastro.Application.
- [PASS] Existe el módulo Miastro.Domain.
- [PASS] Existe el módulo Miastro.Astronomy.Abstractions.
- [PASS] Existe el módulo Miastro.Infrastructure.Persistence.
- [PASS] Existe el módulo Miastro.Infrastructure.Geography.
- [PASS] Existe el módulo Miastro.Infrastructure.Time.
- [PASS] Existe el módulo Miastro.Infrastructure.Platform.Linux.
- [PASS] Existe el módulo Miastro.Infrastructure.Printing.Linux.
- [PASS] Existe el módulo Miastro.Infrastructure.SwissEphemeris.
- [PASS] Existe el módulo Miastro.Graphics.
- [PASS] Existe el módulo Miastro.Graphics.Skia.
- [PASS] Existe el módulo Miastro.Interpretation.
- [PASS] Existe el módulo Miastro.Reports.
- [PASS] Existe el módulo Miastro.Export.
- [PASS] Existe el módulo Miastro.Bootstrap.
- [PASS] Existe el directorio XDG Data.
- [PASS] Existe el directorio XDG Config.
- [PASS] Existe el directorio XDG Cache.
- [PASS] Existe el directorio XDG State.
- [PASS] Existe settings.json en XDG Config.
- [PASS] Existe miastro.db en XDG Data.
- [PASS] Existe logging local en XDG State.
- [PASS] UI no referencia directamente Persistence.
- [PASS] UI no referencia directamente SwissEphemeris.
- [PASS] Domain permanece independiente de frameworks e infraestructura.
- [PASS] Interpretation no depende de Swiss Ephemeris.
- [PASS] Graphics no depende de Persistence.
- [PASS] Persistence no depende de UI.
- [PASS] Existe assets/fonts.
- [PASS] Existe assets/glyphs.
- [PASS] Existe assets/icons.
- [PASS] Existe assets/styles.
- [PASS] Existe data/ephemeris.
- [PASS] Existe data/geodata.
- [PASS] Existe data/licenses.
- [PASS] Existe ADR-001-linux-ubuntu-dotnet10-avalonia.md.
- [PASS] Existe ADR-002-modular-monolith-clean-architecture.md.
- [PASS] Existe ADR-007-sqlite-ef-core.md.
- [PASS] Existe ADR-014-xdg-directories.md.
- [PASS] Existe ADR-015-self-contained-deb.md.
- [PASS] Existe ADR-018-reproducible-versioning.md.
- [PASS] Existe publicación self-contained linux-x64.
- [PASS] La publicación contiene runtime propio.
- [PASS] Existe paquete .deb de Fase 1.
- [PASS] El paquete Miastro está instalado actualmente.
- [PASS] Existe launcher /usr/bin/miastro.
- [PASS] Existe desktop entry instalado.
- [PASS] Existe icono provisional instalado.
- [PASS] Pipeline CI Ubuntu está definido en el repositorio.
- [PENDING] Ejecución remota real de GitHub Actions no verificada en esta sesión.
- [PASS] No se detecta implementación astrológica funcional de producción.

## 14. Resumen

- PASS: 53
- FAIL: 0
- PENDING: 1

La implementación local de Fase 1 satisface los controles técnicos ejecutables en este entorno.

Existe un elemento pendiente independiente del código local: verificar una ejecución remota real del workflow CI una vez que el repositorio se encuentre alojado en GitHub.

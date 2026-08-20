# Swiss Ephemeris — build Linux

## Fuente

- Upstream: https://github.com/aloistr/swisseph.git
- Tag: `v2.10.3final`
- Commit: `af9823fe7b06ffefe3d3968fdc5680be8b5eec5f`
- Versión: `2.10.03`
- Fecha upstream: `2026-04-14T16:31:43+02:00`

## Build

Artefacto:

`libswe.so`

Plataforma:

`linux-x64`

El Makefile oficial compila los objetos con `-fPIC` y genera una
biblioteca ELF compartida.

## Integridad

SHA-256:

`288195bd64154fe284c4d8f436cb92221aa6f36b4a0b45a8bb8f4a4c3969ebe3`

Tamaño:

`1109288` bytes

## Política de carga

- Sin dependencia de instalación global.
- Sin dependencia de `LD_LIBRARY_PATH`.
- Ruta controlada por Miastro.
- Arquitectura validada antes de uso.
- Hash versionado.
- ABI mínima validada mediante símbolos exportados.

## Enlace nativo requerido por Miastro

La build de Miastro realiza el enlace final de `libswe.so`
incluyendo explícitamente:

```text
-lm
```

Motivo:

Swiss Ephemeris utiliza funciones de la biblioteca matemática de C,
incluyendo `atan2`, y la biblioteca debe poder cargarse con
resolución inmediata de símbolos.

El enlace final también utiliza:

```text
-Wl,-z,defs
```

para impedir que se produzca un artefacto compartido con símbolos
sin resolver.

La validación posterior ejecuta:

```text
ldd -r libswe.so
```

y exige cero símbolos no resueltos.

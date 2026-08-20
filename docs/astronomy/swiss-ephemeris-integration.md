# Integración Swiss Ephemeris

## Frontera

Swiss Ephemeris está encapsulado exclusivamente en:

`Miastro.Infrastructure.SwissEphemeris`

El resto del sistema usa:

`Miastro.Astronomy.Abstractions`

No se exponen:

- IDs nativos;
- flags nativos;
- arrays C;
- handles;
- códigos de retorno C;
- buffers nativos.

## Carga

La biblioteca se carga mediante:

`NativeLibrary.Load(ruta_controlada)`

No se usa:

- instalación global;
- nombre de biblioteca sin ruta;
- `LD_LIBRARY_PATH`;
- búsqueda arbitraria del sistema.

## ABI

La frontera usa delegates con:

`CallingConvention.Cdecl`

Los símbolos se resuelven explícitamente con:

`NativeLibrary.GetExport`.

## Integridad

Antes de cargar puede validarse SHA-256.

## Diagnóstico

El diagnóstico informa:

- disponibilidad;
- carga;
- compatibilidad ABI;
- versión de Swiss;
- versión del adaptador;
- arquitectura;
- ruta cargada;
- estado inicial de datos de efemérides.

Los mensajes C crudos no se exponen como mensajes destinados al usuario.

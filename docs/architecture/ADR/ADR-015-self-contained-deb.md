# ADR-015 — Publicación self-contained y paquete .deb

## Estado

Aceptado.

## Decisión

La distribución principal para Ubuntu será:

- `linux-x64`
- self-contained
- paquete `.deb`

Los binarios de aplicación se instalarán bajo `/usr`.

Los datos personales y de ejecución permanecerán fuera del paquete y se gestionarán mediante XDG.

## Desinstalación

La eliminación del paquete no debe borrar datos del usuario.

# ADR-001 — Linux Ubuntu + .NET 10 + Avalonia

## Estado

Aceptado.

## Decisión

Miastro se implementa como aplicación de escritorio para Linux Ubuntu utilizando:

- C#
- .NET 10 LTS
- Avalonia UI
- MVVM

## Consecuencias

La interfaz queda desacoplada de la lógica de aplicación y dominio.

La distribución principal será nativa para Linux x64.

No se introduce dependencia de WPF, WinForms ni tecnologías exclusivas de Windows.

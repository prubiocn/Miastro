# Fase 8 — Matriz triangular de aspectos

## Fuente

La matriz consume exclusivamente los aspectos persistidos en el snapshot natal.

No ejecuta `AspectEngine`, `NatalAspectCalculator` ni Swiss Ephemeris.

## Participantes

Los participantes se obtienen de:

1. `NatalObjectOrder`;
2. `MiastroV1AspectProfile.Instance.IsParticipant`;
3. objetos realmente presentes en el snapshot.

Esto preserva el orden canónico y excluye automáticamente objetos no autorizados por V1, como Nodo, Lilith o Parte de Fortuna.

## Triangularidad

La matriz almacena únicamente celdas donde:

`RowIndex > ColumnIndex`

Por tanto, cada pareja aparece una sola vez y no existe duplicación A-B / B-A.

La diagonal no representa aspectos de un objeto consigo mismo.

## Celdas

Cada celda contiene:

- objeto de fila;
- objeto de columna;
- aspecto o ausencia explícita;
- tipo de aspecto;
- símbolo compacto;
- separación persistida;
- ángulo exacto persistido;
- desviación persistida;
- orbe permitido persistido;
- orbe usado persistido;
- nombre accesible.

## Accesibilidad

Las celdas con aspecto disponen de texto equivalente a:

`Sol — cuadratura — Saturno — orbe 2°14′`

El tipo nunca depende únicamente del color.

## Presentación

La capa headless conserva `AspectKind`.

La UI podrá emplear posteriormente el catálogo vectorial gráfico para el glifo definitivo sin introducir dependencia de Graphics dentro de Application.

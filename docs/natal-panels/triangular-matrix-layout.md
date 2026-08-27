# Fase 8 — matriz triangular visual de aspectos

## Forma

La matriz utiliza el triángulo inferior estricto.

La fila de índice N contiene exactamente N celdas correspondientes a las columnas anteriores.

No se representa la diagonal y no existe duplicación A-B / B-A.

## Fuente de datos

El layout consume exclusivamente `NatalAspectMatrixReadModel`.

No detecta ni recalcula aspectos.

`NatalAspectsPanelViewModel` únicamente proyecta:

- columnas;
- filas;
- celdas ya existentes.

## Celdas sin aspecto

Las celdas sin aspecto permanecen presentes en la geometría triangular.

No son seleccionables y muestran la representación factual definida por el read model.

## Selección

Una celda con aspecto se presenta como botón estándar.

Ratón, Enter o Espacio pasan la misma `NatalAspectMatrixCell` a `SelectedAspectCell`.

Por tanto se reutilizan:

- selección dual;
- sincronización con paneles;
- resaltado dual de rueda;
- aspecto activo.

## Accesibilidad

Cada botón utiliza `AccessibleName` y `HelpText` procedentes del read model.

La matriz incluye instrucciones de teclado.

Se conserva además una lista compacta accesible plegada como alternativa de navegación secuencial por flechas.

## Ancho reducido

La pestaña Aspectos mantiene scroll horizontal automático.

Las celdas tienen ancho estable para no comprimir nombres y símbolos hasta volverlos ilegibles.

## Alcance

El layout no cambia:

- participantes;
- orden canónico;
- pertenencia a MiastroV1AspectProfile;
- orbes;
- aspectos persistidos;
- geometría de la rueda.

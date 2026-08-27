# Fase 8 — Datos y Posiciones

## Datos

Datos mantiene una presentación compacta de cinco columnas:

- glifo;
- nombre;
- grado;
- signo;
- regente o regentes.

Las regencias dobles proceden del catálogo factual ya existente y se muestran
separadas mediante `/`.

## Glifos

Application proporciona una representación textual canónica de cada objeto.

La finalidad es evitar que Avalonia dependa del renderer Skia para controles
de interfaz pequeños.

ASC y MC se presentan como `ASC` y `MC`, no como símbolos planetarios.

## Posiciones

La cabecera compacta de cada fila contiene:

- glifo;
- nombre;
- posición exacta;
- casa;
- movimiento.

Cada fila puede expandirse para mostrar:

- posición exacta;
- casa;
- regente o regentes del signo;
- signo de la cúspide de la casa real;
- regente o regentes de esa casa;
- movimiento persistido.

## Movimiento

La interfaz muestra `MotionText` procedente del read model.

No infiere movimiento a partir de velocidad, longitud ni diferencias entre
efemérides.

## Regencia de casa

La regencia de casa consume `HouseCuspSignText` y `HouseRulersText`.

No utiliza correspondencias fijas del tipo casa 1 = Aries.

## Ángulos

ASC y MC conservan `IsAngle` y muestran una etiqueta `ÁNGULO`.

Siguen siendo seleccionables, pero visualmente no se confunden con planetas.

## Puntos adicionales

Nodo Norte, Nodo Sur, Lilith, Parte de Fortuna, Quirón, Ceres, Palas, Juno y
Vesta utilizan el mismo pipeline factual aunque no estén visibles en una
configuración concreta de la rueda.

## Límite interpretativo

Datos y Posiciones solo muestran hechos técnicos de la carta persistida.

No generan personalidad, destino, misión ni interpretación psicológica.

# Miastro — Estilos semánticos del Scene Graph

## Principio

El Scene Graph conserva intención visual mediante StyleKey.

El backend de render resuelve esa clave contra
NatalSceneStyleCatalog.

La semántica visual no queda dispersa en el renderer.

## Paleta base

Fase 7 utiliza una identidad clara:

- fondo marfil
- texto y cuerpos en carbón
- azul grisáceo como color primario
- grises cálidos para divisiones
- arena suave como acento

No se utiliza tema oscuro en la rueda natal base.

## Jerarquía de grados

Las marcas de grado tienen tres estilos distintos:

- 1 grado: fina y discreta
- 5 grados: peso intermedio
- 10 grados: peso superior

## Ejes

ASC y MC utilizan AngleMajor.

DSC e IC utilizan AngleMinor.

Los ejes secundarios se diferencian también por patrón de línea,
no solamente por color.

## Aspectos

Se definen estilos independientes para:

- AspectMajor
- AspectSecondary

La distinción combina:

- grosor
- patrón de línea
- opacidad

La información no depende exclusivamente del color.

## Posición real y leader lines

RealPositionMark mantiene visible la posición astrológica exacta.

LeaderLine usa un estilo secundario discontinuo para no competir con
el glifo principal.

## Tipografía

Este bloque no incorpora fuentes externas ni copia fuentes del
sistema.

La tipografía ordinaria se empaquetará como recurso controlado en un
bloque específico, con licencia y comportamiento headless
verificables.

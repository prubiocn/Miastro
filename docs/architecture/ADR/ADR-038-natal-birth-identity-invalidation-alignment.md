# ADR-038 — Alineación entre identidad natal e invalidación

Estado: Propuesto durante Fase 6.

## Problema

Una carta invalidada no debe volver a considerarse idéntica a los datos de
nacimiento actuales si cambió cualquier dato que la política de Fase 6
considera natalmente relevante.

Por tanto, toda causa de invalidación debe quedar representada
criptográficamente en la identidad natal.

## Decisión

`NatalInputFingerprint` incluye los campos relevantes utilizados por
`BirthDataNatalChangeDetector`.

Además de fecha, hora, coordenadas, localidad, GeoNames, IANA, TZDB,
resolución UTC y selección ambigua, quedan representados:

- rango horario;
- periodo del día;
- país;
- región;
- subregión;
- estado de resolución histórica;
- candidato ambiguo anterior y posterior;
- offsets de ambos candidatos;
- override manual de coordenadas.

## Dos hashes

`BirthDataHash` identifica los datos de nacimiento y su resolución histórica.

`InputHash` añade a esa identidad las entradas propias del cálculo:

- sistema de casas;
- perfil de cálculo;
- motor/versiones;
- efemérides.

Cambiar Placidus por Koch cambia `InputHash`, pero no `BirthDataHash`.

## Compatibilidad

Fase 6 todavía no está cerrada ni publicada. La ampliación completa el
contrato BirthData V1 antes de su primera release.

No se necesita una nueva tabla ni una nueva columna: la identidad completa
queda incluida en los hashes existentes.

## Garantía

Una modificación que invalida una carta no puede reutilizar accidentalmente
un hash previo salvo que todos los datos natalmente relevantes vuelvan
realmente al mismo estado.

# Fase 8 — resaltado dual en la rueda natal

La selección de una celda de aspecto dispone de representación visual
bidireccional entre paneles y rueda.

La rueda conserva su Scene Graph base. La selección se aplica como una
decoración independiente en InteractionOverlay.

Selección simple:

- resalta el glifo del objeto seleccionado;
- no altera su posición real ni visual;
- no recalcula astronomía.

Selección dual:

- resalta los dos glifos participantes;
- resalta exclusivamente los segmentos gráficos del aspecto existente
  entre ambos objetos;
- admite aspectos divididos visualmente por el recorte del núcleo;
- conserva intactas las líneas de otros aspectos.

La identidad de los objetos se obtiene de object-glyph-{ObjectId}. Las
líneas se localizan por el prefijo estable del aspecto persistido. La
orientación del par puede recibirse en cualquiera de los dos sentidos.

La capa de selección es idempotente: antes de aplicar un nuevo estado se
eliminan únicamente los nodos de overlay creados por la propia selección.

Los cambios de selección no mutan snapshots, longitudes, cúspides,
aspectos persistidos ni layout.

# Fase 7 — jerarquía radial final

La rueda natal sigue la jerarquía visual definitiva:

1. circunferencia exterior del zodiaco;
2. circunferencia de grados zodiacales;
3. circunferencia de aspectos;
4. circunferencia del alma.

Los glifos planetarios no tienen una circunferencia propia visible.
Se distribuyen dentro de la banda amplia comprendida entre la
circunferencia de grados y la circunferencia de aspectos.

Las posiciones astrológicas reales continúan intactas en el modelo
de layout. No se dibuja una marca radial de posición real ni texto
de longitud al lado del glifo. La información se expone mediante
hit-testing y tooltip, reutilizando el read model persistido y sin
recalcular astronomía.

Los leaders se conservan únicamente cuando el algoritmo necesita
desplazar visualmente un glifo por antisolapamiento.

Saturno usa un símbolo vectorial estándar compuesto por cruz
superior, asta vertical y curva inferior unida a la base del asta.

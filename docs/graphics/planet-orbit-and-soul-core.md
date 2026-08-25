# Fase 7 — órbita planetaria y núcleo del alma

La rueda natal incluye dos referencias circulares interiores adicionales.

La órbita planetaria coincide con el radio nominal utilizado por el motor
de placement para la posición real de planetas y puntos. No modifica la
longitud astrológica ni el algoritmo de anti-solapamiento.

El núcleo del alma es una circunferencia central pequeña con relleno opaco.
Se representa como un nodo independiente del Scene Graph. Se pinta después
de las líneas de aspecto para mantener el centro visualmente completamente
limpio.

Los aspectos siguen derivados exclusivamente del snapshot persistido. Esta
decisión no recalcula posiciones, aspectos, casas ni ángulos.

La identidad geométrica histórica de cada aspecto se conserva como una
LineNode única. Si en una evolución posterior se requiere recorte geométrico
real contra el núcleo central, deberá introducirse como una decisión separada
con sus correspondientes invariantes y tests.

No se persisten posiciones gráficas absolutas.

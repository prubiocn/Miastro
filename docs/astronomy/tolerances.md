# Tolerancias de validación — Fase 3

Las pruebas astronómicas no utilizan igualdad exacta de `double` para
comparar fuentes externas.

Tolerancias V1:

- longitud eclíptica: 0.0001°
- velocidad longitudinal: 0.0001°/día
- cúspides: 0.0001°
- ASC: 0.0001°
- MC: 0.0001°

Equivalencia angular:

0.0001° = 0.36 segundos de arco.

La tolerancia permite diferencias menores de representación, conversión
temporal y serialización, pero es suficientemente estricta para detectar
errores de:

- cuerpo;
- flags;
- sistema de casas;
- instante;
- signo;
- grados/radianes;
- referencia tropical/sidérea;
- longitud geográfica.

No se redondea el resultado del adaptador antes de comparar.

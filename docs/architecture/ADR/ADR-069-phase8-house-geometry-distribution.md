# ADR-069 — Distribución natal geométrica basada en casas persistidas

## Estado

Aceptado para Fase 8.

## Contexto

La pestaña Distribución necesita hemisferios, cuadrantes y clasificación angular/sucedente/cadente.

Estas métricas deben ser deterministas y no deben duplicar la lógica que ya resolvió la casa de cada placement durante el cálculo natal.

## Decisión

Usar exclusivamente `HouseNumber` persistido.

Reglas:

- Este: casas 10, 11, 12, 1, 2, 3.
- Oeste: casas 4, 5, 6, 7, 8, 9.
- Superior: casas 7 a 12.
- Inferior: casas 1 a 6.
- Cuadrante I: casas 1 a 3.
- Cuadrante II: casas 4 a 6.
- Cuadrante III: casas 7 a 9.
- Cuadrante IV: casas 10 a 12.
- Angulares: 1, 4, 7, 10.
- Sucedentes: 2, 5, 8, 11.
- Cadentes: 3, 6, 9, 12.

La política de participantes es `NatalDistributionProfile.MiastroV1`.

Si un objeto incluido en el perfil carece de casa persistida, la lectura se rechaza en lugar de inferirla.

## Consecuencias

No hay cálculo geométrico adicional en UI ni Application Reading.

Los planetas cercanos a cúspides mantienen exactamente la casa persistida.

Distribución y Resumen pueden compartir los mismos resultados factuales.

# ADR-068 — DistributionProfile natal de Fase 8

## Estado

Aceptado para Fase 8.

## Contexto

Elementos, modalidades, polaridad, hemisferios, cuadrantes y naturaleza de casas necesitan compartir una política explícita sobre qué objetos participan.

Dispersar esta decisión por ViewModels o controles produciría resultados inconsistentes.

## Decisión

Crear `NatalDistributionProfile`.

El perfil principal V1 incluye exclusivamente los diez planetas:

Sol, Luna, Mercurio, Venus, Marte, Júpiter, Saturno, Urano, Neptuno y Plutón.

Los puntos, asteroides y ángulos no alteran la distribución principal.

Las reglas signo → elemento, modalidad y polaridad se centralizan en `NatalDistributionSignCatalog`.

El predominio requiere un máximo único.

Un empate no se resuelve arbitrariamente.

La lógica es headless y consume hechos derivados del snapshot persistido.

## Consecuencias

La UI no contiene reglas de distribución.

La política puede reutilizarse posteriormente en Resumen e informes.

Los futuros cálculos de hemisferios y cuadrantes compartirán el mismo perfil sin redefinir participantes.

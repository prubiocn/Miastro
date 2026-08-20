# Invariantes del dominio V1

## Ángulos

- Los ángulos deben ser finitos.
- Las longitudes eclípticas se normalizan a `[0°,360°)`.
- La separación mínima está en `[0°,180°]`.

## Signos

- Solo existen 12 signos válidos.
- Cada signo tiene exactamente un opuesto.
- Los ejes zodiacales solo pueden construirse con signos opuestos.

## Casas

- Solo son válidas casas 1–12.
- Cada casa tiene exactamente una casa opuesta.
- Los ejes de casas solo pueden construirse con polos opuestos.

## Objetos

- Solo se aceptan identificadores canónicos definidos por el dominio.
- Nodo Norte V1 es Nodo Verdadero.
- Nodo Sur es siempre derivado a +180°.
- Lilith V1 es Lilith Media.

## Aspectos

- Un ángulo exacto debe estar en `[0°,180°]`.
- El orbe no puede ser negativo.
- Un perfil debe contener aspectos y participantes.
- No puede repetir definiciones del mismo aspecto.
- El incremento por luminar es +1° total.
- La selección de aspecto es determinista.

## Carta

- `Guid.Empty` no es un identificador válido.
- Un objeto no puede aparecer dos veces en la misma carta.
- Las cúspides, si existen, forman un conjunto completo 1–12.

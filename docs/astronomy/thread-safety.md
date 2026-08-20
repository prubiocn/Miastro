# Thread safety de Swiss Ephemeris

Swiss Ephemeris mantiene estado de proceso asociado, entre otros, al
path de efemérides y recursos abiertos.

Miastro no presupone thread safety nativa.

Política V1:

- todas las llamadas al motor se serializan;
- el acceso utiliza `SwissEphemerisGate`;
- se configura el path dentro de la sección protegida;
- no se permiten cálculos concurrentes directos contra la ABI;
- ninguna capa externa recibe el handle nativo.

Esta política prioriza corrección y reproducibilidad sobre paralelismo.

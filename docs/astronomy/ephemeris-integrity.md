# Integridad de datos de efemérides

Antes de una posición real se valida:

1. existencia del directorio;
2. existencia de `manifest.json`;
3. validez del manifiesto;
4. presencia de cada fichero obligatorio;
5. tamaño exacto;
6. SHA-256 exacto.

Estados:

- Available
- Missing
- Corrupt
- UnsupportedRange
- Unknown

Miastro no permite que Swiss Ephemeris continúe hacia un fallback
silencioso cuando un recurso obligatorio está ausente o corrupto.

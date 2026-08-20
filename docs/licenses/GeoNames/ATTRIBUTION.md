# GeoNames attribution — Miastro

Miastro utiliza datos de GeoNames como catálogo geográfico offline.

Fuente oficial:

- https://www.geonames.org/
- https://download.geonames.org/export/dump/

Licencia:

- Creative Commons Attribution 4.0 International (CC BY 4.0).

Datasets de Fase 4:

- cities500.zip
- alternateNamesV2.zip
- countryInfo.txt
- admin1CodesASCII.txt
- admin2Codes.txt
- timeZones.txt

Miastro transforma estos datos a SQLite de solo lectura para funcionamiento
offline. La fecha del snapshot y los SHA-256 exactos quedan registrados en
`data/geography/geonames-source.lock.json`.

#!/usr/bin/env bash
set -euo pipefail

GEODATA="/usr/share/miastro/geodata"
DB="$GEODATA/geonames.sqlite"
MANIFEST="$GEODATA/manifest.json"

test -f "$DB"
test -f "$MANIFEST"
test -r "$DB"

python3 - "$DB" "$MANIFEST" <<'PY'
from pathlib import Path
import hashlib
import json
import sqlite3
import sys

db = Path(sys.argv[1])
manifest_path = Path(sys.argv[2])

manifest = json.loads(
    manifest_path.read_text(encoding="utf-8")
)

data_hash = hashlib.sha256(db.read_bytes()).hexdigest()

expected = manifest.get("databaseSha256")

if expected and data_hash.lower() != expected.lower():
    raise SystemExit(
        "ERROR: hash instalado de geonames.sqlite no coincide."
    )

connection = sqlite3.connect(
    f"file:{db}?mode=ro",
    uri=True,
)

integrity = connection.execute(
    "PRAGMA integrity_check"
).fetchone()[0]

schema = connection.execute(
    "SELECT value FROM metadata "
    "WHERE key='schema_version'"
).fetchone()[0]

locations = connection.execute(
    "SELECT COUNT(*) FROM locations"
).fetchone()[0]

pamplona = connection.execute(
    "SELECT COUNT(*) FROM locations "
    "WHERE normalized_name='pamplona'"
).fetchone()[0]

connection.close()

if integrity != "ok":
    raise SystemExit(
        f"ERROR: integrity_check={integrity}"
    )

if schema not in {"1", "2"}:
    raise SystemExit(
        f"ERROR: schema inesperado: {schema}"
    )

if locations < 1:
    raise SystemExit(
        "ERROR: catálogo instalado vacío."
    )

if pamplona < 1:
    raise SystemExit(
        "ERROR: catálogo instalado sin Pamplona."
    )

print(f"InstalledGeoSchema={schema}")
print(f"InstalledGeoLocations={locations}")
print(f"InstalledGeoSha256={data_hash}")
print("InstalledGeoIntegrity=PASS")
print("InstalledGeoReadOnlyOpen=PASS")
print("InstalledGeoSearchSmoke=PASS")
PY

echo "InstalledGeodata=PASS"

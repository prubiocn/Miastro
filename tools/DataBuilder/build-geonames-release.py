#!/usr/bin/env python3
from __future__ import annotations

import argparse
import hashlib
import json
import sqlite3
import unicodedata
import zipfile
from pathlib import Path

SCHEMA_VERSION = "2"
BUILDER_VERSION = "phase4-geonames-release-2"


def normalize(value: str) -> str:
    value = unicodedata.normalize("NFD", value.strip())
    value = "".join(
        ch for ch in value
        if unicodedata.category(ch) != "Mn"
    )
    return unicodedata.normalize(
        "NFC",
        " ".join(value.lower().split()),
    )


def sha256_file(path: Path) -> str:
    h = hashlib.sha256()
    with path.open("rb") as f:
        for block in iter(lambda: f.read(1024 * 1024), b""):
            h.update(block)
    return h.hexdigest()


def verify_sources(source_dir: Path, lock: dict) -> None:
    for item in lock["files"]:
        path = source_dir / item["name"]
        if not path.is_file():
            raise SystemExit(f"Missing GeoNames source: {path}")
        actual = sha256_file(path)
        if actual.lower() != item["sha256"].lower():
            raise SystemExit(
                f"Hash mismatch for {item['name']}: "
                f"{actual} != {item['sha256']}"
            )


def load_country_info(path: Path) -> dict[str, str]:
    result = {}
    with path.open(encoding="utf-8") as f:
        for line in f:
            if not line.strip() or line.startswith("#"):
                continue
            parts = line.rstrip("\n").split("\t")
            if len(parts) >= 5:
                result[parts[0]] = parts[4]
    return result


def load_admin_codes(path: Path) -> dict[str, str]:
    result = {}
    with path.open(encoding="utf-8") as f:
        for line in f:
            if not line.strip():
                continue
            parts = line.rstrip("\n").split("\t")
            if len(parts) >= 2:
                result[parts[0]] = parts[1]
    return result


def load_timezone_ids(path: Path) -> set[str]:
    result = set()
    with path.open(encoding="utf-8") as f:
        for index, line in enumerate(f):
            if index == 0 and "timezoneId" in line:
                continue
            if not line.strip():
                continue
            parts = line.rstrip("\n").split("\t")
            if len(parts) >= 2:
                result.add(parts[1])
    return result


def iter_zip_lines(path: Path, member: str):
    with zipfile.ZipFile(path) as zf:
        with zf.open(member) as raw:
            for line in raw:
                yield line.decode("utf-8").rstrip("\r\n")


def create_schema(connection: sqlite3.Connection) -> None:
    connection.executescript(
        """
PRAGMA journal_mode=DELETE;
PRAGMA synchronous=FULL;
PRAGMA foreign_keys=ON;
PRAGMA temp_store=MEMORY;
PRAGMA page_size=4096;

CREATE TABLE metadata (
    key TEXT PRIMARY KEY,
    value TEXT NOT NULL
);

CREATE TABLE locations (
    geoname_id INTEGER PRIMARY KEY,
    name TEXT NOT NULL,
    ascii_name TEXT NOT NULL,
    normalized_name TEXT NOT NULL,
    normalized_ascii_name TEXT NOT NULL,
    country TEXT NOT NULL,
    country_code TEXT NOT NULL,
    admin1 TEXT NOT NULL,
    admin1_code TEXT NOT NULL,
    admin2 TEXT NULL,
    admin2_code TEXT NULL,
    latitude REAL NOT NULL CHECK(latitude BETWEEN -90 AND 90),
    longitude REAL NOT NULL CHECK(longitude BETWEEN -180 AND 180),
    timezone_id TEXT NOT NULL,
    population INTEGER NULL,
    feature_class TEXT NOT NULL,
    feature_code TEXT NOT NULL,
    modification_date TEXT NULL
);

CREATE TABLE alternate_names (
    alternate_name_id INTEGER PRIMARY KEY,
    geoname_id INTEGER NOT NULL,
    language TEXT NULL,
    name TEXT NOT NULL,
    normalized_name TEXT NOT NULL,
    is_preferred INTEGER NOT NULL DEFAULT 0,
    is_short INTEGER NOT NULL DEFAULT 0,
    is_colloquial INTEGER NOT NULL DEFAULT 0,
    is_historic INTEGER NOT NULL DEFAULT 0,
    valid_from TEXT NULL,
    valid_to TEXT NULL,
    FOREIGN KEY(geoname_id) REFERENCES locations(geoname_id)
);

CREATE INDEX ix_locations_normalized_name
ON locations(normalized_name);

CREATE INDEX ix_locations_ascii
ON locations(normalized_ascii_name);

CREATE INDEX ix_locations_country
ON locations(country_code);

CREATE INDEX ix_locations_admin1
ON locations(admin1_code);

CREATE INDEX ix_locations_admin2
ON locations(admin2_code);

CREATE INDEX ix_locations_timezone
ON locations(timezone_id);

CREATE INDEX ix_alternate_names_normalized
ON alternate_names(normalized_name);

CREATE INDEX ix_alternate_names_geoname
ON alternate_names(geoname_id);

CREATE INDEX ix_alternate_names_language
ON alternate_names(language);

CREATE VIRTUAL TABLE location_fts USING fts5(
    geoname_id UNINDEXED,
    text,
    tokenize='unicode61 remove_diacritics 2'
);
"""
    )


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--source-dir", required=True)
    parser.add_argument("--lock", required=True)
    parser.add_argument("--output", required=True)
    args = parser.parse_args()

    source_dir = Path(args.source_dir).resolve()
    lock_path = Path(args.lock).resolve()
    output = Path(args.output).resolve()
    manifest_path = output.parent / "manifest.json"

    lock = json.loads(lock_path.read_text(encoding="utf-8"))
    verify_sources(source_dir, lock)

    countries = load_country_info(source_dir / "countryInfo.txt")
    admin1 = load_admin_codes(source_dir / "admin1CodesASCII.txt")
    admin2 = load_admin_codes(source_dir / "admin2Codes.txt")
    valid_timezones = load_timezone_ids(source_dir / "timeZones.txt")

    output.parent.mkdir(parents=True, exist_ok=True)
    if output.exists():
        output.unlink()

    connection = sqlite3.connect(output)
    connection.execute("PRAGMA locking_mode=EXCLUSIVE")
    create_schema(connection)

    city_ids = set()
    inline_alternates = {}
    location_count = 0
    invalid_timezone_count = 0

    connection.execute("BEGIN IMMEDIATE")

    for line in iter_zip_lines(
        source_dir / "cities500.zip",
        "cities500.txt",
    ):
        parts = line.split("\t")
        if len(parts) < 19:
            raise SystemExit(
                f"Invalid cities500 row: {len(parts)} columns"
            )

        geo_id = int(parts[0])
        name = parts[1]
        ascii_name = parts[2]
        alternates = [x for x in parts[3].split(",") if x]
        latitude = float(parts[4])
        longitude = float(parts[5])
        feature_class = parts[6]
        feature_code = parts[7]
        country_code = parts[8]
        admin1_code = parts[10]
        admin2_code = parts[11] or None
        population = int(parts[14]) if parts[14] else None
        timezone_id = parts[17]
        modification_date = parts[18] or None

        if timezone_id not in valid_timezones:
            invalid_timezone_count += 1

        admin1_key = (
            f"{country_code}.{admin1_code}"
            if admin1_code else ""
        )
        admin2_key = (
            f"{country_code}.{admin1_code}.{admin2_code}"
            if admin1_code and admin2_code else ""
        )

        connection.execute(
            """
INSERT INTO locations(
    geoname_id, name, ascii_name,
    normalized_name, normalized_ascii_name,
    country, country_code,
    admin1, admin1_code,
    admin2, admin2_code,
    latitude, longitude, timezone_id,
    population, feature_class, feature_code,
    modification_date
)
VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
""",
            (
                geo_id,
                name,
                ascii_name,
                normalize(name),
                normalize(ascii_name),
                countries.get(country_code, country_code),
                country_code,
                admin1.get(admin1_key, admin1_code),
                admin1_code,
                admin2.get(admin2_key) if admin2_key else None,
                admin2_code,
                latitude,
                longitude,
                timezone_id,
                population,
                feature_class,
                feature_code,
                modification_date,
            ),
        )

        city_ids.add(geo_id)
        inline_alternates[geo_id] = alternates
        location_count += 1

    connection.commit()

    if invalid_timezone_count:
        raise SystemExit(
            f"{invalid_timezone_count} locations reference time zones "
            "missing from GeoNames timeZones.txt"
        )

    alternate_count = 0
    connection.execute("BEGIN IMMEDIATE")

    for line in iter_zip_lines(
        source_dir / "alternateNamesV2.zip",
        "alternateNamesV2.txt",
    ):
        parts = line.split("\t")
        if len(parts) < 8:
            continue

        alternate_id = int(parts[0])
        geo_id = int(parts[1])
        if geo_id not in city_ids:
            continue

        language = parts[2] or None
        name = parts[3]
        if not name:
            continue

        preferred = 1 if parts[4] == "1" else 0
        short = 1 if parts[5] == "1" else 0
        colloquial = 1 if parts[6] == "1" else 0
        historic = 1 if parts[7] == "1" else 0
        valid_from = (
            parts[8]
            if len(parts) > 8 and parts[8]
            else None
        )
        valid_to = (
            parts[9]
            if len(parts) > 9 and parts[9]
            else None
        )

        connection.execute(
            """
INSERT OR IGNORE INTO alternate_names(
    alternate_name_id, geoname_id, language,
    name, normalized_name,
    is_preferred, is_short, is_colloquial, is_historic,
    valid_from, valid_to
)
VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
""",
            (
                alternate_id,
                geo_id,
                language,
                name,
                normalize(name),
                preferred,
                short,
                colloquial,
                historic,
                valid_from,
                valid_to,
            ),
        )
        alternate_count += 1

    connection.commit()

    synthetic_id = -1
    connection.execute("BEGIN IMMEDIATE")

    for geo_id in sorted(inline_alternates):
        existing = {
            row[0]
            for row in connection.execute(
                "SELECT normalized_name "
                "FROM alternate_names "
                "WHERE geoname_id=?",
                (geo_id,),
            )
        }

        for name in sorted(
            set(inline_alternates[geo_id]),
            key=normalize,
        ):
            normalized = normalize(name)
            if not normalized or normalized in existing:
                continue

            connection.execute(
                """
INSERT INTO alternate_names(
    alternate_name_id, geoname_id, language,
    name, normalized_name,
    is_preferred, is_short, is_colloquial, is_historic,
    valid_from, valid_to
)
VALUES (?, ?, NULL, ?, ?, 0, 0, 0, 0, NULL, NULL)
""",
                (synthetic_id, geo_id, name, normalized),
            )

            synthetic_id -= 1
            alternate_count += 1
            existing.add(normalized)

    connection.commit()

    connection.execute("BEGIN IMMEDIATE")

    rows = connection.execute(
        """
SELECT
    geoname_id,
    name,
    ascii_name,
    country,
    admin1,
    COALESCE(admin2, '')
FROM locations
ORDER BY geoname_id
"""
    )

    for geo_id, name, ascii_name, country, region, subregion in rows:
        text = " ".join(
            [name, ascii_name, country, region, subregion]
        )

        connection.execute(
            "INSERT INTO location_fts(geoname_id, text) "
            "VALUES (?, ?)",
            (geo_id, text),
        )

    connection.commit()

    metadata = {
        "schema_version": SCHEMA_VERSION,
        "builder_version": BUILDER_VERSION,
        "provider": "GeoNames",
        "snapshot_date": lock["snapshotDate"],
        "license": lock["license"],
        "dataset": "cities500+alternateNamesV2",
    }

    connection.execute("BEGIN IMMEDIATE")
    for key, value in sorted(metadata.items()):
        connection.execute(
            "INSERT INTO metadata(key, value) VALUES (?, ?)",
            (key, str(value)),
        )
    connection.commit()

    connection.execute("ANALYZE")
    connection.execute("PRAGMA optimize")
    connection.execute("VACUUM")

    integrity = connection.execute(
        "PRAGMA integrity_check"
    ).fetchone()[0]

    if integrity != "ok":
        raise SystemExit(
            f"SQLite integrity check failed: {integrity}"
        )

    stats = {
        "locations": connection.execute(
            "SELECT COUNT(*) FROM locations"
        ).fetchone()[0],
        "alternateNames": connection.execute(
            "SELECT COUNT(*) FROM alternate_names"
        ).fetchone()[0],
        "ftsRows": connection.execute(
            "SELECT COUNT(*) FROM location_fts"
        ).fetchone()[0],
        "timeZones": connection.execute(
            "SELECT COUNT(DISTINCT timezone_id) FROM locations"
        ).fetchone()[0],
    }

    connection.close()

    database_sha256 = sha256_file(output)

    manifest = {
        "provider": "GeoNames",
        "snapshotDate": lock["snapshotDate"],
        "license": lock["license"],
        "schemaVersion": SCHEMA_VERSION,
        "builderVersion": BUILDER_VERSION,
        "database": output.name,
        "databaseSize": output.stat().st_size,
        "databaseSha256": database_sha256,
        "sourceLockSha256": sha256_file(lock_path),
        "statistics": stats,
    }

    manifest_path.write_text(
        json.dumps(
            manifest,
            indent=2,
            ensure_ascii=False,
        ) + "\n",
        encoding="utf-8",
    )

    print(f"GeoNamesLocations={stats['locations']}")
    print(f"GeoNamesAlternateNames={stats['alternateNames']}")
    print(f"GeoNamesFtsRows={stats['ftsRows']}")
    print(f"GeoNamesTimeZones={stats['timeZones']}")
    print(f"GeoNamesDatabaseSize={output.stat().st_size}")
    print(f"GeoNamesDatabaseSha256={database_sha256}")
    print(f"GeoNamesManifest={manifest_path}")
    print("GeoNamesReleaseBuild=PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

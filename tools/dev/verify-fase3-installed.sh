#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
ROOT="$(cd -- "$SCRIPT_DIR/../.." && pwd)"
LIB="/usr/lib/miastro/native/libswe.so"
EPHE="/usr/share/miastro/ephemeris"
APP="/usr/bin/miastro"

export MIASTRO_REPO_ROOT="$ROOT"

test -x "$APP"
test -f "$LIB"
test -f "$EPHE/manifest.json"

EXPECTED_HASH="$(
python3 - <<'PY'
import json

from pathlib import Path
import os

root = Path(os.environ["MIASTRO_REPO_ROOT"])

with open(
    root / "third_party/swisseph/native-manifest.json",
    encoding="utf-8"
) as f:
    print(json.load(f)["sha256"])
PY
)"

ACTUAL_HASH="$(
  sha256sum "$LIB" |
  awk '{print $1}'
)"

if [[ "$EXPECTED_HASH" != "$ACTUAL_HASH" ]]; then
    echo "ERROR: libswe.so instalada no coincide."
    exit 430
fi

UNRESOLVED="$(
  ldd -r "$LIB" 2>&1 |
  grep -E \
    'undefined symbol|not found' \
    || true
)"

if [[ -n "$UNRESOLVED" ]]; then
    echo "$UNRESOLVED"
    exit 431
fi

python3 - <<'PY'
from pathlib import Path
import hashlib
import json

root = Path(
    "/usr/share/miastro/ephemeris"
)

manifest = json.loads(
    (root / "manifest.json").read_text()
)

for item in manifest["files"]:
    if not item["required"]:
        continue

    p = root / item["name"]

    if not p.exists():
        raise SystemExit(
            f"ERROR: falta {item['name']}"
        )

    data = p.read_bytes()

    if len(data) != item["size"]:
        raise SystemExit(
            f"ERROR: tamaño inválido {item['name']}"
        )

    if hashlib.sha256(data).hexdigest() != \
       item["sha256"].lower():
        raise SystemExit(
            f"ERROR: hash inválido {item['name']}"
        )

print("InstalledEphemerisIntegrity=PASS")
PY

TMP="$(mktemp -d)"
trap 'rm -rf "$TMP"' EXIT

cat > "$TMP/smoke.c" <<'EOF_C'
#include <stdio.h>
#include <dlfcn.h>

typedef char *(*version_fn)(char *);

int main(void)
{
    void *h = dlopen(
        "/usr/lib/miastro/native/libswe.so",
        RTLD_NOW | RTLD_LOCAL);

    if (!h) {
        fprintf(
            stderr,
            "%s\n",
            dlerror());

        return 1;
    }

    version_fn version =
        (version_fn)dlsym(
            h,
            "swe_version");

    if (!version) {
        return 2;
    }

    char buffer[256] = {0};

    if (!version(buffer)) {
        return 3;
    }

    printf(
        "InstalledSwissVersion=%s\n",
        buffer);

    dlclose(h);

    return 0;
}
EOF_C

cc \
  -Wall \
  -Wextra \
  -Werror \
  "$TMP/smoke.c" \
  -ldl \
  -o "$TMP/smoke"

"$TMP/smoke"

echo "InstalledLibSweHash=$ACTUAL_HASH"
echo "InstalledNativeABI=PASS"
echo "InstalledNativeSmoke=PASS"

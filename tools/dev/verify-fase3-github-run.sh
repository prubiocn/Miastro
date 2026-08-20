#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
ROOT="$(cd -- "$SCRIPT_DIR/../.." && pwd)"

cd "$ROOT"

EXPECTED_COMMIT="${1:-$(git rev-parse HEAD)}"

echo "=== MIASTRO — VERIFICACIÓN REMOTA FASE 3 ==="
echo "Rama: main"
echo "Commit esperado: $EXPECTED_COMMIT"

RUN_ID=""

for attempt in $(seq 1 24); do
    RUN_ID="$(
        gh run list \
          --workflow ci.yml \
          --branch main \
          --commit "$EXPECTED_COMMIT" \
          --limit 1 \
          --json databaseId \
          --jq '.[0].databaseId // empty'
    )"

    if [[ -n "$RUN_ID" ]]; then
        break
    fi

    echo "Esperando workflow... intento $attempt/24"
    sleep 5
done

if [[ -z "$RUN_ID" ]]; then
    echo "ERROR: no se encontró workflow para el commit."
    exit 470
fi

echo "RUN_ID=$RUN_ID"

set +e
gh run watch \
  "$RUN_ID" \
  --exit-status
WATCH_STATUS=$?
set -e

if [[ "$WATCH_STATUS" -ne 0 ]]; then
    echo
    echo "=== LOGS FALLIDOS ==="

    gh run view \
      "$RUN_ID" \
      --log-failed \
      || true

    echo
    echo "GITHUB_ACTIONS_FASE3=FAIL"
    exit 471
fi

JSON="$(
  gh run view "$RUN_ID" \
    --json \
      headBranch,headSha,status,conclusion,url,jobs
)"

python3 - "$EXPECTED_COMMIT" "$JSON" <<'PY'
import json
import sys

expected = sys.argv[1]
data = json.loads(sys.argv[2])

errors = []

if data.get("headBranch") != "main":
    errors.append("rama distinta de main")

if data.get("headSha") != expected:
    errors.append("commit remoto distinto")

if data.get("status") != "completed":
    errors.append("workflow no completado")

if data.get("conclusion") != "success":
    errors.append(
        f"workflow={data.get('conclusion')}"
    )

required = {
    "Checkout",
    "Setup .NET 10",
    "Restore",
    "Build",
    "Test",
    "Restore linux-x64 runtime",
    "Publish linux-x64 self-contained",
    "Verify published Swiss Ephemeris resources",
    "Build Phase 3 Debian package",
    "Install Phase 3 Debian package",
    "Verify installed Swiss Ephemeris",
    "Smoke installed application",
}

passed = set()

for job in data.get("jobs", []):
    if job.get("conclusion") != "success":
        errors.append(
            f"job {job.get('name')} no exitoso"
        )

    for step in job.get("steps", []):
        if (
            step.get("name") in required
            and step.get("conclusion") == "success"
        ):
            passed.add(step["name"])

for missing in sorted(required - passed):
    errors.append(
        f"paso obligatorio ausente/no exitoso: {missing}"
    )

if errors:
    print("GITHUB_ACTIONS_FASE3=FAIL")

    for error in errors:
        print("ERROR:", error)

    raise SystemExit(472)

print("GITHUB_ACTIONS_FASE3=PASS")

for name in sorted(required):
    print(f"{name}=PASS")

print("Workflow=PASS")
PY

URL="$(
  gh run view "$RUN_ID" \
    --json url \
    --jq '.url'
)"

echo
echo "=== FASE 3 — CI REMOTO: PASS ==="
echo "RUN_ID=$RUN_ID"
echo "COMMIT=$EXPECTED_COMMIT"
echo "URL=$URL"

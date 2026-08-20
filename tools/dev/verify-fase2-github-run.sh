#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
cd "$ROOT"

EXPECTED_COMMIT="f2838f46baa65e9fc0ceac6c1af2b9da33a20e3d"

echo "=== MIASTRO — VERIFICACIÓN REMOTA FASE 2 ==="
echo "Rama: main"
echo "Commit esperado: $EXPECTED_COMMIT"
echo

RUN_ID=""

for attempt in $(seq 1 18); do
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

    echo "Esperando registro del workflow... intento $attempt/18"
    sleep 5
done

if [[ -z "$RUN_ID" ]]; then
    echo "ERROR: no existe run remoto para $EXPECTED_COMMIT"
    exit 230
fi

echo "RUN_ID=$RUN_ID"
echo

set +e
gh run watch "$RUN_ID" --exit-status
WATCH_STATUS=$?
set -e

echo
echo "=== RESUMEN REMOTO ==="

gh run view "$RUN_ID" \
  --json databaseId,headBranch,headSha,status,conclusion,workflowName,createdAt,updatedAt,url,jobs \
  --jq '
  {
    runId: .databaseId,
    workflow: .workflowName,
    branch: .headBranch,
    commit: .headSha,
    status: .status,
    conclusion: .conclusion,
    createdAt: .createdAt,
    updatedAt: .updatedAt,
    url: .url,
    jobs: [
      .jobs[] | {
        name: .name,
        status: .status,
        conclusion: .conclusion,
        steps: [
          .steps[] | {
            name: .name,
            conclusion: .conclusion
          }
        ]
      }
    ]
  }'

if [[ "$WATCH_STATUS" -ne 0 ]]; then
    echo
    echo "=== LOGS DE PASOS FALLIDOS ==="
    gh run view "$RUN_ID" --log-failed || true
    echo
    echo "GITHUB_ACTIONS_FASE2=FAIL"
    exit 231
fi

JSON="$(
    gh run view "$RUN_ID" \
      --json headBranch,headSha,status,conclusion,jobs
)"

python3 - "$EXPECTED_COMMIT" "$JSON" <<'PY'
import json
import sys

expected_commit = sys.argv[1]
data = json.loads(sys.argv[2])

errors = []

if data.get("headBranch") != "main":
    errors.append(f"Rama incorrecta: {data.get('headBranch')}")

if data.get("headSha") != expected_commit:
    errors.append(
        f"Commit incorrecto: {data.get('headSha')} != {expected_commit}"
    )

if data.get("status") != "completed":
    errors.append(f"Workflow no completado: {data.get('status')}")

if data.get("conclusion") != "success":
    errors.append(f"Workflow no exitoso: {data.get('conclusion')}")

required = {
    "Checkout": False,
    "Setup .NET 10": False,
    "Restore": False,
    "Build": False,
    "Test": False,
    "Publish linux-x64 self-contained": False,
}

for job in data.get("jobs", []):
    if job.get("conclusion") != "success":
        errors.append(
            f"Job {job.get('name')} => {job.get('conclusion')}"
        )

    for step in job.get("steps", []):
        name = step.get("name")
        conclusion = step.get("conclusion")

        if name in required:
            if conclusion == "success":
                required[name] = True
            else:
                errors.append(
                    f"Paso {name} => {conclusion}"
                )

for name, ok in required.items():
    if not ok:
        errors.append(f"No validado: {name}")

if errors:
    print("GITHUB_ACTIONS_FASE2=FAIL")
    for error in errors:
        print("ERROR:", error)
    raise SystemExit(232)

print("GITHUB_ACTIONS_FASE2=PASS")
print("Checkout=PASS")
print("DotNet10=PASS")
print("Restore=PASS")
print("Build=PASS")
print("Tests=PASS")
print("PublishSelfContained=PASS")
print("Workflow=PASS")
PY

echo
echo "=== FASE 2 — CI REMOTO: PASS ==="
echo "RUN_ID=$RUN_ID"
echo "COMMIT=$EXPECTED_COMMIT"

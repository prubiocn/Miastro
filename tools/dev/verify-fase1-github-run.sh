#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
cd "$ROOT"

COMMIT="$(git rev-parse HEAD)"

echo "=== MIASTRO — VERIFICACIÓN GITHUB ACTIONS ==="
echo "Rama: main"
echo "Commit esperado: $COMMIT"
echo

echo ">>> EJECUCIONES RECIENTES"
gh run list \
  --workflow ci.yml \
  --branch main \
  --limit 5

echo
echo ">>> BUSCANDO RUN DEL COMMIT"

RUN_ID=""

for attempt in $(seq 1 12); do
    RUN_ID="$(
        gh run list \
          --workflow ci.yml \
          --branch main \
          --commit "$COMMIT" \
          --limit 1 \
          --json databaseId \
          --jq '.[0].databaseId // empty'
    )"

    if [[ -n "$RUN_ID" ]]; then
        break
    fi

    echo "Esperando a que GitHub registre el workflow... intento $attempt/12"
    sleep 5
done

if [[ -z "$RUN_ID" ]]; then
    echo "ERROR: no se encontró una ejecución para el commit $COMMIT"
    exit 130
fi

echo "RUN_ID=$RUN_ID"
echo

echo ">>> ESPERANDO RESULTADO REMOTO"
gh run watch "$RUN_ID" --exit-status

echo
echo ">>> RESUMEN DEL WORKFLOW"

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
              status: .status,
              conclusion: .conclusion
            }
          ]
        }
      ]
    }'

echo
echo ">>> VERIFICACIÓN DE PASOS"

JSON="$(
  gh run view "$RUN_ID" \
    --json headBranch,headSha,status,conclusion,jobs
)"

python3 - "$COMMIT" "$JSON" <<'PY'
import json
import sys

expected_commit = sys.argv[1]
data = json.loads(sys.argv[2])

errors = []

if data.get("headBranch") != "main":
    errors.append(f"rama inesperada: {data.get('headBranch')}")

if data.get("headSha") != expected_commit:
    errors.append(
        f"commit inesperado: {data.get('headSha')} != {expected_commit}"
    )

if data.get("status") != "completed":
    errors.append(f"workflow no completado: {data.get('status')}")

if data.get("conclusion") != "success":
    errors.append(f"workflow no exitoso: {data.get('conclusion')}")

required_fragments = {
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
            f"job fallido: {job.get('name')} => {job.get('conclusion')}"
        )

    for step in job.get("steps", []):
        name = step.get("name", "")
        conclusion = step.get("conclusion")

        for required in required_fragments:
            if required == name:
                if conclusion == "success":
                    required_fragments[required] = True
                else:
                    errors.append(
                        f"paso {required} => {conclusion}"
                    )

for required, ok in required_fragments.items():
    if not ok:
        errors.append(f"paso requerido no validado: {required}")

if errors:
    print("GITHUB_ACTIONS_VALIDATION=FAIL")
    for error in errors:
        print("ERROR:", error)
    raise SystemExit(131)

print("GITHUB_ACTIONS_VALIDATION=PASS")
print("Checkout: PASS")
print(".NET 10: PASS")
print("Restore: PASS")
print("Build: PASS")
print("Tests: PASS")
print("Publish linux-x64 self-contained: PASS")
print("Workflow global: PASS")
PY

echo
echo "=== GITHUB ACTIONS REMOTO: PASS ==="

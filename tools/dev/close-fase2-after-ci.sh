#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
REPORT="$ROOT/MIASTRO_Fase_2_Informe.md"

cd "$ROOT"

IMPLEMENTATION_COMMIT="f2838f46baa65e9fc0ceac6c1af2b9da33a20e3d"
IMPLEMENTATION_RUN_ID="32360029059"
IMPLEMENTATION_RUN_URL="https://github.com/prubiocn/Miastro/actions/runs/32360029059"
IMPLEMENTATION_RUN_CREATED="2026-08-20T10:39:57Z"
IMPLEMENTATION_RUN_UPDATED="2026-08-20T10:46:00Z"

test -f "$REPORT"

python3 - <<'PY'
from pathlib import Path
import re

path = Path("/home/pablo/Aplicaciones/Miastro/MIASTRO_Fase_2_Informe.md")
text = path.read_text()

text = text.replace(
    "Estado previo a validación CI remota:",
    "Estado final:"
)

text = text.replace(
    "- PASS: 43\n- FAIL: 0\n- PENDING: 1",
    "- PASS: 44\n- FAIL: 0\n- PENDING: 0"
)

text = re.sub(
    r"Pendiente único:\n\n- ejecución remota real de GitHub Actions para el commit de Fase 2\.\n\nLa fase no se declara todavía oficialmente cerrada\.",
    """Validación remota completada correctamente.

La Fase 2 queda oficialmente aprobada y cerrada.""",
    text
)

text = text.replace(
    "## 11. Resultado de aceptación previo a CI",
    "## 11. Resultado final de aceptación"
)

text = text.replace(
    "| PASS | 43 |\n| FAIL | 0 |\n| PENDING | 1 |",
    "| PASS | 44 |\n| FAIL | 0 |\n| PENDING | 0 |"
)

text = re.sub(
    r"El único PENDING es la ejecución remota de GitHub Actions\.\n\nLa Fase 2 no se declarará cerrada hasta que dicho workflow termine en SUCCESS\.",
    """GitHub Actions remoto ha finalizado en `SUCCESS`.

### Verificación remota

- Workflow: `Miastro CI`
- Rama: `main`
- Commit técnico verificado: `f2838f46baa65e9fc0ceac6c1af2b9da33a20e3d`
- Run ID: `32360029059`
- Inicio: `2026-08-20T10:39:57Z`
- Finalización: `2026-08-20T10:46:00Z`
- Job `build-test-publish`: PASS
- Checkout: PASS
- Setup .NET 10: PASS
- Restore: PASS
- Build: PASS
- Tests: PASS
- Publish linux-x64 self-contained: PASS
- Estado global: SUCCESS
- Ejecución: https://github.com/prubiocn/Miastro/actions/runs/32360029059

## 12. Cierre oficial

**PASS: 44**

**FAIL: 0**

**PENDING: 0**

**FASE 2 — APROBADA Y CERRADA**

No se inicia automáticamente la Fase 3.

La siguiente fase prevista es:

`Fase 3 — Integración de Swiss Ephemeris en Linux`

y deberá comenzar únicamente mediante una orden independiente.""",
    text
)

path.write_text(text)
PY

grep -q -- "- PASS: 44" "$REPORT"
grep -q -- "- FAIL: 0" "$REPORT"
grep -q -- "- PENDING: 0" "$REPORT"
grep -q "32360029059" "$REPORT"
grep -q "f2838f46baa65e9fc0ceac6c1af2b9da33a20e3d" "$REPORT"
grep -q "FASE 2 — APROBADA Y CERRADA" "$REPORT"

git add \
  MIASTRO_Fase_2_Informe.md \
  tools/dev/verify-fase2-github-run.sh \
  tools/dev/close-fase2-after-ci.sh

git diff --cached --check

git commit -m "Close Phase 2 after successful remote CI verification"

FINAL_COMMIT="$(git rev-parse HEAD)"

git push origin main

REMOTE_COMMIT="$(
  git ls-remote origin refs/heads/main |
  awk '{print $1}'
)"

if [[ "$FINAL_COMMIT" != "$REMOTE_COMMIT" ]]; then
    echo "ERROR: el commit final remoto no coincide."
    exit 240
fi

echo
echo "=== BUSCANDO CI DEL COMMIT FINAL ==="

FINAL_RUN_ID=""

for attempt in $(seq 1 18); do
    FINAL_RUN_ID="$(
        gh run list \
          --workflow ci.yml \
          --branch main \
          --commit "$FINAL_COMMIT" \
          --limit 1 \
          --json databaseId \
          --jq '.[0].databaseId // empty'
    )"

    if [[ -n "$FINAL_RUN_ID" ]]; then
        break
    fi

    echo "Esperando workflow final... intento $attempt/18"
    sleep 5
done

if [[ -z "$FINAL_RUN_ID" ]]; then
    echo "ERROR: no se encontró workflow para el commit final."
    exit 241
fi

echo "FINAL_RUN_ID=$FINAL_RUN_ID"

set +e
gh run watch "$FINAL_RUN_ID" --exit-status
WATCH_STATUS=$?
set -e

if [[ "$WATCH_STATUS" -ne 0 ]]; then
    echo
    echo "=== LOGS FALLIDOS ==="
    gh run view "$FINAL_RUN_ID" --log-failed || true
    echo
    echo "FASE_2_FINAL_CI=FAIL"
    exit 242
fi

JSON="$(
  gh run view "$FINAL_RUN_ID" \
    --json headBranch,headSha,status,conclusion,jobs
)"

python3 - "$FINAL_COMMIT" "$JSON" <<'PY'
import json
import sys

expected = sys.argv[1]
data = json.loads(sys.argv[2])

errors = []

if data.get("headBranch") != "main":
    errors.append("rama final distinta de main")

if data.get("headSha") != expected:
    errors.append("commit final no coincide")

if data.get("status") != "completed":
    errors.append("workflow final no completado")

if data.get("conclusion") != "success":
    errors.append(
        f"workflow final: {data.get('conclusion')}"
    )

required = {
    "Checkout",
    "Setup .NET 10",
    "Restore",
    "Build",
    "Test",
    "Publish linux-x64 self-contained",
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

missing = required - passed

for item in sorted(missing):
    errors.append(f"paso no validado: {item}")

if errors:
    print("FASE_2_FINAL_CI=FAIL")
    for error in errors:
        print("ERROR:", error)
    raise SystemExit(243)

print("FASE_2_FINAL_CI=PASS")
PY

FINAL_URL="$(
    gh run view "$FINAL_RUN_ID" \
      --json url \
      --jq '.url'
)"

echo
echo "=== CIERRE DEFINITIVO FASE 2 ==="
echo "PASS=44"
echo "FAIL=0"
echo "PENDING=0"
echo "ImplementationCommit=$IMPLEMENTATION_COMMIT"
echo "ImplementationRun=$IMPLEMENTATION_RUN_ID"
echo "FinalCommit=$FINAL_COMMIT"
echo "FinalRun=$FINAL_RUN_ID"
echo "FinalRunUrl=$FINAL_URL"
echo "FinalCI=SUCCESS"
echo "REPORT=$REPORT"
echo "FASE_2=APROBADA_Y_CERRADA"
echo "FASE_3=NO_INICIADA"

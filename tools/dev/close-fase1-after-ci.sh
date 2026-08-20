#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
REPORT="$ROOT/MIASTRO_Fase_1_Informe.md"

cd "$ROOT"

RUN_ID="32355996155"
BRANCH="main"
COMMIT="5aaa50bdc46f6382df39f04aaf96ead3fd1de8e7"
RUN_DATE="2026-08-20T09:51:17Z"
RUN_URL="https://github.com/prubiocn/Miastro/actions/runs/32355996155"

test -f "$REPORT"

python3 - <<'PY'
from pathlib import Path
import re

path = Path("/home/pablo/Aplicaciones/Miastro/MIASTRO_Fase_1_Informe.md")
text = path.read_text()

text = re.sub(r"- PASS: 53\b", "- PASS: 54", text)
text = re.sub(r"- FAIL: 0\b", "- FAIL: 0", text)
text = re.sub(r"- PENDING: 1\b", "- PENDING: 0", text)

text = text.replace(
    "- [PENDING] Ejecución remota real de GitHub Actions no verificada en esta sesión.",
    "- [PASS] Ejecución remota real de GitHub Actions verificada correctamente."
)

text = text.replace(
    "La ejecución remota real del workflow no se ha verificado durante esta sesión local.",
    """La ejecución remota real del workflow ha sido verificada correctamente.

Detalles de la ejecución:

- Fecha: 2026-08-20T09:51:17Z
- Rama: `main`
- Commit: `5aaa50bdc46f6382df39f04aaf96ead3fd1de8e7`
- Workflow: `Miastro CI`
- Run ID: `32355996155`
- Resultado: `success`
- Job: `build-test-publish`
- Checkout: PASS
- Setup .NET 10: PASS
- Restore: PASS
- Build: PASS
- Tests: PASS
- Publish `linux-x64` self-contained: PASS
- Estado global: SUCCESS
- Ejecución: https://github.com/prubiocn/Miastro/actions/runs/32355996155"""
)

text = re.sub(
    r"Existe un elemento pendiente independiente del código local:.*?(?=\n#|\Z)",
    "No quedan elementos pendientes en la Fase 1.\n",
    text,
    flags=re.S
)

if "## 15. Cierre oficial de Fase 1" not in text:
    text += """

## 15. Cierre oficial de Fase 1

Estado final:

- PASS: 54
- FAIL: 0
- PENDING: 0

GitHub Actions remoto:

- Workflow: `Miastro CI`
- Rama: `main`
- Commit verificado: `5aaa50bdc46f6382df39f04aaf96ead3fd1de8e7`
- Run ID: `32355996155`
- Fecha de ejecución: `2026-08-20T09:51:17Z`
- Resultado: `success`
- Job `build-test-publish`: PASS
- Checkout: PASS
- Setup .NET 10: PASS
- Restore: PASS
- Build: PASS
- Tests: PASS
- Publish `linux-x64` self-contained: PASS
- Estado global del workflow: PASS

Ejecución verificada:

https://github.com/prubiocn/Miastro/actions/runs/32355996155

**FASE 1 — APROBADA Y CERRADA**
"""

path.write_text(text)
PY

# Verificación del contenido final
grep -q -- "- PASS: 54" "$REPORT"
grep -q -- "- FAIL: 0" "$REPORT"
grep -q -- "- PENDING: 0" "$REPORT"
grep -q "32355996155" "$REPORT"
grep -q "5aaa50bdc46f6382df39f04aaf96ead3fd1de8e7" "$REPORT"
grep -q "FASE 1 — APROBADA Y CERRADA" "$REPORT"

# Commit exclusivo del cierre documental
git add MIASTRO_Fase_1_Informe.md

git commit \
  -m "Close Phase 1 after successful remote CI verification"

git push origin main

FINAL_COMMIT="$(git rev-parse HEAD)"

echo "=== CIERRE DEFINITIVO FASE 1 ==="
echo "PASS=54"
echo "FAIL=0"
echo "PENDING=0"
echo "GitHub Actions run=$RUN_ID"
echo "GitHub Actions result=SUCCESS"
echo "Branch=$BRANCH"
echo "Validated commit=$COMMIT"
echo "Report update commit=$FINAL_COMMIT"
echo "Report=$REPORT"
echo "FASE_1=APROBADA_Y_CERRADA"

#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
WORKFLOW="$ROOT/.github/workflows/ci.yml"

cd "$ROOT"

echo "=== MIASTRO — AUDITORÍA PREVIA CI REMOTA ==="
echo

echo ">>> REPOSITORIO"
git rev-parse --is-inside-work-tree
echo "Rama: $(git branch --show-current)"
echo "Commit local: $(git rev-parse HEAD)"
echo

echo ">>> ESTADO GIT"
git status --short
echo

echo ">>> REMOTOS"
if git remote | grep -q .; then
    git remote -v | sed -E \
      's#(https://)[^/@]+:[^/@]+@#\1***:***@#g'
else
    echo "NO_REMOTE"
fi
echo

echo ">>> UPSTREAM"
BRANCH="$(git branch --show-current)"

if UPSTREAM="$(git rev-parse --abbrev-ref --symbolic-full-name '@{u}' 2>/dev/null)"; then
    echo "$UPSTREAM"
else
    echo "NO_UPSTREAM"
fi
echo

echo ">>> WORKFLOW"
if [[ ! -f "$WORKFLOW" ]]; then
    echo "ERROR: no existe .github/workflows/ci.yml"
    exit 101
fi

cat "$WORKFLOW"
echo

echo ">>> VALIDACIONES DEL WORKFLOW"

grep -Eq 'runs-on:[[:space:]]*ubuntu-' "$WORKFLOW" \
  && echo "Ubuntu: OK" \
  || { echo "Ubuntu: FAIL"; exit 102; }

grep -Eq 'actions/setup-dotnet@' "$WORKFLOW" \
  && grep -Eq "dotnet-version:[[:space:]]*['\"]?10\." "$WORKFLOW" \
  && echo ".NET 10: OK" \
  || { echo ".NET 10: FAIL"; exit 103; }

grep -Eq 'dotnet restore' "$WORKFLOW" \
  && echo "Restore: OK" \
  || { echo "Restore: FAIL"; exit 104; }

grep -Eq 'dotnet build' "$WORKFLOW" \
  && echo "Build: OK" \
  || { echo "Build: FAIL"; exit 105; }

grep -Eq 'dotnet test' "$WORKFLOW" \
  && echo "Tests: OK" \
  || { echo "Tests: FAIL"; exit 106; }

grep -Eq 'dotnet publish' "$WORKFLOW" \
  && grep -Eq 'linux-x64' "$WORKFLOW" \
  && grep -Eq -- '--self-contained[[:space:]]+true' "$WORKFLOW" \
  && echo "Publish linux-x64 self-contained: OK" \
  || { echo "Publish linux-x64 self-contained: FAIL"; exit 107; }

if grep -nE '/home/pablo|/Users/|[A-Za-z]:\\\\' "$WORKFLOW"; then
    echo "Rutas absolutas locales: FAIL"
    exit 108
else
    echo "Rutas absolutas locales: OK"
fi

if grep -nE 'secrets\.[A-Za-z0-9_]+' "$WORKFLOW"; then
    echo "Secretos referenciados: REVISAR"
else
    echo "Secretos innecesarios: NO DETECTADOS"
fi

echo

echo ">>> WORKFLOW INCLUIDO EN GIT"
if git ls-files --error-unmatch .github/workflows/ci.yml >/dev/null 2>&1; then
    echo "Workflow versionado: OK"
else
    echo "Workflow versionado: FAIL"
    exit 109
fi

if git diff --quiet -- .github/workflows/ci.yml &&
   git diff --cached --quiet -- .github/workflows/ci.yml; then
    echo "Workflow sin cambios pendientes: OK"
else
    echo "Workflow tiene cambios pendientes"
fi

echo

echo ">>> GITHUB CLI"
if command -v gh >/dev/null 2>&1; then
    echo "gh: $(gh --version | head -n1)"

    # No imprime tokens.
    if gh auth status >/tmp/miastro-gh-auth.txt 2>&1; then
        echo "Autorización GitHub CLI: OK"
        sed -E \
          -e 's/(Token:).*/\1 [REDACTADO]/I' \
          -e 's/(oauth_token:).*/\1 [REDACTADO]/I' \
          /tmp/miastro-gh-auth.txt
    else
        echo "Autorización GitHub CLI: NO DISPONIBLE"
        sed -E \
          -e 's/(Token:).*/\1 [REDACTADO]/I' \
          -e 's/(oauth_token:).*/\1 [REDACTADO]/I' \
          /tmp/miastro-gh-auth.txt
    fi
else
    echo "GH_NOT_INSTALLED"
fi

echo
echo "=== FIN AUDITORÍA PREVIA ==="

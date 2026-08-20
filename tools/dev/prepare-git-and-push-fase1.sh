#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
cd "$ROOT"

echo "=== MIASTRO — PREPARACIÓN GIT Y PUSH FASE 1 ==="
echo

# ------------------------------------------------------------
# 1. Inicializar repositorio Git
# ------------------------------------------------------------

if [[ ! -d .git ]]; then
    git init -b main
fi

# Identidad Git solo para este repositorio si no existe.
if ! git config --local user.name >/dev/null 2>&1; then
    git config --local user.name "prubiocn"
fi

if ! git config --local user.email >/dev/null 2>&1; then
    # Dirección noreply para no exponer correo personal.
    git config --local user.email "prubiocn@users.noreply.github.com"
fi

# ------------------------------------------------------------
# 2. Remoto
# ------------------------------------------------------------

if git remote get-url origin >/dev/null 2>&1; then
    git remote set-url origin https://github.com/prubiocn/Miastro.git
else
    git remote add origin https://github.com/prubiocn/Miastro.git
fi

echo ">>> REMOTO"
git remote -v
echo

# ------------------------------------------------------------
# 3. Revisar workflow
# ------------------------------------------------------------

WORKFLOW=".github/workflows/ci.yml"

test -f "$WORKFLOW"

grep -Eq 'runs-on:[[:space:]]*ubuntu-' "$WORKFLOW"
grep -Eq 'actions/setup-dotnet@' "$WORKFLOW"
grep -Eq 'dotnet restore' "$WORKFLOW"
grep -Eq 'dotnet build' "$WORKFLOW"
grep -Eq 'dotnet test' "$WORKFLOW"
grep -Eq 'dotnet publish' "$WORKFLOW"
grep -Eq 'linux-x64' "$WORKFLOW"
grep -Eq -- '--self-contained[[:space:]]+true' "$WORKFLOW"

if grep -nE '/home/pablo|/Users/|[A-Za-z]:\\\\' "$WORKFLOW"; then
    echo "ERROR: el workflow contiene una ruta absoluta local."
    exit 120
fi

if grep -nE 'secrets\.[A-Za-z0-9_]+' "$WORKFLOW"; then
    echo "ERROR: el workflow referencia secretos que no son necesarios."
    exit 121
fi

echo "Workflow CI: revisión local OK"
echo

# ------------------------------------------------------------
# 4. Protección frente a artefactos locales
# ------------------------------------------------------------

cat >> .gitignore <<'EOF'

# Salidas locales
artifacts/
*.deb

# Datos de usuario: nunca pertenecen al repositorio
.local/
.cache/
EOF

sort -u .gitignore -o .gitignore

# Comprobación específica de posibles secretos comunes.
if find . \
    -path './.git' -prune -o \
    -path './artifacts' -prune -o \
    -type f \( \
        -name '*.pem' -o \
        -name '*.p12' -o \
        -name '*.pfx' -o \
        -name '*.key' \
    \) -print | grep -q .;
then
    echo "ERROR: se detectó un fichero potencialmente sensible."
    exit 122
fi

# ------------------------------------------------------------
# 5. Estado previo
# ------------------------------------------------------------

echo ">>> ARCHIVOS QUE ENTRARÁN EN EL COMMIT"
git status --short
echo

# ------------------------------------------------------------
# 6. Validación local inmediatamente antes del commit
# ------------------------------------------------------------

dotnet restore Miastro.sln
dotnet build Miastro.sln -c Release --no-restore
dotnet test tests/Miastro.Tests/Miastro.Tests.csproj \
    -c Release \
    --no-build \
    --logger "console;verbosity=minimal"

# ------------------------------------------------------------
# 7. Commit
# ------------------------------------------------------------

git add .

echo
echo ">>> CONTENIDO PREPARADO"
git status --short
echo

# Asegurarnos de que el workflow está realmente staged.
git diff --cached --name-only | grep -Fx '.github/workflows/ci.yml'

git commit -m "Establish Miastro Phase 1 technical baseline and CI"

COMMIT="$(git rev-parse HEAD)"

echo
echo "Commit creado: $COMMIT"

# ------------------------------------------------------------
# 8. Push
# ------------------------------------------------------------

git push -u origin main

echo
echo ">>> ESTADO TRAS PUSH"
git status --short
git branch -vv

echo
echo ">>> COMMIT REMOTO"
REMOTE_COMMIT="$(git ls-remote origin refs/heads/main | awk '{print $1}')"

echo "Local : $COMMIT"
echo "Remoto: $REMOTE_COMMIT"

if [[ "$COMMIT" != "$REMOTE_COMMIT" ]]; then
    echo "ERROR: el commit remoto no coincide con el local."
    exit 123
fi

echo
echo "PUSH_FASE1_OK"
echo "COMMIT=$COMMIT"

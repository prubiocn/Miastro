#!/usr/bin/env bash
set -uo pipefail

if ! command -v xclip >/dev/null 2>&1; then
  echo "ERROR: falta xclip. Instala: sudo apt install -y xclip" >&2
  exit 20
fi

if [[ -z "${DISPLAY:-}" ]]; then
  echo "ERROR: DISPLAY no está definido; se necesita X11/XWayland." >&2
  exit 21
fi

if [[ $# -eq 0 ]]; then
  echo "Uso: run-captured.sh comando [argumentos...]" >&2
  exit 2
fi

TMP_OUTPUT="/tmp/miastro_terminal_output.txt"

set +e
"$@" 2>&1 | tee "$TMP_OUTPUT"
STATUS=${PIPESTATUS[0]}
set -e

xclip -selection clipboard < "$TMP_OUTPUT"

echo
echo "Salida copiada al portapapeles."
echo "Copia temporal: $TMP_OUTPUT"

exit "$STATUS"

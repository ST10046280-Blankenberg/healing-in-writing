#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
REPO_ROOT="$( cd "${SCRIPT_DIR}/.." && pwd )"
PROJECT_FILE="${REPO_ROOT}/HealingInWriting.csproj"

if [ ! -f "$PROJECT_FILE" ]; then
  echo "Error: run this script from within the repo (could not find HealingInWriting.csproj)." >&2
  exit 1
fi

if ! command -v dotnet >/dev/null 2>&1 && { [ -z "${DOTNET_ROOT:-}" ] || [ ! -x "$DOTNET_ROOT/dotnet" ]; }; then
  echo "Error: dotnet CLI not found. Please install .NET 8 SDK." >&2
  exit 1
fi

read -r -p "Enter Google Books API key: " API_KEY
if [ -z "$API_KEY" ]; then
  echo "Error: API key cannot be empty." >&2
  exit 1
fi

if [ -n "${DOTNET_ROOT:-}" ] && [ -x "$DOTNET_ROOT/dotnet" ]; then
  DOTNET_CMD="$DOTNET_ROOT/dotnet"
else
  DOTNET_CMD="$( command -v dotnet )"
fi

pushd "$REPO_ROOT" >/dev/null
$DOTNET_CMD user-secrets set "ApiKeys:GoogleBooks" "$API_KEY"
popd >/dev/null

echo "Google Books API key stored in user secrets. Restart the app to use the updated key."

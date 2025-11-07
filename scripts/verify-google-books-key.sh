#!/usr/bin/env bash
set -euo pipefail

KEY_VALUE="${ApiKeys__GoogleBooks:-${APIKEYS__GOOGLEBOOKS:-${GOOGLE_BOOKS_API_KEY:-}}}"

if [ -z "$KEY_VALUE" ]; then
  echo "Google Books API key missing. Ensure ApiKeys__GoogleBooks (or GOOGLE_BOOKS_API_KEY) is set in the deployment environment." >&2
  exit 1
fi

echo "Google Books API key present (value not shown)."

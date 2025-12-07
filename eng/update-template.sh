#!/usr/bin/env bash
# SPDX-FileCopyrightText: 2025 smdn <smdn@smdn.jp>
# SPDX-License-Identifier: MIT

set -euo pipefail

dry_run=false
base_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
root_dir="$(cd "${base_dir}/../" && pwd)"
rsync_options=(-av)
declare -a exclude_files=(
  "LICENSE.txt"
  "README.md"
  "*.slnx"
  "doc/*"
  "eng/update-template.sh"
  "examples/*"
  "src/PackageProvidedAPI.targets"
  "*/Smdn.Template.Assembly/*"
  "*/Smdn.Template.Assembly/*/*"
)
declare -a delete_files=(
  "eng/InstallProjectAssets.proj"
  "src/ProjectAssets.props"
)

while [[ $# -gt 0 ]]; do
  case "$1" in
    -n|--dry-run)
      dry_run=true
      shift
      ;;
    -*)
      echo "Unknown option: $1" >&2
      exit 1
      ;;
    *)
      dest_dir="$1"
      shift
      break
      ;;
  esac
done

if [[ -z "${dest_dir-}" ]]; then
  echo "Usage: $(basename $0) [-n|--dry-run] DEST_DIR"
  exit 1
fi

$dry_run && rsync_options+=(-n)

for file_exclude in "${exclude_files[@]}"; do
  rsync_options+=(--exclude="$file_exclude")
done

rsync "${rsync_options[@]}" \
  --from0 \
  --files-from=<(cd "$root_dir" && git ls-files -z) \
  "$root_dir"/ \
  "$dest_dir"

\rm -f "${dest_dir}/${delete_files[@]}"

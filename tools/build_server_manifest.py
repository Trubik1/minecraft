#!/usr/bin/env python3
"""
Собирает server-manifest.json из manifest.json.

Серверу не нужны ресурспаки, шейдеры, картинки KubeJS и клиентские моды,
поэтому из манифеста выкидываются:
  * resourcepacks/, shaderpacks/
  * kubejs/assets/
  * моды, перечисленные в server/client-only.txt

Запускать после tools/build_manifest.py:
    python tools/build_server_manifest.py
"""

import json
import os
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
MANIFEST = os.path.join(ROOT, "manifest.json")
OUTPUT = os.path.join(ROOT, "server-manifest.json")
CLIENT_ONLY = os.path.join(ROOT, "server", "client-only.txt")

EXCLUDED_PREFIXES = (
    "resourcepacks/",
    "shaderpacks/",
    "kubejs/assets/",
)

SERVER_KUBEJS = (
    "kubejs/server_scripts/",
    "kubejs/startup_scripts/",
    "kubejs/data/",
    "kubejs/config/",
)


def load_patterns():
    patterns = []
    if not os.path.exists(CLIENT_ONLY):
        return patterns
    with open(CLIENT_ONLY, encoding="utf-8") as fh:
        for line in fh:
            line = line.strip()
            if not line or line.startswith("#"):
                continue
            patterns.append(line.lower())
    return patterns


def is_client_only(path, patterns):
    if not path.startswith("mods/"):
        return False
    name = os.path.basename(path).lower()
    return any(pattern in name for pattern in patterns)


def wanted(path, patterns):
    if path.startswith(EXCLUDED_PREFIXES):
        return False
    if path.startswith("kubejs/") and not path.startswith(SERVER_KUBEJS):
        return False
    if is_client_only(path, patterns):
        return False
    return True


def main():
    if not os.path.exists(MANIFEST):
        sys.exit("manifest.json не найден — сначала запустите tools/build_manifest.py")

    with open(MANIFEST, encoding="utf-8") as fh:
        manifest = json.load(fh)

    patterns = load_patterns()
    files = [item for item in manifest["files"] if wanted(item["path"], patterns)]
    skipped_mods = [
        item["path"] for item in manifest["files"]
        if item["path"].startswith("mods/") and is_client_only(item["path"], patterns)
    ]

    result = {
        "schema": manifest["schema"],
        "packVersion": manifest["packVersion"],
        "baseUrl": manifest["baseUrl"],
        "files": files,
    }

    with open(OUTPUT, "w", encoding="utf-8", newline="\n") as fh:
        json.dump(result, fh, ensure_ascii=False, separators=(",", ":"))
        fh.write("\n")

    total = sum(item["size"] for item in files)
    print(f"server-manifest.json: {len(files)} files, {total / 1024 / 1024:.1f} MB")
    print(f"client-only mods skipped: {len(skipped_mods)}")
    for path in skipped_mods:
        print("  - " + os.path.basename(path))


if __name__ == "__main__":
    main()

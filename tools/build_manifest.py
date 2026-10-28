#!/usr/bin/env python3
"""
Generates manifest.json for the Steampunk & Swords launcher.

The manifest lists every file the launcher must keep in sync with the repo
(mods, config, kubejs, resourcepacks, shaderpacks) together with a SHA-1 of the
file content.  Line endings are normalised (CRLF -> LF) before hashing, so a
file checked out with CRLF and the same file served by GitHub with LF produce
the same hash and the launcher does not re-download it forever.

Because the hash is computed from the committed blob (not the working tree),
the manifest always matches what is published on the git remote - run it after
committing your changes.

Usage:
    python tools/build_manifest.py            # write manifest.json
    python tools/build_manifest.py --check    # compare working tree with HEAD
"""

import argparse
import hashlib
import json
import os
import subprocess
import sys
from datetime import datetime, timezone

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUTPUT = os.path.join(ROOT, "manifest.json")

# Directories whose contents are distributed to players.
INCLUDE_ROOTS = (
    "mods/",
    "config/",
    "kubejs/",
    "defaultconfigs/",
    "resourcepacks/",
    "shaderpacks/",
)

# Files under INCLUDE_ROOTS that stay local to the player.
EXCLUDE_PREFIXES = (
    "config/ars_nouveau/search_index/",
    "config/inventory-particles/cache/",
)
EXCLUDE_FILES = {
    "config/iris.properties",      # player's shader choice
    "config/jei/jei-client.ini",   # player's JEI window layout
}

BASE_URL = "https://raw.githubusercontent.com/Trubik1/minecraft/main/"


def git(*args, text=True):
    return subprocess.run(
        ["git", "-C", ROOT, *args],
        check=True,
        capture_output=True,
        text=text,
    ).stdout


def wanted(path):
    if not path.startswith(INCLUDE_ROOTS):
        return False
    if path in EXCLUDE_FILES:
        return False
    return not path.startswith(EXCLUDE_PREFIXES)


def iter_tree():
    """Yield (path, blob_sha, size) for every tracked file we distribute."""
    out = git("ls-tree", "-r", "-l", "HEAD")
    for line in out.splitlines():
        meta, path = line.split("\t", 1)
        mode, kind, sha, size = meta.split()
        if kind != "blob":
            continue
        if not wanted(path):
            continue
        yield path, sha, int(size)


def hash_blobs(entries):
    """Read blobs from git and return {path: normalised sha1}."""
    result = {}
    proc = subprocess.Popen(
        ["git", "-C", ROOT, "cat-file", "--batch"],
        stdin=subprocess.PIPE,
        stdout=subprocess.PIPE,
    )
    try:
        for path, sha, _size in entries:
            proc.stdin.write(sha.encode() + b"\n")
            proc.stdin.flush()
            header = proc.stdout.readline().decode().split()
            if len(header) < 3 or header[1] != "blob":
                raise RuntimeError(f"cannot read blob {sha} ({path})")
            size = int(header[2])
            data = proc.stdout.read(size)
            proc.stdout.read(1)  # trailing newline
            digest = hashlib.sha1(data.replace(b"\r\n", b"\n")).hexdigest()
            result[path] = digest
    finally:
        proc.stdin.close()
        proc.wait()
    return result


def build():
    entries = sorted(iter_tree())
    if not entries:
        sys.exit("no files matched - are you on the right branch?")

    hashes = hash_blobs(entries)
    files = [
        {"path": path, "size": size, "hash": hashes[path]}
        for path, _sha, size in entries
    ]

    manifest = {
        "schema": 1,
        "packVersion": datetime.now(timezone.utc).strftime("%Y.%m.%d-%H%M"),
        "baseUrl": BASE_URL,
        "files": files,
    }

    with open(OUTPUT, "w", encoding="utf-8", newline="\n") as fh:
        json.dump(manifest, fh, ensure_ascii=False, separators=(",", ":"))
        fh.write("\n")

    total = sum(item["size"] for item in files)
    print(f"manifest.json: {len(files)} files, {total / 1024 / 1024:.1f} MB")
    print(f"packVersion:   {manifest['packVersion']}")


def check():
    with open(OUTPUT, encoding="utf-8") as fh:
        manifest = json.load(fh)
    expected = {item["path"]: item["hash"] for item in manifest["files"]}

    dirty = []
    for path, digest in expected.items():
        full = os.path.join(ROOT, path)
        try:
            with open(full, "rb") as fh:
                data = fh.read()
        except FileNotFoundError:
            dirty.append(f"{path}: missing")
            continue
        actual = hashlib.sha1(data.replace(b"\r\n", b"\n")).hexdigest()
        if actual != digest:
            dirty.append(f"{path}: content differs")

    extra = []
    for path, _sha, _size in iter_tree():
        if path not in expected:
            extra.append(path)

    if dirty or extra:
        print("working tree does NOT match manifest:")
        for line in dirty[:40]:
            print("  " + line)
        for line in extra[:40]:
            print("  extra in HEAD: " + line)
        sys.exit(1)
    print(f"working tree matches manifest ({len(expected)} files)")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--check", action="store_true")
    args = parser.parse_args()
    check() if args.check else build()


if __name__ == "__main__":
    main()

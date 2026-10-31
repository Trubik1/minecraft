# -*- coding: utf-8 -*-
import json, hashlib

ROOT = r"C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords"
with open(f"{ROOT}/manifest.json", "r", encoding="utf-8") as f:
    m = json.load(f)

for item in m["files"]:
    if "7create_aeronautics_2.snbt" in item["path"]:
        print("Manifest entry:", item)
        # Compute local hash
        with open(f"{ROOT}/{item['path']}", "rb") as fh:
            data = fh.read().replace(b"\r\n", b"\n")
            digest = hashlib.sha1(data).hexdigest()
            print("Local file hash:", digest)
            print("Matches?", item["hash"] == digest)

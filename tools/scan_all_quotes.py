# -*- coding: utf-8 -*-
import os, glob

base_dir = r"C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\config\ftbquests\quests"

all_snbt = []
for root, dirs, files in os.walk(base_dir):
    for f in files:
        if f.endswith('.snbt'):
            all_snbt.append(os.path.join(root, f))

print(f"Total SNBT files: {len(all_snbt)}")

for path in all_snbt:
    rel = os.path.relpath(path, base_dir)
    with open(path, 'r', encoding='utf-8', errors='replace') as f:
        for idx, line in enumerate(f):
            s = line.strip()
            # Check for \" at end of string or unclosed quotes
            if s.endswith('\\"') or s.endswith(':\\"') or (s.count('"') % 2 != 0 and not s.startswith('//')):
                print(f"{rel} line {idx+1}: {repr(s)}")

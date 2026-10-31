# -*- coding: utf-8 -*-
import os, glob, re

chapters_dir = r"C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\config\ftbquests\quests\chapters"
total_random = 0
with_table = 0
without_table = 0

samples_without = []
samples_with = []

for f in glob.glob(os.path.join(chapters_dir, "*.snbt")):
    with open(f, 'r', encoding='utf-8') as fh:
        text = fh.read()
    # find blocks like rewards: [ ... ]
    # or find blocks with type: "random"
    # let's find { ... type: "random" ... }
    blocks = re.findall(r'\{[^{}]*type:\s*"random"[^{}]*\}', text)
    for b in blocks:
        total_random += 1
        if "table_id" in b or "table:" in b:
            with_table += 1
            if len(samples_with) < 3:
                samples_with.append((os.path.basename(f), b.strip()))
        else:
            without_table += 1
            if len(samples_without) < 5:
                samples_without.append((os.path.basename(f), b.strip()))

print(f"Total random rewards: {total_random}")
print(f"With table: {with_table}")
print(f"WITHOUT table: {without_table}")

print("\n--- Samples WITH table ---")
for fn, s in samples_with:
    print(f"[{fn}] {s}")

print("\n--- Samples WITHOUT table ---")
for fn, s in samples_without:
    print(f"[{fn}] {s}")

# -*- coding: utf-8 -*-
import os, glob

tables_dir = r"C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\config\ftbquests\quests\reward_tables"
for f in glob.glob(os.path.join(tables_dir, "*.snbt")):
    with open(f, 'r', encoding='utf-8') as fh:
        content = fh.read()
    print("=== File:", os.path.basename(f), "===")
    for line in content.splitlines()[:10]:
        print(" ", line)

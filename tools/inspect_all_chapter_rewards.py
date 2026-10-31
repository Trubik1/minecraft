# -*- coding: utf-8 -*-
import glob, os, re

chapters_dir = r"C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\config\ftbquests\quests\chapters"

for p in sorted(glob.glob(os.path.join(chapters_dir, "*.snbt"))):
    fn = os.path.basename(p)
    with open(p, "r", encoding="utf-8") as f:
        content = f.read()
    
    blocks = re.findall(r'\{[^{}]*type:\s*"random"[^{}]*\}', content)
    if blocks:
        tids = []
        for b in blocks:
            m = re.search(r'table_id:\s*([0-9A-Za-z_L-]+)', b)
            tids.append(m.group(1) if m else "NONE")
        print(f"{fn:35}: {len(blocks)} random rewards | tables: {set(tids)}")

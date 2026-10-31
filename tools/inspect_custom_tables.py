# -*- coding: utf-8 -*-
import os

tables_dir = r"C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\config\ftbquests\quests\reward_tables"
for fn in ['storage_loot.snbt', 'magic_loot.snbt', 'refined_storage_loot.snbt', 'create_loot.snbt', 'rare.snbt', 'uncommon.snbt']:
    p = os.path.join(tables_dir, fn)
    with open(p, 'r', encoding='utf-8') as fh:
        print(f"=== {fn} ===")
        print(fh.read())

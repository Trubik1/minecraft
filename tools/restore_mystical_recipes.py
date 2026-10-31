# -*- coding: utf-8 -*-
import os

path = r"C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\kubejs\server_scripts\progression_recipes.js"

with open(path, "r", encoding="utf-8") as f:
    lines = f.readlines()

new_lines = []
removed_count = 0
for i, line in enumerate(lines):
    # Check lines to comment out / remove:
    # 1. remove({ output: /mysticalagriculture:.*_seeds/ })
    # 2. remove({ output: /mysticalagradditions:.*_seeds/ })
    # 3. remove({ type: 'mysticalagriculture:infusion' })
    # 4. remove({ output: 'mysticalagriculture:prosperity_seed_base' })
    # 5. remove({ output: 'mysticalagriculture:soulium_seed_base' })
    if (
        "output: /mysticalagriculture:.*_seeds/" in line
        or "output: /mysticalagradditions:.*_seeds/" in line
        or "type: 'mysticalagriculture:infusion'" in line
        or "output: 'mysticalagriculture:prosperity_seed_base'" in line
        or "output: 'mysticalagriculture:soulium_seed_base'" in line
    ):
        print(f"Commenting out line {i+1}: {line.strip()}")
        new_lines.append(f"    // [RESTORED DEFAULT RECIPES] {line.lstrip()}")
        removed_count += 1
    else:
        new_lines.append(line)

with open(path, "w", encoding="utf-8") as f:
    f.writelines(new_lines)

print(f"Total lines commented out: {removed_count}")

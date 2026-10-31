# -*- coding: utf-8 -*-
import glob, os

chapters_dir = r"C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\config\ftbquests\quests\chapters"

REPLACEMENTS = {
    "6524675549704257537L": "6524608693172600833L", # storage_loot (5A8C11099EFA8001)
    "6524675549704257538L": "6524608693172600834L", # magic_loot (5A8C11099EFA8002)
    "6524675549704257539L": "6524608693172600835L", # refined_storage_loot (5A8C11099EFA8003)
    "6524675549704257540L": "6524608693172600836L", # create_loot (5A8C11099EFA8004)
}

total_replaced = 0
for p in sorted(glob.glob(os.path.join(chapters_dir, "*.snbt"))):
    with open(p, "r", encoding="utf-8") as f:
        content = f.read()
    
    new_content = content
    for wrong_id, right_id in REPLACEMENTS.items():
        if wrong_id in new_content:
            count = new_content.count(wrong_id)
            new_content = new_content.replace(wrong_id, right_id)
            print(f"{os.path.basename(p)}: replaced {count} occurrences of {wrong_id} -> {right_id}")
            total_replaced += count
            
    if new_content != content:
        with open(p, "w", encoding="utf-8") as f:
            f.write(new_content)

print(f"Total replaced: {total_replaced}")

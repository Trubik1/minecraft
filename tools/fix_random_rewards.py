# -*- coding: utf-8 -*-
import os, glob, re

PROFILE = r"C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords"
CHAPTERS_DIR = os.path.join(PROFILE, "config", "ftbquests", "quests", "chapters")

CHAPTER_TABLE_MAP = {
    "chapter_7_refined_storage.snbt": "6524675549704257539L", # refined_storage_loot
    "ars_nouveau.snbt": "6524675549704257538L",              # magic_loot
    "chapter_twilight_forest.snbt": "6524675549704257538L",  # magic_loot
    "chapter_5_storage.snbt": "6524675549704257537L",        # storage_loot
    "create_connected.snbt": "6524675549704257540L",         # create_loot
    "create.snbt": "6524675549704257540L",                   # create_loot
    "create_aeronautics.snbt": "6524675549704257540L",        # create_loot
    "chapter_advanced_tech.snbt": "5564196992594175882L",    # rare
    "cyclic.snbt": "4196188979167302596L",                   # uncommon
    "chapter_1_base.snbt": "487623848494439020L",            # common
}
DEFAULT_TABLE = "487623848494439020L"

total_fixed = 0

for cpath in glob.glob(os.path.join(CHAPTERS_DIR, "*.snbt")):
    fname = os.path.basename(cpath)
    target_table = CHAPTER_TABLE_MAP.get(fname, DEFAULT_TABLE)
    
    with open(cpath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # We want to find rewards blocks with type: "random" and without table_id
    # e.g.:
    # {
    #     exclude_from_claim_all: true
    #     id: "..."
    #     type: "random"
    # }
    def replacer(match):
        block = match.group(0)
        if "table_id" in block or "table:" in block:
            return block
        
        global total_fixed
        total_fixed += 1
        # Insert table_id before type: "random"
        indent = "\t\t\t\t\t"
        # determine indent from line before type: "random"
        m_indent = re.search(r'(\r?\n)([ \t]*)type:\s*"random"', block)
        if m_indent:
            nl = m_indent.group(1)
            ind = m_indent.group(2)
            inserted = f"{nl}{ind}table_id: {target_table}{nl}{ind}type: \"random\""
            return block[:m_indent.start()] + inserted + block[m_indent.end():]
        else:
            return block.replace('type: "random"', f'table_id: {target_table}\n\t\t\t\t\ttype: "random"')

    new_content = re.sub(r'\{[^{}]*type:\s*"random"[^{}]*\}', replacer, content)
    
    if new_content != content:
        with open(cpath, 'w', encoding='utf-8') as f:
            f.write(new_content)
        print(f"Fixed {fname}")

print(f"Total random rewards fixed: {total_fixed}")

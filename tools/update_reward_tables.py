# -*- coding: utf-8 -*-
import os, glob, re

PROFILE = r"C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords"
TABLES_DIR = os.path.join(PROFILE, "config", "ftbquests", "quests", "reward_tables")
CHAPTERS_DIR = os.path.join(PROFILE, "config", "ftbquests", "quests", "chapters")

# 1. Ensure all reward tables have title and icon
table_titles = {
    "storage_loot.snbt": ("Награда хранилищ", "sophisticatedstorage:diamond_chest"),
    "magic_loot.snbt": ("Магическая награда", "ars_nouveau:source_gem"),
    "refined_storage_loot.snbt": ("Награда цифровых сетей", "refinedstorage:advanced_processor"),
    "create_loot.snbt": ("Инженерная награда Create", "create:precision_mechanism"),
    "common.snbt": ("Обычная награда", "minecraft:iron_ingot"),
    "uncommon.snbt": ("Необычная награда", "minecraft:diamond"),
    "rare.snbt": ("Редкая награда", "minecraft:netherite_scrap"),
    "epic.snbt": ("Эпическая награда", "minecraft:netherite_ingot"),
    "amekanismr_dadvanced_loot_boxr.snbt": ("Продвинутая награда Mekanism", "mekanism:advanced_control_circuit")
}

for fname, (title, icon_item) in table_titles.items():
    p = os.path.join(TABLES_DIR, fname)
    if not os.path.exists(p):
        continue
    with open(p, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Check if title exists
    if "title:" not in content:
        # insert before last }
        content = content.rstrip()
        if content.endswith('}'):
            content = content[:-1].rstrip() + f'\n\ttitle: "{title}"\n}}'
    
    # Check if icon exists
    if "icon:" not in content:
        # insert after id:
        content = re.sub(r'(\tid:\s*"[0-9A-Fa-f]+")', f'\\1\n\ticon: {{\n\t\tid: "{icon_item}"\n\t}}', content)
    
    with open(p, 'w', encoding='utf-8') as f:
        f.write(content)
    print(f"Updated reward table: {fname}")

print("Reward tables updated.")

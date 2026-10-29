import glob, os, re, json

CHAPTER_TITLES = {
    "chapter_1_base.snbt": "Базовое Выживание",
    "chapter_4_2_combat.snbt": "Боевые Искусства и Реликвии",
    "chapter_5_storage.snbt": "Хранилища и Сундуки",
    "chapter_boss_hunting.snbt": "Охота на Боссов и Данжи",
    "chapter_farmers_delight.snbt": "Farmer's Delight: Кулинария",
    "chapter_twilight_forest.snbt": "Сумеречный Лес (Twilight Forest)",
    "aether.snbt": "Эфир (The Aether)",
    "ars_nouveau.snbt": "Ars Nouveau: Магия Источника",
    "cyclic.snbt": "Cyclic: Утилиты и Магия",
    "deeper_and_darker.snbt": "Deeper and Darker (Глубже и Темнее)",
    "undergarden.snbt": "The Undergarden (Подземный Сад)",

    "create.snbt": "Create: Кинетическая Инженерия",
    "create_connected.snbt": "Create: Connected",
    "create_crafts__additions.snbt": "Create: Crafts & Additions (Электричество)",
    "chapter_big_cannons.snbt": "Create: Big Cannons (Тяжёлая Артиллерия)",
    "create_aeronautics.snbt": "Create Aeronautics: Воздухоплавание",
    "7create_aeronautics_2.snbt": "Aeronautics: Воздушный Флот",

    "chapter_7_refined_storage.snbt": "Refined Storage: Цифровые Сети",
    "mekanism.snbt": "Mekanism: Тяжёлая Промышленность",
    "powah.snbt": "Powah: Энергия и Реакторы",
    "pneumaticcraft.snbt": "PneumaticCraft: Сжатый Воздух",
    "solar_flux.snbt": "Solar Flux: Солнечная Энергетика",
    "botany_pots.snbt": "Botany Pots: Автоматические Фермы",
    "chapter_mystical_agriculture.snbt": "Mystical Agriculture: Мистическое Фермерство",
    "chapter_advanced_tech.snbt": "Передовые Технологии (Advanced Tech)",
    "create_schematics.snbt": "Схемы и Чертежи (Create Schematics)",

    "chapter_endgame.snbt": "Эндгейм и Мега-Крафты"
}

GROUP_TITLES = {
    "1643FF0B14990565": "Приключения и Магия",
    "181E781F1BAE7410": "Инженерия и Авиация",
    "0015B593B4889A40": "Индустрия и Технологии",
    "3E8A1F026B5C7D91": "Эндгейм и Мега-Крафты"
}

chapter_id_to_title = {}

print("1. Updating chapters/*.snbt cleanly at root level...")
for filepath in sorted(glob.glob("config/ftbquests/quests/chapters/*.snbt")):
    fname = os.path.basename(filepath)
    if fname not in CHAPTER_TITLES:
        print(f"Warning: unknown chapter file {fname}")
        continue

    title = CHAPTER_TITLES[fname]
    with open(filepath, "r", encoding="utf-8") as f:
        content = f.read()

    # Find root chapter ID
    m_id = re.search(r'^\s*id:\s*"([0-9A-Fa-f]+)"', content, re.M)
    if m_id:
        chapter_id_to_title[m_id.group(1)] = title

    # Remove any existing top-level or misplaced title: "..."
    content = re.sub(r'^\ttitle:\s*".*"\n?', '', content, flags=re.M)

    # Insert title right after filename: "..."\n
    content = re.sub(r'(filename:\s*".*"\n)', r'\1\ttitle: "' + title + '"\n', content, count=1)

    with open(filepath, "w", encoding="utf-8") as f:
        f.write(content)
    print(f"  Fixed {fname} -> '{title}' (Root ID: {m_id.group(1) if m_id else '?'})")

print("\n2. Updating chapter_groups.snbt...")
group_content = "{\n\tchapter_groups: [\n"
for gid, gtitle in GROUP_TITLES.items():
    group_content += f'\t\t{{ id: "{gid}", title: "{gtitle}" }}\n'
group_content += "\t]\n}\n"

with open("config/ftbquests/quests/chapter_groups.snbt", "w", encoding="utf-8") as f:
    f.write(group_content)

print("\n3. Updating data.snbt...")
with open("config/ftbquests/quests/data.snbt", "r", encoding="utf-8") as f:
    data_content = f.read()

data_content = re.sub(r'fallback_locale:\s*".*"', 'fallback_locale: "ru_ru"', data_content)
with open("config/ftbquests/quests/data.snbt", "w", encoding="utf-8") as f:
    f.write(data_content)

print("\n4. Updating lang files (ru_ru.snbt and en_us.snbt)...")
for lang_file in ["config/ftbquests/quests/lang/ru_ru.snbt", "config/ftbquests/quests/lang/en_us.snbt"]:
    with open(lang_file, "r", encoding="utf-8") as f:
        lines = f.readlines()

    other_lines = []
    for line in lines:
        m = re.match(r'^\s*([a-zA-Z0-9_\.]+):\s*(\[|"[^"]*")', line)
        if m and m.group(1).startswith(("chapter.", "chapter_group.")):
            pass
        else:
            other_lines.append(line)

    new_lines = ["{\n"]
    for cid, ctitle in sorted(chapter_id_to_title.items()):
        new_lines.append(f'\tchapter.{cid}.title: "{ctitle}"\n')
    for gid, gtitle in sorted(GROUP_TITLES.items()):
        new_lines.append(f'\tchapter_group.{gid}.title: "{gtitle}"\n')

    for l in other_lines:
        if l.strip() == "{" or l.strip() == "}":
            continue
        new_lines.append(l)
    new_lines.append("}\n")

    with open(lang_file, "w", encoding="utf-8") as f:
        f.writelines(new_lines)
    print(f"  {lang_file} updated!")

print("\n5. Mirroring to kubejs/assets/ftbquests/lang/...")
os.makedirs("kubejs/assets/ftbquests/lang", exist_ok=True)

lang_json = {}
for cid, ctitle in sorted(chapter_id_to_title.items()):
    lang_json[f"chapter.{cid}.title"] = ctitle
for gid, gtitle in sorted(GROUP_TITLES.items()):
    lang_json[f"chapter_group.{gid}.title"] = gtitle

with open("kubejs/assets/ftbquests/lang/ru_ru.json", "w", encoding="utf-8") as f:
    json.dump(lang_json, f, ensure_ascii=False, indent=2)

with open("kubejs/assets/ftbquests/lang/en_us.json", "w", encoding="utf-8") as f:
    json.dump(lang_json, f, ensure_ascii=False, indent=2)

print("  kubejs/assets/ftbquests/lang/ru_ru.json and en_us.json created!")
print("\n=== All Quest Titles Fixed! ===")

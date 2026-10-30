import os, re, json

PROFILE = r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords'
FTB_QUESTS_DIR = os.path.join(PROFILE, 'config', 'ftbquests', 'quests')

# 1. Parse ru_ru.snbt and en_us.snbt
def parse_lang(path):
    translations = {}
    if not os.path.exists(path):
        return translations
    with open(path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Matches: key: "value" or key: ["desc1", "desc2"]
    # We can parse line by line or block by block
    lines = content.splitlines()
    i = 0
    while i < len(lines):
        line = lines[i].strip()
        if not line or line.startswith('{') or line.startswith('}'):
            i += 1
            continue
        
        m = re.match(r'^([\w\.]+):\s*(.*)', line)
        if m:
            key = m.group(1)
            rest = m.group(2).strip()
            if rest.startswith('['):
                # multi-line or single-line list
                desc_lines = []
                if rest.endswith(']') and len(rest) > 1:
                    desc_content = rest
                else:
                    desc_parts = [rest]
                    i += 1
                    while i < len(lines):
                        cur = lines[i].strip()
                        desc_parts.append(cur)
                        if cur.endswith(']'):
                            break
                        i += 1
                    desc_content = " ".join(desc_parts)
                translations[key] = desc_content
            elif rest.startswith('"') and rest.endswith('"'):
                translations[key] = rest[1:-1]
        i += 1
    return translations

print("Parsing translations...")
ru_trans = parse_lang(os.path.join(FTB_QUESTS_DIR, 'lang', 'ru_ru.snbt'))
en_trans = parse_lang(os.path.join(FTB_QUESTS_DIR, 'lang', 'en_us.snbt'))
print(f"Loaded {len(ru_trans)} ru_ru keys, {len(en_trans)} en_us keys")

def get_text(key):
    if key in ru_trans and ru_trans[key]:
        return ru_trans[key]
    if key in en_trans and en_trans[key]:
        return en_trans[key]
    return None

# 2. Update chapter_groups.snbt
groups_path = os.path.join(FTB_QUESTS_DIR, 'chapter_groups.snbt')
with open(groups_path, 'r', encoding='utf-8') as f:
    groups_text = f.read()

# For each { id: "HEX" } or { id: "HEX", title: "..." }
def replace_group(match):
    gid = match.group(1)
    title = get_text(f"chapter_group.{gid}.title")
    if not title:
        title = get_text(f"chapter_group.{gid}")
    if title:
        title_str = title.replace('"', '\\"')
        return f'{{ id: "{gid}", title: "{title_str}" }}'
    return match.group(0)

groups_text = re.sub(r'\{\s*id:\s*"([0-9A-Fa-f]{16})"(?:,\s*title:\s*"[^"]*")?\s*\}', replace_group, groups_text)
with open(groups_path, 'w', encoding='utf-8') as f:
    f.write(groups_text)
print("Updated chapter_groups.snbt")

# 3. Update all chapters/*.snbt
chapters_dir = os.path.join(FTB_QUESTS_DIR, 'chapters')
for fname in os.listdir(chapters_dir):
    if not fname.endswith('.snbt'):
        continue
    cpath = os.path.join(chapters_dir, fname)
    with open(cpath, 'r', encoding='utf-8') as f:
        ctext = f.read()
    
    # Check chapter id: id: "HEX" at root level
    m_cid = re.search(r'\bid:\s*"([0-9A-Fa-f]{16})"', ctext)
    if m_cid:
        cid = m_cid.group(1)
        ctitle = get_text(f"chapter.{cid}.title")
        if ctitle:
            # Check if title: already present in header
            if re.search(r'^\s*title:\s*".*?"', ctext, re.MULTILINE):
                ctext = re.sub(r'^\s*title:\s*".*?"', f'\ttitle: "{ctitle}"', ctext, count=1, flags=re.MULTILINE)
            else:
                # Add title: right after filename: or id:
                ctext = re.sub(r'(\bfilename:\s*"[^"]*"\n)', r'\1\ttitle: "' + ctitle + '"\n', ctext, count=1)

    # Now for each quest block inside quests: [ ... ]
    # Quests have: id: "HEX"
    # We want to ensure each quest has title, subtitle, and description
    def inject_quest_info(quest_match):
        qblock = quest_match.group(0)
        m_qid = re.search(r'\bid:\s*"([0-9A-Fa-f]{16})"', qblock)
        if not m_qid:
            return qblock
        qid = m_qid.group(1)
        
        q_title = get_text(f"quest.{qid}.title")
        q_sub = get_text(f"quest.{qid}.quest_subtitle")
        q_desc = get_text(f"quest.{qid}.quest_desc")
        
        # If title exists in lang and not in qblock
        if q_title and not re.search(r'\btitle:\s*"', qblock):
            q_title_esc = q_title.replace('"', '\\"')
            qblock = re.sub(r'(\bid:\s*"[0-9A-Fa-f]{16}"\n)', r'\1\t\t\ttitle: "' + q_title_esc + '"\n', qblock, count=1)
        
        # If subtitle exists in lang and not in qblock
        if q_sub and not re.search(r'\bsubtitle:\s*"', qblock):
            q_sub_esc = q_sub.replace('"', '\\"')
            qblock = re.sub(r'(\bid:\s*"[0-9A-Fa-f]{16}"\n)', r'\1\t\t\tsubtitle: "' + q_sub_esc + '"\n', qblock, count=1)
            
        # If description exists in lang and not in qblock
        if q_desc and not re.search(r'\bdescription:\s*\[', qblock):
            # q_desc is already formatted like ["line 1", "line 2"]
            qblock = re.sub(r'(\bid:\s*"[0-9A-Fa-f]{16}"\n)', r'\1\t\t\tdescription: ' + q_desc + '\n', qblock, count=1)
            
        return qblock

    # Quest blocks inside quests: [ ... ]
    # Matches each object { ... } inside quests:
    pattern = r'\{\s*(?:[^{}]*|\{[^{}]*\})*id:\s*"[0-9A-Fa-f]{16}"(?:[^{}]*|\{[^{}]*\})*\}'
    # To be safe, do regex replacement on individual quest objects
    # Let's split by quests: [
    if 'quests: [' in ctext:
        header, quests_part = ctext.split('quests: [', 1)
        # Find closing ] for quests
        # Each quest starts with \t\t{ and ends with \t\t}
        def q_replacer(m):
            q_body = m.group(0)
            m_qid = re.search(r'\bid:\s*"([0-9A-Fa-f]{16})"', q_body)
            if not m_qid:
                return q_body
            qid = m_qid.group(1)
            
            q_title = get_text(f"quest.{qid}.title")
            q_sub = get_text(f"quest.{qid}.quest_subtitle")
            q_desc = get_text(f"quest.{qid}.quest_desc")
            
            lines_to_add = []
            if q_title and not re.search(r'\btitle:\s*"', q_body):
                lines_to_add.append(f'\t\t\ttitle: "{q_title}"')
            if q_sub and not re.search(r'\bsubtitle:\s*"', q_body):
                lines_to_add.append(f'\t\t\tsubtitle: "{q_sub}"')
            if q_desc and not re.search(r'\bdescription:\s*\[', q_body):
                lines_to_add.append(f'\t\t\tdescription: {q_desc}')
                
            if lines_to_add:
                # Add right before closing }
                last_brace = q_body.rfind('}')
                addition = "\n".join(lines_to_add) + "\n\t\t"
                q_body = q_body[:last_brace] + addition + q_body[last_brace:]
            return q_body

        quests_part = re.sub(r'\t\t\{[\s\S]*?\n\t\t\}', q_replacer, quests_part)
        ctext = header + 'quests: [' + quests_part

    with open(cpath, 'w', encoding='utf-8') as f:
        f.write(ctext)

print("Inlined all quests across all chapters in profile!")

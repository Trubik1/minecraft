import glob, re

ru_titles = {}
with open('config/ftbquests/quests/lang/ru_ru.snbt', encoding='utf-8') as f:
    for line in f:
        m = re.match(r'^\s*(chapter|chapter_group)\.([0-9A-Fa-f]+)\.title:\s*"(.*)"', line)
        if m:
            ru_titles[m.group(2)] = m.group(3)

en_titles = {}
with open('config/ftbquests/quests/lang/en_us.snbt', encoding='utf-8') as f:
    for line in f:
        m = re.match(r'^\s*(chapter|chapter_group)\.([0-9A-Fa-f]+)\.title:\s*"(.*)"', line)
        if m:
            en_titles[m.group(2)] = m.group(3)

print('--- Chapter Groups ---')
for gid in ['1643FF0B14990565', '181E781F1BAE7410', '0015B593B4889A40', '3E8A1F026B5C7D91']:
    print(f'Group {gid}: RU={ru_titles.get(gid)} | EN={en_titles.get(gid)}')

print('\n--- Chapters ---')
for filepath in sorted(glob.glob('config/ftbquests/quests/chapters/*.snbt')):
    filename = filepath.replace('\\', '/').split('/')[-1]
    with open(filepath, encoding='utf-8') as f:
        content = f.read()
    # match top-level id
    m_id = re.search(r'^\s*id:\s*"([0-9A-Fa-f]+)"', content, re.M)
    cid = m_id.group(1) if m_id else None
    print(f'{filename:35} | ID={cid} | RU={ru_titles.get(cid)} | EN={en_titles.get(cid)}')

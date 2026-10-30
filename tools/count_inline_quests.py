import os, re

chapters_dir = r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\config\ftbquests\quests\chapters'

total_quests = 0
quests_with_title = 0
quests_with_desc = 0

for f in os.listdir(chapters_dir):
    if not f.endswith('.snbt'):
        continue
    with open(os.path.join(chapters_dir, f), 'r', encoding='utf-8') as fh:
        text = fh.read()
    
    # count quests by id: "16 hex chars"
    ids = re.findall(r'id:\s*"([0-9A-Fa-f]{16})"', text)
    # first id is usually chapter id
    total_quests += max(0, len(ids) - 1)
    
    titles = re.findall(r'title:\s*"([^"]+)"', text)
    quests_with_title += max(0, len(titles) - 1)
    
    descs = re.findall(r'description:\s*\[', text)
    quests_with_desc += len(descs)

print(f"Total quests across all chapters: {total_quests}")
print(f"Quests with inline title: {quests_with_title}")
print(f"Quests with inline description: {quests_with_desc}")

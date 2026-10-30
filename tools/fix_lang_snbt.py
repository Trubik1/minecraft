import os

for path in [
    r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\config\ftbquests\quests\lang\ru_ru.snbt',
    r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\server_pack\config\ftbquests\quests\lang\ru_ru.snbt'
]:
    with open(path, 'r', encoding='utf-8') as f:
        text = f.read()
    bad = '\tquest.5E4BC0F59C90433A.quest_subtitle: "\\"\n\tquest.5E4BC0F59C90433A.quest_desc: ["\n'
    good = '\tquest.5E4BC0F59C90433A.quest_subtitle: ""\n\tquest.5E4BC0F59C90433A.quest_desc: [""]\n'
    if bad in text:
        text = text.replace(bad, good)
        with open(path, 'w', encoding='utf-8') as f:
            f.write(text)
        print('Fixed ru_ru:', path)

for path in [
    r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\config\ftbquests\quests\lang\en_us.snbt',
    r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\server_pack\config\ftbquests\quests\lang\en_us.snbt'
]:
    with open(path, 'r', encoding='utf-8') as f:
        text = f.read()
    bad1 = '\tquest.008E65AF545A706E.title: ""&d&lSuper-Critical Phase Shifter""\n\tquest.008E65AF545A706E.quest_subtitle: ""&d&lSPS""\n\tquest.008E65AF545A706E.quest_desc: "[\n'
    good1 = '\tquest.008E65AF545A706E.title: "&d&lSuper-Critical Phase Shifter"\n\tquest.008E65AF545A706E.quest_subtitle: "&d&lSPS"\n\tquest.008E65AF545A706E.quest_desc: [\n'
    if bad1 in text:
        text = text.replace(bad1, good1)
        with open(path, 'w', encoding='utf-8') as f:
            f.write(text)
        print('Fixed en_us:', path)

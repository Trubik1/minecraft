with open(r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\config\ftbquests\quests\lang\en_us.snbt', 'r', encoding='utf-8') as f:
    lines = f.readlines()

print(f'Total lines: {len(lines)}')
for i in range(5713, len(lines)):
    line = lines[i]
    if line.strip().startswith('quest.'):
        print(f'{i+1}: {line[:80]}...')

with open(r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\config\ftbquests\quests\lang\en_us.snbt', 'r', encoding='utf-8') as f:
    lines = f.readlines()

for i in range(5710, min(len(lines), 5735)):
    print(f'{i+1}: {lines[i]}', end='')

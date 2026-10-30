import os, re

p = r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\config\ftbquests\quests\chapters\deeper_and_darker.snbt'
with open(p, 'r', encoding='utf-8') as f:
    text = f.read()

titles = re.findall(r'title:\s*"([^"]+)"', text)
print("Titles:", len(titles), titles[:5])
descriptions = re.findall(r'description:\s*\[(.*?)\]', text, re.DOTALL)
print("Descriptions:", len(descriptions))

# Let's inspect the first quest in deeper_and_darker
quests = re.findall(r'quests:\s*\[(.*)', text, re.DOTALL)
if quests:
    print("First 500 chars of quests array:")
    print(quests[0][:500])

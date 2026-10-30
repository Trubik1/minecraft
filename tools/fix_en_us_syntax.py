import re

def clean_en_us(path):
    with open(path, 'r', encoding='utf-8') as f:
        lines = f.readlines()
    
    # We only need to fix lines from 5713 onwards
    fixed_lines = lines[:5713]
    tail_text = "".join(lines[5713:])
    
    # Replace double double-quotes with single double-quotes: ""..."" -> "..."
    # Pattern: : ""(.*?)""\n -> : "$1"\n
    tail_text = re.sub(r':\s*""(.*?)""\n', r': "\1"\n', tail_text)
    
    # Replace "["..."]" with ["..."]
    tail_text = re.sub(r':\s*"\["(.*?)"\]"', r': ["\1"]', tail_text)
    tail_text = re.sub(r':\s*"\[\n', r': [\n', tail_text)
    tail_text = re.sub(r'\t\]"', r'\t]', tail_text)
    
    # Now check any remaining "" at the start or end of string values
    tail_lines = tail_text.splitlines(keepends=True)
    cleaned_tail = []
    for l in tail_lines:
        # replace any remaining : "" with : "
        if ': ""' in l:
            l = l.replace(': ""', ': "')
        # if line ends with ""\n, replace with "\n
        if l.endswith('""\n'):
            l = l[:-3] + '"\n'
        cleaned_tail.append(l)
        
    full_text = "".join(fixed_lines) + "".join(cleaned_tail)
    
    with open(path, 'w', encoding='utf-8') as f:
        f.write(full_text)
    print('Cleaned', path)

clean_en_us(r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\config\ftbquests\quests\lang\en_us.snbt')
clean_en_us(r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\server_pack\config\ftbquests\quests\lang\en_us.snbt')

# -*- coding: utf-8 -*-
import os, glob

def check_file(path):
    with open(path, 'r', encoding='utf-8', errors='replace') as f:
        lines = f.readlines()
    
    in_str = False
    escape = False
    for line_idx, line in enumerate(lines):
        line_num = line_idx + 1
        i = 0
        while i < len(line):
            ch = line[i]
            if in_str:
                if escape:
                    escape = False
                elif ch == '\\':
                    escape = True
                elif ch == '"':
                    in_str = False
            else:
                if ch == '"':
                    in_str = True
                elif ch == '/' and i + 1 < len(line) and line[i+1] == '/':
                    # comment to end of line
                    break
            i += 1
        if in_str:
            return False, line_num, f"New line without closing string with \" in line: {repr(line.strip())}"
    return True, 0, "OK"

for folder in [
    r"C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\config\ftbquests\quests",
    r"C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\config\ftbquests\quests\chapters",
    r"C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\config\ftbquests\quests\lang"
]:
    for f in os.listdir(folder):
        if f.endswith('.snbt'):
            p = os.path.join(folder, f)
            ok, lnum, err = check_file(p)
            if not ok:
                print(f"ERROR in {f} line {lnum}: {err}")

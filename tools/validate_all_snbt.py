import os, sys
sys.stdout.reconfigure(encoding='utf-8')

def validate_snbt(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # check bracket balancing ignoring strings and comments
    stack = []
    in_str = False
    escape = False
    line_num = 1
    col_num = 0
    
    for i, char in enumerate(content):
        col_num += 1
        if char == '\n':
            line_num += 1
            col_num = 0
            continue
            
        if in_str:
            if escape:
                escape = False
            elif char == '\\':
                escape = True
            elif char == '"':
                in_str = False
            continue
            
        if char == '"':
            in_str = True
            continue
            
        if char in '{[':
            stack.append((char, line_num, col_num))
        elif char in '}]':
            if not stack:
                return False, f"Unexpected closing '{char}' at line {line_num}:{col_num}"
            top, tl, tc = stack.pop()
            if (top == '{' and char != '}') or (top == '[' and char != ']'):
                return False, f"Mismatched bracket: opened '{top}' at {tl}:{tc}, closed with '{char}' at {line_num}:{col_num}"
                
    if in_str:
        return False, f"Unterminated string starting before end of file"
    if stack:
        top, tl, tc = stack[-1]
        return False, f"Unclosed bracket '{top}' opened at {tl}:{tc}"
        
    return True, "Valid"

base_dir = r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\config\ftbquests\quests'
all_valid = True
for root, dirs, files in os.walk(base_dir):
    for f in files:
        if f.endswith('.snbt'):
            fp = os.path.join(root, f)
            ok, msg = validate_snbt(fp)
            if not ok:
                all_valid = False
                print(f"FAILED: {f} -> {msg}")
            else:
                pass

if all_valid:
    print("ALL SNBT files are 100% syntactically valid!")

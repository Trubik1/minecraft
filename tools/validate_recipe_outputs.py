import re, subprocess

with open("kubejs/server_scripts/endgame_recipes.js", encoding="utf-8") as f:
    text = f.read()

out = subprocess.check_output(["python", "tools/find_creative_items.py"], text=True)
valid_creative = set(out.strip().splitlines())

# Extract results of table(...)
table_results = re.findall(r"table\('[^']+',\s*\[[^\]]+\],\s*\{[^}]+\},\s*'([^']+)'\)", text, re.S)
# Extract event.shaped('...') and event.shapeless('...')
shaped_results = re.findall(r"event\.shaped\('([^']+)'", text)
shapeless_results = re.findall(r"event\.shapeless\('([^']+)'", text)

# Extract first element of cableColored tuples
cable_tuples = re.findall(r"\['(cabletiers:[^']+)'", text)

# Dynamic RS colors
rs_colors = re.findall(r"event\.shapeless\(`refinedstorage:\$\{color\}_creative_controller`", text)
rs_color_list = [
    'white', 'orange', 'magenta', 'yellow', 'lime', 'pink', 'gray',
    'light_gray', 'cyan', 'purple', 'blue', 'brown', 'green', 'red', 'black'
]
rs_dynamic = [f"refinedstorage:{c}_creative_controller" for c in rs_color_list]

all_results = set(table_results + shaped_results + shapeless_results + cable_tuples + rs_dynamic)
# Remove template string if matched
all_results = {r for r in all_results if not '${' in r}

print(f"Total distinct creative recipe outputs tested: {len(all_results)}")
missing_from_mod = []
for item in sorted(all_results):
    if item not in valid_creative:
        missing_from_mod.append(item)

if missing_from_mod:
    print(f"Items not found in mod jars ({len(missing_from_mod)}):")
    for m in missing_from_mod:
        print("  -", m)
else:
    print("SUCCESS: ALL RECIPE RESULTS ARE 100% VALID AND EXIST IN MOD JARS!")

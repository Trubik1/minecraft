import subprocess

out = subprocess.check_output(['python', 'tools/find_creative_items.py'], text=True)
all_items = set(out.strip().splitlines())

cable_recipes = []
for it in sorted(all_items):
    if it.startswith('cabletiers:') and '_creative_' in it:
        # e.g. cabletiers:black_creative_autocrafter
        parts = it.replace('cabletiers:', '').split('_creative_')
        color = parts[0]
        device = parts[1]
        cable_recipes.append((it, color, device))

print(f"Total valid cable colored recipes: {len(cable_recipes)}")
js_code = "    const cableColored = [\n"
for it, color, dev in cable_recipes:
    js_code += f"        ['{it}', '{color}', 'cabletiers:creative_{dev}'],\n"
js_code += "    ];\n\n"
js_code += "    cableColored.forEach(([result, color, base]) => {\n"
js_code += "        event.shapeless(result, [base, `minecraft:${color}_dye`]).id(`steampunk:endgame/${result.replace(':', '_')}`);\n"
js_code += "    });\n"

with open("tools/cable_snippet.js", "w", encoding="utf-8") as f:
    f.write(js_code)
print("Snippet written!")

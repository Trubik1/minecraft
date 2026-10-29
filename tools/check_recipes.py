import glob, os

recipe_files = glob.glob("kubejs/server_scripts/*.js")
all_recipes_text = ""
for rf in recipe_files:
    with open(rf, encoding="utf-8") as f:
        all_recipes_text += f.read() + "\n"

# Check each creative item
with open("tools/find_creative_items.py") as f:
    pass

import subprocess
out = subprocess.check_output(["python", "tools/find_creative_items.py"], text=True)
items = [line.strip() for line in out.strip().splitlines() if line.strip()]

has_recipe = []
missing_recipe = []

for item in items:
    if f"'{item}'" in all_recipes_text or f'"{item}"' in all_recipes_text:
        has_recipe.append(item)
    else:
        missing_recipe.append(item)

print(f"Total creative items: {len(items)}")
print(f"Has recipe in KubeJS ({len(has_recipe)}):")
for it in has_recipe:
    print("  +", it)

print(f"\nMissing recipe in KubeJS ({len(missing_recipe)}):")
for it in missing_recipe:
    print("  -", it)

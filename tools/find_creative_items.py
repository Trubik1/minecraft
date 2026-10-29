import zipfile, glob, json, os

creative_items = set()

for jar in glob.glob("mods/*.jar"):
    try:
        with zipfile.ZipFile(jar, "r") as z:
            for name in z.namelist():
                # Look in assets/<mod>/models/item/<item>.json or data/<mod>/items/...
                if "creative" in name.lower() and name.endswith(".json"):
                    if name.startswith("assets/") and "/models/item/" in name:
                        parts = name.split("/")
                        modid = parts[1]
                        itemname = parts[-1].replace(".json", "")
                        creative_items.add(f"{modid}:{itemname}")
                    elif name.startswith("data/") and "/items/" in name:
                        parts = name.split("/")
                        modid = parts[1]
                        itemname = parts[-1].replace(".json", "")
                        creative_items.add(f"{modid}:{itemname}")
    except Exception as e:
        pass

for it in sorted(creative_items):
    print(it)

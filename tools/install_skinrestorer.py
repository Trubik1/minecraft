# -*- coding: utf-8 -*-
import urllib.request, os, shutil, zipfile

URL = "https://cdn.modrinth.com/data/ghrZDhGW/versions/QFkb2iq2/skinrestorer-2.11.0%2B1.21-neoforge.jar"
JAR_NAME = "skinrestorer-2.11.0+1.21-neoforge.jar"

PROFILE = r"C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords"
DESTINATIONS = [
    os.path.join(PROFILE, "mods"),
    r"D:\SteampunkLauncher\mods",
    os.path.join(PROFILE, "server_pack", "mods"),
    r"C:\Users\artem\Desktop\ПАТЧ_СЕРВЕРА\mods"
]

temp_jar = os.path.join(PROFILE, "tools", JAR_NAME)
print(f"Downloading {JAR_NAME}...")
req = urllib.request.Request(URL, headers={'User-Agent': 'MinecraftModpack/1.0'})
with urllib.request.urlopen(req) as resp, open(temp_jar, 'wb') as out:
    shutil.copyfileobj(resp, out)
print(f"Downloaded! Size: {os.path.getsize(temp_jar)} bytes")

for dst in DESTINATIONS:
    if os.path.exists(os.path.dirname(dst)):
        os.makedirs(dst, exist_ok=True)
        target = os.path.join(dst, JAR_NAME)
        shutil.copy2(temp_jar, target)
        print(f"Copied to: {target}")

# Update ПАТЧ_СЕРВЕРА.zip
PATCH_DIR = r'C:\Users\artem\Desktop\ПАТЧ_СЕРВЕРА'
PATCH_ZIP = r'C:\Users\artem\Desktop\ПАТЧ_СЕРВЕРА.zip'
if os.path.exists(PATCH_DIR):
    print("Repacking ПАТЧ_СЕРВЕРА.zip...")
    if os.path.exists(PATCH_ZIP):
        try: os.remove(PATCH_ZIP)
        except: pass
    with zipfile.ZipFile(PATCH_ZIP, 'w', compression=zipfile.ZIP_DEFLATED) as z:
        for root, dirs, files in os.walk(PATCH_DIR):
            for f in files:
                full = os.path.join(root, f)
                rel = os.path.relpath(full, PATCH_DIR)
                z.write(full, rel)
    print(f"ПАТЧ_СЕРВЕРА.zip updated! Size: {os.path.getsize(PATCH_ZIP)/1024/1024:.1f} MB")

# Update Steampunk_Server_Pack.zip
SP_ZIP = r'C:\Users\artem\Desktop\Steampunk_Server_Pack.zip'
if os.path.exists(SP_ZIP):
    print("Adding mod to Steampunk_Server_Pack.zip...")
    with zipfile.ZipFile(SP_ZIP, 'a', compression=zipfile.ZIP_DEFLATED) as z:
        z.write(temp_jar, f"mods/{JAR_NAME}")
    print("Updated Steampunk_Server_Pack.zip")

print("Done installing SkinRestorer!")

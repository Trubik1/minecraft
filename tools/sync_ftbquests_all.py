import shutil, os, zipfile

SRC = r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\config\ftbquests'

DESTS = [
    r'D:\SteampunkLauncher\config\ftbquests',
    r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\server_pack\config\ftbquests',
    r'C:\Users\artem\Desktop\ПАТЧ_СЕРВЕРА\config\ftbquests'
]

for dst in DESTS:
    if os.path.exists(dst):
        shutil.rmtree(dst)
    shutil.copytree(SRC, dst)
    print(f'Synchronized ftbquests to: {dst}')

# Re-zip ПАТЧ_СЕРВЕРА.zip
PATCH_DIR = r'C:\Users\artem\Desktop\ПАТЧ_СЕРВЕРА'
PATCH_ZIP = r'C:\Users\artem\Desktop\ПАТЧ_СЕРВЕРА.zip'

if os.path.exists(PATCH_ZIP):
    os.remove(PATCH_ZIP)

print('Repacking ПАТЧ_СЕРВЕРА.zip...')
with zipfile.ZipFile(PATCH_ZIP, 'w', compression=zipfile.ZIP_DEFLATED) as z:
    for root, dirs, files in os.walk(PATCH_DIR):
        for f in files:
            full = os.path.join(root, f)
            rel = os.path.relpath(full, PATCH_DIR)
            z.write(full, rel)

print(f'Repacked! Size: {os.path.getsize(PATCH_ZIP) / 1024 / 1024:.1f} MB')

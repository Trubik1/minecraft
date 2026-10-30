import os, zipfile, shutil

SERVER_PACK = r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\server_pack'
PROFILE = r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords'
ZIP_PATH = os.path.join(PROFILE, 'Steampunk_Server_Pack.zip')

# Clean unneeded files
for f in ['neoforge-21.1.264-installer.jar', 'installer.log']:
    p = os.path.join(SERVER_PACK, f)
    if os.path.exists(p):
        os.remove(p)

for d in ['crash-reports', '.mixin.out']:
    p = os.path.join(SERVER_PACK, d)
    if os.path.exists(p):
        shutil.rmtree(p)

# Clear old logs
logs_dir = os.path.join(SERVER_PACK, 'logs')
if os.path.exists(logs_dir):
    shutil.rmtree(logs_dir)
os.makedirs(logs_dir, exist_ok=True)

print('Creating ZIP archive:', ZIP_PATH)
file_count = 0
with zipfile.ZipFile(ZIP_PATH, 'w', compression=zipfile.ZIP_DEFLATED) as z:
    for root, dirs, files in os.walk(SERVER_PACK):
        for f in files:
            full = os.path.join(root, f)
            rel = os.path.relpath(full, SERVER_PACK)
            # Prefix with folder name Steampunk_Server/
            arcname = os.path.join('Steampunk_Server', rel)
            z.write(full, arcname)
            file_count += 1
            if file_count % 200 == 0:
                print(f'Packed {file_count} files...')

size_mb = os.path.getsize(ZIP_PATH) / 1024 / 1024
print(f'ZIP Archive created successfully! Total files: {file_count}, Size: {size_mb:.1f} MB')

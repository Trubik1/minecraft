# -*- coding: utf-8 -*-
import os, shutil, zipfile

PROFILE = r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords'
DESTS = [
    r'D:\SteampunkLauncher',
    r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\server_pack',
    r'C:\Users\artem\Desktop\ПАТЧ_СЕРВЕРА'
]

FOLDERS_TO_SYNC = [
    os.path.join('config', 'ftbquests'),
    'kubejs'
]

def copy_tree_safe(src, dst):
    os.makedirs(dst, exist_ok=True)
    for root, dirs, files in os.walk(src):
        rel = os.path.relpath(root, src)
        target_dir = os.path.join(dst, rel)
        os.makedirs(target_dir, exist_ok=True)
        for f in files:
            src_file = os.path.join(root, f)
            dst_file = os.path.join(target_dir, f)
            try:
                shutil.copy2(src_file, dst_file)
            except Exception as e:
                print(f"Warning copying {f}: {e}")

for folder in FOLDERS_TO_SYNC:
    src = os.path.join(PROFILE, folder)
    for base_dst in DESTS:
        dst = os.path.join(base_dst, folder)
        copy_tree_safe(src, dst)
        print(f"Safe synchronized {folder} to: {dst}")

# Repack ПАТЧ_СЕРВЕРА.zip
PATCH_DIR = r'C:\Users\artem\Desktop\ПАТЧ_СЕРВЕРА'
PATCH_ZIP = r'C:\Users\artem\Desktop\ПАТЧ_СЕРВЕРА.zip'

if os.path.exists(PATCH_DIR):
    if os.path.exists(PATCH_ZIP):
        try:
            os.remove(PATCH_ZIP)
        except Exception as e:
            print(f"Warning removing zip: {e}")
    
    print('Repacking ПАТЧ_СЕРВЕРА.zip...')
    with zipfile.ZipFile(PATCH_ZIP, 'w', compression=zipfile.ZIP_DEFLATED) as z:
        for root, dirs, files in os.walk(PATCH_DIR):
            for f in files:
                full = os.path.join(root, f)
                rel = os.path.relpath(full, PATCH_DIR)
                z.write(full, rel)
    
    print(f'Repacked ПАТЧ_СЕРВЕРА.zip! Size: {os.path.getsize(PATCH_ZIP) / 1024 / 1024:.1f} MB')

# Also update Steampunk_Server_Pack.zip if present
SP_DIR = r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\server_pack'
SP_ZIP = r'C:\Users\artem\Desktop\Steampunk_Server_Pack.zip'
if os.path.exists(SP_DIR) and os.path.exists(SP_ZIP):
    print("Updating server_pack in Steampunk_Server_Pack.zip...")
    with zipfile.ZipFile(SP_ZIP, 'a', compression=zipfile.ZIP_DEFLATED) as z:
        for folder in FOLDERS_TO_SYNC:
            f_dir = os.path.join(SP_DIR, folder)
            if os.path.exists(f_dir):
                for root, dirs, files in os.walk(f_dir):
                    for f in files:
                        full = os.path.join(root, f)
                        rel = os.path.relpath(full, SP_DIR)
                        z.write(full, rel)
    print("Updated Steampunk_Server_Pack.zip")

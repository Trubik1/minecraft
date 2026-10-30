import os, shutil, zipfile

DESKTOP = r'C:\Users\artem\Desktop'
PATCH_DIR = os.path.join(DESKTOP, 'ПАТЧ_СЕРВЕРА')
PATCH_ZIP = os.path.join(DESKTOP, 'ПАТЧ_СЕРВЕРА.zip')
SERVER_PACK = r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\server_pack'

if os.path.exists(PATCH_DIR):
    shutil.rmtree(PATCH_DIR)
os.makedirs(PATCH_DIR, exist_ok=True)

# 1. Root files
for f in ['ЗАПУСК_СЕРВЕРА.bat', 'СЕТЬ_ДЛЯ_ДРУЗЕЙ (Playit).bat', 'server.properties']:
    src = os.path.join(SERVER_PACK, f)
    dst = os.path.join(PATCH_DIR, f)
    shutil.copy2(src, dst)
    print(f'Copied {f}')

# 2. runtime (Java 21)
print('Copying runtime...')
shutil.copytree(os.path.join(SERVER_PACK, 'runtime'), os.path.join(PATCH_DIR, 'runtime'))

# 3. Modified/Added Mods
mods_to_patch = [
    'Chunky-NeoForge-1.4.23.jar',
    'GlitchCore-neoforge-1.21.1-2.1.0.0.jar',
    'Placebo-1.21.1-9.9.4.jar',
    'cloth-config-15.0.140-neoforge.jar',
    'create-aeronautics-bundled-1.21.1-1.3.2.jar',
    'cupboard-1.21.1-4.2.jar',
    'sable-neoforge-1.21.1-2.0.6.jar'
]
patch_mods_dir = os.path.join(PATCH_DIR, 'mods')
os.makedirs(patch_mods_dir, exist_ok=True)
for m in mods_to_patch:
    src = os.path.join(SERVER_PACK, 'mods', m)
    if os.path.exists(src):
        shutil.copy2(src, os.path.join(patch_mods_dir, m))
        print(f'Copied mod {m}')
    else:
        print(f'WARNING: mod {m} not found!')

# 4. Configs
patch_config_dir = os.path.join(PATCH_DIR, 'config')
os.makedirs(patch_config_dir, exist_ok=True)
shutil.copy2(os.path.join(SERVER_PACK, 'config', 'modernfix-mixins.properties'), os.path.join(patch_config_dir, 'modernfix-mixins.properties'))
shutil.copy2(os.path.join(SERVER_PACK, 'config', 'neoforge-server.toml'), os.path.join(patch_config_dir, 'neoforge-server.toml'))
shutil.copytree(os.path.join(SERVER_PACK, 'config', 'ftbquests'), os.path.join(patch_config_dir, 'ftbquests'))
print('Copied config')

# 5. KubeJS
patch_kubejs_dir = os.path.join(PATCH_DIR, 'kubejs')
shutil.copytree(os.path.join(SERVER_PACK, 'kubejs'), patch_kubejs_dir)
print('Copied kubejs')

# 6. Instruction
instruction = '''======================================================================
               ИНСТРУКЦИЯ ПО ОБНОВЛЕНИЮ СЕРВЕРА
======================================================================

1. Выдели все файлы и папки из этого архива (или этой папки):
   - ЗАПУСК_СЕРВЕРА.bat
   - СЕТЬ_ДЛЯ_ДРУЗЕЙ (Playit).bat
   - server.properties
   - runtime
   - mods
   - config
   - kubejs

2. Скопируй / перетащи их в свою папку с сервером.
3. Windows спросит: "Заменить файлы в папке назначения?" -> Нажми "Заменить файлы".
4. Запусти "ЗАПУСК_СЕРВЕРА.bat"!

Теперь сервер запускается со встроенной Java 21 без единой ошибки!
======================================================================
'''
with open(os.path.join(PATCH_DIR, 'ИНСТРУКЦИЯ.txt'), 'w', encoding='utf-8') as f:
    f.write(instruction)

# 7. Create ZIP archive
print('Creating patch ZIP...')
if os.path.exists(PATCH_ZIP):
    os.remove(PATCH_ZIP)

with zipfile.ZipFile(PATCH_ZIP, 'w', compression=zipfile.ZIP_DEFLATED) as z:
    for root, dirs, files in os.walk(PATCH_DIR):
        for f in files:
            full = os.path.join(root, f)
            rel = os.path.relpath(full, PATCH_DIR)
            z.write(full, rel)

size_mb = os.path.getsize(PATCH_ZIP) / 1024 / 1024
print(f'Patch created successfully! Size: {size_mb:.1f} MB')
print('Folder:', PATCH_DIR)
print('Zip:', PATCH_ZIP)

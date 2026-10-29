import shutil, os, glob, urllib.request

atm10_mods = r'C:\Users\artem\curseforge\minecraft\Instances\All the Mods 10 - ATM10\mods'
sp_mods = r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\mods'

target_atm10_mods = [
    'AI-Improvements',
    'Clumps',
    'FastWorkbench',
    'FastFurnace',
    'FastSuite',
    'getittogetherdrops',
    'connectivity',
    'alltheleaks',
    'sodium-extra',
    'spark',
    'observable'
]

print("1. Copying proven optimization mods from ATM10...")
for pattern in target_atm10_mods:
    matches = glob.glob(os.path.join(atm10_mods, f'*{pattern}*.jar'))
    for m in matches:
        dest = os.path.join(sp_mods, os.path.basename(m))
        if not os.path.exists(dest):
            shutil.copy2(m, dest)
            print(f"  + Copied from ATM10: {os.path.basename(m)}")
        else:
            print(f"  = Already present: {os.path.basename(m)}")

print("\n2. Downloading high-impact optimization mods from Modrinth...")
modrinth_downloads = [
    ("BadOptimizations-2.4.1-1.21.1.jar", "https://cdn.modrinth.com/data/g96Z4WVZ/versions/S2qthD5S/BadOptimizations-2.4.1-1.21.1.jar"),
    ("noisium-neoforge-2.3.0+mc1.21-1.21.1.jar", "https://cdn.modrinth.com/data/KuNKN7d2/versions/nJBE6tif/noisium-neoforge-2.3.0%2Bmc1.21-1.21.1.jar"),
    ("iris-flywheel-compat-NeoForge-2.4.0.jar", "https://cdn.modrinth.com/data/ndHYMY2K/versions/6ZQz5Ow2/iris-flywheel-compat-NeoForge-2.4.0.jar")
]

req_headers = {'User-Agent': 'SteampunkModpackOptimizer/1.0'}
for fname, url in modrinth_downloads:
    dest = os.path.join(sp_mods, fname)
    if not os.path.exists(dest):
        print(f"  Downloading {fname}...")
        req = urllib.request.Request(url, headers=req_headers)
        with urllib.request.urlopen(req) as resp, open(dest, 'wb') as f:
            f.write(resp.read())
        print(f"  + Downloaded {fname}")
    else:
        print(f"  = Already present: {fname}")

print("\nDone installing optimization mods!")

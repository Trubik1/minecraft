import urllib.request, zipfile, os, shutil

SERVER_PACK = r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords\server_pack'
runtime_dir = os.path.join(SERVER_PACK, 'runtime')
zip_path = os.path.join(SERVER_PACK, 'jre21_temp.zip')

print('Downloading Adoptium JRE 21...')
url = 'https://api.adoptium.net/v3/binary/latest/21/ga/windows/x64/jre/hotspot/normal/eclipse'
req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})
with urllib.request.urlopen(req) as resp, open(zip_path, 'wb') as out:
    shutil.copyfileobj(resp, out)
print('Downloaded size:', os.path.getsize(zip_path))

if os.path.exists(runtime_dir):
    shutil.rmtree(runtime_dir)
os.makedirs(runtime_dir, exist_ok=True)

print('Extracting JRE 21...')
with zipfile.ZipFile(zip_path, 'r') as z:
    z.extractall(runtime_dir)

os.remove(zip_path)

# Normalize folder: if there is an outer folder like jdk-21.0.13+9-jre, move its contents directly to runtime/ or keep it
# Let's find java.exe
java_exe = None
for root, dirs, files in os.walk(runtime_dir):
    if 'java.exe' in files:
        java_exe = os.path.join(root, 'java.exe')
        break

print('Found Java at:', java_exe)

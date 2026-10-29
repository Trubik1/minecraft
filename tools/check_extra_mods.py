import urllib.request, json, sys

sys.stdout.reconfigure(encoding='utf-8', errors='replace')

slugs = ['badoptimizations', 'noisium', 'iris-flw-compat']

for slug in slugs:
    url = f'https://api.modrinth.com/v2/project/{slug}/version?game_versions=[%221.21.1%22]&loaders=[%22neoforge%22]'
    req = urllib.request.Request(url, headers={'User-Agent': 'ModpackOptimizer/1.0'})
    try:
        with urllib.request.urlopen(req) as resp:
            data = json.loads(resp.read().decode('utf-8'))
            if data:
                latest = data[0]
                primary_file = [f for f in latest['files'] if f['primary']][0] if latest['files'] else None
                print(f"{slug}: Version {latest['version_number']}, File: {primary_file['filename'] if primary_file else 'None'}, URL: {primary_file['url'] if primary_file else 'None'}")
            else:
                print(f"{slug}: No 1.21.1 NeoForge version found directly.")
    except Exception as e:
        print(f"{slug}: Error {e}")

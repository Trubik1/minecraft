import urllib.request, os

PROFILE = r'C:\Users\artem\AppData\Roaming\ModrinthApp\profiles\Steampunk & Swords'
SERVER_PACK = os.path.join(PROFILE, 'server_pack')

# 1. Download Chunky
chunky_url = 'https://cdn.modrinth.com/data/fALzjamp/versions/LuFhm4eU/Chunky-NeoForge-1.4.23.jar'
chunky_dst_profile = os.path.join(PROFILE, 'mods', 'Chunky-NeoForge-1.4.23.jar')
chunky_dst_server = os.path.join(SERVER_PACK, 'mods', 'Chunky-NeoForge-1.4.23.jar')

if not os.path.exists(chunky_dst_profile):
    print('Downloading Chunky...')
    urllib.request.urlretrieve(chunky_url, chunky_dst_profile)
    print('Downloaded Chunky to profile mods')

if not os.path.exists(chunky_dst_server):
    import shutil
    shutil.copy2(chunky_dst_profile, chunky_dst_server)
    print('Copied Chunky to server mods')

# 2. Update server.properties
server_properties = '''# Minecraft server properties
# Maximum performance tuning for 2-core / laptop hardware
server-port=25565
motd=\\u00A76\\u00A7lSteampunk & Swords Server
online-mode=false
enforce-secure-profile=false
pvp=true
difficulty=normal
gamemode=survival
view-distance=8
simulation-distance=5
sync-chunk-writes=false
entity-broadcast-range-percentage=80
allow-flight=true
enable-command-block=true
max-tick-time=-1
max-players=20
spawn-protection=0
network-compression-threshold=256
'''

with open(os.path.join(SERVER_PACK, 'server.properties'), 'w', encoding='utf-8') as f:
    f.write(server_properties)
print('Updated server.properties with async writes & sim 5')

# 3. Update neoforge-server.toml
neoforge_server = '''# NeoForge Server Config
removeErroringBlockEntities = true
removeErroringEntities = true
fullBoundingBoxLadders = false
permissionHandler = "neoforge:default_handler"
advertiseDedicatedServerToLan = true
'''

for cfg_dir in [os.path.join(PROFILE, 'config'), os.path.join(SERVER_PACK, 'config')]:
    with open(os.path.join(cfg_dir, 'neoforge-server.toml'), 'w', encoding='utf-8') as f:
        f.write(neoforge_server)
print('Updated neoforge-server.toml')

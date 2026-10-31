# -*- coding: utf-8 -*-
import urllib.request, json

url = "https://api.modrinth.com/v2/version/QFkb2iq2"
req = urllib.request.Request(url, headers={'User-Agent': 'MinecraftModpack/1.0'})
with urllib.request.urlopen(req) as resp:
    data = json.loads(resp.read().decode('utf-8'))
    print("Dependencies:", data.get('dependencies'))

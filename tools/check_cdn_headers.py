# -*- coding: utf-8 -*-
import urllib.request

url = "https://raw.githubusercontent.com/Trubik1/minecraft/main/manifest.json"
req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})
with urllib.request.urlopen(req) as resp:
    for k, v in resp.headers.items():
        if 'cache' in k.lower() or 'age' in k.lower() or 'date' in k.lower() or 'etag' in k.lower():
            print(f"{k}: {v}")

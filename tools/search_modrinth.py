import urllib.request, json, sys

sys.stdout.reconfigure(encoding='utf-8', errors='replace')

url = 'https://api.modrinth.com/v2/search?query=optimization&facets=[[%22project_type:mod%22],[%22categories:neoforge%22],[%22versions:1.21.1%22]]&limit=50&index=downloads'
req = urllib.request.Request(url, headers={'User-Agent': 'ModpackOptimizer/1.0'})
try:
    with urllib.request.urlopen(req) as resp:
        data = json.loads(resp.read().decode('utf-8'))
        print(f"Total found: {data['total_hits']}")
        for hit in data['hits']:
            print(f"- {hit['title']} ({hit['slug']}) [{hit['downloads']} dl]: {hit['description'][:100]}")
except Exception as e:
    print('Error:', e)

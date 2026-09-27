from pathlib import Path
import zipfile
root=Path(__file__).resolve().parents[1]
# Release contains no dependency directories and no other production's assets.
with zipfile.ZipFile(root/'temporary-forever-web.zip','w',zipfile.ZIP_DEFLATED) as z:
 for p in sorted((root/'dist').rglob('*')):
  if p.is_file(): z.write(p,p.relative_to(root/'dist'))
paths=['package.json','package-lock.json','vite.config.js','index.html','README.md','LICENSE','ENGINE_REVISION.txt','music-catalog.json','research-sources.json','.gitignore']
for folder in ['src','public','docs','dist','tools']:
 paths += [str(p.relative_to(root)) for p in (root/folder).rglob('*') if p.is_file() and '__pycache__' not in p.parts]
paths += [str(p.relative_to(root)) for p in (root/'tests').glob('*') if p.suffix in ['.cjs','.json']]
paths += ['engine/LICENSE','engine/README.md','engine/documentation.md','engine/package.json','engine/package-lock.json']
paths += [str(p.relative_to(root)) for p in (root/'engine/src').rglob('*') if p.is_file()]
with zipfile.ZipFile(root/'temporary-forever-source.zip','w',zipfile.ZIP_DEFLATED) as z:
 for p in sorted(set(paths)):z.write(root/p,'temporary-forever/'+p)
for f in ['temporary-forever-web.zip','temporary-forever-source.zip']:
 print(f,round((root/f).stat().st_size/1048576,2),'MiB')

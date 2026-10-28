# Steampunk & Swords

Сборка Minecraft **1.21.1 + NeoForge 21.1.264** с собственным лаунчером и автообновлением.

## Что это за репозиторий

Здесь лежит вся сборка: моды, конфиги, KubeJS-скрипты, квесты, ресурспаки и шейдеры.
Из этого репозитория лаунчер скачивает файлы, поэтому **любое изменение в игре нужно коммитить и пушить** — иначе игроки его не получат.

```
mods/            моды
config/          конфиги
kubejs/          скрипты и ресурсы сборки
resourcepacks/   ресурспаки
shaderpacks/     шейдеры
defaultconfigs/  конфиги по умолчанию для новых миров
server.txt       адрес сервера (name / ip / port)
manifest.json    список файлов сборки + хеши (генерируется автоматически)
launcher-src/    исходники лаунчера (C#)
tools/           скрипты сборки манифеста и лаунчера
server/          скрипты для сервера
```

## Игрокам

1. Скачать `SteampunkLauncher.exe` из папки [`launcher/`](launcher/) (или по прямой ссылке
   `https://raw.githubusercontent.com/Trubik1/minecraft/main/launcher/SteampunkLauncher.exe`).
2. Запустить, ввести ник, выбрать папку (нужно ~4 ГБ).
3. Нажать **ИГРАТЬ**. При первом запуске скачается Java 21, Minecraft, NeoForge и сборка (~2.2 ГБ).
4. Дальше при каждом запуске скачиваются только изменившиеся файлы — обычно 1–20 МБ.

Пароль не нужен: лаунчер работает в офлайн-режиме (как TLauncher).
Настройки хранятся в `%APPDATA%\SteampunkLauncher\config.json`, лог — там же (`launcher.log`).

**Windows SmartScreen.** Файл не подписан цифровой подписью, поэтому при первом запуске
Windows может показать «Windows защитила ваш компьютер». Нажать **Подробнее** → **Выполнить в любом случае**.
Антивирус иногда ругается на неподписанные exe — это ложное срабатывание.

Лаунчер не трогает ваши миры, `options.txt` и личные моды: удаляются только файлы,
которые он сам поставил ранее (список в `<папка игры>\_launcher\installed.json`).

## Как выпустить обновление сборки

```powershell
# 1. изменили моды/конфиги/скрипты
git add -A
git commit -m "что изменилось"

# 2. пересобрать манифест (хеши берутся из HEAD, поэтому сначала коммит)
python tools\build_manifest.py

# 3. закоммитить манифест и запушить
git add manifest.json
git commit -m "Update pack manifest"
git push origin main
```

После пуша игроки получат изменения при следующем запуске лаунчера.

`manifest.json` **нельзя** редактировать руками и нельзя коммитить отдельно от изменений:
если хеши не совпадут, лаунчер будет считать файлы повреждёнными.

## Как выпустить новую версию лаунчера

```powershell
powershell -ExecutionPolicy Bypass -File tools\build_launcher.ps1 -Version 1.0.1
```

Скрипт собирает exe, кладёт его в `launcher\SteampunkLauncher.exe` и пишет `launcher\version.txt`.
Осталось закоммитить и запушить:

```powershell
git add launcher
git commit -m "Launcher 1.0.1"
git push origin main
```

Лаунчер сам проверяет `launcher/version.txt` при запуске и предлагает обновиться.

## Сервер

См. `server/README.md`. Кратко: папка сервера обновляется скриптом
`server/update-server.ps1`, запуск — `server/start-server.bat`.

**Важно:** сервер должен работать с `online-mode=false`, иначе офлайн-игроков не пустит.

## Адрес сервера

Правится в `server.txt`:

```
name=Steampunk & Swords
ip=play.example.com
port=25565
```

Лаунчер читает файл при запуске, так что обновлять сам лаунчер для смены IP не нужно.

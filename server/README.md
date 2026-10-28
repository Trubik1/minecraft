# Сервер Steampunk & Swords

Сервер держится на компьютере одного из игроков и обновляется из этого же репозитория.

## Что нужно один раз

1. **Java 21.** Скачать Temurin 21 (JRE или JDK) и установить, либо взять `java.exe` из папки
   лаунчера: `<папка игры>\runtime\temurin21\...\bin\java.exe`.
   Если `java` не в PATH, `start-server.bat` можно запускать так:
   ```
   set JAVA_EXE=C:\Program Files\Eclipse Adoptium\jdk-21.0.12.1-hotspot\bin\java.exe
   start-server.bat
   ```
2. **NeoForge 21.1.264.** Скачать `neoforge-21.1.264-installer.jar`
   ([maven.neoforged.net](https://maven.neoforged.net/releases/net/neoforged/neoforge/21.1.264/neoforge-21.1.264-installer.jar))
   и выполнить в папке сервера:
   ```
   java -jar neoforge-21.1.264-installer.jar --installServer
   ```
   Появятся `libraries\`, `run.bat`, `user_jvm_args.txt`.
3. **Скрипты из репозитория.** Скопировать папку `server/` в папку сервера (или склонировать репозиторий).
4. **Файлы сборки:**
   ```
   powershell -ExecutionPolicy Bypass -File server\update-server.ps1 -Dir .
   ```
   Скрипт скачает моды, конфиги и KubeJS-скрипты, удалит устаревшие моды.
   Повторный запуск обновляет только изменившееся.
5. **Офлайн-режим.** В `server.properties`:
   ```
   online-mode=false
   enforce-secure-profile=false
   ```
   Без этого офлайн-игроков (без купленной игры) не пустит.
6. **EULA.** Запустить `start-server.bat`, дождаться `eula.txt`, поставить `eula=true`.
7. Запустить снова — сервер поднимется.

## Обновление сервера

```powershell
powershell -ExecutionPolicy Bypass -File server\update-server.ps1 -Dir .
```

Затем перезапустить сервер. Мир (`world/`), `server.properties`, `eula.txt` и логи не трогаются.

## Память

`user_jvm_args.txt` создаётся с `-Xmx6G`. Для сборки из 131 мода рекомендуется 6–10 ГБ
(на 8 ГБ ОЗУ ставить сервер и играть на этой же машине не получится).

## Клиентские моды

Список модов, которые не нужны серверу, лежит в `server/client-only.txt`.
Если сервер падает при старте с ошибкой загрузки мода — добавьте его имя в этот файл,
пересоберите манифест (`python tools/build_server_manifest.py`) и обновите сервер.

`server-manifest.json` собирается из `manifest.json` автоматически: в нём нет
ресурспаков, шейдеров, картинок KubeJS и клиентских модов. При каждом обновлении
сборки админу нужно запустить оба скрипта:

```powershell
python tools\build_manifest.py
python tools\build_server_manifest.py
```

## Если что-то не так

`update-server.ps1` не проверяет хеши, только размеры файлов. Если файл скачался
битым или вы поменяли моды вручную — запустите с ключом `-Force` (перекачает всё):

```powershell
powershell -ExecutionPolicy Bypass -File server\update-server.ps1 -Dir . -Force
```

## Адрес для игроков

Пропишите IP в `server.txt` в корне репозитория и закоммитьте — лаунчер подхватит его
при следующем запуске у всех игроков.

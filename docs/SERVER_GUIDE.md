# Руководство по серверу Steampunk & Swords

## 1. Системные требования
- **Java:** OpenJDK 21 (Eclipse Temurin 21 recommended)
- **Оперативная память:** 8 - 12 GB выделенной RAM (`-Xms4G -Xmx10G`)
- **Ядро:** NeoForge 21.1.264 (Minecraft 1.21.1)

## 2. Параметры запуска
Рекомендуемые флаги JVM:
```bash
java -Xms4G -Xmx10G -XX:+UseG1GC -XX:+ParallelRefProcEnabled -XX:MaxGCPauseMillis=200 -XX:+UnlockExperimentalVMOptions -XX:+DisableExplicitGC -XX:+AlwaysPreTouch -XX:G1NewSizePercent=30 -XX:G1MaxNewSizePercent=40 -XX:G1ReservePercent=20 -XX:G1HeapRegionSize=32M -jar run.jar nogui
```

## 3. Установка обновлений и патчей
Для применения исправлений квестов и рецептов:
1. Остановите сервер командой `stop`.
2. Распакуйте архив `ПАТЧ_СЕРВЕРА.zip` в корневую директорию сервера с заменой файлов:
   - `config/ftbquests/`
   - `kubejs/`
3. Запустите сервер. Прогресс игроков в квестах сохраняется в папке `world/ftbquests/`.

## 4. Полезные консольные команды
- `/reload` — быстрая перезагрузка рецептов KubeJS.
- `/chunky radius 3000` и `/chunky start` — фоновая предгенерация чанков для устранения лагов при полётах на дирижаблях.
- `/ftbquests editing_mode true` — включение режима редактирования квестов администратором.

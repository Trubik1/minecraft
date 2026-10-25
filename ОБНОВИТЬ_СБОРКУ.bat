@echo off
chcp 65001 >nul
title Обновление сборки Steampunk ^& Swords
echo ========================================================
echo   Авто-обновление сборки: Steampunk ^& Swords
echo   Репозиторий: https://github.com/Trubik1/minecraft
echo ========================================================
echo.

set "REPO_ZIP_URL=https://github.com/Trubik1/minecraft/archive/refs/heads/main.zip"
set "TEMP_ZIP=%TEMP%\minecraft_pack_update.zip"
set "TEMP_EXTRACT=%TEMP%\minecraft_pack_extracted"

echo [1/3] Загрузка актуальной версии сборки с GitHub...
powershell -Command "[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; (New-Object System.Net.WebClient).DownloadFile('%REPO_ZIP_URL%', '%TEMP_ZIP%')"
if not exist "%TEMP_ZIP%" (
    echo [ОШИБКА] Не удалось скачать архив с GitHub. Проверьте интернет-соединение.
    pause
    exit /b 1
)

echo [2/3] Распаковка файлов...
if exist "%TEMP_EXTRACT%" rmdir /s /q "%TEMP_EXTRACT%"
powershell -Command "Expand-Archive -Path '%TEMP_ZIP%' -DestinationPath '%TEMP_EXTRACT%' -Force"

echo [3/3] Синхронизация модов, конфигов и скриптов KubeJS...
set "SRC=%TEMP_EXTRACT%\minecraft-main"

if exist "%SRC%\mods" xcopy "%SRC%\mods" "mods\" /E /I /Y /Q >nul
if exist "%SRC%\config" xcopy "%SRC%\config" "config\" /E /I /Y /Q >nul
if exist "%SRC%\kubejs" xcopy "%SRC%\kubejs" "kubejs\" /E /I /Y /Q >nul
if exist "%SRC%\defaultconfigs" xcopy "%SRC%\defaultconfigs" "defaultconfigs\" /E /I /Y /Q >nul
if exist "%SRC%\resourcepacks" xcopy "%SRC%\resourcepacks" "resourcepacks\" /E /I /Y /Q >nul
if exist "%SRC%\shaderpacks" xcopy "%SRC%\shaderpacks" "shaderpacks\" /E /I /Y /Q >nul

echo.
echo Очистка временных файлов...
del /f /q "%TEMP_ZIP%" >nul 2>&1
rmdir /s /q "%TEMP_EXTRACT%" >nul 2>&1

echo.
echo ========================================================
echo   [УСПЕХ] Сборка успешно обновлена до последней версии!
echo   Приятной игры!
echo ========================================================
echo.
pause

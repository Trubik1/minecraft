@echo off
chcp 65001 >nul
echo ========================================================
echo     Steampunk & Swords — Обновление сборки (Git Sync)
echo ========================================================
echo.

set REPO_URL=https://github.com/Trubik1/minecraft.git

if not exist ".git" (
    echo [Первый запуск] Настраиваем синхронизацию с репозиторием...
    git init
    git branch -M main
    git remote add origin %REPO_URL%
    git fetch origin main
    git reset --hard origin/main
) else (
    echo Проверяем и скачиваем обновления...
    git pull origin main
)

if %errorlevel% neq 0 (
    echo.
    echo [ВНИМАНИЕ] Произошла ошибка при скачивании.
    echo Убедитесь, что Minecraft закрыт и есть интернет-соединение.
) else (
    echo.
    echo [УСПЕХ] Сборка успешно синхронизирована с актуальной версией!
    echo Все новые моды, квесты и конфиги на месте.
)

echo.
pause

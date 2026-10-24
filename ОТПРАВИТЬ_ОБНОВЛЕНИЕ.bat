@echo off
chcp 65001 >nul
echo ========================================================
echo     Steampunk & Swords — Отправка обновления (Git Push)
echo ========================================================
echo.

git status --short
echo.
echo Добавляем измененные и новые файлы...
git add mods/ config/ kubejs/ resourcepacks/ shaderpacks/ defaultconfigs/
echo.

set /p msg="Введите описание обновления (или нажмите Enter для авто): "
if "%msg%"=="" set msg=Update %date% %time%

git commit -m "%msg%"
echo.
echo Отправляем файлы в удаленный репозиторий...
git push origin main
if %errorlevel% neq 0 (
    echo.
    echo [ОШИБКА] Не удалось отправить файлы. Проверьте подключение к интернету или ветку.
) else (
    echo.
    echo [УСПЕХ] Обновление успешно отправлено! Друг может обновляться.
)

echo.
pause

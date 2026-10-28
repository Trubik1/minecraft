@echo off
chcp 65001 >nul
title Steampunk ^& Swords - server
cd /d "%~dp0"

if not exist "eula.txt" (
    echo.
    echo [!] eula.txt not found. Start the server once, accept the EULA, then run this again.
    echo.
    pause
    exit /b 1
)

set "ARGS="
for /r "libraries\net\neoforged\neoforge" %%f in (win_args.txt) do set "ARGS=%%f"
if not defined ARGS (
    echo.
    echo [!] NeoForge is not installed. See server\README.md, step 2.
    echo.
    pause
    exit /b 1
)

if not exist "user_jvm_args.txt" (
    echo -Xms2G> "user_jvm_args.txt"
    echo -Xmx6G>> "user_jvm_args.txt"
    echo Created user_jvm_args.txt with 6 GB of RAM. Edit it if needed.
)

java -Xms2G -Xmx6G @user_jvm_args.txt @"%ARGS%" nogui
echo.
echo Server stopped.
pause

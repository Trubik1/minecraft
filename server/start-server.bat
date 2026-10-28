@echo off
chcp 65001 >nul
title Steampunk ^& Swords - server
cd /d "%~dp0"

rem Optional: set JAVA_EXE to a full path if "java" is not on PATH,
rem e.g.  set JAVA_EXE=C:\Program Files\Eclipse Adoptium\jdk-21\bin\java.exe
if defined JAVA_EXE (set "JAVA=%JAVA_EXE%") else (set "JAVA=java")

if not exist "eula.txt" (
    echo.
    echo [!] eula.txt not found.
    echo     Start the server once, accept the EULA in eula.txt, then run this again.
    echo.
    pause
    exit /b 1
)

set "ARGS="
for /r "libraries\net\neoforged\neoforge" %%f in (win_args.txt) do set "ARGS=%%f"
if not defined ARGS (
    echo.
    echo [!] NeoForge is not installed.
    echo     See server\README.md, step 2: run the NeoForge installer with --installServer
    echo.
    pause
    exit /b 1
)

if not exist "user_jvm_args.txt" (
    echo -Xms2G> "user_jvm_args.txt"
    echo -Xmx6G>> "user_jvm_args.txt"
    echo Created user_jvm_args.txt with 6 GB of RAM. Edit it if needed.
)

"%JAVA%" @user_jvm_args.txt @"%ARGS%" nogui
echo.
echo Server stopped.
pause

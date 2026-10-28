@echo off
chcp 65001 >nul
title Steampunk ^& Swords - 24/7 Server Watchdog
cd /d "%~dp0"

if defined JAVA_EXE (set "JAVA=%JAVA_EXE%") else (set "JAVA=java")

if not exist "eula.txt" (
    echo eula=true> "eula.txt"
    echo Created eula.txt with eula=true.
)

set "ARGS="
for /r "libraries\net\neoforged\neoforge" %%f in (win_args.txt) do set "ARGS=%%f"
if not defined ARGS (
    echo.
    echo [!] NeoForge is not installed.
    echo     Please run the NeoForge 21.1.264 installer with --installServer first.
    echo.
    pause
    exit /b 1
)

:: Optimized JVM arguments for i3 3rd Gen (2 cores / 4 threads) + 12GB RAM Laptop
:: 6GB Max heap leaves 6GB for OS, background tasks, and native JVM memory to prevent disk swap thrashing.
set JVM_OPTS=-Xms4G -Xmx6G ^
  -XX:+UseG1GC ^
  -XX:ParallelGCThreads=2 ^
  -XX:ConcGCThreads=1 ^
  -XX:InitiatingHeapOccupancyPercent=45 ^
  -XX:G1ReservePercent=15 ^
  -XX:MaxGCPauseMillis=50 ^
  -XX:+AlwaysPreTouch ^
  -XX:+DisableExplicitGC

:server_loop
cls
echo ========================================================
echo   Steampunk ^& Swords Dedicated Server (24/7 Watchdog)
echo   Target CPU: 2 Cores / 4 Threads ^| RAM: 6GB Allocated
echo ========================================================
echo.
"%JAVA%" %JVM_OPTS% @"%ARGS%" nogui

echo.
echo [!] Server stopped or crashed.
echo [!] Auto-restarting in 10 seconds... (Press Ctrl+C to cancel)
timeout /t 10
goto server_loop

# Собирает SteampunkLauncher.exe и кладёт его в dist/.
# Пример:  powershell -ExecutionPolicy Bypass -File tools\build_launcher.ps1 -Version 1.0.1
param(
    [string]$Version = "1.0.0",
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$project = Join-Path $root "launcher-src\SteampunkLauncher.csproj"

$dotnetRoot = $env:DOTNET_ROOT
if (-not $dotnetRoot) { $dotnetRoot = Join-Path $env:LOCALAPPDATA "Microsoft\dotnet" }
$dotnet = Join-Path $dotnetRoot "dotnet.exe"
if (-not (Test-Path $dotnet)) {
    throw ".NET SDK не найден в $dotnetRoot. Установите SDK 8 или задайте DOTNET_ROOT."
}

$text = [System.IO.File]::ReadAllText($project)
$text = [regex]::Replace($text, '<Version>[^<]+</Version>', "<Version>$Version</Version>")
$text = [regex]::Replace($text, '<FileVersion>[^<]+</FileVersion>', "<FileVersion>$Version.0</FileVersion>")
$text = [regex]::Replace($text, '<AssemblyVersion>[^<]+</AssemblyVersion>', "<AssemblyVersion>$Version.0</AssemblyVersion>")
[System.IO.File]::WriteAllText($project, $text)

Write-Host "Сборка версии $Version..." -ForegroundColor Cyan
& $dotnet build $project -c $Configuration --nologo -v minimal
if ($LASTEXITCODE -ne 0) { throw "Сборка не удалась" }

$source = Join-Path $root "launcher-src\bin\$Configuration\net48\SteampunkLauncher.exe"
$dist = Join-Path $root "launcher"
New-Item -ItemType Directory -Force -Path $dist | Out-Null
Copy-Item $source (Join-Path $dist "SteampunkLauncher.exe") -Force
$versionFile = Join-Path $dist "version.txt"
$versionText = ("{0}`nSteampunk & Swords launcher`n" -f $Version)
[System.IO.File]::WriteAllText($versionFile, $versionText, (New-Object System.Text.UTF8Encoding($false)))

$exe = Get-Item (Join-Path $dist "SteampunkLauncher.exe")
Write-Host ("Готово: {0} ({1:N0} КБ, v{2})" -f $exe.FullName, ($exe.Length / 1KB), $Version) -ForegroundColor Green
Write-Host "Не забудьте закоммитить папку launcher/ и запушить." -ForegroundColor Yellow

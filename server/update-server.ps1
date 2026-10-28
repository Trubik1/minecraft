<#
    Синхронизирует серверную часть сборки с репозиторием.
    Скачивает только изменившиеся файлы, удаляет устаревшие моды.

    Пример:
        powershell -ExecutionPolicy Bypass -File server\update-server.ps1
        powershell -ExecutionPolicy Bypass -File server\update-server.ps1 -Dir D:\Server -Force
#>
param(
    [string]$Dir = "",
    [string]$ManifestUrl = "https://raw.githubusercontent.com/Trubik1/minecraft/main/server-manifest.json",
    [switch]$Force
)

$ErrorActionPreference = "Stop"
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

if (-not $Dir) { $Dir = Split-Path -Parent $PSScriptRoot }
$Dir = [System.IO.Path]::GetFullPath($Dir)
New-Item -ItemType Directory -Force -Path $Dir | Out-Null
$stateFile = Join-Path $Dir "server-state.json"

Write-Host "Папка сервера: $Dir" -ForegroundColor Cyan
Write-Host "Манифест: $ManifestUrl"

$manifest = Invoke-RestMethod -Uri $ManifestUrl -UseBasicParsing
if (-not $manifest.files) { throw "Пустой манифест" }
$baseUrl = $manifest.baseUrl.TrimEnd('/')

$installed = @()
if (Test-Path $stateFile) { $installed = @(Get-Content $stateFile -Raw | ConvertFrom-Json) }

$wanted = New-Object 'System.Collections.Generic.HashSet[string]' ([StringComparer]::OrdinalIgnoreCase)
foreach ($file in $manifest.files) { [void]$wanted.Add($file.path) }

$downloaded = 0
$bytes = 0

foreach ($file in $manifest.files) {
    $rel = $file.path.Replace('\', '/')
    $target = Join-Path $Dir ($rel.Replace('/', [System.IO.Path]::DirectorySeparatorChar))
    $upToDate = (Test-Path $target)
    if ($upToDate -and -not $Force) {
        $local = Get-Item $target
        if ($local.Length -ne $file.size) { $upToDate = $false }
    }
    if ($upToDate -and -not $Force) { continue }

    $parent = Split-Path -Parent $target
    if ($parent) { New-Item -ItemType Directory -Force -Path $parent | Out-Null }

    $encoded = ($rel.Split('/') | ForEach-Object { [Uri]::EscapeDataString($_) }) -join '/'
    $url = "$baseUrl/$encoded"
    Write-Host ("  + {0} ({1:N0} КБ)" -f $rel, ($file.size / 1KB))
    Invoke-WebRequest -Uri $url -OutFile $target -UseBasicParsing
    $downloaded++
    $bytes += $file.size
}

$deleted = 0
foreach ($old in $installed) {
    if ($wanted.Contains($old)) { continue }
    $target = Join-Path $Dir ($old.Replace('/', [System.IO.Path]::DirectorySeparatorChar))
    if (Test-Path $target) {
        Remove-Item $target -Force
        $deleted++
        Write-Host "  - удалён $old" -ForegroundColor DarkYellow
    }
}

$manifest.files.path | ConvertTo-Json -Compress | Set-Content $stateFile -Encoding UTF8

Write-Host ""
Write-Host ("Готово: скачано {0} файлов ({1:N1} МБ), удалено {2}. Версия сборки {3}." -f `
    $downloaded, ($bytes / 1MB), $deleted, $manifest.packVersion) -ForegroundColor Green
Write-Host "Не забудьте: в server.properties должно быть online-mode=false" -ForegroundColor Yellow

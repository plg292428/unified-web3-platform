# 重启后端服务脚本
# 功能：停止现有服务，等待，然后重新启动

param(
    [int]$WaitSeconds = 3,
    [string]$ServicePath = "$PSScriptRoot\..\..\src\Backend\UnifiedPlatform.WebApi"
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   重启后端服务" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 1. 停止服务
Write-Host "[步骤 1/2] 停止现有服务..." -ForegroundColor Yellow
$dotnetProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object {
    $_.Path -like "*UnifiedPlatform.WebApi*" -or 
    $_.CommandLine -like "*UnifiedPlatform.WebApi*"
}

if ($dotnetProcesses) {
    Write-Host "找到 $($dotnetProcesses.Count) 个后端服务进程，正在停止..." -ForegroundColor Yellow
    $dotnetProcesses | Stop-Process -Force
    Write-Host "[OK] 已停止服务" -ForegroundColor Green
} else {
    Write-Host "[信息] 未找到运行中的服务" -ForegroundColor Gray
}

# 2. 等待
Write-Host "[等待] 等待 $WaitSeconds 秒..." -ForegroundColor Gray
Start-Sleep -Seconds $WaitSeconds

# 3. 启动服务
Write-Host "[步骤 2/2] 启动服务..." -ForegroundColor Yellow

$ProjectRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$publishPath = "$ProjectRoot\src\publish\backend"

if (Test-Path $publishPath) {
    Set-Location $publishPath
    Start-Process -FilePath "dotnet" -ArgumentList "UnifiedPlatform.WebApi.dll" -NoNewWindow
    Write-Host "[OK] 服务已启动" -ForegroundColor Green
    Write-Host ""
    Write-Host "服务地址:" -ForegroundColor Cyan
    Write-Host "  - HTTP: http://localhost:5000" -ForegroundColor White
    Write-Host "  - Swagger: http://localhost:5000/swagger" -ForegroundColor White
} else {
    Write-Host "[警告] 未找到发布目录，尝试从项目目录启动..." -ForegroundColor Yellow
    Set-Location "$ProjectRoot\$ServicePath"
    Start-Process -FilePath "dotnet" -ArgumentList "run" -NoNewWindow
    Write-Host "[OK] 服务已启动（开发模式）" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   重启完成" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan


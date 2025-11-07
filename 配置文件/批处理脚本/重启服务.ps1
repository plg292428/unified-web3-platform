# 重启 UnifiedWeb3Platform 服务
$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  重启 UnifiedWeb3Platform 服务" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$projectPath = "D:\claude code\plg\UnifiedWeb3Platform\src\Backend\UnifiedPlatform.WebApi"

# 1. 停止当前服务
Write-Host "[1] 停止当前服务..." -ForegroundColor Yellow
Write-Host ""

# 查找占用5195端口的进程
$portProcesses = Get-NetTCPConnection -LocalPort 5195 -ErrorAction SilentlyContinue | Select-Object -ExpandProperty OwningProcess -Unique

if ($portProcesses) {
    foreach ($pid in $portProcesses) {
        try {
            $process = Get-Process -Id $pid -ErrorAction SilentlyContinue
            if ($process) {
                Write-Host "  发现进程: $($process.ProcessName) (PID: $pid)" -ForegroundColor Gray
                Stop-Process -Id $pid -Force -ErrorAction SilentlyContinue
                Write-Host "  [OK] 已停止进程 PID: $pid" -ForegroundColor Green
            }
        } catch {
            Write-Host "  [X] 停止进程失败: $pid" -ForegroundColor Red
        }
    }
} else {
    Write-Host "  未发现占用5195端口的进程" -ForegroundColor Gray
}

# 查找并停止 UnifiedPlatform.WebApi.exe
$webApiProcesses = Get-Process -Name "UnifiedPlatform.WebApi" -ErrorAction SilentlyContinue
if ($webApiProcesses) {
    foreach ($process in $webApiProcesses) {
        Write-Host "  发现进程: $($process.ProcessName) (PID: $($process.Id))" -ForegroundColor Gray
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        Write-Host "  [OK] 已停止进程 PID: $($process.Id)" -ForegroundColor Green
    }
} else {
    Write-Host "  未发现 UnifiedPlatform.WebApi.exe 进程" -ForegroundColor Gray
}

Write-Host ""
Write-Host "[2] 等待进程完全退出..." -ForegroundColor Yellow
Start-Sleep -Seconds 2

Write-Host ""
Write-Host "[3] 重新编译项目..." -ForegroundColor Yellow
Write-Host ""

Set-Location $projectPath

$buildResult = dotnet build 2>&1
$buildOutput = $buildResult | Out-String

if ($LASTEXITCODE -eq 0) {
    Write-Host "  [OK] 编译成功" -ForegroundColor Green
} else {
    Write-Host "  [X] 编译失败" -ForegroundColor Red
    Write-Host ""
    Write-Host $buildOutput -ForegroundColor Red
    Write-Host ""
    Write-Host "请检查错误信息后重试" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "[4] 启动服务..." -ForegroundColor Yellow
Write-Host ""
Write-Host "  使用 HTTPS profile 启动" -ForegroundColor Cyan
Write-Host "  服务地址:" -ForegroundColor Gray
Write-Host "    HTTP:  http://localhost:5195" -ForegroundColor Gray
Write-Host "    HTTPS: https://localhost:5196" -ForegroundColor Gray
Write-Host "    Swagger: http://localhost:5195/swagger" -ForegroundColor Gray
Write-Host ""
Write-Host "  按 Ctrl+C 停止服务" -ForegroundColor Yellow
Write-Host ""

# 启动服务
dotnet run --launch-profile https


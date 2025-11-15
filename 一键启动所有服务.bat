@echo off
chcp 65001 >nul
title 一键启动所有服务

echo.
echo ========================================
echo   一键启动所有服务
echo ========================================
echo.

cd /d "%~dp0"

echo [1/3] 检查后端服务状态...
netstat -ano | findstr ":5000" | findstr "LISTENING" >nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] 后端服务已在运行
) else (
    echo [启动] 正在启动后端服务...
    start "后端服务" /min powershell -NoExit -Command "cd '%~dp0src\Backend\UnifiedPlatform.WebApi'; Write-Host '后端服务运行中...' -ForegroundColor Green; Write-Host '请保持此窗口打开' -ForegroundColor Yellow; Write-Host ''; dotnet run"
    timeout /t 10 /nobreak >nul
    echo [OK] 后端服务已启动
)

echo.
echo [2/3] 检查前端服务状态...
netstat -ano | findstr ":8443" | findstr "LISTENING" >nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] 前端服务已在运行
) else (
    echo [启动] 正在启动前端服务...
    start "前端服务" /min powershell -NoExit -Command "cd '%~dp0src\Frontend\web-app'; Write-Host '前端服务运行中...' -ForegroundColor Green; Write-Host '请保持此窗口打开' -ForegroundColor Yellow; Write-Host ''; npm run dev"
    timeout /t 8 /nobreak >nul
    echo [OK] 前端服务已启动
)

echo.
echo [3/3] 检查 Cloudflare Tunnel 状态...
tasklist | findstr "cloudflared.exe" >nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] Cloudflare Tunnel 已在运行
) else (
    echo [启动] 正在启动 Cloudflare Tunnel...
    if exist "%~dp0cloudflared.exe" (
        start "Cloudflare Tunnel" /min powershell -NoExit -Command "cd '%~dp0'; Write-Host 'Cloudflare Tunnel 运行中...' -ForegroundColor Green; Write-Host '请保持此窗口打开' -ForegroundColor Yellow; Write-Host ''; .\cloudflared.exe tunnel run"
        timeout /t 5 /nobreak >nul
        echo [OK] Cloudflare Tunnel 已启动
    ) else (
        echo [错误] 未找到 cloudflared.exe
        echo [提示] 请先运行 快速安装Cloudflared.bat 或 配置CloudflareTunnel.bat
    )
)

echo.
echo ========================================
echo   启动完成
echo ========================================
echo.
echo 服务状态:
echo   - 后端服务: http://localhost:5000
echo   - 前端服务: https://localhost:8443
echo   - 前端域名: https://www.a292428dsj.dpdns.org
echo   - 后端 API: https://api.a292428dsj.dpdns.org
echo.
echo 提示:
echo   - 所有服务窗口已最小化到任务栏
echo   - 请保持这些窗口打开，关闭会导致服务停止
echo   - 等待 30-60 秒让 Tunnel 建立连接
echo   - 然后访问: https://www.a292428dsj.dpdns.org/
echo.
pause


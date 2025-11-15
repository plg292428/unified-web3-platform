@echo off
chcp 65001 >nul
title 启动后端 API Tunnel
echo ========================================
echo   启动后端 API Tunnel
echo ========================================
echo.

cd /d "%~dp0"

REM 检查 cloudflared
if exist "cloudflared.exe" (
    set CLOUDFLARED_CMD=cloudflared.exe
) else (
    where cloudflared >nul 2>&1
    if %errorlevel% equ 0 (
        set CLOUDFLARED_CMD=cloudflared
    ) else (
        echo [错误] Cloudflared 未找到
        echo 请先安装 Cloudflared 或确保 cloudflared.exe 在项目目录中
        pause
        exit /b 1
    )
)

echo [✓] Cloudflared 已找到
echo.

REM 检查后端服务
netstat -ano | findstr ":5000" | findstr "LISTENING" >nul
if %errorlevel% neq 0 (
    echo [警告] 后端服务未运行在端口 5000
    echo.
    echo 请先启动后端服务:
    echo   运行: 启动后端服务.bat
    echo   或: 快速启动后端.bat
    echo.
    set /p continue="是否继续启动 Tunnel? (Y/N): "
    if /i not "%continue%"=="Y" (
        exit /b 0
    )
    echo.
    echo [提示] 请确保后端服务在 Tunnel 启动后运行
    echo.
) else (
    echo [✓] 后端服务正在运行 (端口 5000)
    echo.
)

echo 正在启动后端 API Tunnel (api)...
echo.
echo API 地址: https://api.a292428dsj.dpdns.org
echo 本地服务: http://localhost:5000
echo.
echo 注意: 请保持此窗口打开，Tunnel 需要持续运行
echo 按 Ctrl+C 停止 Tunnel
echo.
echo ========================================
echo.

%CLOUDFLARED_CMD% tunnel run api

pause


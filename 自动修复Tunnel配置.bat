@echo off
chcp 65001 >nul
title 自动修复 Cloudflare Tunnel 配置

echo.
echo ========================================
echo   自动修复 Cloudflare Tunnel 配置
echo ========================================
echo.

cd /d "%~dp0"

REM 检查 PowerShell 版本
powershell -Command "if ($PSVersionTable.PSVersion.Major -lt 5) { exit 1 }" >nul 2>&1
if %errorlevel% neq 0 (
    echo [错误] 需要 PowerShell 5.0 或更高版本
    pause
    exit /b 1
)

echo [信息] 正在运行 PowerShell 修复脚本...
echo.

powershell -ExecutionPolicy Bypass -File "%~dp0自动修复Tunnel配置.ps1"

pause


@echo off
chcp 65001 >nul
title 一键部署后端

echo.
echo ========================================
echo   一键部署后端服务
echo ========================================
echo.

cd /d "%~dp0"

REM 检查 PowerShell 是否可用
powershell -Command "Get-Host" >nul 2>&1
if %errorlevel% neq 0 (
    echo [错误] 未找到 PowerShell
    echo 请确保已安装 PowerShell 5.1 或更高版本
    pause
    exit /b 1
)

echo [信息] 正在执行自动部署脚本...
echo.

REM 执行 PowerShell 部署脚本
powershell -ExecutionPolicy Bypass -File "配置文件\批处理脚本\自动部署后端.ps1"

if %errorlevel% neq 0 (
    echo.
    echo [错误] 部署过程中出现错误
    echo 请检查上面的错误信息
    pause
    exit /b 1
)

echo.
echo [完成] 部署流程已执行完成
pause


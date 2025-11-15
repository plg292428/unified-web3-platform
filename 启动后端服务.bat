@echo off
chcp 65001 >nul
title 启动后端服务

echo.
echo ========================================
echo   启动后端服务
echo ========================================
echo.

cd /d "%~dp0"

SET "BACKEND_DIR=src\Backend\UnifiedPlatform.WebApi"

if not exist "%BACKEND_DIR%" (
    echo [错误] 找不到后端目录: %BACKEND_DIR%
    echo.
    pause
    exit /b 1
)

echo [信息] 正在启动后端服务...
echo [信息] 后端目录: %CD%\%BACKEND_DIR%
echo.
echo 后端服务将在以下地址运行:
echo   - HTTP: http://localhost:5000
echo   - Swagger: http://localhost:5000/swagger
echo   - Health: http://localhost:5000/health
echo.
echo 提示: 请保持此窗口打开，关闭窗口将停止服务
echo.

cd "%BACKEND_DIR%"

REM 检查 dotnet 是否安装
where dotnet >nul 2>&1
if %errorlevel% neq 0 (
    echo [错误] 未找到 dotnet 命令
    echo 请先安装 .NET SDK
    echo.
    pause
    exit /b 1
)

REM 启动服务
echo [启动] 正在启动后端服务...
echo.
dotnet run

pause


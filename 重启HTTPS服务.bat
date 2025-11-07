@echo off
chcp 65001 >nul
echo ========================================
echo   停止服务并使用 HTTPS Profile 重启
echo ========================================
echo.

cd /d "%~dp0"

echo [步骤1] 停止当前运行的服务...
for /f "tokens=5" %%i in ('netstat -ano ^| findstr ":5195" ^| findstr "LISTENING"') do (
    echo   停止进程: %%i
    taskkill /PID %%i /F >nul 2>&1
)
timeout /t 3 /nobreak >nul
echo   [OK] 服务已停止
echo.

echo [步骤2] 使用 HTTPS Profile 启动服务...
echo   配置: HTTP (5195) + HTTPS (5196)
echo   请等待服务启动...
echo.

cd src\Backend\UnifiedPlatform.WebApi
dotnet run --launch-profile https

pause



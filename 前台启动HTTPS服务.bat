@echo off
chcp 65001 >nul
echo ========================================
echo   前台启动服务 (HTTPS Profile)
echo ========================================
echo.
echo [提示] 服务将在前台运行，您可以看到所有启动日志
echo [提示] 按 Ctrl+C 可以停止服务
echo.

cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"

echo [步骤1] 检查并停止旧服务...
for /f "tokens=5" %%i in ('netstat -ano ^| findstr ":5195" ^| findstr "LISTENING"') do (
    echo   停止进程: %%i
    taskkill /PID %%i /F >nul 2>&1
)
timeout /t 2 /nobreak >nul
echo.

echo [步骤2] 使用 HTTPS Profile 启动服务...
echo   配置: HTTP (5195) + HTTPS (5196)
echo   请查看下面的启动日志，确认 HTTPS 端口是否启动
echo.
echo ========================================
echo.

dotnet run --launch-profile https

echo.
echo ========================================
echo 服务已停止
pause



@echo off
chcp 65001 >nul
cd /d "%~dp0"

echo ========================================
echo   停止并重新启动服务（查看完整日志）
echo ========================================
echo.

echo [1] 查找并停止所有 UnifiedPlatform.WebApi 进程...
for /f "tokens=2" %%i in ('tasklist /fi "imagename eq UnifiedPlatform.WebApi.exe" /fo csv ^| findstr /v "PID"') do (
    set PID=%%i
    set PID=!PID:"=!
    if defined PID (
        echo   停止进程 PID: !PID!
        taskkill /PID !PID! /F >nul 2>&1
    )
)

timeout /t 2 /nobreak >nul

echo [OK] 已停止所有服务进程
echo.

echo [2] 清理日志文件...
if exist "startup_log.txt" del /f /q "startup_log.txt"
if exist "startup_error.txt" del /f /q "startup_error.txt"
echo [OK] 已清理日志文件
echo.

echo [3] 在前台启动服务（查看完整启动日志）...
echo    注意: 服务将在前台运行，请查看控制台输出
echo    查找以下关键信息:
echo      - Now listening on: http://localhost:5195
echo      - Now listening on: https://localhost:5196
echo      - 任何错误或警告信息
echo.
echo    按 Ctrl+C 停止服务
echo.
pause

cd src\Backend\UnifiedPlatform.WebApi
dotnet run --launch-profile https



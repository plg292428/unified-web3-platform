@echo off
chcp 65001 >nul
echo ========================================
echo   前台启动服务 - 实时查看输出
echo ========================================
echo.
echo [提示] 服务将在前台运行，您可以看到所有输出
echo [提示] 按 Ctrl+C 可以停止服务
echo.
echo 正在启动服务...
echo.

cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"

dotnet run

echo.
echo 服务已停止
pause




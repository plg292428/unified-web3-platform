@echo off
chcp 65001 >nul
echo ========================================
echo   使用 HTTPS Profile 启动服务
echo ========================================
echo.
echo [提示] 服务将同时运行 HTTP 和 HTTPS
echo [提示] HTTP:  http://localhost:5195
echo [提示] HTTPS: https://localhost:5196
echo.

cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"

echo [启动服务]...
dotnet run --launch-profile https

pause



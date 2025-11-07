@echo off
chcp 65001 >nul
echo ========================================
echo   Quick Test - Project Run
echo ========================================
echo.

cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"

echo [Step 1] Build project...
dotnet build >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo   [ERROR] Build failed
    pause
    exit /b 1
)
echo   [OK] Build successful
echo.

echo [Step 2] Starting project...
echo   [提示] 项目将启动，请等待几秒钟...
echo   [提示] 按 Ctrl+C 停止服务
echo.

dotnet run

pause


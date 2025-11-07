@echo off
chcp 65001 >nul
echo ========================================
echo   Test Run Project
echo ========================================
echo.

cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"

echo [Step 1] Building project...
dotnet build >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo   [ERROR] Build failed
    echo   [提示] Please check the errors above
    pause
    exit /b 1
)
echo   [OK] Build successful
echo.

echo [Step 2] Starting WebApi service...
echo   [提示] Service will start in background
echo   [提示] Press Ctrl+C to stop
echo.

start /B dotnet run --no-build > test_run.log 2>&1

echo   [等待] Service starting...
timeout /t 5 >nul

echo [Step 3] Testing health endpoint...
curl -k https://localhost:5001/health 2>nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] Health endpoint responding
) else (
    echo   [WARN] Health endpoint not responding
    echo   [提示] Service may still be starting, please wait...
)

echo.
echo [Step 4] Testing Swagger UI...
echo   [提示] Open browser: https://localhost:5001/swagger
echo.

echo [信息] Service is running
echo   Log file: test_run.log
echo   Press any key to stop service...
pause >nul

echo.
echo [停止] Stopping service...
taskkill /F /IM dotnet.exe >nul 2>&1
echo   [OK] Service stopped

pause


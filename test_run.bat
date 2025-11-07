@echo off
chcp 65001 >nul
echo ========================================
echo   Test Project Run
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
echo   [提示] 项目将在后台启动
echo   [提示] 等待5秒后测试连接...
echo.

start /B dotnet run --no-build > test_output.txt 2>&1

timeout /t 8 /nobreak >nul

echo [Step 3] Test health endpoint...
curl -k -s https://localhost:7266/health 2>nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] Health endpoint accessible
) else (
    echo   [WARN] Health endpoint test failed
    echo   [提示] Service may still be starting, please wait...
)

echo.
echo [Step 4] Test Swagger endpoint...
curl -k -s https://localhost:7266/swagger 2>nul | findstr /C:"swagger" >nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] Swagger UI accessible
) else (
    echo   [WARN] Swagger UI test failed
)

echo.
echo ========================================
echo   Test Complete
echo ========================================
echo.
echo Service URLs:
echo   - HTTPS: https://localhost:7266
echo   - HTTP:  http://localhost:5195
echo   - Swagger: https://localhost:7266/swagger
echo   - Health: https://localhost:7266/health
echo.
echo [提示] 查看 test_output.txt 了解详细输出
echo [提示] 按任意键停止服务...
pause >nul

taskkill /F /IM dotnet.exe /FI "WINDOWTITLE eq *UnifiedPlatform.WebApi*" >nul 2>&1


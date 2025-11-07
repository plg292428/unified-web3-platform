@echo off
chcp 65001 >nul
echo ========================================
echo   Test Database Connection
echo ========================================
echo.

cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"

echo [测试] 读取连接字符串...
for /f "tokens=2 delims=:" %%a in ('findstr /C:"DefaultConnection" appsettings.json') do (
    set CONN_STRING=%%a
    set CONN_STRING=!CONN_STRING:"=!
    set CONN_STRING=!CONN_STRING:,=!
    set CONN_STRING=!CONN_STRING: =!
)

echo   连接字符串: %CONN_STRING%
echo.

echo [测试] 尝试连接数据库...
echo   注意: 这需要数据库已创建
echo.

dotnet run --no-build --no-launch-profile >nul 2>&1 &
timeout /t 3 >nul

curl -k https://localhost:5001/health 2>nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] API服务运行正常，数据库连接可能成功
) else (
    echo   [提示] 请手动运行项目测试数据库连接
    echo   .\run_backend.bat
)

echo.
pause


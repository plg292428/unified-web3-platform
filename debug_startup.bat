@echo off
chcp 65001 >nul
echo ========================================
echo   Debug Startup Issues
echo ========================================
echo.

cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"

echo [Step 1] Check build...
dotnet build 2>&1 | findstr /C:"error" /C:"warning" /C:"成功"
if %ERRORLEVEL% NEQ 0 (
    echo   [WARN] Build may have issues
)
echo.

echo [Step 2] Check appsettings.json...
if exist "appsettings.json" (
    echo   [OK] appsettings.json exists
    findstr /C:"DefaultConnection" appsettings.json
) else (
    echo   [ERROR] appsettings.json not found
)
echo.

echo [Step 3] Check database connection...
echo   [提示] 尝试连接数据库...
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "SELECT name FROM sys.databases WHERE name = 'SmallTarget'" -W -h -1 2>nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] SmallTarget database exists
) else (
    echo   [ERROR] Cannot connect to database or database does not exist
    echo   [提示] 请检查数据库连接
)
echo.

echo [Step 4] Try to start service with detailed output...
echo   [提示] 这将显示详细的启动信息
echo   [提示] 按 Ctrl+C 停止
echo.
pause

dotnet run

pause


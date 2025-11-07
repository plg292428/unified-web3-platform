@echo off
chcp 65001 >nul
echo ========================================
echo   Diagnose Service Issues
echo ========================================
echo.

cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"

echo [Step 1] Check dotnet process...
tasklist /FI "IMAGENAME eq dotnet.exe" 2>nul | findstr "dotnet.exe" >nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] dotnet process is running
) else (
    echo   [ERROR] No dotnet process found
    echo   [提示] Service may have crashed
)
echo.

echo [Step 2] Check ports...
netstat -ano | findstr ":7266" >nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] Port 7266 is in use
) else (
    echo   [WARN] Port 7266 is not listening
)

netstat -ano | findstr ":5195" >nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] Port 5195 is in use
) else (
    echo   [WARN] Port 5195 is not listening
)
echo.

echo [Step 3] Check database...
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "SELECT name FROM sys.databases WHERE name = 'SmallTarget'" -W -h -1 >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo   [OK] SmallTarget database exists
) else (
    echo   [ERROR] SmallTarget database not found
    echo   [提示] This will cause startup failure
)
echo.

echo ========================================
echo   Diagnosis Complete
echo ========================================
echo.
echo Next: Try to run service in foreground to see errors
echo   .\run_backend.bat
echo.
pause


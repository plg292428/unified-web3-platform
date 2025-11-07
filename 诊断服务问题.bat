@echo off
chcp 65001 >nul
echo ========================================
echo   Diagnose Service Issues
echo ========================================
echo.

cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"

echo [Step 1] Check if service is running...
tasklist /FI "IMAGENAME eq dotnet.exe" | findstr "dotnet.exe" >nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] dotnet process found
) else (
    echo   [ERROR] No dotnet process running
    echo   [提示] Service may have crashed or not started
)
echo.

echo [Step 2] Check port availability...
netstat -ano | findstr ":7266" >nul
if %ERRORLEVEL% EQU 0 (
    echo   [INFO] Port 7266 is in use
    netstat -ano | findstr ":7266"
) else (
    echo   [WARN] Port 7266 is not in use
    echo   [提示] Service may not be listening
)

netstat -ano | findstr ":5195" >nul
if %ERRORLEVEL% EQU 0 (
    echo   [INFO] Port 5195 is in use
    netstat -ano | findstr ":5195"
) else (
    echo   [WARN] Port 5195 is not in use
    echo   [提示] Service may not be listening
)
echo.

echo [Step 3] Check database connection...
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "SELECT name FROM sys.databases WHERE name = 'SmallTarget'" -W -h -1 2>nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] SmallTarget database exists
) else (
    echo   [ERROR] SmallTarget database not found
    echo   [提示] This may cause service startup failure
)
echo.

echo [Step 4] Try to start service in foreground...
echo   [提示] This will show any error messages
echo   [提示] Press Ctrl+C to stop
echo.
pause

dotnet run


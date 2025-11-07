@echo off
chcp 65001 >nul
echo ========================================
echo   Create Database Migration
echo ========================================
echo.

cd /d "%~dp0src\Backend\UnifiedPlatform.DbService"

echo [Step 1] Check EF Tools...
dotnet ef --version >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo   [ERROR] EF Tools not installed
    echo   [提示] Installing EF Tools...
    dotnet tool install --global dotnet-ef
)
echo   [OK] EF Tools available
echo.

echo [Step 2] Set startup project...
set STARTUP_PROJECT=..\UnifiedPlatform.WebApi\UnifiedPlatform.WebApi.csproj
echo   [OK] Startup project: %STARTUP_PROJECT%
echo.

echo [Step 3] Create migration...
echo   Please enter migration name (or press Enter for 'InitialCreate'):
set /p MIGRATION_NAME=
if "%MIGRATION_NAME%"=="" set MIGRATION_NAME=InitialCreate

dotnet ef migrations add %MIGRATION_NAME% --startup-project %STARTUP_PROJECT% --context StDbContext

if %ERRORLEVEL% EQU 0 (
    echo.
    echo   [OK] Migration created successfully
    echo.
    echo [Step 4] Apply migration to database...
    echo   [提示] This will create/update the database
    echo   Press any key to continue or Ctrl+C to cancel...
    pause >nul
    
    dotnet ef database update --startup-project %STARTUP_PROJECT% --context StDbContext
    
    if %ERRORLEVEL% EQU 0 (
        echo.
        echo   [OK] Database updated successfully
    ) else (
        echo.
        echo   [ERROR] Database update failed
        echo   [提示] Please check connection string in appsettings.json
    )
) else (
    echo.
    echo   [ERROR] Migration creation failed
    echo   [提示] Please check the error messages above
)

echo.
pause


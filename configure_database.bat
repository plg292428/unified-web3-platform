@echo off
chcp 65001 >nul
echo ========================================
echo   Database Configuration
echo ========================================
echo.

cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"

echo [Step 1] Check database connection...
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "SELECT @@VERSION" >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo   [ERROR] Cannot connect to LocalDB
    echo   [提示] Please ensure SQL Server LocalDB is installed
    echo.
    pause
    exit /b 1
) else (
    echo   [OK] LocalDB connection successful
)
echo.

echo [Step 2] Check database exists...
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "IF EXISTS (SELECT * FROM sys.databases WHERE name = 'UnifiedPlatform') SELECT 'Database exists' ELSE SELECT 'Database does not exist'" -W -h -1 2>nul
echo.

echo [Step 3] Create database if not exists...
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'UnifiedPlatform') CREATE DATABASE UnifiedPlatform" 2>nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] Database created or already exists
) else (
    echo   [WARN] Database creation may have failed
)
echo.

echo [Step 4] Verify database...
sqlcmd -S "(localdb)\MSSQLLocalDB" -d "UnifiedPlatform" -Q "SELECT DB_NAME() AS CurrentDatabase" -W -h -1 2>nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] Database connection verified
) else (
    echo   [ERROR] Cannot connect to database
)
echo.

echo ========================================
echo   Database Configuration Complete
echo ========================================
echo.
echo Next steps:
echo   1. Run database migrations
echo   2. Or restore from backup
echo.
pause


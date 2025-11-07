@echo off
chcp 65001 >nul
echo ========================================
echo   Check Database Connection
echo ========================================
echo.

echo [检查] SQL Server LocalDB...
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "SELECT @@VERSION" >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo   [警告] 无法连接到LocalDB
    echo   [提示] 请确保已安装SQL Server LocalDB
    echo.
    echo   安装方法：
    echo   1. 下载并安装SQL Server Express
    echo   2. 或使用Visual Studio Installer安装LocalDB
    echo.
) else (
    echo   [OK] LocalDB连接成功
    echo.
    echo [检查] UnifiedPlatform数据库...
    sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "IF EXISTS (SELECT * FROM sys.databases WHERE name = 'UnifiedPlatform') SELECT 'Database exists' ELSE SELECT 'Database does not exist'" 2>nul
    echo.
    echo [提示] 如果数据库不存在，需要运行数据库迁移
)

echo.
pause


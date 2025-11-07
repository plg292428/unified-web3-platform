@echo off
chcp 65001 >nul
echo ========================================
echo   Apply Database Migration
echo ========================================
echo.

cd /d "%~dp0src\Backend\UnifiedPlatform.DbService"

set STARTUP_PROJECT=..\UnifiedPlatform.WebApi\UnifiedPlatform.WebApi.csproj

echo [检查] 迁移文件...
if exist "Migrations" (
    echo   [OK] Migrations目录存在
    dir /b Migrations 2>nul | find /c ".cs" >nul
    if %ERRORLEVEL% EQU 0 (
        echo   [OK] 找到迁移文件
    ) else (
        echo   [WARN] 未找到迁移文件
        echo   [提示] 请先运行 create_migration.bat 创建迁移
        pause
        exit /b 1
    )
) else (
    echo   [ERROR] Migrations目录不存在
    echo   [提示] 请先运行 create_migration.bat 创建迁移
    pause
    exit /b 1
)

echo.
echo [应用] 应用数据库迁移...
echo   [警告] 这将创建/更新数据库表结构
echo   Press any key to continue or Ctrl+C to cancel...
pause >nul

dotnet ef database update --startup-project %STARTUP_PROJECT% --context StDbContext

if %ERRORLEVEL% EQU 0 (
    echo.
    echo   [OK] 数据库迁移应用成功
    echo.
    echo [验证] 检查数据库表...
    sqlcmd -S "(localdb)\MSSQLLocalDB" -d "UnifiedPlatform" -Q "SELECT COUNT(*) AS TableCount FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'" -W -h -1 2>nul
) else (
    echo.
    echo   [ERROR] 数据库迁移失败
    echo   [提示] 请检查：
    echo     1. 数据库连接字符串是否正确
    echo     2. 数据库是否存在
    echo     3. 是否有足够的权限
)

echo.
pause


@echo off
chcp 65001 >nul
echo ========================================
echo   运行数据库迁移
echo ========================================
echo.

cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"

echo [步骤1] 设置环境变量...
set ConnectionStrings__DefaultConnection=Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UnifiedWeb3Platform;Integrated Security=True;TrustServerCertificate=True;
echo   [OK] 环境变量已设置
echo.

echo [步骤2] 运行数据库迁移...
echo   命令: dotnet ef database update --project ..\UnifiedPlatform.DbService --context StDbContext
echo.

dotnet ef database update --project ..\UnifiedPlatform.DbService --context StDbContext --startup-project .

if %ERRORLEVEL% EQU 0 (
    echo.
    echo [OK] 数据库迁移完成！
) else (
    echo.
    echo [ERROR] 数据库迁移失败
    echo.
    echo 请检查:
    echo   1. dotnet ef 工具是否已安装: dotnet tool install --global dotnet-ef
    echo   2. 数据库连接字符串是否正确
    echo   3. 数据库 UnifiedWeb3Platform 是否存在
)

echo.
pause


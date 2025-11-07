@echo off
chcp 65001 >nul
echo ========================================
echo   Diagnostic - 问题诊断
echo ========================================
echo.

cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"

echo [检查1] 编译状态...
dotnet build >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo   [OK] 编译成功
) else (
    echo   [ERROR] 编译失败
    dotnet build
    pause
    exit /b 1
)
echo.

echo [检查2] 数据库连接字符串...
findstr /C:"DefaultConnection" appsettings.json
echo.

echo [检查3] 检查端口占用...
netstat -ano | findstr ":7266" >nul
if %ERRORLEVEL% EQU 0 (
    echo   [WARN] 端口7266已被占用
    netstat -ano | findstr ":7266"
) else (
    echo   [OK] 端口7266未被占用
)

netstat -ano | findstr ":5195" >nul
if %ERRORLEVEL% EQU 0 (
    echo   [WARN] 端口5195已被占用
    netstat -ano | findstr ":5195"
) else (
    echo   [OK] 端口5195未被占用
)
echo.

echo [检查4] 检查数据库...
echo   [提示] 测试数据库连接...
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "SELECT name FROM sys.databases WHERE name = 'SmallTarget'" -W -h -1 2>nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] SmallTarget数据库存在
) else (
    echo   [ERROR] 无法连接到数据库或数据库不存在
    echo   [提示] 请检查：
    echo     1. SQL Server LocalDB是否安装
    echo     2. SmallTarget数据库是否存在
    echo     3. 连接字符串是否正确
)
echo.

echo [检查5] 检查ip2region.xdb文件...
if exist "ip2region.xdb" (
    echo   [OK] ip2region.xdb文件存在
) else (
    echo   [WARN] ip2region.xdb文件不存在（可选）
)
echo.

echo ========================================
echo   Diagnostic Complete
echo ========================================
echo.
echo 如果所有检查都通过，请运行:
echo   .\debug_run.bat
echo   查看详细的启动错误信息
echo.
pause


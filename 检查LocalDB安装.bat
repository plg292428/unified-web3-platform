@echo off
chcp 65001 >nul
echo ========================================
echo   Check LocalDB Installation
echo ========================================
echo.

echo [检查1] 检查LocalDB命令是否可用...
where sqllocaldb >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo   [OK] sqllocaldb 命令可用
) else (
    echo   [ERROR] sqllocaldb 命令不可用
    echo   [提示] LocalDB可能未安装，或PATH环境变量未配置
    echo   [提示] 请安装LocalDB或重启计算机
    echo.
    echo   [安装方法]
    echo   1. 通过Visual Studio Installer安装（推荐）
    echo      - 打开Visual Studio Installer
    echo      - 修改Visual Studio安装
    echo      - 选择"单个组件"标签
    echo      - 搜索并勾选"SQL Server Express LocalDB"
    echo      - 点击"修改"完成安装
    echo.
    echo   2. 独立安装SQL Server Express
    echo      - 访问: https://www.microsoft.com/sql-server/sql-server-downloads
    echo      - 下载Express版本
    echo      - 安装时选择LocalDB功能
    echo.
    pause
    exit /b 1
)
echo.

echo [检查2] 列出所有LocalDB实例...
sqllocaldb info
echo.

echo [检查3] 检查MSSQLLocalDB实例...
sqllocaldb info MSSQLLocalDB >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo   [OK] MSSQLLocalDB 实例存在
    echo.
    echo   [检查4] 检查实例状态...
    sqllocaldb info MSSQLLocalDB | findstr "State" | findstr "Running" >nul
    if %ERRORLEVEL% EQU 0 (
        echo   [OK] MSSQLLocalDB 正在运行
    ) else (
        echo   [INFO] MSSQLLocalDB 未运行，正在启动...
        sqllocaldb start MSSQLLocalDB
        timeout /t 2 >nul
        echo   [OK] MSSQLLocalDB 已启动
    )
) else (
    echo   [WARN] MSSQLLocalDB 实例不存在
    echo   [提示] 尝试创建实例...
    sqllocaldb create MSSQLLocalDB
    if %ERRORLEVEL% EQU 0 (
        echo   [OK] MSSQLLocalDB 实例已创建
        sqllocaldb start MSSQLLocalDB
        timeout /t 2 >nul
    ) else (
        echo   [ERROR] 无法创建实例，请检查权限
    )
)
echo.

echo [检查5] 测试数据库连接...
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "SELECT @@VERSION" -W -h -1 >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo   [OK] 数据库连接成功
    sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "SELECT @@VERSION" -W -h -1
) else (
    echo   [ERROR] 数据库连接失败
    echo   [提示] 请检查：
    echo     1. LocalDB是否已启动
    echo     2. 连接字符串是否正确
)
echo.

echo ========================================
echo   LocalDB Check Complete
echo ========================================
echo.
echo [下一步] 如果所有检查都通过，运行:
echo   .\fix_database_connection.bat
echo   创建SmallTarget数据库
echo.
pause


@echo off
chcp 65001 >nul
echo ========================================
echo   Fix Database Connection
echo ========================================
echo.

REM 设置LocalDB和sqlcmd的完整路径
set "LOCALDB_PATH=C:\Program Files\Microsoft SQL Server\150\Tools\Binn\sqllocaldb.exe"
set "SQLCMD_PATH=C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE"

REM 如果默认路径不存在，尝试在PATH中查找
if not exist "%LOCALDB_PATH%" (
    where sqllocaldb >nul 2>&1
    if %ERRORLEVEL% EQU 0 (
        set "LOCALDB_PATH=sqllocaldb"
    )
)

if not exist "%SQLCMD_PATH%" (
    where sqlcmd >nul 2>&1
    if %ERRORLEVEL% EQU 0 (
        set "SQLCMD_PATH=sqlcmd"
    )
)

echo [Step 1] 检查LocalDB状态...
"%LOCALDB_PATH%" info MSSQLLocalDB >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo   [ERROR] LocalDB未安装或PATH未配置
    echo   [提示] 请安装SQL Server Express或LocalDB
    echo   [提示] 下载地址: https://www.microsoft.com/sql-server/sql-server-downloads
    echo   [提示] 或重启计算机以使PATH生效
    pause
    exit /b 1
) else (
    echo   [OK] LocalDB已安装
)

"%LOCALDB_PATH%" info MSSQLLocalDB | findstr "State" | findstr "Running" >nul
if %ERRORLEVEL% NEQ 0 (
    echo   [INFO] LocalDB未运行，正在启动...
    "%LOCALDB_PATH%" start MSSQLLocalDB
    timeout /t 3 >nul
) else (
    echo   [OK] LocalDB正在运行
)
echo.

echo [Step 2] 检查SmallTarget数据库...
"%SQLCMD_PATH%" -S "(localdb)\MSSQLLocalDB" -Q "SELECT name FROM sys.databases WHERE name = 'SmallTarget'" -W -h -1 2>nul | findstr "SmallTarget" >nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] SmallTarget数据库存在
) else (
    echo   [WARN] SmallTarget数据库不存在
    echo   [提示] 是否创建SmallTarget数据库？(Y/N)
    set /p CREATE_DB=
    if /i "%CREATE_DB%"=="Y" (
        echo   [创建] 正在创建数据库...
        "%SQLCMD_PATH%" -S "(localdb)\MSSQLLocalDB" -Q "CREATE DATABASE SmallTarget" 2>nul
        if %ERRORLEVEL% EQU 0 (
            echo   [OK] 数据库创建成功
            echo   [提示] 注意：数据库是空的，需要运行迁移或导入数据
        ) else (
            echo   [ERROR] 数据库创建失败
        )
    )
)
echo.

echo [Step 3] 测试数据库连接...
"%SQLCMD_PATH%" -S "(localdb)\MSSQLLocalDB" -d "SmallTarget" -Q "SELECT DB_NAME() AS CurrentDatabase" -W -h -1 2>nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] 数据库连接成功
) else (
    echo   [ERROR] 数据库连接失败
    echo   [提示] 请检查：
    echo     1. LocalDB是否运行
    echo     2. 数据库是否存在
    echo     3. 权限是否足够
)
echo.

echo ========================================
echo   Database Fix Complete
echo ========================================
echo.
pause


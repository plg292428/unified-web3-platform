@echo off
chcp 65001 >nul
echo ========================================
echo   项目完成情况检测报告
echo ========================================
echo.

echo [1] 项目目录结构检查...
if exist "src\Frontend\web-app" (
    echo   [OK] Frontend 目录存在
) else (
    echo   [X] Frontend 目录不存在
)
if exist "src\Backend\UnifiedPlatform.WebApi" (
    echo   [OK] WebApi 目录存在
) else (
    echo   [X] WebApi 目录不存在
)
if exist "src\Backend\UnifiedPlatform.DbService" (
    echo   [OK] DbService 目录存在
) else (
    echo   [X] DbService 目录不存在
)
if exist "src\Backend\UnifiedPlatform.Shared" (
    echo   [OK] Shared 目录存在
) else (
    echo   [X] Shared 目录不存在
)
if exist "src\Libraries\HFastKit" (
    echo   [OK] HFastKit 目录存在
) else (
    echo   [X] HFastKit 目录不存在
)
if exist "src\Libraries\Nblockchain" (
    echo   [OK] Nblockchain 目录存在
) else (
    echo   [X] Nblockchain 目录不存在
)
echo.

echo [2] 解决方案文件检查...
if exist "UnifiedPlatform.sln" (
    echo   [OK] 解决方案文件存在
) else (
    echo   [X] 解决方案文件不存在
)
if exist "src\Backend\UnifiedPlatform.sln" (
    echo   [OK] Backend 解决方案文件存在
) else (
    echo   [X] Backend 解决方案文件不存在
)
echo.

echo [3] 数据库连接配置检查...
if exist "src\Backend\UnifiedPlatform.WebApi\appsettings.json" (
    echo   [OK] 配置文件存在
    findstr /C:"DefaultConnection" "src\Backend\UnifiedPlatform.WebApi\appsettings.json" >nul
    if %ERRORLEVEL% EQU 0 (
        echo   [OK] 数据库连接字符串已配置
        findstr /C:"SmallTarget" "src\Backend\UnifiedPlatform.WebApi\appsettings.json" >nul
        if %ERRORLEVEL% EQU 0 (
            echo   数据库名称: SmallTarget
        )
    ) else (
        echo   [X] 数据库连接字符串未配置
    )
) else (
    echo   [X] 配置文件不存在
)
echo.

echo [4] 服务运行状态检查...
netstat -ano | findstr ":5195" >nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] 服务正在运行 (端口 5195)
    for /f "tokens=5" %%i in ('netstat -ano ^| findstr ":5195" ^| findstr "LISTENING"') do (
        echo   进程 ID: %%i
        goto :found
    )
    :found
) else (
    echo   [X] 服务未运行
)
echo.

echo [5] API 健康检查...
curl -s http://localhost:5195/health >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo   [OK] API 响应正常
    curl -s http://localhost:5195/health
    echo.
) else (
    echo   [X] API 无法访问
)
echo.

echo [6] Swagger UI 可访问性检查...
curl -s http://localhost:5195/swagger >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo   [OK] Swagger UI 可访问
    echo   访问地址: http://localhost:5195/swagger
) else (
    echo   [X] Swagger UI 无法访问
)
echo.

echo [7] 项目编译状态检查...
cd src\Backend\UnifiedPlatform.WebApi
dotnet build --no-incremental 2>&1 | findstr /C:"成功" /C:"succeeded" /C:"错误" /C:"error" >nul
if %ERRORLEVEL% EQU 0 (
    dotnet build --no-incremental 2>&1 | findstr /C:"成功" /C:"succeeded" >nul
    if %ERRORLEVEL% EQU 0 (
        echo   [OK] 项目编译成功
    ) else (
        echo   [X] 项目编译有错误
    )
) else (
    echo   [?] 编译状态未知
)
cd ..\..\..
echo.

echo [8] 数据库迁移文件检查...
if exist "src\Backend\UnifiedPlatform.DbService\Migrations" (
    echo   [OK] 迁移文件目录存在
    dir /b "src\Backend\UnifiedPlatform.DbService\Migrations\*.cs" 2>nul | find /c /v "" >temp_count.txt
    set /p MIGRATION_COUNT=<temp_count.txt
    del temp_count.txt
    echo   迁移文件数量: %MIGRATION_COUNT%
) else (
    echo   [X] 迁移文件目录不存在
)
echo.

echo [9] 工具脚本检查...
if exist "前台启动服务.bat" (
    echo   [OK] 前台启动脚本存在
) else (
    echo   [X] 前台启动脚本不存在
)
if exist "停止服务.bat" (
    echo   [OK] 停止服务脚本存在
) else (
    echo   [X] 停止服务脚本不存在
)
if exist "fix_database_connection.ps1" (
    echo   [OK] 数据库修复脚本存在
) else (
    echo   [X] 数据库修复脚本不存在
)
echo.

echo ========================================
echo   检测完成
echo ========================================
pause



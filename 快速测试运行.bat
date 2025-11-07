@echo off
chcp 65001 >nul
echo ========================================
echo   快速测试运行（临时模式）
echo ========================================
echo.
echo [提示] 如果LocalDB未安装，服务将以最小模式运行
echo [提示] 部分功能（Web3、缓存等）可能不可用
echo [提示] 但可以访问Swagger UI和健康检查端点
echo.
pause

cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"

echo [编译] 编译项目...
dotnet build >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo   [ERROR] 编译失败
    dotnet build
    pause
    exit /b 1
)
echo   [OK] 编译成功
echo.

echo [运行] 启动服务...
echo   [提示] 服务启动后，请访问：
echo      - Swagger UI: https://localhost:7266/swagger
echo      - 健康检查: http://localhost:5195/health
echo   [提示] 如果看到错误信息，请检查：
echo      1. LocalDB是否安装
echo      2. SmallTarget数据库是否存在
echo      3. 端口是否被占用
echo.
echo   [提示] 按 Ctrl+C 停止服务
echo.

dotnet run

pause


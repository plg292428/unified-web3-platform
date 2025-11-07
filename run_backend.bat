@echo off
chcp 65001 >nul
echo ========================================
echo   Run UnifiedPlatform WebApi
echo ========================================
echo.

cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"

echo [检查] 编译项目...
dotnet build >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo   [错误] 编译失败，请先修复错误
    pause
    exit /b 1
)

echo   [OK] 编译成功
echo.
echo [启动] 运行WebApi项目...
echo   访问地址: https://localhost:5001
echo   Swagger UI: https://localhost:5001/swagger
echo   健康检查: https://localhost:5001/health
echo.
echo   按 Ctrl+C 停止服务
echo.

dotnet run

pause


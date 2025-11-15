@echo off
chcp 65001 >nul
echo ========================================
echo   启动后端服务
echo ========================================
echo.

cd /d %~dp0

echo [步骤 1] 检查后端项目...
if not exist "src\Backend\UnifiedPlatform.WebApi" (
    echo   [错误] 后端项目不存在
    pause
    exit /b 1
)
echo   [OK] 后端项目存在

echo.
echo [步骤 2] 启动后端服务...
cd src\Backend\UnifiedPlatform.WebApi
echo   当前目录: %CD%
echo   执行命令: dotnet run
echo.
echo   等待服务启动（约 10-20 秒）...
echo   成功后应该看到: "Now listening on: http://localhost:5000"
echo.
echo   ========================================
echo   提示: 按 Ctrl+C 可以停止服务
echo   ========================================
echo.

dotnet run

pause


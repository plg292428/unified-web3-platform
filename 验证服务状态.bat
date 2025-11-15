@echo off
chcp 65001 >nul
echo ========================================
echo   验证服务状态
echo ========================================
echo.

echo [检查] 后端服务状态...
curl -s http://localhost:5000/api/store/categories >nul 2>&1
if %errorlevel% equ 0 (
    echo   [OK] 后端服务运行正常
    echo.
    echo [测试] API 响应...
    curl -s http://localhost:5000/api/store/categories
    echo.
    echo.
) else (
    echo   [FAIL] 后端服务未运行
    echo.
    echo 请执行以下步骤:
    echo   1. 运行: .\启动所有服务.bat
    echo   2. 或手动启动后端服务:
    echo      cd src\Backend\UnifiedPlatform.WebApi
    echo      dotnet run
    echo.
)

echo [检查] 前端服务状态...
curl -s http://localhost:5173 >nul 2>&1
if %errorlevel% equ 0 (
    echo   [OK] 前端服务运行正常
) else (
    echo   [WARN] 前端服务未运行
    echo   启动命令: cd src\Frontend\web-app ^&^& npm run dev
)

echo.
echo ========================================
echo   服务地址
echo ========================================
echo   前端页面: http://localhost:5173
echo   后端 API:  http://localhost:5000
echo   Swagger:   http://localhost:5000/swagger
echo.
pause


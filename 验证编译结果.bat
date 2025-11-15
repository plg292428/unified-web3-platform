@echo off
chcp 65001 >nul
echo ========================================
echo   验证前端编译结果
echo ========================================
echo.

echo [检查] 前端服务状态...
curl -s http://localhost:5173 >nul 2>&1
if %errorlevel% equ 0 (
    echo   [OK] 前端服务运行正常
    echo   访问地址: http://localhost:5173
) else (
    echo   [FAIL] 前端服务未运行
    echo   请运行: .\run_frontend.bat
    echo.
    pause
    exit /b 1
)

echo.
echo [检查] 后端服务状态...
curl -s http://localhost:5000/api/store/categories >nul 2>&1
if %errorlevel% equ 0 (
    echo   [OK] 后端服务运行正常
    echo   访问地址: http://localhost:5000
    echo.
    echo [测试] API 连接...
    curl -s http://localhost:5000/api/store/categories
    echo.
    echo.
) else (
    echo   [WARN] 后端服务未运行
    echo   请运行: .\启动所有服务.bat
    echo   或手动启动: cd src\Backend\UnifiedPlatform.WebApi ^&^& dotnet run
    echo.
)

echo ========================================
echo   测试完成
echo ========================================
echo.
echo 下一步操作:
echo   1. 在浏览器中打开: http://localhost:5173
echo   2. 按 F12 打开开发者工具
echo   3. 查看控制台（Console）标签
echo   4. 检查以下日志:
echo      - "API Base URL from serverConfig.json: http://localhost:5000"
echo      - 或 "使用默认开发环境 API 地址: http://localhost:5000"
echo   5. 如果后端服务未运行，会看到清晰的错误提示:
echo      "无法连接到服务器，请检查后端服务是否运行（http://localhost:5000）"
echo.
pause


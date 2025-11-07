@echo off
chcp 65001 >nul
echo ========================================
echo   Test API Endpoints
echo ========================================
echo.

echo [测试] 健康检查端点...
echo.

curl -k https://localhost:5001/health 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo   [警告] API服务可能未运行
    echo   [提示] 请先运行 run_backend.bat 启动服务
) else (
    echo.
    echo   [OK] API服务运行正常
)

echo.
echo [测试] Swagger UI...
echo   请在浏览器中访问: https://localhost:5001/swagger
echo.

pause


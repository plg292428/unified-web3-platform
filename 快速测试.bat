@echo off
chcp 65001 >nul
echo ========================================
echo   UnifiedWeb3Platform 快速测试
echo ========================================
echo.

echo [1/3] 检查后端服务状态...
netstat -ano | findstr ":5000" >nul
if %errorlevel% equ 0 (
    echo   [OK] 后端服务正在运行 (端口 5000)
) else (
    echo   [WARN] 后端服务未运行
    echo   提示: 请先运行 run_backend.bat 启动后端服务
    echo.
    pause
    exit /b 1
)

echo.
echo [2/3] 运行 API 测试...
powershell -ExecutionPolicy Bypass -File "测试脚本\test_api.ps1"
if %errorlevel% neq 0 (
    echo   [FAIL] API 测试失败
    pause
    exit /b 1
)

echo.
echo [3/3] 运行数据库测试...
powershell -ExecutionPolicy Bypass -File "测试脚本\test_database.ps1"

echo.
echo ========================================
echo   测试完成
echo ========================================
echo.
pause


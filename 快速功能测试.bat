@echo off
chcp 65001 >nul
echo ========================================
echo   功能测试 - 快速检查
echo ========================================
echo.

echo [1] 检查后端服务...
curl -s http://localhost:5000/api/store/categories >nul 2>&1
if %errorlevel% equ 0 (
    echo   [OK] 后端服务运行正常
) else (
    echo   [FAIL] 后端服务未运行，请先启动后端服务
    echo   启动命令: cd src\Backend\UnifiedPlatform.WebApi ^&^& dotnet run
    echo.
    pause
    exit /b 1
)

echo.
echo [2] 检查前端服务...
curl -s http://localhost:5173 >nul 2>&1
if %errorlevel% equ 0 (
    echo   [OK] 前端服务运行正常
) else (
    echo   [WARN] 前端服务未运行，请启动前端服务
    echo   启动命令: cd src\Frontend\web-app ^&^& npm run dev
)

echo.
echo [3] 测试商品分类 API...
curl -s http://localhost:5000/api/store/categories
echo.
echo.

echo [4] 测试商品列表 API...
curl -s "http://localhost:5000/api/store/products?page=1&pageSize=3"
echo.
echo.

echo ========================================
echo   测试完成
echo ========================================
echo.
echo 访问地址:
echo   前端: http://localhost:5173
echo   后端: http://localhost:5000
echo   Swagger: http://localhost:5000/swagger
echo.
echo 功能测试步骤:
echo   1. 打开浏览器访问 http://localhost:5173
echo   2. 测试商品浏览、搜索、筛选
echo   3. 测试钱包连接
echo   4. 测试购物流程
echo   5. 测试订单管理
echo.
pause


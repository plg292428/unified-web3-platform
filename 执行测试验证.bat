@echo off
chcp 65001 >nul
echo ========================================
echo   执行测试验证
echo ========================================
echo.

echo [步骤 1] 检查后端服务状态...
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
    echo   正在启动后端服务...
    start "后端服务" cmd /k "cd /d %~dp0 && run_backend.bat"
    timeout /t 3 /nobreak >nul
    echo   [OK] 后端服务启动命令已执行
    echo   等待服务启动（约 15-30 秒）...
    timeout /t 20 /nobreak >nul
    echo.
    echo [验证] 后端服务是否启动成功...
    curl -s http://localhost:5000/api/store/categories >nul 2>&1
    if %errorlevel% equ 0 (
        echo   [OK] 后端服务已成功启动
    ) else (
        echo   [WARN] 后端服务可能还在启动中
        echo   请检查后端服务窗口是否正常运行
    )
)

echo.
echo [步骤 2] 检查前端服务状态...
curl -s http://localhost:5173 >nul 2>&1
if %errorlevel% equ 0 (
    echo   [OK] 前端服务运行正常
) else (
    echo   [WARN] 前端服务未运行
    echo   请运行: .\run_frontend.bat
)

echo.
echo [步骤 3] 测试商品列表 API...
curl -s "http://localhost:5000/api/store/products?page=1&pageSize=3" >nul 2>&1
if %errorlevel% equ 0 (
    echo   [OK] 商品列表 API 正常
) else (
    echo   [WARN] 商品列表 API 无法访问（后端服务可能未运行）
)

echo.
echo ========================================
echo   测试验证完成
echo ========================================
echo.
echo 服务地址:
echo   前端页面: http://localhost:5173
echo   后端 API:  http://localhost:5000
echo   Swagger:   http://localhost:5000/swagger
echo.
echo 下一步操作:
echo   1. 在浏览器中打开: http://localhost:5173
echo   2. 按 F12 打开开发者工具
echo   3. 查看控制台（Console）标签
echo   4. 检查以下内容:
echo.
echo   如果后端服务未运行:
echo     - 应该进入访客模式
echo     - 控制台显示: "后端服务未运行，进入访客模式"
echo     - 不跳转到 Error500 页面
echo     - 可以浏览商品（只读模式）
echo.
echo   如果后端服务已运行:
echo     - 正常加载配置
echo     - 可以正常使用所有功能
echo     - 不跳转到 Error500 页面
echo.
pause


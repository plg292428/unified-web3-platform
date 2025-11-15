@echo off
chcp 65001 >nul
echo ========================================
echo   启动前端服务
echo ========================================
echo.

cd /d %~dp0

echo [步骤 1] 检查前端项目...
if not exist "src\Frontend\web-app" (
    echo   [错误] 前端项目不存在
    pause
    exit /b 1
)
echo   [OK] 前端项目存在

echo.
echo [步骤 2] 检查 node_modules...
cd src\Frontend\web-app
if not exist "node_modules" (
    echo   [WARN] node_modules 不存在，需要先安装依赖
    echo   执行: npm install
    pause
    exit /b 1
)
echo   [OK] node_modules 存在

echo.
echo [步骤 3] 启动前端服务...
echo   当前目录: %CD%
echo   执行命令: npm run dev
echo.
echo   等待服务启动（约 5-10 秒）...
echo   成功后应该看到: "Local: http://localhost:5173"
echo.
echo   ========================================
echo   提示: 按 Ctrl+C 可以停止服务
echo   ========================================
echo.

npm run dev

pause


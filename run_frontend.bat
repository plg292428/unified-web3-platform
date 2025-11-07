@echo off
chcp 65001 >nul
echo ========================================
echo   Run UnifiedPlatform Frontend
echo ========================================
echo.

cd /d "%~dp0src\Frontend\web-app"

echo [检查] Node.js环境...
where node >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo   [错误] Node.js未安装，请先安装Node.js
    pause
    exit /b 1
)

node --version
echo   [OK] Node.js已安装
echo.

echo [检查] 依赖包...
if not exist "node_modules" (
    echo   [安装] 正在安装依赖包...
    call npm install
    if %ERRORLEVEL% NEQ 0 (
        echo   [错误] 依赖包安装失败
        pause
        exit /b 1
    )
) else (
    echo   [OK] 依赖包已存在
)
echo.

echo [启动] 运行前端开发服务器...
echo   访问地址: http://localhost:5173
echo.
echo   按 Ctrl+C 停止服务
echo.

call npm run dev

pause


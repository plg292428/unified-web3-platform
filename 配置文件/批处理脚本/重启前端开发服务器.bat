@echo off
chcp 65001 >nul
echo ========================================
echo   重启前端开发服务器
echo ========================================
echo.

cd /d "%~dp0\..\..\src\Frontend\web-app"

echo [1] 停止当前开发服务器...
echo.

REM 查找并停止占用3000端口的进程
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":3000" ^| findstr "LISTENING"') do (
    echo 发现进程 PID: %%a
    taskkill /F /PID %%a >nul 2>&1
    if errorlevel 1 (
        echo   停止失败
    ) else (
        echo   已停止进程 PID: %%a
    )
)

echo.
echo [2] 等待进程完全退出...
timeout /t 2 /nobreak >nul

echo.
echo [3] 验证.env文件...
if exist ".env" (
    echo   [OK] .env文件存在
    echo   内容:
    type .env
) else (
    echo   [X] .env文件不存在
)

echo.
echo [4] 启动开发服务器...
echo.
echo   使用环境变量配置:
echo     VITE_API_BASE_URL=http://localhost:5195
echo     VITE_APP_NAME=UnifiedWeb3Platform
echo.
echo   开发服务器地址: http://localhost:3000
echo   按 Ctrl+C 停止服务器
echo.

npm run dev

pause


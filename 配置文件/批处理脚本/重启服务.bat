@echo off
chcp 65001 >nul
echo ========================================
echo   重启 UnifiedWeb3Platform 服务
echo ========================================
echo.

cd /d "%~dp0\..\..\src\Backend\UnifiedPlatform.WebApi"

echo [1] 停止当前服务...
echo.

REM 查找并停止 dotnet 进程（UnifiedPlatform.WebApi）
for /f "tokens=2" %%a in ('netstat -ano ^| findstr ":5195" ^| findstr "LISTENING"') do (
    echo 发现进程 PID: %%a
    taskkill /F /PID %%a >nul 2>&1
    if errorlevel 1 (
        echo   停止失败
    ) else (
        echo   已停止进程 PID: %%a
    )
)

REM 查找并停止 UnifiedPlatform.WebApi.exe
taskkill /F /IM UnifiedPlatform.WebApi.exe >nul 2>&1
if errorlevel 1 (
    echo 未发现 UnifiedPlatform.WebApi.exe 进程
) else (
    echo 已停止 UnifiedPlatform.WebApi.exe
)

echo.
echo [2] 等待进程完全退出...
timeout /t 2 /nobreak >nul

echo.
echo [3] 重新编译项目...
echo.
dotnet build
if errorlevel 1 (
    echo.
    echo [X] 编译失败，请检查错误信息
    pause
    exit /b 1
)

echo.
echo [4] 启动服务...
echo.
echo 使用 HTTPS profile 启动...
echo 服务地址: http://localhost:5195 和 https://localhost:5196
echo.
echo 按 Ctrl+C 停止服务
echo.

dotnet run --launch-profile https

pause


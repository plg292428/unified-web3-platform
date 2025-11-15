@echo off
chcp 65001 >nul
echo ========================================
echo   启动后端服务（查看完整输出）
echo ========================================
echo.

echo [1/3] 停止所有 dotnet 进程...
for /f "tokens=2" %%i in ('tasklist /FI "IMAGENAME eq dotnet.exe" /FO LIST ^| findstr /I "PID"') do (
    echo   结束进程: %%i
    taskkill /PID %%i /F >nul 2>&1
)
timeout /t 2 /nobreak >nul
echo [OK] 已停止所有进程
echo.

echo [2/3] 检查项目...
cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"
if not exist "UnifiedPlatform.WebApi.csproj" (
    echo [错误] 项目文件不存在
    pause
    exit /b 1
)
echo [OK] 项目文件存在
echo.

echo [3/3] 启动后端服务...
echo   请查看下面的输出，如果有错误会显示出来
echo   如果看到 "Now listening on: http://localhost:5000" 说明启动成功
echo   按 Ctrl+C 可以停止服务
echo.
dotnet run

pause


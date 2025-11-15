@echo off
chcp 65001 >nul
echo ========================================
echo   修复并启动后端服务
echo ========================================
echo.

echo [1/5] 停止所有 dotnet 进程...
for /f "tokens=2" %%i in ('tasklist /FI "IMAGENAME eq dotnet.exe" /FO LIST ^| findstr /I "PID"') do (
    echo   结束进程: %%i
    taskkill /PID %%i /F >nul 2>&1
)
timeout /t 2 /nobreak >nul
echo [OK] 已停止所有进程
echo.

echo [2/5] 应用数据库迁移...
cd /d "%~dp0src\Backend\UnifiedPlatform.DbService"
dotnet ef database update
if %errorlevel% neq 0 (
    echo [错误] 数据库迁移失败
    pause
    exit /b 1
)
echo [OK] 数据库迁移完成
echo.

echo [3/5] 清理编译缓存...
cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"
if exist "bin" rmdir /s /q bin >nul 2>&1
if exist "obj" rmdir /s /q obj >nul 2>&1
echo [OK] 清理完成
echo.

echo [4/5] 重新编译项目...
dotnet build
if %errorlevel% neq 0 (
    echo [错误] 编译失败
    pause
    exit /b 1
)
echo [OK] 编译成功
echo.

echo [5/5] 启动后端服务...
echo   请等待服务启动（约 10-20 秒）...
echo   如果看到 "Now listening on: http://localhost:5000" 说明启动成功
echo   按 Ctrl+C 可以停止服务
echo.
dotnet run

pause


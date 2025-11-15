@echo off
chcp 65001 >nul
echo ========================================
echo   完整重启后端服务
echo ========================================
echo.

echo [1/4] 正在停止所有后端服务进程...
for /f "tokens=2" %%i in ('tasklist /FI "IMAGENAME eq dotnet.exe" /FO LIST ^| findstr /I "PID"') do (
    echo   结束进程: %%i
    taskkill /PID %%i /F >nul 2>&1
)

timeout /t 3 /nobreak >nul
echo [OK] 已停止所有进程
echo.

echo [2/4] 检查 SQL Server 服务状态...
sc query MSSQLSERVER | findstr /I "STATE" >nul
if %errorlevel% equ 0 (
    echo [OK] SQL Server 服务已找到
) else (
    echo [WARN] 未找到 SQL Server 服务，请手动检查
)
echo.

echo [3/4] 清理编译缓存...
cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"
if exist "bin" (
    echo   清理 bin 目录...
    rmdir /s /q bin >nul 2>&1
)
if exist "obj" (
    echo   清理 obj 目录...
    rmdir /s /q obj >nul 2>&1
)
echo [OK] 清理完成
echo.

echo [4/4] 启动后端服务...
echo   请等待服务启动（约 10-20 秒）...
echo   如果看到 "Now listening on: http://localhost:5000" 说明启动成功
echo   按 Ctrl+C 可以停止服务
echo.
dotnet run

pause


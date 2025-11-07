@echo off
chcp 65001 >nul
echo ========================================
echo   停止运行中的服务
echo ========================================
echo.

echo [正在查找运行中的服务进程...]
for /f "tokens=2" %%i in ('tasklist /FI "IMAGENAME eq dotnet.exe" /FO LIST ^| findstr /I "PID"') do (
    echo   找到进程: %%i
    taskkill /PID %%i /F >nul 2>&1
    if %ERRORLEVEL% EQU 0 (
        echo   [OK] 已停止进程 %%i
    ) else (
        echo   [INFO] 进程 %%i 可能已停止或无法停止
    )
)

echo.
echo [检查 UnifiedPlatform.WebApi 进程...]
for /f "tokens=2" %%i in ('tasklist /FI "WINDOWTITLE eq UnifiedPlatform.WebApi*" /FO LIST 2^>nul ^| findstr /I "PID"') do (
    echo   找到进程: %%i
    taskkill /PID %%i /F >nul 2>&1
    if %ERRORLEVEL% EQU 0 (
        echo   [OK] 已停止进程 %%i
    )
)

echo.
echo [等待进程完全释放文件...]
timeout /t 2 /nobreak >nul

echo.
echo 服务已停止
pause



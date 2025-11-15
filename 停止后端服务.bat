@echo off
chcp 65001 >nul
echo ========================================
echo   停止后端服务
echo ========================================
echo.

echo 正在查找并结束后端服务进程...
for /f "tokens=2" %%i in ('tasklist /FI "IMAGENAME eq dotnet.exe" /FO LIST ^| findstr /I "PID"') do (
    echo 找到进程: %%i
    taskkill /PID %%i /F >nul 2>&1
)

timeout /t 2 /nobreak >nul

echo.
echo [OK] 已停止所有后端服务进程
echo.
pause


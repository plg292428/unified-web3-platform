@echo off
chcp 65001 >nul
echo ========================================
echo   Check Service Running Status
echo ========================================
echo.

echo [检查] 检查服务进程...
tasklist /FI "IMAGENAME eq dotnet.exe" | findstr "dotnet.exe" >nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] dotnet进程正在运行
) else (
    echo   [INFO] 未找到运行中的dotnet进程
    echo   [提示] 服务可能未启动或已停止
)
echo.

echo [测试] 测试健康检查端点...
curl -s http://localhost:5195/health 2>nul
if %ERRORLEVEL% EQU 0 (
    echo.
    echo   [OK] 服务正在运行
    echo   [提示] 访问 https://localhost:7266/swagger 查看API文档
) else (
    echo   [WARN] 无法连接到服务
    echo   [提示] 服务可能还在启动中，请稍候再试
    echo   [提示] 或检查是否有错误信息
)

echo.
echo [测试] 测试HTTPS端点...
curl -k -s https://localhost:7266/health 2>nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] HTTPS端点正常
) else (
    echo   [INFO] HTTPS端点测试失败（可能正在启动）
)

echo.
echo ========================================
echo   Service Status Check Complete
echo ========================================
echo.
echo 如果服务未运行，请执行:
echo   .\run_backend.bat
echo.
pause


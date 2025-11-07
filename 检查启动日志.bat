@echo off
chcp 65001 >nul
cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"

echo ========================================
echo   检查启动日志中的错误
echo ========================================
echo.

echo [1] 清理并编译项目...
call dotnet clean >nul 2>&1
call dotnet build 2>&1 | findstr /i "error 错误 成功 succeeded" | findstr /v /i "warning"

echo.
echo [2] 启动服务并捕获日志...
echo    注意: 服务将在前台运行，10秒后检查端口状态
echo.

start /b cmd /c "dotnet run --launch-profile https > ..\..\startup_log.txt 2> ..\..\startup_error.txt"

timeout /t 12 /nobreak >nul

echo.
echo [3] 检查端口状态...
netstat -ano | findstr ":5196" | findstr "LISTENING"
if %errorlevel% equ 0 (
    echo [OK] HTTPS 端口 5196 正在监听
) else (
    echo [X] HTTPS 端口 5196 未启动
)

netstat -ano | findstr ":5195" | findstr "LISTENING"
if %errorlevel% equ 0 (
    echo [OK] HTTP 端口 5195 正在监听
) else (
    echo [X] HTTP 端口 5195 未启动
)

echo.
echo [4] 启动日志 (最后30行):
if exist "..\..\startup_log.txt" (
    powershell -Command "Get-Content '..\..\startup_log.txt' -Tail 30 | ForEach-Object { Write-Host $_ }"
) else (
    echo [X] 日志文件未生成
)

echo.
echo [5] 错误日志:
if exist "..\..\startup_error.txt" (
    powershell -Command "$content = Get-Content '..\..\startup_error.txt' -Tail 20; if ($content) { $content | ForEach-Object { Write-Host $_ -ForegroundColor Red } } else { Write-Host '  无错误' }"
) else (
    echo [OK] 无错误日志文件
)

echo.
echo [6] 查找关键错误信息:
if exist "..\..\startup_log.txt" (
    powershell -Command "Get-Content '..\..\startup_log.txt' | Select-String -Pattern 'listening|Listening|绑定|bind|error|Error|异常|Exception|HTTPS|https|5196|5195|failed|Failed' | Select-Object -First 15"
)
if exist "..\..\startup_error.txt" (
    powershell -Command "Get-Content '..\..\startup_error.txt' | Select-String -Pattern 'error|Error|异常|Exception|failed|Failed' | Select-Object -First 10"
)

echo.
echo ========================================
echo   检查完成
echo ========================================
pause



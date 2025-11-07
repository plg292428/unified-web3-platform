@echo off
chcp 65001 >nul
echo ========================================
echo   Debug Run - 查看详细错误信息
echo ========================================
echo.

cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"

echo [Step 1] 编译项目...
dotnet build
if %ERRORLEVEL% NEQ 0 (
    echo   [ERROR] 编译失败，请查看上面的错误信息
    pause
    exit /b 1
)
echo.

echo [Step 2] 检查数据库连接字符串...
findstr /C:"DefaultConnection" appsettings.json
echo.

echo [Step 3] 运行项目（查看详细输出）...
echo   [提示] 这将显示所有启动信息和错误
echo   [提示] 如果启动失败，错误信息会显示在这里
echo   [提示] 按 Ctrl+C 停止
echo.

dotnet run

pause


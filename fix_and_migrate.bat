@echo off
chcp 65001 >nul
echo ========================================
echo   修复端口占用并创建数据库迁移
echo ========================================
echo.

echo [Step 1] 检查并关闭占用端口5195的进程...
for /f "tokens=5" %%a in ('netstat -ano ^| findstr :5195') do (
    set PID=%%a
    echo 发现进程ID: %%a
    taskkill /F /PID %%a >nul 2>&1
    if %ERRORLEVEL% EQU 0 (
        echo   进程已关闭
    ) else (
        echo   无法关闭进程（可能已不存在）
    )
)
echo.

echo [Step 2] 创建数据库迁移...
cd /d "%~dp0src\Backend\UnifiedPlatform.DbService"
dotnet ef migrations add InitialCreate --startup-project ..\UnifiedPlatform.WebApi
if %ERRORLEVEL% NEQ 0 (
    echo   [ERROR] 创建迁移失败
    pause
    exit /b 1
)
echo.

echo [Step 3] 应用迁移到数据库...
dotnet ef database update --startup-project ..\UnifiedPlatform.WebApi
if %ERRORLEVEL% NEQ 0 (
    echo   [ERROR] 应用迁移失败
    pause
    exit /b 1
)
echo.

echo [OK] 数据库迁移完成！
echo.
pause


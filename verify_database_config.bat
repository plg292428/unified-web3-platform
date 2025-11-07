@echo off
chcp 65001 >nul
echo ========================================
echo   Verify Database Configuration
echo ========================================
echo.

cd /d "%~dp0src\Backend\UnifiedPlatform.WebApi"

echo [检查] 读取连接字符串...
findstr /C:"DefaultConnection" appsettings.json
echo.

echo [检查] 数据库名称...
findstr /C:"Initial Catalog" appsettings.json | findstr /C:"SmallTarget" >nul
if %ERRORLEVEL% EQU 0 (
    echo   [OK] 数据库已配置为: SmallTarget
) else (
    echo   [WARN] 数据库名称可能不是SmallTarget
)

echo.
echo [提示] 配置已更新为使用现有SmallTarget数据库
echo   无需创建新数据库或运行迁移
echo   可以直接运行项目
echo.
pause


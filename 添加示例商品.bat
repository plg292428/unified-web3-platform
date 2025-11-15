@echo off
chcp 65001 >nul
title 添加示例商品到数据库

echo.
echo ========================================
echo   添加示例商品到数据库
echo ========================================
echo.

cd /d "%~dp0"

echo [1/2] 检查数据库连接...
echo.

REM 检查 sqlcmd 是否可用
where sqlcmd >nul 2>&1
if %errorlevel% neq 0 (
    echo [错误] 未找到 sqlcmd 工具
    echo.
    echo 请选择以下方式之一：
    echo   1. 安装 SQL Server 命令行工具
    echo   2. 使用 SQL Server Management Studio 执行 添加示例商品.sql
    echo   3. 使用 Visual Studio 的 SQL Server 对象资源管理器
    echo.
    echo SQL 脚本已保存到: 添加示例商品.sql
    echo.
    pause
    exit /b 1
)

echo [2/2] 执行 SQL 脚本添加商品...
echo.

sqlcmd -S localhost -d UnifiedWeb3Platform -U UnifiedWebAppUser -P "P@ssword_2025!" -C -i "添加示例商品.sql"

if %errorlevel% equ 0 (
    echo.
    echo ========================================
    echo   商品添加成功！
    echo ========================================
    echo.
    echo 请刷新前端页面查看商品
    echo 访问: http://localhost:5173
    echo.
) else (
    echo.
    echo [错误] SQL 执行失败
    echo.
    echo 请检查：
    echo   1. 数据库服务是否运行
    echo   2. 数据库连接信息是否正确
    echo   3. 用户权限是否足够
    echo.
    echo 或者手动执行 SQL 脚本: 添加示例商品.sql
    echo.
)

pause


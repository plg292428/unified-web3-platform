@echo off
chcp 65001 >nul
echo ========================================
echo   数据库迁移修复脚本
echo ========================================
echo.

echo [步骤 1] 检查数据库连接...
echo 数据库: UnifiedWeb3Platform
echo 用户: UnifiedWebAppUser
echo.

echo [步骤 2] 执行 SQL 脚本创建新表...
echo 正在执行: 创建新表脚本.sql
echo.

sqlcmd -S localhost -d UnifiedWeb3Platform -U UnifiedWebAppUser -P "P@ssword_2025!" -i "创建新表脚本.sql" -W

if %errorlevel% equ 0 (
    echo.
    echo [OK] SQL 脚本执行成功！
    echo.
    echo [步骤 3] 验证表创建...
    echo.
    sqlcmd -S localhost -d UnifiedWeb3Platform -U UnifiedWebAppUser -P "P@ssword_2025!" -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME IN ('ProductImages', 'ProductReviews', 'ProductSpecifications') ORDER BY TABLE_NAME" -W
    
    echo.
    echo ========================================
    echo   修复完成！
    echo ========================================
    echo.
    echo 下一步操作:
    echo   1. 启动后端服务: .\run_backend.bat
    echo   2. 运行测试: .\快速测试.bat
    echo.
) else (
    echo.
    echo [FAIL] SQL 脚本执行失败！
    echo.
    echo 请检查:
    echo   1. 数据库服务是否运行
    echo   2. 数据库连接信息是否正确
    echo   3. 用户权限是否足够
    echo.
)

pause


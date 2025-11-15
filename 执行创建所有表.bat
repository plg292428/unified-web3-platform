@echo off
chcp 65001 >nul
echo ========================================
echo   创建所有购物表
echo ========================================
echo.

echo [执行] 正在创建所有表...
echo 这可能需要几分钟时间，请耐心等待...
echo.

sqlcmd -S localhost -d UnifiedWeb3Platform -U UnifiedWebAppUser -P "P@ssword_2025!" -i "创建所有购物表.sql" -W

if %errorlevel% equ 0 (
    echo.
    echo [OK] SQL 脚本执行完成！
    echo.
    echo [验证] 检查表创建状态...
    echo.
    sqlcmd -S localhost -d UnifiedWeb3Platform -U UnifiedWebAppUser -P "P@ssword_2025!" -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME IN ('Product', 'ProductCategory', 'Orders', 'OrderItems', 'ShoppingCartItem', 'ProductInventory', 'OrderPaymentLogs', 'WalletUserProfiles', 'ProductImages', 'ProductReviews', 'ProductSpecifications') ORDER BY TABLE_NAME" -W -h -1
    
    echo.
    echo ========================================
    echo   执行完成
    echo ========================================
    echo.
    echo 如果所有表都已创建，可以继续下一步测试
    echo.
) else (
    echo.
    echo [FAIL] SQL 脚本执行失败！
    echo.
    echo 请检查:
    echo   1. 数据库服务是否运行
    echo   2. 数据库连接信息是否正确
    echo   3. 查看上方的错误信息
    echo.
    echo 建议: 在 SSMS 中手动执行 SQL 脚本查看详细错误
    echo 文件: 创建所有购物表.sql
    echo.
)

pause


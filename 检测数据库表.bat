@echo off
chcp 65001 >nul
echo ========================================
echo   数据库表检测脚本
echo ========================================
echo.

echo [检测 1] 检查核心购物表...
sqlcmd -S localhost -d UnifiedWeb3Platform -U UnifiedWebAppUser -P "P@ssword_2025!" -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME IN ('Product', 'ProductCategory', 'Orders', 'OrderItems', 'ShoppingCartItem', 'ProductInventory') ORDER BY TABLE_NAME" -W -h -1

echo.
echo [检测 2] 检查商品管理增强表...
sqlcmd -S localhost -d UnifiedWeb3Platform -U UnifiedWebAppUser -P "P@ssword_2025!" -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME IN ('ProductImages', 'ProductReviews', 'ProductSpecifications') ORDER BY TABLE_NAME" -W -h -1

echo.
echo [检测 3] 检查迁移记录...
sqlcmd -S localhost -d UnifiedWeb3Platform -U UnifiedWebAppUser -P "P@ssword_2025!" -Q "SELECT MigrationId FROM __EFMigrationsHistory WHERE MigrationId LIKE '%Shopping%' OR MigrationId LIKE '%ProductReview%' OR MigrationId LIKE '%ProductImage%' OR MigrationId LIKE '%ProductSpecification%' ORDER BY MigrationId" -W -h -1

echo.
echo [检测 4] 检查表结构...
echo.
echo ProductImages 表列数:
sqlcmd -S localhost -d UnifiedWeb3Platform -U UnifiedWebAppUser -P "P@ssword_2025!" -Q "SELECT COUNT(*) AS ColumnCount FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ProductImages'" -W -h -1

echo.
echo ProductReviews 表列数:
sqlcmd -S localhost -d UnifiedWeb3Platform -U UnifiedWebAppUser -P "P@ssword_2025!" -Q "SELECT COUNT(*) AS ColumnCount FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ProductReviews'" -W -h -1

echo.
echo ProductSpecifications 表列数:
sqlcmd -S localhost -d UnifiedWeb3Platform -U UnifiedWebAppUser -P "P@ssword_2025!" -Q "SELECT COUNT(*) AS ColumnCount FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ProductSpecifications'" -W -h -1

echo.
echo ========================================
echo   检测完成
echo ========================================
echo.
echo 如果表未创建，请执行:
echo   创建新表脚本_简化版.sql
echo.
pause


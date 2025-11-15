-- ============================================
-- 修复迁移冲突：标记已存在的迁移为已应用
-- ============================================

USE UnifiedWeb3Platform;
GO

-- 检查并标记 AddShoppingTables 迁移（如果表已存在）
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Orders')
   AND EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Product')
   AND NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20251111052419_AddShoppingTables')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20251111052419_AddShoppingTables', '8.0.0');
    PRINT 'AddShoppingTables 迁移已标记为已应用';
END
ELSE IF EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20251111052419_AddShoppingTables')
BEGIN
    PRINT 'AddShoppingTables 迁移记录已存在';
END
ELSE
BEGIN
    PRINT 'AddShoppingTables 迁移未标记（表可能未完全创建）';
END
GO

-- 检查并标记 AddProductReviewImageSpecificationOnly 迁移（如果表已存在）
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ProductImages')
   AND EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ProductReviews')
   AND EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ProductSpecifications')
   AND NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20251112073154_AddProductReviewImageSpecificationOnly')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20251112073154_AddProductReviewImageSpecificationOnly', '8.0.0');
    PRINT 'AddProductReviewImageSpecificationOnly 迁移已标记为已应用';
END
ELSE IF EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20251112073154_AddProductReviewImageSpecificationOnly')
BEGIN
    PRINT 'AddProductReviewImageSpecificationOnly 迁移记录已存在';
END
ELSE
BEGIN
    PRINT 'AddProductReviewImageSpecificationOnly 迁移未标记（表可能未完全创建）';
END
GO

-- 显示当前迁移记录
SELECT MigrationId, ProductVersion 
FROM __EFMigrationsHistory 
ORDER BY MigrationId;
GO

-- 显示购物相关表
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN (
    'Product', 'ProductCategory', 'Orders', 'OrderItems', 
    'ShoppingCartItem', 'ProductInventory', 'OrderPaymentLogs',
    'ProductImages', 'ProductReviews', 'ProductSpecifications'
)
ORDER BY TABLE_NAME;
GO

PRINT '========================================';
PRINT '修复完成！';
PRINT '========================================';
GO


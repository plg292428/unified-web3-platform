-- 直接添加商品的 SQL 脚本（简化版，避免编码问题）
USE UnifiedWeb3Platform;
GO

-- 清空现有测试数据（可选）
-- DELETE FROM ProductInventory WHERE ProductId IN (1,2,3,4,5,6);
-- DELETE FROM Product WHERE ProductId IN (1,2,3,4,5,6);
-- DELETE FROM ProductCategory WHERE CategoryId IN (1,2,3);
-- GO

-- 添加分类
IF NOT EXISTS (SELECT 1 FROM ProductCategory WHERE CategoryId = 1)
BEGIN
    SET IDENTITY_INSERT ProductCategory ON;
    INSERT INTO ProductCategory (CategoryId, Name, Slug, Description, ParentCategoryId, SortOrder, IsActive, CreateTime, UpdateTime)
    VALUES 
    (1, '硬件设备', 'hardware', '官方认证的 Web3 设备', NULL, 1, 1, GETUTCDATE(), GETUTCDATE()),
    (2, '数字藏品', 'digital-collectibles', '限量发行的 NFT 藏品', NULL, 2, 1, GETUTCDATE(), GETUTCDATE()),
    (3, '增值服务', 'value-services', '链上增值服务', NULL, 3, 1, GETUTCDATE(), GETUTCDATE());
    SET IDENTITY_INSERT ProductCategory OFF;
    PRINT 'Categories added';
END
GO

-- 添加商品
IF NOT EXISTS (SELECT 1 FROM Product WHERE ProductId = 1)
BEGIN
    DECLARE @Now DATETIME = GETUTCDATE();
    
    SET IDENTITY_INSERT Product ON;
    INSERT INTO Product (ProductId, CategoryId, Name, Subtitle, Description, ThumbnailUrl, Price, Currency, ChainId, Sku, IsPublished, CreateTime, UpdateTime)
    VALUES 
    (1, 1, 'Bitget Web3 Vault', '冷存储签名器', '专为 Bitget 用户打造的硬件 Vault，提供安全的冷存储解决方案', NULL, 399.00, 'USDT', 1, 'BG-VLT-001', 1, @Now, @Now),
    (2, 2, 'Bitget Genesis NFT', '限量 Genesis 系列', 'Bitget 生态限量 Genesis NFT，具有独特的收藏价值', NULL, 129.00, 'USDT', 137, 'BG-NFT-001', 1, @Now, @Now),
    (3, 3, '链上风控加速', '链上风控增强服务', '快速提升企业链上风控能力，保障资产安全', NULL, 259.00, 'USDT', 56, 'BG-SRV-001', 1, @Now, @Now),
    (4, 1, 'Web3 安全钱包', '多链支持硬件钱包', '支持 Ethereum、Polygon、BSC 等多条链的硬件钱包', NULL, 299.00, 'USDT', 1, 'BG-WLT-002', 1, @Now, @Now),
    (5, 2, 'Bitget 纪念 NFT', '平台纪念系列', 'Bitget 平台纪念 NFT，限量发行 1000 枚', NULL, 89.00, 'USDT', 137, 'BG-NFT-002', 1, @Now, @Now),
    (6, 3, '智能合约审计服务', '专业安全审计', '提供专业的智能合约安全审计服务，保障合约安全', NULL, 599.00, 'USDT', 1, 'BG-SRV-002', 1, @Now, @Now);
    SET IDENTITY_INSERT Product OFF;
    PRINT 'Products added';
    
    -- 添加库存
    SET IDENTITY_INSERT ProductInventory ON;
    INSERT INTO ProductInventory (InventoryId, ProductId, QuantityAvailable, QuantityReserved, UpdateTime)
    VALUES 
    (1, 1, 25, 0, @Now),
    (2, 2, 80, 0, @Now),
    (3, 3, 120, 0, @Now),
    (4, 4, 50, 0, @Now),
    (5, 5, 1000, 0, @Now),
    (6, 6, 10, 0, @Now);
    SET IDENTITY_INSERT ProductInventory OFF;
    PRINT 'Inventory added';
END
ELSE
BEGIN
    PRINT 'Products already exist';
END
GO

-- 验证
SELECT COUNT(*) AS ProductCount FROM Product WHERE IsPublished = 1;
SELECT COUNT(*) AS CategoryCount FROM ProductCategory WHERE IsActive = 1;
GO


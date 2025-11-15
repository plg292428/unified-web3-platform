-- 添加示例商品到数据库的 SQL 脚本
-- 使用方法：在 SQL Server Management Studio 中执行，或使用 sqlcmd 命令行工具

USE UnifiedWeb3Platform;
GO

-- 检查并添加分类
IF NOT EXISTS (SELECT 1 FROM ProductCategory WHERE CategoryId = 1)
BEGIN
    INSERT INTO ProductCategory (CategoryId, Name, Slug, Description, ParentCategoryId, SortOrder, IsActive, CreateTime, UpdateTime)
    VALUES 
    (1, '硬件设备', 'hardware', '官方认证的 Web3 设备', NULL, 1, 1, GETUTCDATE(), GETUTCDATE()),
    (2, '数字藏品', 'digital-collectibles', '限量发行的 NFT 藏品', NULL, 2, 1, GETUTCDATE(), GETUTCDATE()),
    (3, '增值服务', 'value-services', '链上增值服务', NULL, 3, 1, GETUTCDATE(), GETUTCDATE());
    
    PRINT '分类已添加';
END
ELSE
BEGIN
    PRINT '分类已存在，跳过';
END
GO

-- 检查并添加商品
IF NOT EXISTS (SELECT 1 FROM Product WHERE ProductId = 1)
BEGIN
    DECLARE @Now DATETIME = GETUTCDATE();
    
    INSERT INTO Product (CategoryId, Name, Subtitle, Description, ThumbnailUrl, Price, Currency, ChainId, Sku, IsPublished, CreateTime, UpdateTime)
    VALUES 
    (1, 'Bitget Web3 Vault', '冷存储签名器', '专为 Bitget 用户打造的硬件 Vault，提供安全的冷存储解决方案', NULL, 399.00, 'USDT', 1, 'BG-VLT-001', 1, @Now, @Now),
    (2, 'Bitget Genesis NFT', '限量 Genesis 系列', 'Bitget 生态限量 Genesis NFT，具有独特的收藏价值', NULL, 129.00, 'USDT', 137, 'BG-NFT-001', 1, @Now, @Now),
    (3, '链上风控加速', '链上风控增强服务', '快速提升企业链上风控能力，保障资产安全', NULL, 259.00, 'USDT', 56, 'BG-SRV-001', 1, @Now, @Now),
    (1, 'Web3 安全钱包', '多链支持硬件钱包', '支持 Ethereum、Polygon、BSC 等多条链的硬件钱包', NULL, 299.00, 'USDT', 1, 'BG-WLT-002', 1, @Now, @Now),
    (2, 'Bitget 纪念 NFT', '平台纪念系列', 'Bitget 平台纪念 NFT，限量发行 1000 枚', NULL, 89.00, 'USDT', 137, 'BG-NFT-002', 1, @Now, @Now),
    (3, '智能合约审计服务', '专业安全审计', '提供专业的智能合约安全审计服务，保障合约安全', NULL, 599.00, 'USDT', 1, 'BG-SRV-002', 1, @Now, @Now);
    
    PRINT '商品已添加';
    
    -- 添加库存
    INSERT INTO ProductInventory (ProductId, QuantityAvailable, QuantityReserved, UpdateTime)
    SELECT ProductId, 
           CASE ProductId 
               WHEN 1 THEN 25
               WHEN 2 THEN 80
               WHEN 3 THEN 120
               WHEN 4 THEN 50
               WHEN 5 THEN 1000
               WHEN 6 THEN 10
           END,
           0,
           @Now
    FROM Product
    WHERE ProductId BETWEEN 1 AND 6;
    
    PRINT '库存已添加';
END
ELSE
BEGIN
    PRINT '商品已存在，跳过';
END
GO

-- 显示结果
PRINT '商品添加完成！';
SELECT COUNT(*) AS PublishedProductCount FROM Product WHERE IsPublished = 1;
SELECT COUNT(*) AS ActiveCategoryCount FROM ProductCategory WHERE IsActive = 1;
GO


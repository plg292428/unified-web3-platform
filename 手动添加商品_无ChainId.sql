-- 手动添加商品的 SQL 脚本（不指定 ChainId，避免外键约束问题）
USE UnifiedWeb3Platform;
GO

-- 检查分类是否存在
IF EXISTS (SELECT 1 FROM ProductCategory WHERE CategoryId = 1)
BEGIN
    PRINT '分类已存在';
    
    -- 检查是否已有商品
    IF NOT EXISTS (SELECT 1 FROM Product WHERE ProductId = 1)
    BEGIN
        DECLARE @Now DATETIME = GETUTCDATE();
        DECLARE @CategoryHardware INT = (SELECT CategoryId FROM ProductCategory WHERE Slug = 'hardware');
        DECLARE @CategoryDigital INT = (SELECT CategoryId FROM ProductCategory WHERE Slug = 'digital-collectibles');
        DECLARE @CategoryService INT = (SELECT CategoryId FROM ProductCategory WHERE Slug = 'value-services');
        
        IF @CategoryHardware IS NOT NULL AND @CategoryDigital IS NOT NULL AND @CategoryService IS NOT NULL
        BEGIN
            -- 添加商品（不指定 ChainId，设为 NULL，避免外键约束问题）
            INSERT INTO Product (CategoryId, Name, Subtitle, Description, ThumbnailUrl, Price, Currency, ChainId, Sku, IsPublished, CreateTime, UpdateTime)
            VALUES 
            (@CategoryHardware, 'Bitget Web3 Vault', '冷存储签名器', '专为 Bitget 用户打造的硬件 Vault，提供安全的冷存储解决方案', NULL, 399.00, 'USDT', NULL, 'BG-VLT-001', 1, @Now, @Now),
            (@CategoryDigital, 'Bitget Genesis NFT', '限量 Genesis 系列', 'Bitget 生态限量 Genesis NFT，具有独特的收藏价值', NULL, 129.00, 'USDT', NULL, 'BG-NFT-001', 1, @Now, @Now),
            (@CategoryService, '链上风控加速', '链上风控增强服务', '快速提升企业链上风控能力，保障资产安全', NULL, 259.00, 'USDT', NULL, 'BG-SRV-001', 1, @Now, @Now),
            (@CategoryHardware, 'Web3 安全钱包', '多链支持硬件钱包', '支持 Ethereum、Polygon、BSC 等多条链的硬件钱包', NULL, 299.00, 'USDT', NULL, 'BG-WLT-002', 1, @Now, @Now),
            (@CategoryDigital, 'Bitget 纪念 NFT', '平台纪念系列', 'Bitget 平台纪念 NFT，限量发行 1000 枚', NULL, 89.00, 'USDT', NULL, 'BG-NFT-002', 1, @Now, @Now),
            (@CategoryService, '智能合约审计服务', '专业安全审计', '提供专业的智能合约安全审计服务，保障合约安全', NULL, 599.00, 'USDT', NULL, 'BG-SRV-002', 1, @Now, @Now);
            
            PRINT '商品已添加';
            
            -- 添加库存
            INSERT INTO ProductInventory (ProductId, QuantityAvailable, QuantityReserved, UpdateTime)
            SELECT ProductId, 
                   CASE 
                       WHEN Sku = 'BG-VLT-001' THEN 25
                       WHEN Sku = 'BG-NFT-001' THEN 80
                       WHEN Sku = 'BG-SRV-001' THEN 120
                       WHEN Sku = 'BG-WLT-002' THEN 50
                       WHEN Sku = 'BG-NFT-002' THEN 1000
                       WHEN Sku = 'BG-SRV-002' THEN 10
                   END,
                   0,
                   @Now
            FROM Product
            WHERE ProductId IN (SELECT TOP 6 ProductId FROM Product ORDER BY ProductId DESC);
            
            PRINT '库存已添加';
        END
        ELSE
        BEGIN
            PRINT '错误: 无法找到所有分类';
        END
    END
    ELSE
    BEGIN
        PRINT '商品已存在，跳过';
    END
END
ELSE
BEGIN
    PRINT '错误: 分类不存在，请先添加分类';
END
GO

-- 显示结果
SELECT COUNT(*) AS ProductCount FROM Product WHERE IsPublished = 1;
SELECT 
    p.ProductId,
    p.Name,
    p.Price,
    p.Currency,
    c.Name AS CategoryName,
    i.QuantityAvailable AS Stock
FROM Product p
LEFT JOIN ProductCategory c ON p.CategoryId = c.CategoryId
LEFT JOIN ProductInventory i ON p.ProductId = i.ProductId
WHERE p.IsPublished = 1
ORDER BY p.ProductId;
GO


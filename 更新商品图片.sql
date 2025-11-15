-- 更新商品图片的 SQL 脚本
USE UnifiedWeb3Platform;
GO

-- 使用占位图片服务（placeholder.com）或使用实际图片 URL
-- 这里使用 placeholder.com 作为示例，您可以根据需要替换为实际图片 URL

DECLARE @Now DATETIME = GETUTCDATE();

-- 更新商品缩略图
UPDATE Product 
SET ThumbnailUrl = 'https://via.placeholder.com/400x400/4A90E2/FFFFFF?text=Bitget+Web3+Vault',
    UpdateTime = @Now
WHERE Sku = 'BG-VLT-001';

UPDATE Product 
SET ThumbnailUrl = 'https://via.placeholder.com/400x400/9B59B6/FFFFFF?text=Bitget+Genesis+NFT',
    UpdateTime = @Now
WHERE Sku = 'BG-NFT-001';

UPDATE Product 
SET ThumbnailUrl = 'https://via.placeholder.com/400x400/E74C3C/FFFFFF?text=链上风控加速',
    UpdateTime = @Now
WHERE Sku = 'BG-SRV-001';

UPDATE Product 
SET ThumbnailUrl = 'https://via.placeholder.com/400x400/27AE60/FFFFFF?text=Web3+安全钱包',
    UpdateTime = @Now
WHERE Sku = 'BG-WLT-002';

UPDATE Product 
SET ThumbnailUrl = 'https://via.placeholder.com/400x400/F39C12/FFFFFF?text=Bitget+纪念+NFT',
    UpdateTime = @Now
WHERE Sku = 'BG-NFT-002';

UPDATE Product 
SET ThumbnailUrl = 'https://via.placeholder.com/400x400/3498DB/FFFFFF?text=智能合约审计',
    UpdateTime = @Now
WHERE Sku = 'BG-SRV-002';

PRINT '商品图片已更新';

-- 显示更新结果
SELECT 
    ProductId,
    Name,
    ThumbnailUrl,
    Sku
FROM Product
WHERE IsPublished = 1
ORDER BY ProductId;
GO


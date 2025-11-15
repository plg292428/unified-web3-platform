-- 更新商品图片的 SQL 脚本（使用 Unsplash 免费图片服务）
USE UnifiedWeb3Platform;
GO

DECLARE @Now DATETIME = GETUTCDATE();

-- 使用 Unsplash 的随机图片服务，每个商品使用不同的图片
-- 格式: https://source.unsplash.com/400x400/?关键词

UPDATE Product 
SET ThumbnailUrl = 'https://images.unsplash.com/photo-1639762681485-074b7f938ba0?w=400&h=400&fit=crop',
    UpdateTime = @Now
WHERE Sku = 'BG-VLT-001';

UPDATE Product 
SET ThumbnailUrl = 'https://images.unsplash.com/photo-1639322537504-6427a16b0a28?w=400&h=400&fit=crop',
    UpdateTime = @Now
WHERE Sku = 'BG-NFT-001';

UPDATE Product 
SET ThumbnailUrl = 'https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?w=400&h=400&fit=crop',
    UpdateTime = @Now
WHERE Sku = 'BG-SRV-001';

UPDATE Product 
SET ThumbnailUrl = 'https://images.unsplash.com/photo-1518546305927-5a555bb7020d?w=400&h=400&fit=crop',
    UpdateTime = @Now
WHERE Sku = 'BG-WLT-002';

UPDATE Product 
SET ThumbnailUrl = 'https://images.unsplash.com/photo-1620321023374-d1a68fbc720d?w=400&h=400&fit=crop',
    UpdateTime = @Now
WHERE Sku = 'BG-NFT-002';

UPDATE Product 
SET ThumbnailUrl = 'https://images.unsplash.com/photo-1558494949-ef010cbdcc31?w=400&h=400&fit=crop',
    UpdateTime = @Now
WHERE Sku = 'BG-SRV-002';

PRINT '商品图片已更新为 Unsplash 图片';

-- 显示更新结果
SELECT 
    ProductId,
    Name,
    LEFT(ThumbnailUrl, 80) AS ThumbnailUrl,
    Sku
FROM Product
WHERE IsPublished = 1
ORDER BY ProductId;
GO


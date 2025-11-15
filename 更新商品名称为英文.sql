USE UnifiedWeb3Platform;
GO

-- 更新商品分类名称为英文
UPDATE ProductCategory SET Name = 'Hardware Devices', Description = 'Official certified Web3 devices' WHERE Slug = 'hardware';
UPDATE ProductCategory SET Name = 'Digital Collectibles', Description = 'Limited edition NFT collectibles' WHERE Slug = 'digital-collectibles';
UPDATE ProductCategory SET Name = 'Value-Added Services', Description = 'On-chain value-added services' WHERE Slug = 'value-services';
GO

-- 更新商品名称为英文
UPDATE Product SET 
    Name = 'Bitget Web3 Vault',
    Subtitle = 'Cold Storage Signer',
    Description = 'Hardware Vault designed for Bitget users, providing secure cold storage solutions'
WHERE Sku = 'BG-VLT-001';
GO

UPDATE Product SET 
    Name = 'Bitget Genesis NFT',
    Subtitle = 'Limited Genesis Series',
    Description = 'Limited edition Genesis NFT from Bitget ecosystem with unique collectible value'
WHERE Sku = 'BG-NFT-001';
GO

UPDATE Product SET 
    Name = 'On-Chain Risk Control Acceleration',
    Subtitle = 'Enhanced Risk Control Service',
    Description = 'Rapidly improve enterprise on-chain risk control capabilities and ensure asset security'
WHERE Sku = 'BG-SRV-001';
GO

UPDATE Product SET 
    Name = 'Web3 Security Wallet',
    Subtitle = 'Multi-Chain Hardware Wallet',
    Description = 'Hardware wallet supporting multiple chains including Ethereum, Polygon, BSC, and more'
WHERE Sku = 'BG-WLT-002';
GO

UPDATE Product SET 
    Name = 'Bitget Commemorative NFT',
    Subtitle = 'Platform Commemorative Series',
    Description = 'Bitget platform commemorative NFT, limited edition of 1000 pieces'
WHERE Sku = 'BG-NFT-002';
GO

UPDATE Product SET 
    Name = 'Smart Contract Audit Service',
    Subtitle = 'Professional Security Audit',
    Description = 'Professional smart contract security audit services to ensure contract safety'
WHERE Sku = 'BG-SRV-002';
GO

PRINT 'Product names and categories have been updated to English';

-- 验证更新结果
SELECT ProductId, Name, Subtitle, Sku FROM Product WHERE IsPublished = 1 ORDER BY ProductId;
SELECT CategoryId, Name, Description, Slug FROM ProductCategory WHERE IsActive = 1 ORDER BY CategoryId;
GO


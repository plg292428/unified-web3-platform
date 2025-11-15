-- ============================================
-- 创建所有购物相关表
-- 包括核心购物表和商品管理增强表
-- ============================================

USE UnifiedWeb3Platform;
GO

-- ============================================
-- 1. 创建 ProductCategory 表（商品分类）
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductCategory]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProductCategory] (
        [CategoryId] int IDENTITY(1,1) NOT NULL,
        [Name] nvarchar(64) NOT NULL,
        [Slug] nvarchar(128) NULL,
        [Description] nvarchar(512) NULL,
        [ParentCategoryId] int NULL,
        [SortOrder] int NOT NULL DEFAULT 0,
        [IsActive] bit NOT NULL DEFAULT 1,
        [CreateTime] datetime NOT NULL DEFAULT (getutcdate()),
        [UpdateTime] datetime NOT NULL DEFAULT (getutcdate()),
        CONSTRAINT [PK_ProductCategory_CategoryId] PRIMARY KEY CLUSTERED ([CategoryId] ASC),
        CONSTRAINT [FK_ProductCategory_ProductCategory] FOREIGN KEY ([ParentCategoryId]) 
            REFERENCES [dbo].[ProductCategory] ([CategoryId])
    );
    
    CREATE NONCLUSTERED INDEX [IX_ProductCategory_ParentCategoryId] 
        ON [dbo].[ProductCategory] ([ParentCategoryId] ASC);
    CREATE UNIQUE NONCLUSTERED INDEX [IX_ProductCategory_Slug] 
        ON [dbo].[ProductCategory] ([Slug] ASC) WHERE [Slug] IS NOT NULL;
    
    PRINT 'ProductCategory 表创建成功';
END
ELSE
BEGIN
    PRINT 'ProductCategory 表已存在';
END
GO

-- ============================================
-- 2. 创建 Product 表（商品）
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Product]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Product] (
        [ProductId] bigint IDENTITY(1,1) NOT NULL,
        [CategoryId] int NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Subtitle] nvarchar(256) NULL,
        [Description] nvarchar(max) NULL,
        [ThumbnailUrl] nvarchar(256) NULL,
        [Price] decimal(19,6) NOT NULL,
        [Currency] nvarchar(16) NOT NULL DEFAULT 'USDT',
        [ChainId] int NULL,
        [Sku] nvarchar(64) NULL,
        [IsPublished] bit NOT NULL,
        [CreateTime] datetime NOT NULL DEFAULT (getutcdate()),
        [UpdateTime] datetime NOT NULL DEFAULT (getutcdate()),
        CONSTRAINT [PK_Product_ProductId] PRIMARY KEY CLUSTERED ([ProductId] ASC),
        CONSTRAINT [FK_Product_ProductCategory] FOREIGN KEY ([CategoryId]) 
            REFERENCES [dbo].[ProductCategory] ([CategoryId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Product_ChainNetworkConfig] FOREIGN KEY ([ChainId]) 
            REFERENCES [dbo].[ChainNetworkConfig] ([ChainId])
    );
    
    CREATE NONCLUSTERED INDEX [IX_Product_CategoryId] 
        ON [dbo].[Product] ([CategoryId] ASC);
    CREATE NONCLUSTERED INDEX [IX_Product_ChainId] 
        ON [dbo].[Product] ([ChainId] ASC);
    
    PRINT 'Product 表创建成功';
END
ELSE
BEGIN
    PRINT 'Product 表已存在';
END
GO

-- ============================================
-- 3. 创建 ProductInventory 表（商品库存）
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductInventory]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProductInventory] (
        [InventoryId] bigint IDENTITY(1,1) NOT NULL,
        [ProductId] bigint NOT NULL,
        [QuantityAvailable] int NOT NULL DEFAULT 0,
        [QuantityReserved] int NOT NULL DEFAULT 0,
        [UpdateTime] datetime NOT NULL DEFAULT (getutcdate()),
        CONSTRAINT [PK_ProductInventory_InventoryId] PRIMARY KEY CLUSTERED ([InventoryId] ASC),
        CONSTRAINT [FK_ProductInventory_Product] FOREIGN KEY ([ProductId]) 
            REFERENCES [dbo].[Product] ([ProductId]) ON DELETE CASCADE
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX [IX_ProductInventory_ProductId] 
        ON [dbo].[ProductInventory] ([ProductId] ASC);
    
    PRINT 'ProductInventory 表创建成功';
END
ELSE
BEGIN
    PRINT 'ProductInventory 表已存在';
END
GO

-- ============================================
-- 4. 创建 Orders 表（订单）
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Orders] (
        [OrderId] bigint IDENTITY(1,1) NOT NULL,
        [OrderNumber] nvarchar(32) NOT NULL,
        [Uid] int NOT NULL,
        [TotalAmount] decimal(19,6) NOT NULL,
        [Currency] nvarchar(16) NOT NULL DEFAULT 'USDT',
        [Status] int NOT NULL,
        [PaymentMode] int NOT NULL DEFAULT 0,
        [PaymentStatus] int NOT NULL DEFAULT 0,
        [PaymentMethod] nvarchar(32) NULL,
        [PaymentProviderType] nvarchar(64) NULL,
        [PaymentProviderName] nvarchar(64) NULL,
        [PaymentWalletAddress] nvarchar(128) NULL,
        [PaymentWalletLabel] nvarchar(64) NULL,
        [ChainId] int NULL,
        [PaymentTransactionHash] nvarchar(128) NULL,
        [PaymentConfirmations] int NULL,
        [PaymentSubmittedTime] datetime NULL,
        [PaymentConfirmedTime] datetime NULL,
        [PaymentSignaturePayload] nvarchar(max) NULL,
        [PaymentSignatureResult] nvarchar(max) NULL,
        [PaymentFailureReason] nvarchar(512) NULL,
        [PaymentExpiresAt] datetime NULL,
        [CreateTime] datetime NOT NULL DEFAULT (getutcdate()),
        [PaidTime] datetime NULL,
        [CancelTime] datetime NULL,
        [CompleteTime] datetime NULL,
        [Remark] nvarchar(512) NULL,
        CONSTRAINT [PK_Order_OrderId] PRIMARY KEY CLUSTERED ([OrderId] ASC),
        CONSTRAINT [FK_Orders_User] FOREIGN KEY ([Uid]) 
            REFERENCES [dbo].[User] ([Uid]) ON DELETE CASCADE,
        CONSTRAINT [FK_Orders_ChainNetworkConfig] FOREIGN KEY ([ChainId]) 
            REFERENCES [dbo].[ChainNetworkConfig] ([ChainId])
    );
    
    CREATE NONCLUSTERED INDEX [IX_Orders_Uid] 
        ON [dbo].[Orders] ([Uid] ASC);
    CREATE NONCLUSTERED INDEX [IX_Orders_ChainId] 
        ON [dbo].[Orders] ([ChainId] ASC);
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Orders_OrderNumber] 
        ON [dbo].[Orders] ([OrderNumber] ASC);
    
    PRINT 'Orders 表创建成功';
END
ELSE
BEGIN
    PRINT 'Orders 表已存在';
END
GO

-- ============================================
-- 5. 创建 OrderItems 表（订单商品）
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderItems]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[OrderItems] (
        [OrderItemId] bigint IDENTITY(1,1) NOT NULL,
        [OrderId] bigint NOT NULL,
        [ProductId] bigint NOT NULL,
        [ProductName] nvarchar(128) NOT NULL,
        [UnitPrice] decimal(19,6) NOT NULL,
        [Quantity] int NOT NULL,
        [Subtotal] decimal(19,6) NOT NULL,
        CONSTRAINT [PK_OrderItem_OrderItemId] PRIMARY KEY CLUSTERED ([OrderItemId] ASC),
        CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) 
            REFERENCES [dbo].[Orders] ([OrderId]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrderItems_Product_ProductId] FOREIGN KEY ([ProductId]) 
            REFERENCES [dbo].[Product] ([ProductId]) ON DELETE CASCADE
    );
    
    CREATE NONCLUSTERED INDEX [IX_OrderItems_OrderId] 
        ON [dbo].[OrderItems] ([OrderId] ASC);
    CREATE NONCLUSTERED INDEX [IX_OrderItems_ProductId] 
        ON [dbo].[OrderItems] ([ProductId] ASC);
    
    PRINT 'OrderItems 表创建成功';
END
ELSE
BEGIN
    PRINT 'OrderItems 表已存在';
END
GO

-- ============================================
-- 6. 创建 ShoppingCartItem 表（购物车）
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ShoppingCartItem]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ShoppingCartItem] (
        [CartItemId] bigint IDENTITY(1,1) NOT NULL,
        [Uid] int NOT NULL,
        [ProductId] bigint NOT NULL,
        [Quantity] int NOT NULL DEFAULT 1,
        [CreateTime] datetime NOT NULL DEFAULT (getutcdate()),
        [UpdateTime] datetime NOT NULL DEFAULT (getutcdate()),
        CONSTRAINT [PK_ShoppingCartItem_CartItemId] PRIMARY KEY CLUSTERED ([CartItemId] ASC),
        CONSTRAINT [FK_ShoppingCartItem_User] FOREIGN KEY ([Uid]) 
            REFERENCES [dbo].[User] ([Uid]) ON DELETE CASCADE,
        CONSTRAINT [FK_ShoppingCartItem_Product] FOREIGN KEY ([ProductId]) 
            REFERENCES [dbo].[Product] ([ProductId]) ON DELETE CASCADE
    );
    
    CREATE NONCLUSTERED INDEX [IX_ShoppingCartItem_ProductId] 
        ON [dbo].[ShoppingCartItem] ([ProductId] ASC);
    CREATE UNIQUE NONCLUSTERED INDEX [IX_ShoppingCartItem_Uid_ProductId] 
        ON [dbo].[ShoppingCartItem] ([Uid] ASC, [ProductId] ASC);
    
    PRINT 'ShoppingCartItem 表创建成功';
END
ELSE
BEGIN
    PRINT 'ShoppingCartItem 表已存在';
END
GO

-- ============================================
-- 7. 创建 OrderPaymentLogs 表（订单支付日志）
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderPaymentLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[OrderPaymentLogs] (
        [OrderPaymentLogId] bigint IDENTITY(1,1) NOT NULL,
        [OrderId] bigint NOT NULL,
        [PaymentStatus] int NOT NULL,
        [EventType] nvarchar(64) NOT NULL DEFAULT 'status_change',
        [Message] nvarchar(512) NULL,
        [RawData] nvarchar(max) NULL,
        [CreateTime] datetime NOT NULL DEFAULT (getutcdate()),
        CONSTRAINT [PK_OrderPaymentLog_OrderPaymentLogId] PRIMARY KEY CLUSTERED ([OrderPaymentLogId] ASC),
        CONSTRAINT [FK_OrderPaymentLog_Order] FOREIGN KEY ([OrderId]) 
            REFERENCES [dbo].[Orders] ([OrderId]) ON DELETE CASCADE
    );
    
    CREATE NONCLUSTERED INDEX [IX_OrderPaymentLogs_OrderId] 
        ON [dbo].[OrderPaymentLogs] ([OrderId] ASC);
    
    PRINT 'OrderPaymentLogs 表创建成功';
END
ELSE
BEGIN
    PRINT 'OrderPaymentLogs 表已存在';
END
GO

-- ============================================
-- 8. 创建 WalletUserProfiles 表（钱包用户档案）
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WalletUserProfiles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[WalletUserProfiles] (
        [WalletUserProfileId] bigint IDENTITY(1,1) NOT NULL,
        [Uid] int NOT NULL,
        [ProviderType] nvarchar(64) NOT NULL,
        [ProviderName] nvarchar(64) NULL,
        [WalletAddress] nvarchar(128) NOT NULL,
        [AddressLabel] nvarchar(128) NULL,
        [PreferredChainId] int NULL,
        [CreateTime] datetime NOT NULL DEFAULT (getutcdate()),
        [LastUsedTime] datetime NULL,
        [IsPrimary] bit NOT NULL DEFAULT 0,
        CONSTRAINT [PK_WalletUserProfile_WalletUserProfileId] PRIMARY KEY CLUSTERED ([WalletUserProfileId] ASC),
        CONSTRAINT [FK_WalletUserProfile_User] FOREIGN KEY ([Uid]) 
            REFERENCES [dbo].[User] ([Uid]) ON DELETE CASCADE
    );
    
    CREATE NONCLUSTERED INDEX [IX_WalletUserProfiles_Uid] 
        ON [dbo].[WalletUserProfiles] ([Uid] ASC);
    CREATE UNIQUE NONCLUSTERED INDEX [IX_WalletUserProfiles_Uid_ProviderType_WalletAddress] 
        ON [dbo].[WalletUserProfiles] ([Uid] ASC, [ProviderType] ASC, [WalletAddress] ASC);
    
    PRINT 'WalletUserProfiles 表创建成功';
END
ELSE
BEGIN
    PRINT 'WalletUserProfiles 表已存在';
END
GO

-- ============================================
-- 9. 创建 ProductImages 表（商品图片）
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductImages]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProductImages] (
        [ImageId] bigint IDENTITY(1,1) NOT NULL,
        [ProductId] bigint NOT NULL,
        [ImageUrl] nvarchar(512) NOT NULL,
        [ImageType] nvarchar(32) NOT NULL DEFAULT 'gallery',
        [SortOrder] int NOT NULL DEFAULT 0,
        [IsPrimary] bit NOT NULL DEFAULT 0,
        [CreateTime] datetime NOT NULL DEFAULT (getutcdate()),
        CONSTRAINT [PK_ProductImage_ImageId] PRIMARY KEY CLUSTERED ([ImageId] ASC),
        CONSTRAINT [FK_ProductImage_Product] FOREIGN KEY ([ProductId]) 
            REFERENCES [dbo].[Product] ([ProductId]) ON DELETE CASCADE
    );
    
    CREATE NONCLUSTERED INDEX [IX_ProductImages_ProductId] 
        ON [dbo].[ProductImages] ([ProductId] ASC);
    
    PRINT 'ProductImages 表创建成功';
END
ELSE
BEGIN
    PRINT 'ProductImages 表已存在';
END
GO

-- ============================================
-- 10. 创建 ProductReviews 表（商品评价）
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductReviews]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProductReviews] (
        [ReviewId] bigint IDENTITY(1,1) NOT NULL,
        [ProductId] bigint NOT NULL,
        [Uid] int NOT NULL,
        [OrderId] bigint NULL,
        [Rating] int NOT NULL,
        [Content] nvarchar(2000) NULL,
        [IsApproved] bit NOT NULL DEFAULT 0,
        [IsVisible] bit NOT NULL DEFAULT 1,
        [CreateTime] datetime NOT NULL DEFAULT (getutcdate()),
        [UpdateTime] datetime NOT NULL DEFAULT (getutcdate()),
        CONSTRAINT [PK_ProductReview_ReviewId] PRIMARY KEY CLUSTERED ([ReviewId] ASC),
        CONSTRAINT [FK_ProductReview_Product] FOREIGN KEY ([ProductId]) 
            REFERENCES [dbo].[Product] ([ProductId]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProductReview_User] FOREIGN KEY ([Uid]) 
            REFERENCES [dbo].[User] ([Uid]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProductReview_Order] FOREIGN KEY ([OrderId]) 
            REFERENCES [dbo].[Orders] ([OrderId])
    );
    
    CREATE NONCLUSTERED INDEX [IX_ProductReviews_ProductId] 
        ON [dbo].[ProductReviews] ([ProductId] ASC);
    CREATE NONCLUSTERED INDEX [IX_ProductReviews_Uid] 
        ON [dbo].[ProductReviews] ([Uid] ASC);
    CREATE NONCLUSTERED INDEX [IX_ProductReviews_OrderId] 
        ON [dbo].[ProductReviews] ([OrderId] ASC);
    
    PRINT 'ProductReviews 表创建成功';
END
ELSE
BEGIN
    PRINT 'ProductReviews 表已存在';
END
GO

-- ============================================
-- 11. 创建 ProductSpecifications 表（商品规格）
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductSpecifications]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProductSpecifications] (
        [SpecificationId] bigint IDENTITY(1,1) NOT NULL,
        [ProductId] bigint NOT NULL,
        [SpecificationName] nvarchar(64) NOT NULL,
        [SpecificationValue] nvarchar(128) NOT NULL,
        [PriceAdjustment] decimal(19,6) NOT NULL DEFAULT 0,
        [StockQuantity] int NULL,
        [SortOrder] int NOT NULL DEFAULT 0,
        [IsEnabled] bit NOT NULL DEFAULT 1,
        [CreateTime] datetime NOT NULL DEFAULT (getutcdate()),
        [UpdateTime] datetime NOT NULL DEFAULT (getutcdate()),
        CONSTRAINT [PK_ProductSpecification_SpecificationId] PRIMARY KEY CLUSTERED ([SpecificationId] ASC),
        CONSTRAINT [FK_ProductSpecification_Product] FOREIGN KEY ([ProductId]) 
            REFERENCES [dbo].[Product] ([ProductId]) ON DELETE CASCADE
    );
    
    CREATE NONCLUSTERED INDEX [IX_ProductSpecifications_ProductId] 
        ON [dbo].[ProductSpecifications] ([ProductId] ASC);
    
    PRINT 'ProductSpecifications 表创建成功';
END
ELSE
BEGIN
    PRINT 'ProductSpecifications 表已存在';
END
GO

-- ============================================
-- 12. 标记迁移为已应用
-- ============================================
IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20251111052419_AddShoppingTables')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20251111052419_AddShoppingTables', '8.0.0');
    PRINT 'AddShoppingTables 迁移记录已添加';
END
ELSE
BEGIN
    PRINT 'AddShoppingTables 迁移记录已存在';
END
GO

IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20251112073154_AddProductReviewImageSpecificationOnly')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20251112073154_AddProductReviewImageSpecificationOnly', '8.0.0');
    PRINT 'AddProductReviewImageSpecificationOnly 迁移记录已添加';
END
ELSE
BEGIN
    PRINT 'AddProductReviewImageSpecificationOnly 迁移记录已存在';
END
GO

-- ============================================
-- 13. 验证表创建
-- ============================================
SELECT 
    'ProductCategory' AS TableName,
    CASE WHEN EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductCategory]') AND type in (N'U'))
         THEN '已创建' ELSE '未创建' END AS Status
UNION ALL
SELECT 
    'Product',
    CASE WHEN EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Product]') AND type in (N'U'))
         THEN '已创建' ELSE '未创建' END
UNION ALL
SELECT 
    'ProductInventory',
    CASE WHEN EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductInventory]') AND type in (N'U'))
         THEN '已创建' ELSE '未创建' END
UNION ALL
SELECT 
    'Orders',
    CASE WHEN EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND type in (N'U'))
         THEN '已创建' ELSE '未创建' END
UNION ALL
SELECT 
    'OrderItems',
    CASE WHEN EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderItems]') AND type in (N'U'))
         THEN '已创建' ELSE '未创建' END
UNION ALL
SELECT 
    'ShoppingCartItem',
    CASE WHEN EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ShoppingCartItem]') AND type in (N'U'))
         THEN '已创建' ELSE '未创建' END
UNION ALL
SELECT 
    'OrderPaymentLogs',
    CASE WHEN EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderPaymentLogs]') AND type in (N'U'))
         THEN '已创建' ELSE '未创建' END
UNION ALL
SELECT 
    'WalletUserProfiles',
    CASE WHEN EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WalletUserProfiles]') AND type in (N'U'))
         THEN '已创建' ELSE '未创建' END
UNION ALL
SELECT 
    'ProductImages',
    CASE WHEN EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductImages]') AND type in (N'U'))
         THEN '已创建' ELSE '未创建' END
UNION ALL
SELECT 
    'ProductReviews',
    CASE WHEN EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductReviews]') AND type in (N'U'))
         THEN '已创建' ELSE '未创建' END
UNION ALL
SELECT 
    'ProductSpecifications',
    CASE WHEN EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductSpecifications]') AND type in (N'U'))
         THEN '已创建' ELSE '未创建' END
ORDER BY TableName;
GO

PRINT '========================================';
PRINT '所有表创建完成！';
PRINT '========================================';
GO


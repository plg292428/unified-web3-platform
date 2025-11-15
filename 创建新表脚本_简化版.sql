-- ============================================
-- 创建商品管理增强相关表（简化版）
-- 迁移: AddProductReviewImageSpecificationOnly
-- 日期: 2025-11-12
-- ============================================

USE UnifiedWeb3Platform;

-- 1. 创建 ProductImages 表（商品图片）
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
END;

-- 2. 创建 ProductReviews 表（商品评价）
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
END;

-- 3. 创建 ProductSpecifications 表（商品规格）
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
END;

-- 4. 标记迁移为已应用
IF NOT EXISTS (SELECT * FROM [dbo].[__EFMigrationsHistory] 
               WHERE [MigrationId] = '20251112073154_AddProductReviewImageSpecificationOnly')
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20251112073154_AddProductReviewImageSpecificationOnly', '8.0.0');
END;

-- 5. 验证表创建
SELECT 
    'ProductImages' AS TableName,
    CASE WHEN EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductImages]') AND type in (N'U'))
         THEN '已创建' ELSE '未创建' END AS Status
UNION ALL
SELECT 
    'ProductReviews' AS TableName,
    CASE WHEN EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductReviews]') AND type in (N'U'))
         THEN '已创建' ELSE '未创建' END AS Status
UNION ALL
SELECT 
    'ProductSpecifications' AS TableName,
    CASE WHEN EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductSpecifications]') AND type in (N'U'))
         THEN '已创建' ELSE '未创建' END AS Status;


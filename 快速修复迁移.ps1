# 快速修复数据库迁移问题
# 直接创建三个新表并标记迁移为已应用

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$ErrorActionPreference = "Continue"

Write-Host "========================================" -ForegroundColor Green
Write-Host "  数据库迁移快速修复" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

$server = "localhost"
$database = "UnifiedWeb3Platform"
$user = "UnifiedWebAppUser"
$password = "P@ssword_2025!"

# SQL 语句数组（使用单引号字符串避免转义问题）
$sqlStatements = @(
    'IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[ProductImages]'') AND type in (N''U'')) BEGIN CREATE TABLE [dbo].[ProductImages] ([ImageId] bigint IDENTITY(1,1) NOT NULL, [ProductId] bigint NOT NULL, [ImageUrl] nvarchar(512) NOT NULL, [ImageType] nvarchar(32) NOT NULL DEFAULT ''gallery'', [SortOrder] int NOT NULL DEFAULT 0, [IsPrimary] bit NOT NULL DEFAULT 0, [CreateTime] datetime NOT NULL DEFAULT (getutcdate()), CONSTRAINT [PK_ProductImage_ImageId] PRIMARY KEY CLUSTERED ([ImageId] ASC), CONSTRAINT [FK_ProductImage_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([ProductId]) ON DELETE CASCADE); CREATE NONCLUSTERED INDEX [IX_ProductImages_ProductId] ON [dbo].[ProductImages] ([ProductId] ASC); END',
    'IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[ProductReviews]'') AND type in (N''U'')) BEGIN CREATE TABLE [dbo].[ProductReviews] ([ReviewId] bigint IDENTITY(1,1) NOT NULL, [ProductId] bigint NOT NULL, [Uid] int NOT NULL, [OrderId] bigint NULL, [Rating] int NOT NULL, [Content] nvarchar(2000) NULL, [IsApproved] bit NOT NULL DEFAULT 0, [IsVisible] bit NOT NULL DEFAULT 1, [CreateTime] datetime NOT NULL DEFAULT (getutcdate()), [UpdateTime] datetime NOT NULL DEFAULT (getutcdate()), CONSTRAINT [PK_ProductReview_ReviewId] PRIMARY KEY CLUSTERED ([ReviewId] ASC), CONSTRAINT [FK_ProductReview_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([ProductId]) ON DELETE CASCADE, CONSTRAINT [FK_ProductReview_User] FOREIGN KEY ([Uid]) REFERENCES [dbo].[User] ([Uid]) ON DELETE CASCADE, CONSTRAINT [FK_ProductReview_Order] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([OrderId])); CREATE NONCLUSTERED INDEX [IX_ProductReviews_ProductId] ON [dbo].[ProductReviews] ([ProductId] ASC); CREATE NONCLUSTERED INDEX [IX_ProductReviews_Uid] ON [dbo].[ProductReviews] ([Uid] ASC); CREATE NONCLUSTERED INDEX [IX_ProductReviews_OrderId] ON [dbo].[ProductReviews] ([OrderId] ASC); END',
    'IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[ProductSpecifications]'') AND type in (N''U'')) BEGIN CREATE TABLE [dbo].[ProductSpecifications] ([SpecificationId] bigint IDENTITY(1,1) NOT NULL, [ProductId] bigint NOT NULL, [SpecificationName] nvarchar(64) NOT NULL, [SpecificationValue] nvarchar(128) NOT NULL, [PriceAdjustment] decimal(19,6) NOT NULL DEFAULT 0, [StockQuantity] int NULL, [SortOrder] int NOT NULL DEFAULT 0, [IsEnabled] bit NOT NULL DEFAULT 1, [CreateTime] datetime NOT NULL DEFAULT (getutcdate()), [UpdateTime] datetime NOT NULL DEFAULT (getutcdate()), CONSTRAINT [PK_ProductSpecification_SpecificationId] PRIMARY KEY CLUSTERED ([SpecificationId] ASC), CONSTRAINT [FK_ProductSpecification_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([ProductId]) ON DELETE CASCADE); CREATE NONCLUSTERED INDEX [IX_ProductSpecifications_ProductId] ON [dbo].[ProductSpecifications] ([ProductId] ASC); END',
    'IF NOT EXISTS (SELECT * FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = ''20251112073154_AddProductReviewImageSpecificationOnly'') BEGIN INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (''20251112073154_AddProductReviewImageSpecificationOnly'', ''8.0.0''); END'
)

Write-Host "[1/4] 创建 ProductImages 表..." -ForegroundColor Cyan
$result1 = sqlcmd -S $server -d $database -U $user -P $password -Q $sqlStatements[0] -W 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "  [OK] ProductImages 表处理完成" -ForegroundColor Green
} else {
    Write-Host "  [FAIL] 创建失败: $result1" -ForegroundColor Red
}

Write-Host "`n[2/4] 创建 ProductReviews 表..." -ForegroundColor Cyan
$result2 = sqlcmd -S $server -d $database -U $user -P $password -Q $sqlStatements[1] -W 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "  [OK] ProductReviews 表处理完成" -ForegroundColor Green
} else {
    Write-Host "  [FAIL] 创建失败: $result2" -ForegroundColor Red
}

Write-Host "`n[3/4] 创建 ProductSpecifications 表..." -ForegroundColor Cyan
$result3 = sqlcmd -S $server -d $database -U $user -P $password -Q $sqlStatements[2] -W 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "  [OK] ProductSpecifications 表处理完成" -ForegroundColor Green
} else {
    Write-Host "  [FAIL] 创建失败: $result3" -ForegroundColor Red
}

Write-Host "`n[4/4] 标记迁移为已应用..." -ForegroundColor Cyan
$result4 = sqlcmd -S $server -d $database -U $user -P $password -Q $sqlStatements[3] -W 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "  [OK] 迁移记录已添加" -ForegroundColor Green
} else {
    Write-Host "  [FAIL] 添加失败: $result4" -ForegroundColor Red
}

Write-Host "`n验证表创建状态..." -ForegroundColor Cyan
$verifyQuery = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME IN ('ProductImages', 'ProductReviews', 'ProductSpecifications') ORDER BY TABLE_NAME"
$verifyResult = sqlcmd -S $server -d $database -U $user -P $password -Q $verifyQuery -W -h -1 2>&1

if ($LASTEXITCODE -eq 0) {
    $tables = $verifyResult | Where-Object { $_ -match "^\w+" }
    Write-Host "`n已创建的表:" -ForegroundColor Green
    foreach ($table in $tables) {
        Write-Host "  [OK] $table" -ForegroundColor Green
    }
    
    if ($tables.Count -eq 3) {
        Write-Host "`n========================================" -ForegroundColor Green
        Write-Host "  修复完成！所有表已创建成功" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
    } else {
        Write-Host "`n[WARN] 部分表可能未创建，请检查" -ForegroundColor Yellow
    }
} else {
    Write-Host "  [WARN] 无法验证表状态" -ForegroundColor Yellow
}

Write-Host ""


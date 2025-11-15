using Microsoft.EntityFrameworkCore;
using UnifiedPlatform.DbService.Entities;

// 这个脚本用于添加示例商品到数据库
// 使用方法：在 Program.cs 或启动时调用，或通过 EF Core 迁移

public class SeedProducts
{
    public static async Task SeedAsync(StDbContext dbContext)
    {
        // 检查是否已有分类
        if (!await dbContext.ProductCategories.AnyAsync())
        {
            // 添加分类
            var categories = new[]
            {
                new ProductCategory
                {
                    CategoryId = 1,
                    Name = "硬件设备",
                    Slug = "hardware",
                    Description = "官方认证的 Web3 设备",
                    ParentCategoryId = null,
                    SortOrder = 1,
                    IsActive = true,
                    CreateTime = DateTime.UtcNow,
                    UpdateTime = DateTime.UtcNow
                },
                new ProductCategory
                {
                    CategoryId = 2,
                    Name = "数字藏品",
                    Slug = "digital-collectibles",
                    Description = "限量发行的 NFT 藏品",
                    ParentCategoryId = null,
                    SortOrder = 2,
                    IsActive = true,
                    CreateTime = DateTime.UtcNow,
                    UpdateTime = DateTime.UtcNow
                },
                new ProductCategory
                {
                    CategoryId = 3,
                    Name = "增值服务",
                    Slug = "value-services",
                    Description = "链上增值服务",
                    ParentCategoryId = null,
                    SortOrder = 3,
                    IsActive = true,
                    CreateTime = DateTime.UtcNow,
                    UpdateTime = DateTime.UtcNow
                }
            };

            dbContext.ProductCategories.AddRange(categories);
            await dbContext.SaveChangesAsync();
        }

        // 检查是否已有商品
        if (!await dbContext.Products.AnyAsync())
        {
            var now = DateTime.UtcNow;

            // 添加商品
            var products = new[]
            {
                new Product
                {
                    CategoryId = 1,
                    Name = "Bitget Web3 Vault",
                    Subtitle = "冷存储签名器",
                    Description = "专为 Bitget 用户打造的硬件 Vault，提供安全的冷存储解决方案",
                    ThumbnailUrl = null,
                    Price = 399m,
                    Currency = "USDT",
                    ChainId = 1, // Ethereum
                    Sku = "BG-VLT-001",
                    IsPublished = true,
                    CreateTime = now,
                    UpdateTime = now
                },
                new Product
                {
                    CategoryId = 2,
                    Name = "Bitget Genesis NFT",
                    Subtitle = "限量 Genesis 系列",
                    Description = "Bitget 生态限量 Genesis NFT，具有独特的收藏价值",
                    ThumbnailUrl = null,
                    Price = 129m,
                    Currency = "USDT",
                    ChainId = 137, // Polygon
                    Sku = "BG-NFT-001",
                    IsPublished = true,
                    CreateTime = now,
                    UpdateTime = now
                },
                new Product
                {
                    CategoryId = 3,
                    Name = "链上风控加速",
                    Subtitle = "链上风控增强服务",
                    Description = "快速提升企业链上风控能力，保障资产安全",
                    ThumbnailUrl = null,
                    Price = 259m,
                    Currency = "USDT",
                    ChainId = 56, // BSC
                    Sku = "BG-SRV-001",
                    IsPublished = true,
                    CreateTime = now,
                    UpdateTime = now
                },
                new Product
                {
                    CategoryId = 1,
                    Name = "Web3 安全钱包",
                    Subtitle = "多链支持硬件钱包",
                    Description = "支持 Ethereum、Polygon、BSC 等多条链的硬件钱包",
                    ThumbnailUrl = null,
                    Price = 299m,
                    Currency = "USDT",
                    ChainId = 1,
                    Sku = "BG-WLT-002",
                    IsPublished = true,
                    CreateTime = now,
                    UpdateTime = now
                },
                new Product
                {
                    CategoryId = 2,
                    Name = "Bitget 纪念 NFT",
                    Subtitle = "平台纪念系列",
                    Description = "Bitget 平台纪念 NFT，限量发行 1000 枚",
                    ThumbnailUrl = null,
                    Price = 89m,
                    Currency = "USDT",
                    ChainId = 137,
                    Sku = "BG-NFT-002",
                    IsPublished = true,
                    CreateTime = now,
                    UpdateTime = now
                },
                new Product
                {
                    CategoryId = 3,
                    Name = "智能合约审计服务",
                    Subtitle = "专业安全审计",
                    Description = "提供专业的智能合约安全审计服务，保障合约安全",
                    ThumbnailUrl = null,
                    Price = 599m,
                    Currency = "USDT",
                    ChainId = 1,
                    Sku = "BG-SRV-002",
                    IsPublished = true,
                    CreateTime = now,
                    UpdateTime = now
                }
            };

            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();

            // 添加库存
            var inventories = new[]
            {
                new ProductInventory
                {
                    ProductId = products[0].ProductId,
                    QuantityAvailable = 25,
                    QuantityReserved = 0,
                    UpdateTime = now
                },
                new ProductInventory
                {
                    ProductId = products[1].ProductId,
                    QuantityAvailable = 80,
                    QuantityReserved = 0,
                    UpdateTime = now
                },
                new ProductInventory
                {
                    ProductId = products[2].ProductId,
                    QuantityAvailable = 120,
                    QuantityReserved = 0,
                    UpdateTime = now
                },
                new ProductInventory
                {
                    ProductId = products[3].ProductId,
                    QuantityAvailable = 50,
                    QuantityReserved = 0,
                    UpdateTime = now
                },
                new ProductInventory
                {
                    ProductId = products[4].ProductId,
                    QuantityAvailable = 1000,
                    QuantityReserved = 0,
                    UpdateTime = now
                },
                new ProductInventory
                {
                    ProductId = products[5].ProductId,
                    QuantityAvailable = 10,
                    QuantityReserved = 0,
                    UpdateTime = now
                }
            };

            dbContext.ProductInventories.AddRange(inventories);
            await dbContext.SaveChangesAsync();
        }
    }
}


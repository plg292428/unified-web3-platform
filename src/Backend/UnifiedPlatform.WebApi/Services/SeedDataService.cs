using Microsoft.EntityFrameworkCore;
using UnifiedPlatform.DbService.Entities;

namespace UnifiedPlatform.WebApi.Services
{
    public class SeedDataService
    {
        private readonly StDbContext _dbContext;
        private readonly ILogger<SeedDataService> _logger;

        public SeedDataService(StDbContext dbContext, ILogger<SeedDataService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task SeedProductsAsync()
        {
            try
            {
                // 检查是否已有分类
                if (!await _dbContext.ProductCategories.AnyAsync())
                {
                    var categories = new[]
                    {
                        new ProductCategory
                        {
                            Name = "Hardware Devices",
                            Slug = "hardware",
                            Description = "Official certified Web3 devices",
                            ParentCategoryId = null,
                            SortOrder = 1,
                            IsActive = true,
                            CreateTime = DateTime.UtcNow,
                            UpdateTime = DateTime.UtcNow
                        },
                        new ProductCategory
                        {
                            Name = "Digital Collectibles",
                            Slug = "digital-collectibles",
                            Description = "Limited edition NFT collectibles",
                            ParentCategoryId = null,
                            SortOrder = 2,
                            IsActive = true,
                            CreateTime = DateTime.UtcNow,
                            UpdateTime = DateTime.UtcNow
                        },
                        new ProductCategory
                        {
                            Name = "Value-Added Services",
                            Slug = "value-services",
                            Description = "On-chain value-added services",
                            ParentCategoryId = null,
                            SortOrder = 3,
                            IsActive = true,
                            CreateTime = DateTime.UtcNow,
                            UpdateTime = DateTime.UtcNow
                        }
                    };

                    _dbContext.ProductCategories.AddRange(categories);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("已添加商品分类");
                }

                // 检查是否已有商品
                if (!await _dbContext.Products.AnyAsync())
                {
                    // 获取分类ID（如果分类已存在）
                    var categoryHardware = await _dbContext.ProductCategories.FirstOrDefaultAsync(c => c.Slug == "hardware");
                    var categoryDigital = await _dbContext.ProductCategories.FirstOrDefaultAsync(c => c.Slug == "digital-collectibles");
                    var categoryService = await _dbContext.ProductCategories.FirstOrDefaultAsync(c => c.Slug == "value-services");

                    if (categoryHardware == null || categoryDigital == null || categoryService == null)
                    {
                        _logger.LogWarning("分类不存在，无法添加商品");
                        return;
                    }

                    var now = DateTime.UtcNow;

                    var products = new[]
                    {
                        new Product
                        {
                            CategoryId = categoryHardware.CategoryId,
                            Name = "Bitget Web3 Vault",
                            Subtitle = "Cold Storage Signer",
                            Description = "Hardware Vault designed for Bitget users, providing secure cold storage solutions",
                            ThumbnailUrl = null,
                            Price = 399m,
                            Currency = "USDT",
                            ChainId = null,
                            Sku = "BG-VLT-001",
                            IsPublished = true,
                            CreateTime = now,
                            UpdateTime = now
                        },
                        new Product
                        {
                            CategoryId = categoryDigital.CategoryId,
                            Name = "Bitget Genesis NFT",
                            Subtitle = "Limited Genesis Series",
                            Description = "Limited edition Genesis NFT from Bitget ecosystem with unique collectible value",
                            ThumbnailUrl = null,
                            Price = 129m,
                            Currency = "USDT",
                            ChainId = null,
                            Sku = "BG-NFT-001",
                            IsPublished = true,
                            CreateTime = now,
                            UpdateTime = now
                        },
                        new Product
                        {
                            CategoryId = categoryService.CategoryId,
                            Name = "On-Chain Risk Control Acceleration",
                            Subtitle = "Enhanced Risk Control Service",
                            Description = "Rapidly improve enterprise on-chain risk control capabilities and ensure asset security",
                            ThumbnailUrl = null,
                            Price = 259m,
                            Currency = "USDT",
                            ChainId = null,
                            Sku = "BG-SRV-001",
                            IsPublished = true,
                            CreateTime = now,
                            UpdateTime = now
                        },
                        new Product
                        {
                            CategoryId = categoryHardware.CategoryId,
                            Name = "Web3 Security Wallet",
                            Subtitle = "Multi-Chain Hardware Wallet",
                            Description = "Hardware wallet supporting multiple chains including Ethereum, Polygon, BSC, and more",
                            ThumbnailUrl = null,
                            Price = 299m,
                            Currency = "USDT",
                            ChainId = null,
                            Sku = "BG-WLT-002",
                            IsPublished = true,
                            CreateTime = now,
                            UpdateTime = now
                        },
                        new Product
                        {
                            CategoryId = categoryDigital.CategoryId,
                            Name = "Bitget Commemorative NFT",
                            Subtitle = "Platform Commemorative Series",
                            Description = "Bitget platform commemorative NFT, limited edition of 1000 pieces",
                            ThumbnailUrl = null,
                            Price = 89m,
                            Currency = "USDT",
                            ChainId = null,
                            Sku = "BG-NFT-002",
                            IsPublished = true,
                            CreateTime = now,
                            UpdateTime = now
                        },
                        new Product
                        {
                            CategoryId = categoryService.CategoryId,
                            Name = "Smart Contract Audit Service",
                            Subtitle = "Professional Security Audit",
                            Description = "Professional smart contract security audit services to ensure contract safety",
                            ThumbnailUrl = null,
                            Price = 599m,
                            Currency = "USDT",
                            ChainId = null,
                            Sku = "BG-SRV-002",
                            IsPublished = true,
                            CreateTime = now,
                            UpdateTime = now
                        }
                    };

                    _dbContext.Products.AddRange(products);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"已添加 {products.Length} 个商品");

                    // 添加库存
                    var inventories = new[]
                    {
                        new ProductInventory { ProductId = products[0].ProductId, QuantityAvailable = 25, QuantityReserved = 0, UpdateTime = now },
                        new ProductInventory { ProductId = products[1].ProductId, QuantityAvailable = 80, QuantityReserved = 0, UpdateTime = now },
                        new ProductInventory { ProductId = products[2].ProductId, QuantityAvailable = 120, QuantityReserved = 0, UpdateTime = now },
                        new ProductInventory { ProductId = products[3].ProductId, QuantityAvailable = 50, QuantityReserved = 0, UpdateTime = now },
                        new ProductInventory { ProductId = products[4].ProductId, QuantityAvailable = 1000, QuantityReserved = 0, UpdateTime = now },
                        new ProductInventory { ProductId = products[5].ProductId, QuantityAvailable = 10, QuantityReserved = 0, UpdateTime = now }
                    };

                    _dbContext.ProductInventories.AddRange(inventories);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("已添加商品库存");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加商品数据失败: {Message}", ex.Message);
                _logger.LogError(ex, "堆栈跟踪: {StackTrace}", ex.StackTrace);
                // 重新抛出异常，让调用者知道失败
                throw;
            }
        }
    }
}


using System;
using System.Collections.Generic;

namespace UnifiedPlatform.DbService.Entities
{
    public partial class Product
    {
        public long ProductId { get; set; }

        public int CategoryId { get; set; }

        public string Name { get; set; } = null!;

        public string? Subtitle { get; set; }

        public string? Description { get; set; }

        public string? ThumbnailUrl { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; } = "USDT";

        public int? ChainId { get; set; }

        public string? Sku { get; set; }

        public bool IsPublished { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public virtual ChainNetworkConfig? Chain { get; set; }

        public virtual ProductCategory Category { get; set; } = null!;

        public virtual ProductInventory? Inventory { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();

        public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

        public virtual ICollection<ProductSpecification> ProductSpecifications { get; set; } = new List<ProductSpecification>();
    }
}

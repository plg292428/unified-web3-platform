using System;
using System.Collections.Generic;

namespace UnifiedPlatform.DbService.Entities
{
    public partial class ProductCategory
    {
        public int CategoryId { get; set; }

        public string Name { get; set; } = null!;

        public string? Slug { get; set; }

        public string? Description { get; set; }

        public int? ParentCategoryId { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public virtual ProductCategory? ParentCategory { get; set; }

        public virtual ICollection<ProductCategory> Children { get; set; } = new List<ProductCategory>();

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

using System;

namespace UnifiedPlatform.DbService.Entities
{
    public partial class ProductInventory
    {
        public long InventoryId { get; set; }

        public long ProductId { get; set; }

        public int QuantityAvailable { get; set; }

        public int QuantityReserved { get; set; }

        public DateTime UpdateTime { get; set; }

        public virtual Product Product { get; set; } = null!;
    }
}

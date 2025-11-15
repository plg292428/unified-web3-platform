using System;

namespace UnifiedPlatform.DbService.Entities
{
    public partial class OrderItem
    {
        public long OrderItemId { get; set; }

        public long OrderId { get; set; }

        public long ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal Subtotal { get; set; }

        public virtual Order Order { get; set; } = null!;

        public virtual Product Product { get; set; } = null!;
    }
}

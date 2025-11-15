using System;

namespace UnifiedPlatform.DbService.Entities
{
    public partial class ShoppingCartItem
    {
        public long CartItemId { get; set; }

        public int Uid { get; set; }

        public long ProductId { get; set; }

        public int Quantity { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public virtual Product Product { get; set; } = null!;

        public virtual User UidNavigation { get; set; } = null!;
    }
}

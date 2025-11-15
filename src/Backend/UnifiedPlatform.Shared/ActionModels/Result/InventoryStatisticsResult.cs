namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// 库存统计结果
    /// </summary>
    public class InventoryStatisticsResult
    {
        /// <summary>
        /// 总商品数
        /// </summary>
        public int TotalProducts { get; set; }

        /// <summary>
        /// 有库存的商品数
        /// </summary>
        public int TotalProductsWithInventory { get; set; }

        /// <summary>
        /// 总可用库存
        /// </summary>
        public int TotalQuantityAvailable { get; set; }

        /// <summary>
        /// 总预留库存
        /// </summary>
        public int TotalQuantityReserved { get; set; }

        /// <summary>
        /// 总实际可用库存（可用 - 预留）
        /// </summary>
        public int TotalAvailableQuantity { get; set; }

        /// <summary>
        /// 低库存商品数（<= 10）
        /// </summary>
        public int LowStockCount { get; set; }

        /// <summary>
        /// 缺货商品数（<= 0）
        /// </summary>
        public int OutOfStockCount { get; set; }
    }
}


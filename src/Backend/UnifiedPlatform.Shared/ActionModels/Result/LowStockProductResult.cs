namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// 低库存商品结果
    /// </summary>
    public class LowStockProductResult
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// 可用库存数量
        /// </summary>
        public int QuantityAvailable { get; set; }

        /// <summary>
        /// 预留库存数量
        /// </summary>
        public int QuantityReserved { get; set; }

        /// <summary>
        /// 实际可用数量（可用 - 预留）
        /// </summary>
        public int AvailableQuantity { get; set; }

        /// <summary>
        /// 预警阈值
        /// </summary>
        public int Threshold { get; set; }
    }
}

